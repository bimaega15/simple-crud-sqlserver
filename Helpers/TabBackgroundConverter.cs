using System.Globalization;

namespace MiniTrello.Helpers
{
    public class TabBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int selected && parameter is string paramStr && int.TryParse(paramStr, out int tabIndex))
                return selected == tabIndex ? Color.FromArgb("#E3F2FD") : Colors.Transparent;
            return Colors.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
