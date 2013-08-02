//using System;

//namespace whereless.ViewModel
//{
//    public interface IServiceAgent
//    {
//        void GetPeople(EventHandler<GetPeopleCompletedEventArgs> callback);
//        void UpdatePerson(Person p, EventHandler<UpdatePersonCompletedEventArgs> callback);
//    }

//    public class ServiceAgent : IServiceAgent
//    {
//        public void GetPeople(EventHandler<GetPeopleCompletedEventArgs> callback)
//        {
//            PeopleServiceClient proxy = new PeopleServiceClient();
//            proxy.GetPeopleCompleted += callback;
//            proxy.GetPeopleAsync();
//        }

//        public void UpdatePerson(Person p, EventHandler<UpdatePersonCompletedEventArgs> callback)
//        {
//            PeopleServiceClient proxy = new PeopleServiceClient();
//            proxy.UpdatePersonCompleted += callback;
//            proxy.UpdatePersonAsync(p);
//        }
//    }
//}
