using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Blocks.Constants
{
    public static class OutboxEventStatuses
    {
        public readonly static string Created = "created";
        public readonly static string Sent = "sent";
    }
}
