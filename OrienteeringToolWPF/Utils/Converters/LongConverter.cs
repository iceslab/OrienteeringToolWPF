using System;
using System.Globalization;
using System.Windows.Data;

namespace OrienteeringToolWPF.Utils.Converters
{
    class LongConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                throw new ArgumentNullException("Argument must not be null");
            if (!(value is long))
                throw new ArgumentException("Argument must be long");
            if (parameter == null)
                throw new ArgumentNullException("Paramter must not be null");
            if (!(parameter is string))
                throw new ArgumentException("Parameter must be string");

            if ((long)value <= 0)
                value = Properties.Resources.None;

            return string.Format((string)parameter, value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
