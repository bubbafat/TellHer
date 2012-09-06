using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StructureMap;

namespace TellHer.Domain
{
    public static class LogManager
    {
        static LogManager()
        {
            Log = ObjectFactory.GetInstance<ILogging>();
        }

        public static ILogging Log { get; private set; }
    }
}
