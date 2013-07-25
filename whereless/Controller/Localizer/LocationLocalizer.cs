using log4net;
using System.Threading;
using whereless.Controller.WiFi;

namespace whereless.Controller.Localizer
{
    class LocationLocalizer
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(LocationLocalizer));

        private readonly WaitHandle[] _threadControls = new WaitHandle[3];
        private readonly WaitHandle _play;
        private readonly SensorToLocalizer<SensorOutput> _inputQueue;
        private SensorOutput _input;

        public LocationLocalizer(WaitHandle stopThread, WaitHandle pauseThread, WaitHandle playThread, SensorToLocalizer<SensorOutput> input)
        {
            _inputQueue = input;
            _threadControls[0] = stopThread;
            _threadControls[1] = pauseThread;
            _threadControls[2] = input.FullHandle;
            _play = playThread;
        }

        public void LocationLocalizerLoop()
        {
            while (true)
            {
                int handle = WaitHandle.WaitAny(_threadControls);
                if (handle == 0)
                {
                    break;

                }
                else if (handle == 1)
                {
                    Log.Debug("LocationLocalizer thread paused");
                    // Wait until the play event is fired
                    _play.WaitOne();
                    Log.Debug("LocationLocalizer thread played");
                }
                else if (handle == 2)
                {
                    // false -> _inputQueue is closed
                    // WiFiSensor has been terminated after the previous waitAny call
                    // and program is going to be terminated, too
                    // next loop will take care of this
                    if (_inputQueue.Take(out _input))
                    {
                        // TODO call algorithm
                        Log.Debug(_input.ToString());
                    }
                }
            }
            Log.Debug("LocationLocalizer thread has been stopped");
        }
    }
}
