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

        public const string Unknown = "UNKNWON";
        private Location _currLocation = null;
        private bool _unknown = false;

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
                                 SensorToLocalizer<SensorOutput> input)
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
                                Initialize(_input, ref _currLocation, ref _unknown);
                            }
                            // normal execution
                            else
                            {
                                Algorithm(_input, ref _currLocation, ref _unknown);
                            }

                            Debug.Assert(_currLocation != null, "location != null");
                            UpdateCurrentLocationCallback.Invoke(_currLocation);
                        }
                    }
                }
            }
            Log.Debug("LocationLocalizer thread has been stopped");
        }


        private static void Initialize(IList<IMeasure> input, ref Location currLocation, ref bool unknown)
        {
            using (var uow = ModelHelper.GetUnitOfWork())
            {
                var locations = uow.GetAll<Location>();
                if (locations.Count > 0)
                {
                    foreach (var location in locations)
                    {
                        if (location.TestInput(input))
                        {
                            currLocation = location;
                            location.UpdateStats(input);
                            break;
                        }
                    }
                }
                uow.Commit();
            }
            
            if (currLocation == null)
            {
                currLocation = ModelHelper.EntitiesFactory
                                      .CreateLocation(Unknown, input);
                unknown = true;
            }
        }

        private static void Algorithm(IList<IMeasure> input, ref Location currLocation, ref bool unknown)
        {
            using (var uow = ModelHelper.GetUnitOfWork())
            {
                // currLocation may be updated by other threads
                if (!unknown)
                {
                    Log.Debug("Trying to retrieve Location" + currLocation.Name);
                    currLocation = uow.GetLocationByName(currLocation.Name);
                    Log.Debug(currLocation == null ? "Nothing Retrieved" : "Retrieved" + currLocation.Name);
                }

                // currLocation may be null if updated by others threads
                //it could also be unknown
                // proximity bias
                if (currLocation != null && currLocation.TestInput(input))
                {
                    currLocation.UpdateStats(input);
                }
                else
                {
                    Location oldLocation = currLocation;
                    currLocation = null;

                    var locations = ModelHelper.GetLocationRepository().GetAll();
                    if (locations.Count > 0)
                    {
                        foreach (var location in locations
                            .Where(location => location != oldLocation))
                        {
                            if (location.TestInput(input))
                            {
                                unknown = false;
                                currLocation = location;
                                location.UpdateStats(input);
                                break;
                            }
                        }
                    }
                    if (currLocation == null)
                    {
                        currLocation = ModelHelper.EntitiesFactory
                                                  .CreateLocation(Unknown, input);
                        unknown = true;
                    }
                }
                uow.Commit();
            }
        }

        public void ForceLocation(string name)
        {
            Log.Debug("Force Location called");
            lock (this)
            {
                using (var uow = ModelHelper.GetUnitOfWork())
                {
                    _currLocation = uow.GetLocationByName(name);
                    Debug.Assert(_currLocation != null, "currLocation != null");
                    _currLocation.ForceLocation(_input);
                    _unknown = false;
                    uow.Commit();
                }
                UpdateCurrentLocationCallback(_currLocation);
            }
        }

        public void ForceUnknown()
        {
            lock (this)
            {
                Log.Debug("Force Unknown called");
                using (var uow = ModelHelper.GetUnitOfWork())
                {
                    _currLocation = ModelHelper.EntitiesFactory
                                      .CreateLocation(Unknown, _input);
                    _unknown = true;
                    uow.Commit();
                }
                UpdateCurrentLocationCallback(_currLocation);
            }
        }

        public void RegisterLocation(string name)
        {
            lock (this)
            {
                Log.Debug("Register Location called");
                using (var uow = ModelHelper.GetUnitOfWork())
                {
                    if (!_unknown)
                    {
                        _currLocation = ModelHelper.EntitiesFactory
                                                   .CreateLocation(name, _input);
                    } 
                    else
                    {
                        _currLocation.Name = name;
                        _unknown = false;
                    }
                    uow.Save(_currLocation);
                    uow.Commit();
                }
                UpdateCurrentLocationCallback(_currLocation);
            }
        }
       

    }
}
