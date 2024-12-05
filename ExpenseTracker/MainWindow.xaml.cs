using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls; // Required for TextBox

namespace ExpenseTracker
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public ObservableCollection<Expense> Expenses { get; set; }
        public Budget Budget { get; set; }
        public ChartValues<decimal> ExpenseChartValues { get; set; }
        public ChartValues<decimal> IncomeChartValues { get; set; }
        public string[] ChartLabels { get; set; }
        public Func<double, string> ChartLabelFormatter { get; set; }

        private Expense _selectedExpense;
        private int _selectedIndex = -1;

        private string _alertMessage;
        public string AlertMessage
        {
            get => _alertMessage;
            set
            {
                _alertMessage = value;
                OnPropertyChanged();
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            DescriptionInput.Text = "Description";
            AmountInput.Text = "Amount";
            DescriptionInput.Foreground = Brushes.Gray;
            AmountInput.Foreground = Brushes.Gray;

            DescriptionInput.GotFocus += TextBox_GotFocus;
            DescriptionInput.LostFocus += TextBox_LostFocus;
            AmountInput.GotFocus += TextBox_GotFocus;
            AmountInput.LostFocus += TextBox_LostFocus;

            Expenses = new ObservableCollection<Expense>();
            Budget = new Budget { MonthlyLimit = 00 }; // Default budget.

            ExpenseChartValues = new ChartValues<decimal>();
            IncomeChartValues = new ChartValues<decimal>();
            ChartLabels = Array.Empty<string>();
            ChartLabelFormatter = value => value.ToString("C", CultureInfo.CurrentCulture);

            DataContext = this;
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs eventArgs)
        {
            var textBox = sender as TextBox;
            if (textBox != null && (textBox.Text == "Description" || textBox.Text == "Amount"))
            {
                textBox.Text = string.Empty;
                textBox.Foreground = Brushes.Black;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox != null && string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = textBox.Name == "DescriptionInput" ? "Description" : "Amount";
                textBox.Foreground = Brushes.Gray;
            }
        }

        private void BudgetInput_GotFocus(object sender, RoutedEventArgs e)
        {
            if (BudgetInput.Text == "Enter Monthly Budget")
            {
                BudgetInput.Text = string.Empty;
                BudgetInput.Foreground = Brushes.Black;
            }
        }

        private void BudgetInput_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(BudgetInput.Text))
            {
                BudgetInput.Text = "Enter Monthly Budget";
                BudgetInput.Foreground = Brushes.Gray;
            }
        }

        private void SetBudgetButton_Click(object sender, RoutedEventArgs e)
        {
            if (decimal.TryParse(BudgetInput.Text, out var newBudget))
            {
                Budget.MonthlyLimit = newBudget;
                Budget.TotalExpenses = 0; // Reset expenses for a new budget.
                UpdateBudgetAlert();
                OnPropertyChanged(nameof(Budget));
            }
            else
            {
                MessageBox.Show("Please enter a valid numeric value.");
            }
        }

        private void AddExpenseButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var expense = new Expense
                {
                    Description = DescriptionInput.Text,
                    Amount = decimal.Parse(AmountInput.Text),
                    Category = (CategoryInput.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content.ToString(),
                    Date = DateTime.Now,
                    IsIncome = IsIncomeCheckBox.IsChecked ?? false
                };

                Expenses.Add(expense);

                // Modify only the total expenses for tracking
                if (expense.IsIncome)
                {
                    Budget.TotalExpenses += expense.Amount; // Increase total expenses for spending
                }
                else
                {
                    Budget.TotalExpenses -= expense.Amount; // Decrease total expenses for income
                }

                UpdateBudgetAlert();
                RefreshExpenseList();
                UpdateChart();

                DescriptionInput.Clear();
                AmountInput.Clear();
                CategoryInput.SelectedIndex = -1;
                IsIncomeCheckBox.IsChecked = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void ExpenseListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedExpense = ExpenseListView.SelectedItem as Expense;
            _selectedIndex = ExpenseListView.SelectedIndex;

            if (_selectedExpense != null)
            {
                // Populate fields for editing
                DescriptionInput.Text = _selectedExpense.Description;
                AmountInput.Text = _selectedExpense.Amount.ToString(CultureInfo.InvariantCulture);
                IsIncomeCheckBox.IsChecked = _selectedExpense.IsIncome;

                // Select the corresponding category
                var categoryItem = CategoryInput.Items
                    .Cast<ComboBoxItem>()
                    .FirstOrDefault(item => item.Content.ToString() == _selectedExpense.Category);
                CategoryInput.SelectedItem = categoryItem;
            }
        }

      /*  private void EditExpenseButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedExpense == null)
            {
                MessageBox.Show("Please select an expense to edit.");
                return;
            }

            // Fields are already populated by SelectionChanged event; user can edit them directly.
            MessageBox.Show("Edit the fields and click 'Save Changes' to update the expense.");
        }*/

        private void SaveExpenseButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedExpense == null || _selectedIndex == -1)
            {
                MessageBox.Show("No expense selected to edit and save changes.");
                return;
            }

            try
            {
                // Update the selected expense
                _selectedExpense.Description = DescriptionInput.Text;
                _selectedExpense.Amount = decimal.Parse(AmountInput.Text);
                _selectedExpense.Category = (CategoryInput.SelectedItem as ComboBoxItem)?.Content.ToString();
                _selectedExpense.IsIncome = IsIncomeCheckBox.IsChecked ?? false;
                _selectedExpense.Date = DateTime.Now;

                // Update the collection to reflect changes
                Expenses[_selectedIndex] = _selectedExpense;

                // Refresh the list view and chart
                RefreshExpenseList();
                UpdateChart();

                // Clear the inputs after saving
                DescriptionInput.Clear();
                AmountInput.Clear();
                CategoryInput.SelectedIndex = -1;
                IsIncomeCheckBox.IsChecked = false;

                MessageBox.Show("Expense updated successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while saving changes: {ex.Message}");
            }
        }

        private void UpdateChart()
        {
            var groupedExpenses = Expenses.GroupBy(expense => expense.Date.ToShortDateString())
                                  .OrderBy(g => g.Key)
                                  .Select(g => new
                                  {
                                      Date = g.Key,
                                      Expenses = g.Where(exp => !exp.IsIncome).Sum(exp => exp.Amount),
                                      Income = g.Where(exp => exp.IsIncome).Sum(exp => exp.Amount)
                                  });

            if (!groupedExpenses.Any())
            {
                Console.WriteLine("No data to display in chart.");
            }

            ExpenseChartValues.Clear();
            IncomeChartValues.Clear();
            ChartLabels = groupedExpenses.Select(g => g.Date).ToArray();

            foreach (var group in groupedExpenses)
            {
                ExpenseChartValues.Add(group.Expenses);
                IncomeChartValues.Add(group.Income);
            }

            OnPropertyChanged(nameof(ExpenseChartValues));
            OnPropertyChanged(nameof(IncomeChartValues));
            OnPropertyChanged(nameof(ChartLabels));
        }

        private void UpdateBudgetAlert()
        {
            if (Budget.TotalExpenses > Budget.MonthlyLimit)
                AlertMessage = "Warning: Budget limit exceeded!";
            else if (Budget.TotalExpenses > Budget.MonthlyLimit * 0.8m)
                AlertMessage = "Caution: Approaching budget limit.";
            else
                AlertMessage = string.Empty;
        }

        private void RefreshExpenseList()
        {
            ExpenseListView.ItemsSource = null;
            ExpenseListView.ItemsSource = Expenses;
        }

        private void ExportToCsvButton_Click(object sender, RoutedEventArgs eventArgs)
        {
            var csv = string.Join(Environment.NewLine, Expenses.Select(e => $"{e.Description},{e.Category},{e.Amount},{e.Date},{e.IsIncome}"));
            File.WriteAllText("Expenses.csv", csv);
            MessageBox.Show("Expenses exported to Expenses.csv");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class Expense
    {
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string Category { get; set; }
        public DateTime Date { get; set; }
        public bool IsIncome { get; set; }
    }

    public class Budget : INotifyPropertyChanged
    {
        private decimal _monthlyLimit;
        private decimal _totalExpenses;

        public decimal MonthlyLimit
        {
            get => _monthlyLimit;
            set { _monthlyLimit = value; OnPropertyChanged(); OnPropertyChanged(nameof(RemainingBudget)); }
        }

        public decimal TotalExpenses
        {
            get => _totalExpenses;
            set { _totalExpenses = value; OnPropertyChanged(); OnPropertyChanged(nameof(RemainingBudget)); }
        }

        public decimal RemainingBudget => MonthlyLimit - TotalExpenses;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
