using System;
using System.Globalization;
using System.Windows.Data;

namespace OrienteeringToolWPF.Utils
{
    class DateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value), "Argument must not be null");
            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter), "Paramter must not be null");
            if (!(parameter is string))
                throw new ArgumentException(nameof(parameter), "Parameter must be string");

            if (!(value is DateTime))
            {
                value = Properties.Resources.None;
            }

            return string.Format((string)parameter, value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
