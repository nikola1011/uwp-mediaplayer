﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace UWP_HelloWorld_NewYear.Converters
{
    public class IntCountToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return !((int)value == 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return (bool)value ? 1 : 0;
        }
    }
}
