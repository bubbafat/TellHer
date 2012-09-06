using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TellHer.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace TellHer.Data
{
    public class OutgoingSmsMessage
    {
        public int Id { get; set; }
        public DateTime UtcWhenAdded { get; set; }
        public string Destination { get; set; }
        public string Message { get; set; }
        public int Attempts { get; set; }
        public DateTime NextAttempt { get; set; }

        public static OutgoingSmsMessage CreateWithDefaults(string destination, string message)
        {
            DateTime utcNow = DateTime.UtcNow;

            return new OutgoingSmsMessage
            {
                UtcWhenAdded = utcNow,
                Destination = destination,
                Message = message,
                Attempts = 0,
                NextAttempt = utcNow,
            };
        }
    }
}
