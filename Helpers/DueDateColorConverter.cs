using System.Globalization;

namespace MiniTrello.Helpers
{
    public class DueDateColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dueDate)
            {
                if (dueDate < DateTime.Today) return Color.FromArgb("#F44336");       // Overdue - Red
                if (dueDate <= DateTime.Today.AddDays(2)) return Color.FromArgb("#FF9800"); // Soon - Orange
                return Color.FromArgb("#757575"); // Normal - Gray
            }
            return Color.FromArgb("#757575");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
