using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;

namespace TellHer.Data
{
    public sealed class TellHerDb : DbContext, IDataStore
    {
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<DailyIdea> DailyIdeas { get; set; }
        public DbSet<OutgoingSmsMessage> OutgoingMessages { get; set; }
        public DbSet<LogItem> LogItems { get; set; }
        public DbSet<BlockedNumber> BlockedNumbers { get; set; }
        public DbSet<IncomingMessageLog> IncomingMessageLogs { get; set; }
        public DbSet<SentSmsMessageLogEntry> SentSmsMessageLogEntries { get; set; }

        IQueryable<Subscription> IDataStore.Subscriptions
        {
            get { return Subscriptions; }
        }

        IQueryable<DailyIdea> IDataStore.DailyIdeas
        {
            get { return DailyIdeas; }
        }

        void IDataStore.Save(DailyIdea idea)
        {
            if (!DailyIdeas.Any(di => di.Id == idea.Id))
            {
                DailyIdeas.Add(idea);
            }

            SaveChanges();
        }

        void IDataStore.Save(Subscription sub)
        {
            if (!Subscriptions.Any(s => s.Id == sub.Id))
            {
                Subscriptions.Add(sub);
            }

            SaveChanges();
        }

        void IDataStore.Remove(Subscription sub)
        {
            if (sub != null)
            {
                foreach (Subscription s in Subscriptions.Where(s => s.Phone == sub.Phone))
                {
                    Subscriptions.Remove(s);
                }

                SaveChanges();
            }
        }


        IQueryable<OutgoingSmsMessage> IDataStore.OutgoingMessages
        {
            get { return OutgoingMessages; }
        }

        void IDataStore.Save(OutgoingSmsMessage msg)
        {
            if (!OutgoingMessages.Any(m => m.Id == msg.Id))
            {
                OutgoingMessages.Add(msg);
            }

            SaveChanges();
        }

        void IDataStore.RemoveAllForPhone(string phone)
        {
            foreach (OutgoingSmsMessage msg in OutgoingMessages.Where(m => m.Destination == phone))
            {
                OutgoingMessages.Remove(msg);
            }

            foreach (Subscription sub in Subscriptions.Where(s => s.Phone == phone))
            {
                Subscriptions.Remove(sub);
            }

            SaveChanges();
        }

        void IDataStore.Remove(OutgoingSmsMessage message)
        {
            if (message != null)
            {
                foreach (OutgoingSmsMessage msg in OutgoingMessages.Where(m => m.Id == message.Id))
                {
                    OutgoingMessages.Remove(msg);
                }

                SaveChanges();
            }
        }


        void IDataStore.Save(IEnumerable<OutgoingSmsMessage> block)
        {
            foreach (OutgoingSmsMessage msg in block)
            {
                ((IDataStore)this).Save(msg);
            }
        }


        void IDataStore.Remove(DailyIdea lookup)
        {
            foreach (DailyIdea i in DailyIdeas.Where(l => l.Id == lookup.Id))
            {
                DailyIdeas.Remove(i);
            }

            SaveChanges();
        }


        void IDataStore.ScheduleMessage(OutgoingSmsMessage msg, Subscription sub)
        {
            OutgoingMessages.Add(msg);
            Subscription toUpdate = Subscriptions.Where(s => s.Id == sub.Id).FirstOrDefault();
            if (toUpdate != null)
            {
                toUpdate.Next = DateTime.Today.AddDays(1).AddHours(4).ToUniversalTime();
            }

            SaveChanges();
        }


        IQueryable<LogItem> IDataStore.LogItems
        {
            get { return LogItems; }
        }

        void IDataStore.Save(LogItem log)
        {
            LogItems.Add(log);
            SaveChanges();
        }

        IQueryable<BlockedNumber> IDataStore.BlockedNumbers
        {
            get { return BlockedNumbers; }
        }

        void IDataStore.Save(BlockedNumber number)
        {
            if (!BlockedNumbers.Any(bn => bn.Id == number.Id))
            {
                BlockedNumbers.Add(number);
            }

            SaveChanges();
        }


        void IDataStore.Remove(IEnumerable<BlockedNumber> blocks)
        {
            foreach (BlockedNumber bn in blocks)
            {
                BlockedNumbers.Remove(bn);
            }

            SaveChanges();
        }


        IQueryable<IncomingMessageLog> IDataStore.IncomingMessageLogs
        {
            get
            {
                return IncomingMessageLogs;
            }
        }

        void IDataStore.Add(IncomingMessageLog msg)
        {
            IncomingMessageLogs.Add(msg);
            SaveChanges();
        }


        IQueryable<SentSmsMessageLogEntry> IDataStore.SentSmsMessageLogEntries
        {
            get { return SentSmsMessageLogEntries; }
        }

        void IDataStore.Save(SentSmsMessageLogEntry entry)
        {
            SentSmsMessageLogEntries.Add(entry);
            SaveChanges();
        }
    }
}
