using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TellHer.Data
{
    public class Subscription
    {
        public int Id { get; set; }
        public string Phone { get; set; }
        public DateTime Next { get; set; }
    }
}
