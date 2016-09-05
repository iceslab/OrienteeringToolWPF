using System;
using System.Globalization;
using System.Windows.Data;

namespace OrienteeringToolWPF.Utils
{
    class TimestampConverter : IValueConverter
    {
        // TODO: Parameter must contain relative start time (or something else must be done to get correct time)
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                throw new ArgumentNullException("Argument must not be null");
            if (parameter == null)
                throw new ArgumentNullException("Paramter must not be null");
            if (!(parameter is string))
                throw new ArgumentException("Parameter must be string");

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
