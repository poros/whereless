using System;
using System.Configuration;
using System.Threading;
using log4net;
using whereless.LocalizationService.Localizer;
using whereless.LocalizationService.WiFi;

namespace whereless.LocalizationService
{
    public class ServiceController
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ServiceController));

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

        private readonly LocalizationAlgorithm _localizationAlgorithm = CreateLocalizationAlgorithm();

        public WiFiSensor.RadioOffCallbackDelegate RadioOffCallback
        {
            get { return _sensor.RadioOffCallback; }
            set { _sensor.RadioOffCallback = value; }
        }

        public LocationLocalizer.UpdateCurrentLocationCallbackDelegate UpdateCurrentLocationCallback
        {
            get { return _localizer.UpdateCurrentLocationCallback; }
            set { _localizer.UpdateCurrentLocationCallback = value; }
        }

        private static LocalizationAlgorithm CreateLocalizationAlgorithm()
        {
            LocalizationAlgorithm tmp;
            string algName = ConfigurationManager.AppSettings["localizationAlgorithm"];
            if (algName == null)
            {
                throw new ConfigurationErrorsException("Unable to find localizationAlgorithm key");
            }
            if (algName.Equals("SimpleLocalization"))
            {
                tmp = new SimpleLocalization();
                Log.Debug("SimpleLocalization Algorithm Instantiated");
            }
            else
            {
                throw new ConfigurationErrorsException("Localization Aglorithm configuration value not allowed");
            }
            return tmp;
        }

        public ServiceController()
        {
            _sensor = new WiFiSensor(stopThread: _sensorStop, pauseThread: _sensorPause,
                playThread: _sensorPlay, output: _sensorOutputLocalizerInput);
            _localizer = new LocationLocalizer(stopThread: _localizerStop, pauseThread: _localizerPause,
                playThread: _localizerPlay, input: _sensorOutputLocalizerInput, algorithm: _localizationAlgorithm);

            _sensorState = ThreadState.Playing;
            _localizerState = ThreadState.Playing;

            _sensorThread = new Thread(new ThreadStart(_sensor.WiFiSensorLoop));
            _localizerThread = new Thread(new ThreadStart(_localizer.LocationLocalizerLoop));

            _sensorThread.IsBackground = true;
            _localizerThread.IsBackground = true;

            _sensorThread.Name = "WiFiSensorThread";
            _localizerThread.Name = "LocationLocalizerThread";
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

        public void ForceLocation(string name)
        {
            _localizer.ForceLocation(name);
        }

        public void ForceUnknown()
        {
            _localizer.ForceUnknown();
        }

        public void RegisterLocation(string name)
        {
            _localizer.RegisterLocation(name);
        }
    }
}
