using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace TellHer.Domain
{
    public static class SmsResponseStrings
    {
        public static string Join_SorryBetaFull()
        {
            return "Sorry - we are currently not accepting new subscriptions.  Keep an eye on http://www.tellhernow.com for details.";
        }

        public static string Join_AdminBetaReject()
        {
            return "We just turned another person away ...";
        }

        public static string PublicHelp()
        {
            return "Join by texting \"join\", quit by texting \"quit\" and visit http://tellhernow.com for more information.";
        }

        public static string Add_Failed_ExistingIdea(int id)
        {
            return Format("An idea with that exact text exists with ID {0}", id);
        }

        public static string Add_Success_AddedNewIdea(int id)
        {
            return Format("Added new idea with ID {0}", id);
        }

        public static string Add_Help()
        {
            return "Usage: ADD <message>   Example:  ADD this is the new idea";
        }

        public static string Join_Created()
        {
            return "Your subscription has been created.  You can get help by texting \"help\" or unsubscribe by texting \"quit\" anytime.";
        }

        public static string Join_AlreadyExists()
        {
            return "A subscription for this number already exists.  You can get help by texting \"help\" or unsubscribe by texting \"quit\" anytime.";
        }

        public static string Quit_AllRemoved(string phone)
        {
            return Format("All subscriptions have been removed for phone number: {0}.  Text \"join\" anytime to start again.", phone);
        }

        private static string Format(string format, params object[] args)
        {
            return string.Format(CultureInfo.InvariantCulture, format, args);
        }

        public static string Last_Help()
        {
            return "Usage: LAST [log | error]";
        }
    }
}
