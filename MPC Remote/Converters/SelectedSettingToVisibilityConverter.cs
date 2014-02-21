using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

/// <summary>
/// A type converter for visibility and boolean values.
/// </summary>

namespace MPC_Remote
{
    public class SelectedSettingToVisibilityConverter : IValueConverter
    {
        private AppSettings AppSettings = new AppSettings();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string DefaultSetting = (string)value;
            string SelectedSetting = AppSettings.DefaultSetting;

            if (DefaultSetting == SelectedSetting)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility visibility = (Visibility)value;
            return (visibility == Visibility.Visible);
        }
    }
}