using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace ConvertersLib
{
    [ValueConversion(typeof(String), typeof(DateTime))]
    public class StringToDateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime dt;

            if (DateTime.TryParse(value.ToString(), out dt))
            {
                if (dt.Equals(default))
                {
                    return "Ще не проведено";
                }
            }
            return dt.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DateTime date;

            if (DateTime.TryParse(value.ToString(), out date))
            {
                return date;
            }

            return DependencyProperty.UnsetValue;
        }
    }
}