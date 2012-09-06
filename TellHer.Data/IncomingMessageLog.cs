using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TellHer.Data
{
    public class IncomingMessageLog
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string From { get; set; }
        public string Message { get; set; }
    }
}
