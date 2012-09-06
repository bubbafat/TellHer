using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TellHer.Sms
{
    public class MockTwilioRequestValidator : ITwilioRequestValidator
    {
        public bool IsValid(string pathAndQueryString, string expectedSignature, System.Collections.Specialized.NameValueCollection post)
        {
            return true;
        }
    }
}
