using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentication.Application.Models
{
    public class CreateTokenModel
    {
        public required Guid UserId { get; set; }
        public required string UserEmail { get; set; }
    }
}
