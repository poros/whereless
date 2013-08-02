using System;
using System.Threading;
using whereless.LocalizationService.Localizer;
using whereless.LocalizationService.WiFi;

namespace whereless.LocalizationService
{
    public class ServiceController
    {
        private enum ThreadState
        {
            Playing,
            Paused,
            Stopped
        }

        private readonly WiFiSensor _sensor;

        private readonly Thread _sensorThread;
        private ThreadState _sensorState;
        private readonly AutoResetEvent _sensorStop = new AutoResetEvent(false);
        private readonly AutoResetEvent _sensorPause = new AutoResetEvent(false);
        private readonly AutoResetEvent _sensorPlay = new AutoResetEvent(false);

        private readonly LocationLocalizer _localizer;

        private readonly Thread _localizerThread;
        private ThreadState _localizerState;
        private readonly AutoResetEvent _localizerStop = new AutoResetEvent(false);
        private readonly AutoResetEvent _localizerPause = new AutoResetEvent(false);
        private readonly AutoResetEvent _localizerPlay = new AutoResetEvent(false);

        private readonly SensorToLocalizer<SensorOutput> _sensorOutputLocalizerInput = 
            new SensorToLocalizer<SensorOutput>();


        public WiFiSensor.RadioOffCallbackDelegate RadioOffCallback
        {
            get { return _sensor.RadioOffDelegate; }
            set { _sensor.RadioOffDelegate = value; }
        }


        public ServiceController()
        {
            _sensor = new WiFiSensor(stopThread: _sensorStop, pauseThread: _sensorPause,
                playThread: _sensorPlay, output: _sensorOutputLocalizerInput);
            _localizer = new LocationLocalizer(stopThread: _localizerStop, pauseThread: _localizerPause,
                playThread: _localizerPlay, input: _sensorOutputLocalizerInput);
            
            _sensorState = ThreadState.Playing;
            _localizerState = ThreadState.Playing;

            _sensorThread = new Thread(new ThreadStart(_sensor.WiFiSensorLoop));
            _localizerThread = new Thread(new ThreadStart(_localizer.LocationLocalizerLoop));

            _sensorThread.IsBackground = true;
            _localizerThread.IsBackground = true;
        }


        public void Start()
        {
            _sensorThread.Start();
            _localizerThread.Start();
        }

        public void Pause()
        {
            if (_sensorState == ThreadState.Playing)
            {
                _sensorState = ThreadState.Paused;
                _sensorPause.Set();
            }

            if (_localizerState == ThreadState.Playing)
            {
                _localizerState = ThreadState.Paused;
                _localizerPause.Set();
            }
        }

        public void Play()
        {
            if (_sensorState == ThreadState.Paused)
            {
                _sensorPlay.Set();
                _sensorState = ThreadState.Playing;
            }

            if (_localizerState == ThreadState.Paused)
            {
                _localizerPlay.Set();
                _localizerState = ThreadState.Playing;
            }
        }

        public void Stop()
        {
            if (_sensorState != ThreadState.Stopped)
            {
                if (_sensorState == ThreadState.Paused)
                {
                    _sensorPlay.Set();
                }
                _sensorStop.Set();
                _sensorState = ThreadState.Stopped;
            }

            if (_localizerState != ThreadState.Stopped)
            {
                if (_localizerState == ThreadState.Paused)
                {
                    _localizerPlay.Set();
                }
                _localizerStop.Set();
                _localizerState = ThreadState.Stopped;
            }

            _sensorThread.Join();
            _localizerThread.Join();
        }

        private void Join()
        {
            if (_sensorState != ThreadState.Stopped || _localizerState != ThreadState.Stopped)
            {
                throw new InvalidOperationException("ServiceController needs to be stopped before!!!");
            }
            
        }
    }
}
