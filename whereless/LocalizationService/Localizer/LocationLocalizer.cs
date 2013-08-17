using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using log4net;
using System.Threading;
using whereless.LocalizationService.WiFi;
using whereless.Model;
using whereless.Model.Entities;
using whereless.Model.ValueObjects;

namespace whereless.LocalizationService.Localizer
{
    public class LocationLocalizer
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(LocationLocalizer));

        public const string Unknown = "UNKNOWN";
        private Location _currLocation = null;
        private bool _unknown = false;
        private readonly LocalizationAlgorithm _algorithm;

        private readonly WaitHandle[] _threadControls = new WaitHandle[3];
        private readonly WaitHandle _play;
        private readonly SensorToLocalizer<SensorOutput> _inputQueue;
        private SensorOutput _sensorOutput;
        private IList<IMeasure> _input = null; 

        // Vars for updateCurrentLocation callback
        public delegate void UpdateCurrentLocationCallbackDelegate(Location currLocation);
        private UpdateCurrentLocationCallbackDelegate _updateCurrentLocationCallback;
        public UpdateCurrentLocationCallbackDelegate UpdateCurrentLocationCallback
        {
            get { return _updateCurrentLocationCallback; }
            set { _updateCurrentLocationCallback += value; }
        }


        public LocationLocalizer(WaitHandle stopThread, WaitHandle pauseThread, WaitHandle playThread,
                                 SensorToLocalizer<SensorOutput> input, LocalizationAlgorithm algorithm)
        {
            _inputQueue = input;
            _algorithm = algorithm;

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
                    if (_inputQueue.Take(out _sensorOutput))
                    {

                        Log.Debug(_sensorOutput.ToString());

                        // Other threads can access LocationLocalizer in order to change
                        // to update currLocation (forceLocation, registerLocation)
                        lock (this)
                        {
                            _input = _sensorOutput.Measures;

                            // first time initialization
                            if (_currLocation == null)
                            {
                                //Initialize(_input, ref _currLocation, ref _unknown);
                                _algorithm.Initialize(_input, ref _currLocation, ref _unknown);
                            }
                            // normal execution
                            else
                            {
                                //Algorithm(_input, ref _currLocation, ref _unknown);
                                _algorithm.Localize(_input, ref _currLocation, ref _unknown);
                            }

                            Debug.Assert(_currLocation != null, "location != null");
                            UpdateCurrentLocationCallback.Invoke(_currLocation);
                        }
                    }
                }
            }
            Log.Debug("LocationLocalizer thread has been stopped");
        }


        public void ForceLocation(string name)
        {
            lock (this)
            {
                Log.Debug("Force Location called");
                _algorithm.ForceLocation(name, _input, ref _currLocation, ref _unknown);
                UpdateCurrentLocationCallback(_currLocation);
            }
        }

        public void ForceUnknown()
        {
            lock (this)
            {
                Log.Debug("Force Unknown called");
                _algorithm.ForceUnknown(_input, ref _currLocation, ref _unknown);
                UpdateCurrentLocationCallback(_currLocation);
            }
        }

        public void RegisterLocation(string name)
        {
            lock (this)
            {
                Log.Debug("Register Location called");
                _algorithm.RegisterLocation(name, _input, ref _currLocation, ref _unknown);
                UpdateCurrentLocationCallback(_currLocation);
            }
        }
       
    }
}
