using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace TellHer.Domain
{
    public class TraceLogger : ILogging
    {
        public void Trace(string format, params object[] args)
        {
            System.Diagnostics.Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, format, args));
        }

        public void Trace(string message)
        {
            System.Diagnostics.Trace.WriteLine(message);
        }

        public void Debug(string message)
        {
            Trace(message);
        }

        public void Debug(string format, params object[] args)
        {
            Trace(format, args);
        }

        public void Info(string message)
        {
            Trace(message);
        }

        public void Info(string format, params object[] args)
        {
            Trace(format, args);
        }

        public void Warn(string message)
        {
            Trace(message);
        }

        public void Warn(string format, params object[] args)
        {
            Trace(format, args);
        }

        public void Error(string message)
        {
            Trace(message);
        }

        public void Error(Exception ex)
        {
            if (ex != null)
            {
                Trace(ex.ToString());
            }
        }

        public void Error(string message, Exception ex)
        {
            Trace("{0} {1}", message, ex);
        }

        public void Error(string format, params object[] args)
        {
            Trace(format, args);
        }
    }
}
