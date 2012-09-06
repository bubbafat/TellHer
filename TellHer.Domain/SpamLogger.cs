using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace TellHer.Domain
{
    public class SpamLogger
    {
        TimeSpan _frequency;
        DateTime _next;

        public SpamLogger(TimeSpan frequency)
        {
            _frequency = frequency;
            _next = DateTime.UtcNow;
        }

        public void Spam(string message)
        {
            if (DateTime.UtcNow > _next)
            {
                LogManager.Log.Info(message);
                _next = _next.Add(_frequency);
            }
        }
    }
}
