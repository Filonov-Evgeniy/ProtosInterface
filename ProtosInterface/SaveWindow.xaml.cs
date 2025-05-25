using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ProtosInterface
{
    /// <summary>
    /// Логика взаимодействия для SaveWindow.xaml
    /// </summary>
    public partial class SaveWindow : Window
    {
        public enum SaveOption { SaveAsNew, SaveChanges, Cancel }
        public SaveOption Result { get; private set; } = SaveOption.Cancel;

        public SaveWindow()
        {
            InitializeComponent();
        }

        private void SaveAsNew_Click(object sender, RoutedEventArgs e)
        {
            Result = SaveOption.SaveAsNew;
            Close();
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            Result = SaveOption.SaveChanges;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
