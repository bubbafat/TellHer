using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TellHer.Data
{
    public class BlockedNumber
    {
        public int Id { get; set; }
        public string Phone { get; set; }
        public DateTime Blocked { get; set; }
        public DateTime Expires { get; set; }
    }
}
