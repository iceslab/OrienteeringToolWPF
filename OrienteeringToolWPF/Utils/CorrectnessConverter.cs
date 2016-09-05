using OrienteeringToolWPF.Enumerations;
using System;
using System.Globalization;
using System.Windows.Data;

namespace OrienteeringToolWPF.Utils
{
    class CorrectnessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                throw new ArgumentNullException("Argument must not be null");
            if (!(value is Correctness))
                throw new ArgumentException("Argument must be Correctness enum");
            if (parameter == null)
                throw new ArgumentNullException("Paramter must not be null");
            if (!(parameter is Order))
                throw new ArgumentException("Parameter must be Order enum");

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
