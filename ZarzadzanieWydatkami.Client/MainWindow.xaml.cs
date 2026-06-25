using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Data;

namespace ZarzadzanieWydatkami.Client
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // Metoda do autozapisu po edycji wiersza
        private void DataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (DataContext is MainViewModel viewModel)
            {
                Dispatcher.InvokeAsync(() =>
                {
                    if (viewModel.ZapiszWydatekCommand.CanExecute(null))
                    {
                        viewModel.ZapiszWydatekCommand.Execute(null);
                    }
                });
            }
        }
        private void BudzetTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            // Sprawdzamy, czy wciśnięto dokładnie klawisz Enter
            if (e.Key == Key.Enter)
            {
                if (sender is TextBox textBox)
                {
                    // Wymuszamy na programie przesłanie kwoty do logiki od razu
                    BindingOperations.GetBindingExpression(textBox, TextBox.TextProperty)?.UpdateSource();

                    // Odznaczamy pole tekstowe (kursor znika), dając ładny wizualny efekt "zatwierdzenia"
                    Keyboard.ClearFocus();
                }
            }
        }
    }
}