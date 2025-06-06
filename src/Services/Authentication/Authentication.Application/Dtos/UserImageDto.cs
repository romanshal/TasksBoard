using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Authentication.Application.Dtos
{
    public class UserImageDto
    {
        public byte[]? Image { get; set; }
        public string? ImageExtension { get; set; }
    }
}
