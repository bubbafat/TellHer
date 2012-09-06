using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TellHer.Domain;
using System.Configuration;

namespace WorkerRole1
{
    public class TellHerServiceConfiguration : IConfiguration
    {
        readonly object _lock = new object();

        string _authToken;
        string _accountSid;
        string _fromNumber;
        string _adminNumber;
 
        public int BetaLimit
        {
            get { return 50; }
        }

        public string AuthToken
        {
            get { return GetCachedValue("TwilioToken", ref _authToken); }
        }

        public string AccountSid
        {
            get { return GetCachedValue("TwilioAccount", ref _accountSid); }
        }

        public string AdminNumber
        {
            get { return GetCachedValue("AdminNumber", ref _fromNumber); }
        }

        public string FromNumber
        {
            get { return GetCachedValue("TwilioFrom", ref _adminNumber); }
        }

        private string GetCachedValue(string appSetting, ref string cachedValue)
        {
            if (cachedValue == null)
            {
                lock (_lock)
                {
                    if (cachedValue == null)
                    {
                        cachedValue = ConfigurationManager.AppSettings[appSetting];
                    }
                }
            }

            return cachedValue;
        }
    }
}
