﻿using OrienteeringToolWPF.Enumerations;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace OrienteeringToolWPF.Utils.Converters
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

            switch ((Correctness)value)
            {
                case Correctness.PRESENT:
                    // Only because there is no fallthrough if something besides break is under case label
                    if ((Order)parameter == Order.UNORDERED)
                        goto case Correctness.CORRECT;
                    return Brushes.Yellow;
                case Correctness.CORRECT:
                    return Brushes.LightGreen;
                case Correctness.INVALID:
                    return new SolidColorBrush(Color.FromRgb(255, 63, 63));
                default:
                    return Brushes.Transparent;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
