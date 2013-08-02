using System.Collections.Generic;
using System.Text;
using whereless.LocalizationService.WiFi;
using whereless.Model.ValueObjects;

namespace whereless.Model.Entities
{
    public abstract class Location
    {
        // factory for creating child entities 
        protected static readonly ulong T = (ulong) WiFiSensor.ScanTime;
        private string _name;

        public virtual int Id { get; protected set; }
        public virtual string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public virtual ulong Time { get; set; }

        private IList<Activity> _activityList;

        public virtual IList<Activity> ActivityList
        {
            get { return _activityList; }
            set { _activityList = value; }
        }

        //NHibernate specific in case of double-side reference
        public virtual void AddActivity(Activity activity)
        {
            _activityList.Add(activity);
            //activity.LocationReference = this;
        }


        protected Location()
        {
            _activityList = new List<Activity>();
        }

        protected Location(string name)
        {
            _name = name;
            _activityList = new List<Activity>();
        }

        // test if the input measures are compatible with the location
        public abstract bool TestInput(IList<IMeasure> measures);
        // update statistics of the location with the input measures
        public abstract void UpdateStats(IList<IMeasure> measures);

        public virtual void StartActivities()
        {
            foreach (var activity in _activityList)
            {
                activity.Start();
            }
        }

        public virtual void StopActivities()
        {
            foreach (var activity in _activityList)
            {
                activity.Stop();
            }
        }
        
        public override string ToString()
        {
            StringBuilder buffer = new StringBuilder();
            foreach (var activity in ActivityList)
            {
                buffer.AppendLine(activity.ToString());
            }
            return (this.GetType().Name + ": " + "Name = " + Name + "; Time = " + Time + "; Activities = {\n" + buffer + "};");
        }
    }
}
