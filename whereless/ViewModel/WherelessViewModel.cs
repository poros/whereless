using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using log4net;
using System.Linq;
using whereless.LocalizationService;
using whereless.Model;
using whereless.Model.Entities;


namespace whereless.ViewModel
{
    public class WherelessViewModel : ViewModelBase
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(WherelessViewModel));
        private static readonly WherelessViewModel Instance = new WherelessViewModel();

        private WherelessViewModel()
        {
            // Current Location is defaulted to Unknown. It will be updated by the LocationService
            _currentLocation = ModelHelper.EntitiesFactory.CreateLocation("Unknown");
            _currentLocation.TotalTime = 0;
            _locations = ModelHelper.GetLocationRepository().GetAll();

        }

        static public WherelessViewModel GetInstance()
        {
            return Instance;
        }

        public ServiceController WherelessService { get; set; }

        private bool _servicePaused = false;
        private bool _radioOff = false;
        private Location _currentLocation;
        private IList<Location> _locations;



        public enum WindowsStat : int
        {
            Known,
            Unknown,
            Paused,
            Radiooff
        }

        private string _statusForWindow;
        public string StatusForWindow
        {
            get { return _statusForWindow; }
            set { _statusForWindow = value; }
        }


        public Location CurrentLocation
        {
            get { return _currentLocation; }
            set
            {
                // Time update will work???
                // if (_currentLocation != value)
                _currentLocation = value;

                setWindowStatus(value.Name.Equals("UNKNOWN") == true ? WindowsStat.Unknown : WindowsStat.Known);
                OnPropertyChanged("CurrentLocation");
                OnPropertyChanged("StatusForWindow");
            }
        }

        public IList<Location> Locations
        {
            get { return _locations; }
            set
            {
                _locations = value;
                OnPropertyChanged("Locations");
            }
        }

        public bool RadioOff
        {
            get { return _radioOff; }
            set
            {
                if (_radioOff != value)
                {
                    _radioOff = value;
                    setWindowStatus(WindowsStat.Radiooff);
                    OnPropertyChanged("RadioOff");
                    OnPropertyChanged("StatusForWindow");
                }
            }
        }

        public bool ServicePaused
        {
            get { return _servicePaused; }
            set
            {
                if (_servicePaused != value)
                {
                    _servicePaused = value;
                    setWindowStatus(WindowsStat.Paused);
                    OnPropertyChanged("ServicePaused");
                    OnPropertyChanged("StatusForWindow");
                }
            }
        }


        // REMARK Called by UI
        public void PauseService()
        {
            if (!ServicePaused)
            {
                Debug.Assert(WherelessService != null, "WherelessService != null");
                WherelessService.Pause();
                ServicePaused = true;
            }
        }

        // REMARK Called by UI
        public void PlayService()
        {
            if (ServicePaused)
            {
                Debug.Assert(WherelessService != null, "WherelessService != null");
                WherelessService.Play();
                ServicePaused = false;
            }
        }

        // REMARK Called by UI
        public void RegisterLocation(string name)
        {
            WherelessService.RegisterLocation(name);
        }

        // REMARK Called by UI
        public void ForceLocation(string name)
        {
            WherelessService.ForceLocation(name);
        }

        // REMARK Called by UI
        public void ForceUnknown()
        {
            WherelessService.ForceUnknown();
        }

        // REMARK Use it in the LocationService as delegate value
        public void UpdateRadioOff(bool off)
        {
            RadioOff = off;
            Log.Debug(off ? "Radio state changed: off" : "Radio state changed: on");
        }

        // REMARK Use it in the LocationService as delegate value
        public void UpdateCurrentLocation(Location location)
        {
            CurrentLocation = location;
            Log.Debug("ViewModel Current Location updated: " + location);
        }

        // REMARK Remember to trigger it when update the collection by the ui (like add or remove one)
        // ObservableLocation is not used because we do not own the collection or refer to it directly,
        // we need to trigger a change into the database each time. The way to do it with ObservableCollection
        // would be to make each entity implement INotifyPropertyChanged, but there are issues with Lazy Loading
        // (NOTE is not enabled right now!!!). But I think is simplest to do like this, even if not efficient.
        // Reload everything once the UI trigger a change

        // REMARK now is synchronous. Is it better to do it asynchronously?
        // I don't think so, at least in the constructor
        public void UpdateLocations()
        {
            Locations = ModelHelper.GetLocationRepository().GetAll();
        }


        /*****ASYNC TASK*****/


        public class AddActivityToLocationInfo
        {
            public string locationName;
            public string activityName;
            public string pathfile;
            public string argument;
            public string activityType;

            public AddActivityToLocationInfo(string locationName, string activityName, string pathfile, string argument,
                                             string activityType)
            {
                this.locationName = locationName;
                this.activityName = activityName;
                this.pathfile = pathfile;
                this.argument = argument;
                this.activityType = activityType;
            }
        }

        public void AddActivityToLocationCallback(Object toCastInfo)
        {
            var info = (AddActivityToLocationInfo)toCastInfo;
            var a = ModelHelper.EntitiesFactory.CreateActivity(info.activityName, info.pathfile, info.argument, info.activityType);
            using (var uow = ModelHelper.GetUnitOfWork())
            {
                var location = uow.GetLocationByName(info.locationName);
                location.AddActivity(a);

                uow.Commit();

                Log.Debug("Activity added to Location:" + location);
            }
        }

        public void AddActivityToLocation(Location location, string activityName, string pathfile, string argument, string activityType)
        {
            var a = ModelHelper.EntitiesFactory.CreateActivity(activityName, pathfile, argument, activityType);

            location.AddActivity(a);
            var rep = ModelHelper.GetRepository<Location>();
            rep.Update(location);

            Log.Debug("Activity added to Location:" + location);
        }

        public void AddActivityToLocation(string locationName, string activityName, string pathfile, string argument, string activityType)
        {
            var info = new AddActivityToLocationInfo(
                locationName, activityName, pathfile, argument, activityName);
            ThreadPool.QueueUserWorkItem(new WaitCallback(AddActivityToLocationCallback), info);
        }

        public class DeleteActivityFromLocationInfo
        {
            public string locationName;
            public int activityId;

            public DeleteActivityFromLocationInfo(string locationName, int activityId)
            {
                this.locationName = locationName;
                this.activityId = activityId;
            }
        }

        public void DeleteActivityFromLocationCallback(Object toCastInfo)
        {
            var info = (DeleteActivityFromLocationInfo) toCastInfo;
            using (var uow = ModelHelper.GetUnitOfWork())
            {
                Location loc = uow.GetLocationByName(info.locationName);

                bool removed = false;
                for (int i = 0; i < loc.ActivityList.Count && !removed; i++)
                {
                    if (loc.ActivityList[i].Id == info.activityId)
                    {
                        loc.ActivityList.RemoveAt(i);
                        removed = true;
                    }
                }
                Debug.Assert(removed, "No activity was removed");
                uow.Commit();
            }

            // To trigger the interface update (sure, this interface is a mess...)
            //UpdateCurrentLocation(CurrentLocation);
        }

        public void DeleteActivityFromLocation(string locationName, int activityId)
        {
            DeleteActivityFromLocationInfo info = new DeleteActivityFromLocationInfo(
                locationName, activityId);
            ThreadPool.QueueUserWorkItem(new WaitCallback(DeleteActivityFromLocationCallback), info);
        }

        public class DeleteLocationInfo
        {
            public string locationName;
        
            public DeleteLocationInfo(string locationName)
            {
                this.locationName = locationName;
            }
        }

        public void DeleteLocationCallback(Object toCastInfo)
        {
            bool paused = false;
            var info = (DeleteLocationInfo)toCastInfo;
            if (info.locationName.Equals(CurrentLocation.Name))
            {
                ForceUnknown();
                PauseService();
                paused = true;
            }
            using (var uow = ModelHelper.GetUnitOfWork())
            {
                Location loc = uow.GetLocationByName(info.locationName);
                if (loc != null)
                {
                    uow.Delete(loc);
                }
                uow.Commit();
            }
            if (paused)
            {
                PlayService();
                ForceUnknown();
            }
        }

        public void DeleteLocation(string locationName)
        {
            DeleteLocationInfo info = new DeleteLocationInfo(locationName);
            ThreadPool.QueueUserWorkItem(new WaitCallback(DeleteLocationCallback), info);
        }


        public void setWindowStatus(WindowsStat ws)
        {
            switch (ws)
            {
                case WindowsStat.Known:
                    StatusForWindow = "KNOWN";
                    break;

                case WindowsStat.Unknown:
                    StatusForWindow = "UNKNOWN";
                    break;
                case WindowsStat.Paused:
                    StatusForWindow = "PAUSED";
                    break;
                case WindowsStat.Radiooff:
                    StatusForWindow = "RADIOOFF";
                    break;
            }
        }

    }
}
