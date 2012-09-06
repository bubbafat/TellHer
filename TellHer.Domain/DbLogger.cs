using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TellHer.Data;

using System.Globalization;

namespace TellHer.Domain
{
    public class DatabaseLogger : ILogging
    {
        IDataStore _db = DataStore.GetInstance();

        public void Trace(string message)
        {
            System.Diagnostics.Trace.WriteLine(message);
        }

        public void Trace(string format, params object[] args)
        {
            Trace(string.Format(CultureInfo.InvariantCulture, format, args));
        }

        private void Save(string level, string message, Exception ex)
        {
            string exString = null;
            if(ex != null)
            {
                exString = ex.ToString();
                if(exString.Length > 4000)
                {
                    exString = exString.Substring(0, 4000);
                }
            }

            DateTime now = DateTime.UtcNow;

            _db.Save(new LogItem
            {
                Date = now,
                Level = level,
                Message = message,
                Exception = exString,
            });

            Trace("{0} [{1}] {2}", now, level, message);
            if (ex != null)
            {
                Trace(ex.ToString());
            }
        }

        public void Debug(string message)
        {
            Save("Debug", message, null);
        }

        public void Debug(string format, params object[] args)
        {
            Debug(string.Format(CultureInfo.InvariantCulture, format, args));
        }

        public void Info(string message)
        {
            Save("Information", message, null);
        }

        public void Info(string format, params object[] args)
        {
            Info(string.Format(CultureInfo.InvariantCulture, format, args));
        }

        public void Warn(string message)
        {
            Save("Warning", message, null);
        }

        public void Warn(string format, params object[] args)
        {
            Warn(string.Format(CultureInfo.InvariantCulture, format, args));
        }

        public void Error(string message)
        {
            Save("Error", message, null);
        }

        public void Error(Exception ex)
        {
            Error("Exception Reported", ex);
        }

        public void Error(string message, Exception ex)
        {
            Save("Error", message, ex);
        }

        public void Error(string format, params object[] args)
        {
            Error(string.Format(CultureInfo.InvariantCulture, format, args));
        }
    }
}
