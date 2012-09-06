using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TellHer.Domain
{
    public interface ISubscriptionService
    {
        void Add(IncomingSmsMessage message);
    }
}
