using System.Globalization;

namespace MiniTrello.Helpers
{
    public class PriorityColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int priority)
            {
                return priority switch
                {
                    0 => Color.FromArgb("#4CAF50"),  // Low - Green
                    1 => Color.FromArgb("#FF9800"),  // Medium - Orange
                    2 => Color.FromArgb("#F44336"),  // High - Red
                    _ => Color.FromArgb("#9E9E9E")
                };
            }
            return Color.FromArgb("#9E9E9E");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
