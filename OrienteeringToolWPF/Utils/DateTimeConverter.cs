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
                throw new ArgumentNullException("Argument must not be null");
            if (!(value is DateTime))
                value = Properties.Resources.None;
            if (parameter == null)
                throw new ArgumentNullException("Paramter must not be null");
            if (!(parameter is string))
                throw new ArgumentException("Parameter must be string");

            return string.Format((string)parameter, value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
