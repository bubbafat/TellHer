using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TellHer.Domain;

using TellHer.Data;

namespace TellHer.Sms
{
    public class SmsSenderQueue : IOutgoingSmsQueue
    {
        ISmsTransport _transport = SmsTransport.GetInstance();
        IDataStore _store = DataStore.GetInstance();
        ThrottledProcessor<SmsSenderMessage> _processor;
        int _delay = 0;

        private readonly HashSet<int> queued = new HashSet<int>();
        private readonly object _lock = new object();

        public SmsSenderQueue()
        {
            _delay = _transport.DelayMs;

            _processor = new ThrottledProcessor<SmsSenderMessage>(
                _delay, 
                SendMessage, 
                SendComplete, 
                SendException);
        }

        private void SendMessage(SmsSenderMessage msg)
        {
            msg.Result = _transport.Send(msg.Message.Destination, msg.Message.Message);
            _store.Save(msg.Result);
        }

        private void SendComplete(SmsSenderMessage sms)
        {
            if (sms.Complete != null)
            {
                sms.Complete(sms.Result, sms.Message);

                lock (_lock)
                {
                    queued.Remove(sms.Message.Id);
                }
            }
        }

        private void SendException(SmsSenderMessage sms, Exception ex)
        {
            if (sms.Failed != null)
            {
                sms.Failed(sms.Message, ex);

                lock (_lock)
                {
                    queued.Remove(sms.Message.Id);
                }
            }
        }

        public void Send(OutgoingSmsMessage msg, SendCompleteCallback complete, SmsSenderExceptionThrownCallback exceptionThrown)
        {
            ThrowIfDisposed();

            lock (_lock)
            {
                if (!queued.Contains(msg.Id))
                {
                    _processor.Add(new SmsSenderMessage
                    {
                        Message = msg,
                        Complete = complete,
                        Failed = exceptionThrown,
                    });

                    queued.Add(msg.Id);
                }
            }
        }

        public int Length
        {
            get
            {
                ThrowIfDisposed();
                return _processor.Length;
            }
        }

        public int MessagesPerMinute 
        {
            get
            {
                ThrowIfDisposed();

                int msPerMinute = 60 * 1000;
                if (_delay >= msPerMinute)
                {
                    return 1;
                }

                return (int)(msPerMinute / _delay);
            }
        }

        internal class SmsSenderMessage
        {
            public OutgoingSmsMessage Message { get; set; }
            public SendCompleteCallback Complete { get; set; }
            public SmsSenderExceptionThrownCallback Failed { get; set; }
            public SentSmsMessageLogEntry Result { get; set; } 
        }

        private void ThrowIfDisposed()
        {
            if(_disposed)
            {
                throw new ObjectDisposedException("SmsSender");
            }
        }

        bool _disposed = false;
        public void Dispose()
        {
            if (!_disposed)
            {
                using (_processor) { }

                _disposed = true;

                GC.SuppressFinalize(this);
            }
        }
    }
}
