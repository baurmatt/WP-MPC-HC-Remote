using System;
using System.Windows.Data;

namespace MPC_Remote
{
    public class PlayPauseButtonConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (int)value == 2 ? "appbar.transport.pause.rest.png" : "appbar.transport.play.rest.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
