﻿using Common.Blocks.Configurations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Common.Blocks.Extensions
{
    public static class AuthenticationExtensions
    {
        public static AuthenticationBuilder AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwt = configuration.GetRequiredSection("Authentication:Jwt").Get<JwtCofiguration>() ?? throw new InvalidOperationException("Configuration section 'Jwt' not found.");

            var builder = services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.SaveToken = true;
                    //options.Authority = builder.Configuration["Authentication:Jwt:Authority"] ?? throw new InvalidOperationException("Configuration setting 'Authority' not found");
                    //options.Audience = builder.Configuration["Authentication:Jwt:Audience"] ?? throw new InvalidOperationException("Configuration setting 'Audience' not found");

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ClockSkew = TimeSpan.Zero,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwt.Issuer ?? throw new InvalidOperationException("Configuration setting 'Issuer' not found"),
                        ValidAudience = jwt.Audience ?? throw new InvalidOperationException("Configuration setting 'Audience' not found"),
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Secret ?? throw new InvalidOperationException("Configuration setting 'Secret' not found")))
                    };
                });

            return builder;
        }
    }
}
