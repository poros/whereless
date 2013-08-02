using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace whereless.ViewModel
{
    public class ViewModelBase : INotifyPropertyChanged
    {

        //    public bool IsDesignTime
        //    {
        //        get
        //        {
        //            return (Application.Current == null) || (Application.Current.GetType() == typeof(Application));
        //        }
        //    }


        //    public event PropertyChangedEventHandler PropertyChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}