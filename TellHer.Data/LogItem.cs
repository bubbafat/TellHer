using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace TellHer.Data
{
    public class LogItem
    {
        public int ID { get; set; }

        public DateTime Date { get; set; }

        [StringLength(50)]
        public string Level { get; set; }

        [StringLength(4000)]
        public string Message { get; set; }

        [StringLength(4000)]
        public string Exception { get; set; }
    }
}
