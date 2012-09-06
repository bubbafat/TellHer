using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TellHer.Data;

namespace TellHer.Test.Common
{
    public class TestDataStore : IDataStore
    {
        static Dictionary<int, Subscription> _subscriptions = new Dictionary<int, Subscription>();
        static Dictionary<int, DailyIdea> _dailyIdeas = new Dictionary<int, DailyIdea>();
        static Dictionary<int, BlockedNumber> _blockedNumbers = new Dictionary<int, BlockedNumber>();
        static Dictionary<int, LogItem> _logItems = new Dictionary<int, LogItem>();
        static Dictionary<int, OutgoingSmsMessage> _outgoingMessages = new Dictionary<int, OutgoingSmsMessage>();
        static Dictionary<int, IncomingMessageLog> _incomingMessageLogs = new Dictionary<int, IncomingMessageLog>();
        static Dictionary<int, SentSmsMessageLogEntry> _sentSmsMessageLogEntries = new Dictionary<int, SentSmsMessageLogEntry>();

        static int _globalId = 0;
        static readonly object _lock = new object();

        public static void Reset()
        {
            lock (_lock)
            {
                _subscriptions.Clear();
                _dailyIdeas.Clear();
                _blockedNumbers.Clear();
                _logItems.Clear();
                _outgoingMessages.Clear();
                _incomingMessageLogs.Clear();
                _sentSmsMessageLogEntries.Clear();
            }
        }

        static private int NextId()
        {
            _globalId++;
            return _globalId;
        }

        public IQueryable<Subscription> Subscriptions
        {
            get
            {
                lock (_lock)
                { return _subscriptions.Values.AsQueryable(); }
            }
        }

        public IQueryable<DailyIdea> DailyIdeas
        {
            get
            {
                lock (_lock)
                { return _dailyIdeas.Values.AsQueryable(); }
            }
        }

        public IQueryable<OutgoingSmsMessage> OutgoingMessages
        {
            get
            {
                lock (_lock)
                { return _outgoingMessages.Values.AsQueryable(); }
            }
        }

        public IQueryable<LogItem> LogItems
        {
            get
            {
                lock (_lock)
                { return _logItems.Values.AsQueryable(); }
            }
        }

        public IQueryable<BlockedNumber> BlockedNumbers
        {
            get
            {
                lock (_lock)
                { return _blockedNumbers.Values.AsQueryable(); }
            }
        }

        IQueryable<IncomingMessageLog> IDataStore.IncomingMessageLogs
        {
            get
            {
                lock (_lock)
                { return _incomingMessageLogs.Values.AsQueryable(); }
            }
        }

        void IDataStore.Add(IncomingMessageLog msg)
        {
            lock (_lock)
            {
                if (msg.Id == 0)
                {
                    msg.Id = TestDataStore.NextId();
                }

                _incomingMessageLogs[msg.Id] = msg;
            }
        }

        public void Save(DailyIdea idea)
        {
            lock (_lock)
            {
                if (idea.Id == 0)
                {
                    idea.Id = NextId();
                }

                _dailyIdeas[idea.Id] = idea;
            }
        }

        public void Save(Subscription sub)
        {
            lock (_lock)
            {
                if (sub.Id == 0)
                {
                    sub.Id = NextId();
                }

                _subscriptions[sub.Id] = sub;
            }
        }

        public void Save(OutgoingSmsMessage msg)
        {
            lock (_lock)
            {
                if (msg.Id == 0)
                {
                    msg.Id = NextId();
                }

                _outgoingMessages[msg.Id] = msg;
            }
        }

        public void Save(IEnumerable<OutgoingSmsMessage> block)
        {
            foreach (OutgoingSmsMessage msg in block)
            {
                Save(msg);
            }
        }

        public void Save(LogItem log)
        {
            lock (_lock)
            {
                if (log.ID == 0)
                {
                    log.ID = NextId();
                }

                _logItems[log.ID] = log;
            }
        }

        public void Save(BlockedNumber number)
        {
            lock (_lock)
            {
                if (number.Id == 0)
                {
                    number.Id = NextId();
                }

                _blockedNumbers[number.Id] = number;
            }
        }

        public void Remove(Subscription sub)
        {
            lock (_lock)
            {
                _subscriptions.Remove(sub.Id);
            }
        }

        public void RemoveAllForPhone(string phone)
        {
            lock (_lock)
            {
                IList<OutgoingSmsMessage> outgoing = OutgoingMessages.Where(m => m.Destination == phone).ToList();
                foreach (OutgoingSmsMessage msg in outgoing)
                {
                    Remove(msg);
                }

                IList<Subscription> subs = Subscriptions.Where(s => s.Phone == phone).ToList();
                foreach (Subscription sub in subs)
                {
                    Remove(sub);
                }
            }
        }

        public void Remove(OutgoingSmsMessage msg)
        {
            lock (_lock)
            {
                _outgoingMessages.Remove(msg.Id);
            }
        }

        public void Remove(DailyIdea lookup)
        {
            lock (_lock)
            {

                _dailyIdeas.Remove(lookup.Id);
            }
        }

        public void ScheduleMessage(OutgoingSmsMessage msg, Subscription sub)
        {
            lock (_lock)
            {
                Save(msg);
                Subscription toUpdate = Subscriptions.Where(s => s.Id == sub.Id).FirstOrDefault();
                if (toUpdate != null)
                {
                    toUpdate.Next = DateTime.Today.AddDays(1).ToUniversalTime().AddHours(2);
                }
            }
        }

        public void Remove(IEnumerable<BlockedNumber> blocks)
        {
            lock (_lock)
            {
                foreach (BlockedNumber bn in blocks)
                {
                    _blockedNumbers.Remove(bn.Id);
                }
            }
        }


        public IQueryable<SentSmsMessageLogEntry> SentSmsMessageLogEntries
        {
            get
            {
                lock (_lock)
                {
                    return _sentSmsMessageLogEntries.Values.AsQueryable();
                }
            }
        }

        public void Save(SentSmsMessageLogEntry entry)
        {
            lock (_lock)
            {
                if (entry.Id == 0)
                {
                    entry.Id = NextId();
                }

                _sentSmsMessageLogEntries[entry.Id] = entry;
            }
        }
    }
}
