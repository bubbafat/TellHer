using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TellHer.Domain;
using TellHer.Data;
using StructureMap;
using System.Configuration;
using System.Globalization;

namespace TellHer.SubscriptionService
{
    internal abstract class ClientAction
    {
        IConfiguration _Configuration = ObjectFactory.GetInstance<IConfiguration>();

        public abstract string Action
        {
            get;
        }

        protected static void Say(string destination, string message)
        {
            IDataStore store = ObjectFactory.GetInstance<IDataStore>();
            OutgoingSmsMessage msg = OutgoingSmsMessage.CreateWithDefaults(destination, message);
            store.Save(msg);
        }

        protected void Say(string destination, string format, params object[] args)
        {
            Say(destination, string.Format(CultureInfo.InvariantCulture, format, args));
        }

        protected void GeneralHelp(IncomingSmsMessage message)
        {
            Say(message.From, SmsResponseStrings.PublicHelp());
        }

        public void Perform(IncomingSmsMessage message)
        {
            if (message.From == _Configuration.AdminNumber)
            {
                PerformAdmin(message);
                return;
            }

            IDataStore store = ObjectFactory.GetInstance<IDataStore>();
            if (store.Subscriptions.Any(s => s.Phone == message.From))
            {
                PerformSubscriber(message);
                return;
            }

            PerformUnknownUser(message);
        }

        protected virtual void PerformUnknownUser(IncomingSmsMessage message)
        {
            GeneralHelp(message);
        }

        protected virtual void PerformSubscriber(IncomingSmsMessage message)
        {
            GeneralHelp(message);
        }

        protected virtual void PerformAdmin(IncomingSmsMessage message)
        {
            GeneralHelp(message);
        }

        protected static bool TryCrackId(string input, out int id)
        {
            id = -1;

            // <command> <id> [anything here is an error]
            // So we try to crack 3 parts - that validates that we only get "<command> <id>" and not "<cmd> <id> <other>"
            string[] parts = input.Split(new char[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 2)
            {
                return false;
            }

            return int.TryParse(parts[1], out id);
        }

        protected static bool TryCrackMessage(string input, out string message)
        {
            message = null;

            // <command> <message>
            string[] parts = input.Split(new char[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 2)
            {
                return false;
            }

            message = parts[1];

            return true;
        }

        protected static bool TryCrackIdMessage(string input, out int id, out string message)
        {
            message = null;
            id = -1;

            // <command> <id> <message>
            string[] parts = input.Split(new char[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 3)
            {
                return false;
            }

            message = parts[2];
            return int.TryParse(parts[1], out id);
        }
    }
}
