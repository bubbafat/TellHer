using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TellHer.Domain
{
    public interface ILogging
    {
        void Trace(string message);
        void Trace(string format, params object[] args);

        void Debug(string message);
        void Debug(string format, params object[] args);

        void Info(string message);
        void Info(string format, params object[] args);

        void Warn(string message);
        void Warn(string format, params object[] args);

        void Error(string message);
        void Error(Exception ex);
        void Error(string message, Exception ex);
        void Error(string format, params object[] args);
    }
}
