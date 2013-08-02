//using System.Collections.ObjectModel;

//namespace whereless.ViewModel
//{
//    public class PeopleViewModel : ViewModelBase
//    {
//        IServiceAgent _ServiceAgent;
//        Person _Person;
//        ObservableCollection<Person> _People;

//        public PeopleViewModel() : this(new ServiceAgent()) {}

//        public PeopleViewModel(IServiceAgent serviceAgent)
//        {
//            if (!IsDesignTime)
//            {
//                _ServiceAgent = serviceAgent;
//                GetPeople();
//            }
//        }

//        #region Properties

//        public Person Person
//        {
//            get
//            {
//                return _Person;
//            }
//            set
//            {
//                if (_Person != value)
//                {
//                    _Person = value;
//                    OnNotifyPropertyChanged("Person");
//                }
//            }
//        }

//        public ObservableCollection<Person> People {
//            get
//            {
//                return _People;
//            }
//            set
//            {
//                if (_People != value)
//                {
//                    _People = value;
//                    OnNotifyPropertyChanged("People");
//                }
//            }

//        }

//        #endregion        
        
//        public void GetPeople()
//        {
//            _ServiceAgent.GetPeople((s,e) => this.People = e.Result);
//        }

//        public void UpdatePerson()
//        {
//            _ServiceAgent.UpdatePerson(this.Person, UpdatePerson_Completed);
//        }

//        void UpdatePerson_Completed(object sender, UpdatePersonCompletedEventArgs e)
//        {
//            PeopleEventBus.OnOperationCompleted(this, new OperationCompletedEventArgs { OperationStatus = e.Result });
//        }
//    }
//}
