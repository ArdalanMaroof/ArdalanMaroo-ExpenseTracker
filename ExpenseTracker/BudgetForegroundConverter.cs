using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ExpenseTracker
{
    public class BudgetForegroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal remainingBudget)
            {
                if (remainingBudget < 0)
                    return Brushes.Red;  // Budget exceeded
                else if (remainingBudget < 0.2m * (parameter as decimal? ?? 1))
                    return Brushes.Orange;  // Approaching limit
                else
                    return Brushes.Green;  // Within limit
            }

            return Brushes.Black;  // Default
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
