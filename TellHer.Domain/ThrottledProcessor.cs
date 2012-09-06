using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;

namespace TellHer.Domain
{
    public delegate void ThrottledProcessorFailedCallback<T>(T value, Exception exception);

    public class ThrottledProcessor<T> : IDisposable
    {
        private readonly ConcurrentQueue<T> _queue;
        private readonly Timer _timer;
        private volatile int _callbacksInFlight = 0;
        private object _inTimerCallbackLock = new object();

        public ThrottledProcessor(int msBetween, Action<T> callback, Action<T> successCallback, ThrottledProcessorFailedCallback<T> failedCallback)
        {
            _queue = new ConcurrentQueue<T>();
            
            TimerConfig<T> config = new TimerConfig<T>
            {
                Callback = callback,
                SuccessCallback = successCallback,
                FailedCallback = failedCallback,
            };

            _timer = new Timer(OnTimerFired, config, 0, msBetween);
        }

        private void OnTimerFired(object timerConfig)
        {
            lock (_inTimerCallbackLock)
            {
                T callbackParam;
                if (_queue.TryDequeue(out callbackParam))
                {
                    TimerConfig<T> config = (TimerConfig<T>)timerConfig;

                    try
                    {
                        _callbacksInFlight++;

                        bool runComplete = false;
                        Exception loggedException = null;
                        try
                        {
                            config.Callback(callbackParam);
                            runComplete = true;
                        }
                        catch (Exception ex)
                        {
                            LogManager.Log.Error("Error in Throttled Processor Action", ex);
                            loggedException = ex;
                        }

                        try
                        {
                            if (runComplete)
                            {
                                if (config.SuccessCallback != null)
                                {
                                    config.SuccessCallback(callbackParam);
                                }
                            }
                            else
                            {
                                
                                if (config.FailedCallback != null)
                                {
                                    config.FailedCallback(callbackParam, loggedException);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            LogManager.Log.Error("Error in Throttled Processor Reporting Callback", ex);
                        }
                    }
                    finally
                    {
                        _callbacksInFlight--;
                    }
                }
            }
        }

        public void Add(T value)
        {
            ThrowIfDisposed();
            _queue.Enqueue(value);
        }

        public int Length
        {
            get
            {
                ThrowIfDisposed();
                lock (_inTimerCallbackLock)
                {
                    return _queue.Count + _callbacksInFlight;
                }
            }
        }

        internal class TimerConfig<TCallbackParam>
        {
            public Action<TCallbackParam> Callback { get; set; }
            public Action<TCallbackParam> SuccessCallback { get; set; }
            public ThrottledProcessorFailedCallback<TCallbackParam> FailedCallback { get; set; }
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException("ThrottledProcessor");
            }
        }

        bool _disposed = false;

        public void Dispose()
        {
            if (!_disposed)
            {
                // wait until the current callback is done
                lock (_inTimerCallbackLock)
                {
                    if (_timer != null)
                    {
                        // disable the timer
                        _timer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                    }

                    // and now dispose it
                    using (_timer) { }
                }

                _disposed = true;
                GC.SuppressFinalize(this);
            }
        }
    }
}
