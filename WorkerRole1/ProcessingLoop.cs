using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Threading;

using TellHer.Data;
using TellHer.Domain;

using Configuration = TellHer.Domain.Configuration;


namespace WorkerRole1
{
    public class ProcessingLoop
    {
        IConfiguration _Configuration = Configuration.GetInstance();

        public void Run()
        {
            LogManager.Log.Info("WorkerRole.Run Loop Started");

            using (IOutgoingSmsQueue outgoing = OutgoingSmsQueue.GetInstance())
            {
                outgoing.Send(OutgoingSmsMessage.CreateWithDefaults(_Configuration.AdminNumber,
                    "WorkerRole.Run Loop Started"), 
                    null, 
                    null);

                while (true)
                {
                    // waiting for the queue to drain ensures we won't pick up the same one twice
                    if (outgoing.Length == 0)
                    {
                        IDataStore store = DataStore.GetInstance();

                        ProcessOutgoingMessages(outgoing, store);
                        ProcessSubscriptions(outgoing, store);
                    }
                    else
                    {
                        // there is already a back-log do don't go adding more to it...
                        Thread.Sleep(5 * 1000);
                    }
                }
            }
        }

        private void ProcessOutgoingMessages(IOutgoingSmsQueue outgoing, IDataStore store)
        {
            // first check the messages that were never attempted sorted by oldest to newest
            IList<OutgoingSmsMessage> messages = store.OutgoingMessages
                .Where(m => m.NextAttempt < DateTime.UtcNow)
                .OrderBy(m => m.UtcWhenAdded)
                .Take(outgoing.MessagesPerMinute)
                .ToList();

            // queue them
            foreach (OutgoingSmsMessage msg in messages)
            {
                outgoing.Send(msg, MessageSent, MessageSendThrewException);
            }
        }

        private static void ProcessSubscriptions(IOutgoingSmsQueue outgoing, IDataStore store)
        {
            DailyIdea idea = DailyIdea.TodaysIdea(DateTime.Today.ToUniversalTime());
            if (idea != null)
            {
                // now deal with the existing subscriptions
                // all where doing here is creating entries in the OutgoingSmsMessage table and updating the Next field on the 
                // Subscription.  These will be picked up on the next pass through the loop by the message sender.
                IList<Subscription> toProcess = store.Subscriptions.Where(s => s.Next < DateTime.UtcNow)
                    .Take(outgoing.MessagesPerMinute)
                    .ToList();

                foreach (Subscription sub in toProcess)
                {
                    OutgoingSmsMessage msg = OutgoingSmsMessage.CreateWithDefaults(sub.Phone, idea.Idea);
                    store.ScheduleMessage(msg, sub);
                }
            }
        }

        void MessageSent(SentSmsMessageLogEntry result, OutgoingSmsMessage message)
        {
            IDataStore store = DataStore.GetInstance();

            if (result.Status == (int)MessageSendStatus.Success)
            {
                // delete copy local to this store
                store.Remove(store.OutgoingMessages.Where(m => m.Id == message.Id).FirstOrDefault());
            }
            else
            {
                ProcessSendFailure(message);
            }
        }

        private void ProcessSendFailure(OutgoingSmsMessage msg)
        {
            IDataStore store = DataStore.GetInstance();

            // reload from the local store
            msg = store.OutgoingMessages.Where(m => m.Id == msg.Id).First();
            if (msg != null)
            {
                if (msg.Attempts < 10)
                {
                    msg.Attempts++;

                    DateTime next;
                    if (msg.Attempts < 10)
                    {
                        next = DateTime.UtcNow.AddMinutes(msg.Attempts * 10);
                    }
                    else
                    {
                        next = DateTime.UtcNow.AddHours(18);
                    }

                    msg.NextAttempt = next;
                    store.Save(msg);
                }
                else
                {

                    SentSmsMessageLogEntry entry = new SentSmsMessageLogEntry
                    {
                        Date = DateTime.UtcNow,
                        Destination = msg.Destination,
                        Message = msg.Message,
                        Status = (int)MessageSendStatus.Abandoned,
                        Details = string.Format(CultureInfo.InvariantCulture, "Fatal SMS Send error - giving up on sending message {0} - it was attempted {1} times.",
                                                msg.Id,
                                                msg.Attempts),
                    };

                    store.Save(entry);
                    store.Remove(msg);
                }
            }
        }

        void MessageSendThrewException(OutgoingSmsMessage msg, Exception ex)
        {
            LogManager.Log.Error("Exception sending SMS message", ex);
            ProcessSendFailure(msg);
        }
    }
}
