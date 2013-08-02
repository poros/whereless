using System.Collections.Generic;
using System.Collections.ObjectModel;
using log4net;
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
            _currentLocation.Time = 0;
            _locations = ModelHelper.GetLocationRepository().GetAll();
        }

        public static WherelessViewModel GetWherelessViewModel()
        {
            return Instance;
        }


        private bool _servicePaused = false;
        private bool _radioOff = false;
        private Location _currentLocation;
        private IList<Location> _locations;


        public Location CurrentLocation
        {
            get { return _currentLocation; }
            set
            {
                // Time update will work???
                // if (_currentLocation != value)
                _currentLocation = value;
                OnPropertyChanged("CurrentLocation");
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
                    OnPropertyChanged("RadioOff");
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
                    OnPropertyChanged("ServicePaused");
                }
            }
        }


        // REMARK Called by UI
        public void PauseService()
        {
            ServicePaused = true;
            // TODO pause ServiceController
        }

        // REMARK Use it in the LocationService as delegate value
        public void UpdateRadioOff(bool off)
        {
            RadioOff = off;
            if (off)
            {
                Log.Debug("Radio state changed: off");
            }
            else
            {
                Log.Debug("Radio state changed: on");
            }
        }

        // REMARK Use it in the LocationService as delegate value
        public void UpdateCurrentLocation(Location location)
        {
            CurrentLocation = location;
        }

        // REMARK Remember to trigger it when update the collection by the ui (like add or remove one)
        // ObservableLocation is not used because we do not own the collection or refer to it directly,
        // we need to trigger a change into the database each time. The way to do it with ObservableCollection
        // would be to make each entity implement INotifyPropertyChanged, but there are issues with Lazy Loading
        // (NOTE is not enabled right now!!!). But I think is simplest to do like this, even if not efficient.
        // Reload everything once the UI trigger a change

        // REMARK now is synchronous. Is it better to do it asynchronously?
        // I don0't think so at least in the constructor
        public void GetLocations()
        {
            Locations = ModelHelper.GetLocationRepository().GetAll();
        }

        //public void UpdatePerson()
        //{
        //    _ServiceAgent.UpdatePerson(this.Person, UpdatePerson_Completed);
        //}

        //void UpdatePerson_Completed(object sender, UpdatePersonCompletedEventArgs e)
        //{
        //    PeopleEventBus.OnOperationCompleted(this, new OperationCompletedEventArgs { OperationStatus = e.Result });
        //}
    }
}
