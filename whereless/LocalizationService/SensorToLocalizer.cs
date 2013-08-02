using System.Threading;
using log4net;

namespace whereless.LocalizationService
{
    public class SensorToLocalizer<T> 
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SensorToLocalizer<T>));
        private readonly ManualResetEvent _full = new ManualResetEvent(false);
        private bool _closed = false;
        private T _element;

        public ManualResetEvent FullHandle
        {
            get { return _full; }
        }

        public bool IsClosed
        {
            get { return _closed;  }
        }

        public void Close()
        {
            lock (this)
            {
                _closed = true;
                _full.Set();
            }
        }

        public bool LossyPut(T tmp)
        {
            lock (this)
            {
                if (_closed)
                {
                    return false;
                }
                _element = tmp;
                _full.Set();
                return true;
            }
        }

        public bool Take(out T tmp)
        {
            // not necessary (maybe)
            // alternative with _close ManualResetEvent
            //var handle = WaitHandle.WaitAny(new WaitHandle[2] {_full, _close});
            //_full.WaitOne();

            lock (this)
            {
                if (!_closed)
                {
                    tmp = _element;
                    _element = default(T);
                    _full.Reset();
                    return true;
                }
                else
                {
                    tmp = default(T);
                    return false;
                }
            }
        }
    }
}
