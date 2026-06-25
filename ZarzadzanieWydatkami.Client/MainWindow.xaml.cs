using System.Windows;

namespace ZarzadzanieWydatkami.Client
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // Ta linijka łączy naszego XAML-a z przygotowanym ViewModel-em
            DataContext = new MainViewModel();
        }

        private void DataGrid_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }
    }
}