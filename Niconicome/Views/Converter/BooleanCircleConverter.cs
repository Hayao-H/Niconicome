using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Niconicome.Views.Converter
{
    class BooleanCircleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not bool booleanVal) throw new InvalidOperationException($"This Converter can only accept bool, not \"{value.GetType().Name}\"");

            return booleanVal ? "〇" : "×";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
