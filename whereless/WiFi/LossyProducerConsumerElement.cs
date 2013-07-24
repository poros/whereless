using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace whereless.WiFi
{
    public class LossyProducerConsumerElement<T> where T : class 
    {
        private readonly ManualResetEvent _full = new ManualResetEvent(false);
        private T _element;

        public void LossyPut(T tmp)
        {
            lock (this)
            {
                _element = tmp;
                _full.Set();
            }
        }

        public T Take()
        {
            _full.WaitOne();
            lock (this)
            {
                T tmp = _element;
                _element = null;
                _full.Reset();
                return tmp;
            }
        }
    }
}
