using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TellHer.Domain;
using TellHer.Data;

using TellHer.SubscriptionService.Actions;
using System.Globalization;

namespace TellHer.SubscriptionService
{
    public class SmsCommandProcessor : ICommandProcessor
    {
        Dictionary<string, ClientAction> _actions = new Dictionary<string, ClientAction>
        {
            { Help.Name, new Help() },
            { Join.Name, new Join() },
            { Quit.Name, new Quit() },
            // some aliases we should support
            { "stop", new Quit() },
            { "unsubscribe", new Quit() },
            { Sub.Name, new Sub() },
            { Actions.Add.Name, new Add() },
            { Remove.Name, new Remove() },
            { Edit.Name, new Edit() },
            { Block.Name, new Block() },
            { Unblock.Name, new Unblock() },
            { Today.Name, new Today() },
            { Ping.Name, new Ping() },
            { Last.Name, new Last() },
            { Status.Name, new Status() },
            { Say.Name, new Say() },
            { Suggest.Name, new Suggest() },
        };

        ThrottledProcessor<IncomingSmsMessage> _messageProcessor;

        public SmsCommandProcessor()
        {
            _messageProcessor = new ThrottledProcessor<IncomingSmsMessage>(
                100,
                ProcessReceivedMessage,
                ReceivedMessageComplete,
                ReceivedMessageFailed);
        }

        public void Add(IncomingSmsMessage message)
        {
            _messageProcessor.Add(message);
        }

        private void ProcessReceivedMessage(IncomingSmsMessage message)
        {
            if (NumberBlocker.IsBlocked(message.From))
            {
                LogManager.Log.Info("SMS received from blocked number {0}.  Message: {1}",
                    message.From,
                    message.Message);

                // skip it entirely
                return;
            }

            string command = GetCommand(message);
            ClientAction action = GetActionOrHelp(command);
            action.Perform(message);
        }

        private void ReceivedMessageComplete(IncomingSmsMessage message)
        {
            // nothing to do just yet
        }

        private void ReceivedMessageFailed(IncomingSmsMessage message, Exception ex)
        {
            LogManager.Log.Error(string.Format(CultureInfo.InvariantCulture, "Error processing message ({0} - {1})", message.From, message.Message), ex);
        }

        private ClientAction GetActionOrHelp(string name)
        {
            ClientAction action;
            if (_actions.TryGetValue(name.ToLowerInvariant(), out action))
            {
                return action;
            }

            return new Help();
        }

        private static string GetCommand(IncomingSmsMessage message)
        {
            if (message == null)
            {
                return string.Empty;
            }

            if (string.IsNullOrWhiteSpace(message.Message))
            {
                return string.Empty;
            }

            string[] parts = message.Message.Split(new char[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
            return parts[0];
        }
    }
}
