## Expense Tracker with Budget Analysis
## Project Overview
The Expense Tracker with Budget Analysis is a WPF application that helps users track their daily expenses, categorize them, and compare them against a set budget. It provides a user-friendly interface where users can enter their monthly budget, add income or expense entries, view a real-time chart of their expenses and income, and export their data to a CSV file for future reference.

## Prerequisites
To run the application, ensure you have the following installed:

- .NET Framework (WPF Application)
  
- Visual Studio with WPF support
  
- LiveCharts (Used for graphical charts):
  
- You can install this via NuGet package manager in Visual Studio.

## Features

•	Set Monthly Budget: Allows the user to input a monthly budget and track expenses against it.

•	Add Expense/Income: Users can add individual expense or income entries with a description, amount, and category.

•	View Expenses: Displays a list of all expenses and income entries in a table format.

•	Chart Visualization: Provides a line chart showing expenses and income trends over time.

•	Budget Alert: Shows alerts when the total expenses exceed or approach the monthly budget.

•	CSV Export: Allows users to export all their tracked expenses to a CSV file for easy reporting.

## Usage Instructions
1.	Set Budget:
   
•	Enter your monthly budget in the text box and click Set Budget. The remaining budget will be updated.

2.	Add Expense/Income:
   
•	Enter a description, amount, and category for the entry. Check the box if it is income.

•	Click Add to save the entry.

•	The expenses list, budget, and graph will update accordingly.

3.	View Expenses List:

•	All recorded expenses/income will be displayed in the Expenses List.

•	You can edit an entry by selecting it from the list and clicking Save Edit/Changes after making updates.

4.	Export to CSV:
   
•	Click the Export to CSV button to download a CSV file of your expenses.



## Code Structure
## Main Components:

1.	MainWindow.xaml
   
   o	This is the main user interface file that defines the layout and controls (e.g., text boxes, buttons, charts) for the application.
   
   o	The file includes WPF elements like TextBox, Button, ComboBox, and ListView to collect user input and display data.

2.	MainWindow.xaml.cs
   
  o	This is the code-behind for the main window that contains the logic for managing the budget, adding expenses, updating charts, and handling user interactions.

3.	Expense Class
   
  o	Represents individual expense/income entries with properties such as Description, Amount, Category, Date, and IsIncome (to indicate whether the entry is an expense or income).

4.	Budget Class
   
  o	Contains properties to manage the budget such as MonthlyLimit, TotalExpenses, and calculates the RemainingBudget. It also supports data binding to automatically update the UI.

5.	Chart Integration (LiveCharts)
   
  o	The application uses the LiveCharts library to visualize income and expense trends. ChartValues<decimal> is used to store chart data, and CartesianChart is used to render the line graph.

6.	CSV Export Functionality
   
  o	The export feature allows users to save all their expense data in CSV format for further analysis or record-keeping.


## Key Functions and Methods

1. SetBudgetButton_Click (Set Monthly Budget)

    •	Description: Sets the monthly budget and resets the total expenses when a new budget is entered.

    •	Parameters: None.

    •	Logic:
     o	Takes the budget input from the user, validates it, and updates the MonthlyLimit property of the Budget object.

     o	Resets the total expenses and updates the UI to reflect the new budget.

3. AddExpenseButton_Click (Add Expense/Income)
   •	Description: Adds a new expense or income entry to the list.
   •	Parameters: None.
   •	Logic:
     o	Collects values from the user input fields (DescriptionInput, AmountInput, CategoryInput, IsIncomeCheckBox).
     o	Creates an Expense object and adds it to the Expenses collection.
     o	Updates the total expenses in the Budget object and refreshes the chart and list of expenses.

4. ExpenseListView_SelectionChanged: Updates the UI fields with the selected expense details for editing.

5. SaveExpenseButton_Click: Saves the changes made to an existing expense, updates the list, and refreshes the chart.

6. UpdateChart
   •	Description: Updates the expense and income line chart based on the latest data.
   •	Parameters: None.
   •	Logic:
     o	Groups expenses by date and calculates the total expenses and income for each day.
     o	Updates the ChartLabels, ExpenseChartValues, and IncomeChartValues properties.

5. UpdateBudgetAlert
   •	Description: Checks if the total expenses exceed or approach the budget limit and updates the alert message accordingly.
   •	Parameters: None.
   •	Logic:
     o	If expenses exceed 100% of the budget, displays a warning. If the expenses exceed 80%, it shows a caution message.

6. ExportToCsvButton_Click (Export Expenses to CSV)
   •	Description: Exports the current expenses data to a CSV file.
   •	Parameters: None.
   •	Logic:
     o	Iterates over the list of Expenses and creates a CSV-formatted string.
     o	Saves the string to a file named Expenses.csv and notifies the user via a message box.

7. TextBox_GotFocus & TextBox_LostFocus
   •	Description: Clears placeholder text in input fields when focused, and restores it if left empty.
   •	Parameters: sender (the textbox being interacted with).
   •	Logic:
     o	When a text box gains focus, it clears the placeholder text if present.
     o	When it loses focus, if the text box is empty, it restores the placeholder text.

## Data Binding and Notifications
   •	The application uses data binding extensively to update the UI automatically when the data changes.
   •	The INotifyPropertyChanged interface is implemented in both the MainWindow and Budget classes to notify the UI of changes to properties like RemainingBudget and AlertMessage.

## Libraries Used
   •	LiveCharts: A charting library for rendering graphs and visualizing trends in expenses and income.
     o	Documentation: LiveCharts Documentation
   •	WPF (Windows Presentation Foundation): For building the user interface and managing user interactions.

## How to Run the Project
1.	Clone or download the repository to your local machine.
2.	Open the solution file (ExpenseTracker.sln) in Visual Studio.
3.	Build the project by selecting Build > Build Solution.
4.	Run the project using Debug > Start Without Debugging.

## Future Enhancements
   •	Add categories for expense tracking.
   •	Improve budget alert system with customizable thresholds.
   •	Integrate a database to persist expenses and budgets.
   •	Add support for multi-currency conversion.



