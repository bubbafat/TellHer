using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

namespace TellHer.Sms
{
    public interface ITwilioRequestValidator
    {
        bool IsValid(string pathAndQueryString, string expectedSignature, NameValueCollection post);
    }
}
