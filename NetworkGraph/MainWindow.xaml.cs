using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.EntityFrameworkCore;

namespace NetworkGraph
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly AppDbContext _context;
        //private ObservableCollection<ViewModel> _networks;

        public MainWindow()
        {
            InitializeComponent();
            LoadOperationsData();
        }

        private void LoadOperationsData()
        {

            using (var context = new AppDbContext())
            {
                var calculator = new NetworkCalculator(context);
                var results = calculator.CalculateTimings();

                OperationsDataGrid.ItemsSource = results;

                var testClassObject = new DefineOperationOrder(context);
                testClassObject.DefineOperationOrderMethod(results);
            }
        }

    }

    public class BoolToYesNoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value is bool b && b) ? "Да" : "Нет";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}