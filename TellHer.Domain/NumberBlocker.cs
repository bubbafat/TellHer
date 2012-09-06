using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TellHer.Data;


namespace TellHer.Domain
{
    public static class NumberBlocker
    {
        public static bool IsBlocked(string number)
        {
            IDataStore store = DataStore.GetInstance();
            return store.BlockedNumbers.Any(bn => bn.Phone == number && bn.Expires > DateTime.UtcNow);
        }

        public static void Block(string number, int days)
        {
            IDataStore store = DataStore.GetInstance();

            BlockedNumber bn = new BlockedNumber
            {
                Phone = number,
                Blocked = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddDays(days),
            };

            store.Save(bn);

            // now update any subscriptions...
            IList<Subscription> subs = store.Subscriptions.Where(s => s.Phone == number).ToList();
            foreach (Subscription sub in subs)
            {
                if (sub.Next < bn.Expires)
                {
                    sub.Next = bn.Expires;
                    store.Save(sub);
                }
            }
        }

        public static void Unblock(string number)
        {
            IDataStore store = DataStore.GetInstance();
            IList<BlockedNumber> blocks = store.BlockedNumbers.Where(bn => bn.Phone == number).ToList();
            store.Remove(blocks);

            // and set subscriptions to fire now
            IList<Subscription> subs = store.Subscriptions.Where(s => s.Phone == number).ToList();
            foreach (Subscription sub in subs)
            {
                sub.Next = DateTime.UtcNow;
                store.Save(sub);
            }
        }
    }
}
