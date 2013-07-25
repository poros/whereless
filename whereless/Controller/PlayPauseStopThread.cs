//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;

//namespace whereless.Controller
//{
//    public class PlayPauseStopThread
//    {
//        public enum ThreadState
//        {
//            Playing,
//            Paused,
//            Stopped
//        }

//        private readonly Thread _thread;
//        private ThreadState _state;
//        private readonly AutoResetEvent _Stop = new AutoResetEvent(false);
//        private readonly AutoResetEvent _Pause = new AutoResetEvent(false);
//        private readonly AutoResetEvent _Play = new AutoResetEvent(false);

//        public PlayPauseStopThread(ThreadStart threadStart)
//        {
//            _state = ThreadState.Playing;
//            _thread = new Thread(threadStart);
//            _thread.Start();
//        }


//    }
//}
