using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Blocks.Exceptons
{
    public class NotFoundException<T> : Exception where T : class
    {
        public NotFoundException() : base($"Entity '{nameof(T)}' was not found.")
        {
        }

        public NotFoundException(string message)
            : base(message)
        { }

        public NotFoundException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
