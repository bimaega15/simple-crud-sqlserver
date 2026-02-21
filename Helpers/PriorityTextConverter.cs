using System.Globalization;

namespace MiniTrello.Helpers
{
    public class PriorityTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int priority)
                return priority switch { 0 => "Low", 1 => "Medium", 2 => "High", _ => "Unknown" };
            return "Unknown";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
