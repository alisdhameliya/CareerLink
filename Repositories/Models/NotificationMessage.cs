using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repositories.Models
{
    namespace Repositories.Models
    {
        public class NotificationMessage
        {
            public int UserId { get; set; }
            public string Content { get; set; }
            public string Type { get; set; }
            public DateTime Timestamp { get; set; }
        }
    }
}