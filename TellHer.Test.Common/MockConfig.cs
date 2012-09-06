using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TellHer.Domain;

namespace TellHer.Test.Common
{
    public class MockConfig : IConfiguration
    {
        public int BetaLimit
        {
            get { return 100; }
        }

        public string AuthToken
        {
            get { return "12345"; }
        }

        public string AccountSid
        {
            get { return "AC123451234512345"; }
        }

        public string AdminNumber
        {
            get { return "+15555551212"; }
        }

        public string FromNumber
        {
            get { return "+15555551818"; }
        }
    }
}
