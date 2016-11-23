using System;
using System.Globalization;
using System.Windows.Data;

namespace OrienteeringToolWPF.Utils.Converters
{
    class TimestampConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter), "Paramter must not be null");
            if (!(parameter is string))
                throw new ArgumentException(nameof(parameter), "Parameter must be string");

            if (value is long && (long)value > 0)
            {
                var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                value = dtDateTime.AddMilliseconds((long)value);
                //DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime();
                //value = epoch.AddSeconds(System.Convert.ToDouble((long)value));
            }
            else
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
