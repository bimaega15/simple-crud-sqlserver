using System.Globalization;

namespace MiniTrello.Helpers
{
    public class StatusTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int status)
                return status switch { 0 => "Backlog", 1 => "In Progress", 2 => "Q.A", 3 => "Completed", _ => "Unknown" };
            return "Unknown";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
