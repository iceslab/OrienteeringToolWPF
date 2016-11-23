using System;
using System.Globalization;
using System.Windows.Data;

namespace OrienteeringToolWPF.Utils.Converters
{
    class DeltaTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null)
                throw new ArgumentNullException(nameof(parameter), "Paramter must not be null");
            if (!(parameter is string))
                throw new ArgumentException(nameof(parameter), "Parameter must be string");

            if (value is long)
            {
                if ((long)value >= 0)
                    parameter = "+" + parameter;
                else if ((long)value < 0)
                    parameter = "-" + parameter;

                var seconds = (long)value / 1000;
                var minutes = seconds / 60;
                seconds -= minutes * 60;
                return string.Format((string)parameter, minutes, seconds);
            }
            else
            {
                return Properties.Resources.None;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
