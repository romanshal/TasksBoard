using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Application.Models.Requests
{
    public class UpdateBoardMessageRequest
    {
        public Guid BoardId { get; set; }
        public Guid BoardMessageId { get; set; }
        public required string Message { get; set; }
    }
}
