using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Blocks.Exceptions
{
    public class ForbiddenException : Exception
    {
        public ForbiddenException() : base($"Forbidden access.")
        {
        }

        public ForbiddenException(string message)
            : base(message)
        { }

        public ForbiddenException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
