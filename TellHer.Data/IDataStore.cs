using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TellHer.Data
{
    public interface IDataStore
    {
        IQueryable<Subscription> Subscriptions { get; }
        IQueryable<DailyIdea> DailyIdeas { get; }
        IQueryable<OutgoingSmsMessage> OutgoingMessages { get; }
        IQueryable<LogItem> LogItems { get; }
        IQueryable<BlockedNumber> BlockedNumbers { get; }
        IQueryable<IncomingMessageLog> IncomingMessageLogs { get; }
        IQueryable<SentSmsMessageLogEntry> SentSmsMessageLogEntries { get; }

        void Add(IncomingMessageLog msg);
        void Save(SentSmsMessageLogEntry entry);
        void Save(DailyIdea idea);
        void Save(Subscription sub);
        void Save(OutgoingSmsMessage msg);
        void Save(IEnumerable<OutgoingSmsMessage> block);
        void Save(LogItem log);
        void Save(BlockedNumber number);

        void Remove(Subscription sub);
        void RemoveAllForPhone(string phone);
        void Remove(OutgoingSmsMessage msg);

        void Remove(DailyIdea lookup);

        void ScheduleMessage(OutgoingSmsMessage msg, Subscription sub);

        void Remove(IEnumerable<BlockedNumber> blocks);
    }
}
