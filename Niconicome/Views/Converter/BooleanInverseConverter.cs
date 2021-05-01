using System;
using System.Globalization;
using System.Windows.Data;

namespace Niconicome.Views.Converter
{
    class BooleanInverseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not bool booleanVal) throw new InvalidOperationException($"This Converter can only accept bool, not \"{value.GetType().Name}\"");

            return !booleanVal;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
