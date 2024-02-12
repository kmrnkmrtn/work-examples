using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amoba.MAUI
{
    public class StringToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var color = value.ToString();
            switch (color)
            {
                case "Black":
                    return Colors.Black;
                case "White":
                    return Colors.White;
                case "Green":
                    return Colors.Green;
                default:
                    return Colors.Black;
            }
        }

        public object? ConvertBack(object? value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
