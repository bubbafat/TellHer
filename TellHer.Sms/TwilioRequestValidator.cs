using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using TellHer.Domain;
using Configuration = TellHer.Domain.Configuration;

namespace TellHer.Sms
{
    public class TwilioRequestValidator : ITwilioRequestValidator
    {
        IConfiguration _Configuration = Configuration.GetInstance();

        public bool IsValid(string pathAndQueryString, string expectedSignature, System.Collections.Specialized.NameValueCollection post)
        {
            if (string.IsNullOrEmpty(pathAndQueryString))
            {
                return false;
            }

            if (string.IsNullOrEmpty(expectedSignature))
            {
                return false;
            }

            string input = BuildInputString(pathAndQueryString, post);
            string hash = Sign(input);

            if (hash != expectedSignature)
            {
                LogManager.Log.Debug(input);
                LogManager.Log.Debug(hash);
            }

            return hash == expectedSignature;
        }

        private static string BuildInputString(string pathAndQueryString, System.Collections.Specialized.NameValueCollection post)
        {
            StringBuilder toSign = new StringBuilder(pathAndQueryString);

            List<string> inOrderKeys = post.AllKeys.ToList();
            inOrderKeys.Sort(StringComparer.Ordinal);

            foreach (string key in inOrderKeys)
            {
                toSign.AppendFormat("{0}{1}", key, post[key]);
            }

            return toSign.ToString();
        }

        private string Sign(string data)
        {
            using (HMACSHA1 sha1 = new HMACSHA1(Encoding.UTF8.GetBytes(_Configuration.AuthToken)))
            {
                byte[] hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(data));
                return Convert.ToBase64String(hash);
            }
        }
    }
}
