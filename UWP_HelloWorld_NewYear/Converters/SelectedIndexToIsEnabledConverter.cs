using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace UWP_HelloWorld_NewYear.Converters
{
    public class SelectedIndexToIsEnabledConverter : IValueConverter
    {
        /// <summary>
        /// Converts the SelectedIndex int value to IsEnabled bool property
        /// </summary>
        /// <returns>Returns true if something is selected (SelectedIndex != -1), otherwise false</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return !((int)value == -1);
        }

        /// <summary>
        /// Converts the IsEnabled bool property to SelectedIndex int value 
        /// </summary>
        /// <returns>Returns 1 if something IsEnabled (IsEnabled == true), otherwise -1</returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return (bool)value ? -1 : 1;
        }
    }
}
