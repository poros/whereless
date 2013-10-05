using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace whereless
{
    [ValueConversion(typeof(ulong), typeof(String))]
    class StreakToStringConverter : IValueConverter
    {

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ulong dt = (ulong)value;

            int days = (int)(dt / (1000 * 60 * 60 * 24));
            int hours = (int)((dt / (1000 * 60 * 60)) % 24);
            int minutes = (int)((dt / (1000 * 60)) % 60);
            int seconds = (int)((dt / 1000) % 60);
            int milliseconds = (int)(dt % 1000);


            if (days != 0)
            {
                return (days + "d " + hours + "h " + minutes + "m ");
            }
            else
            {
                if (hours != 0)
                {
                    return (hours + "h " + minutes + "m ");
                }
                
                //if (minutes != 0)
                //{
                    return (minutes + "m" );
                //}

            }
        }
    }
}
