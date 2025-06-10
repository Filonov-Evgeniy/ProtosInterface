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
    /// Логика взаимодействия для RenameForm.xaml
    /// </summary>
    public partial class RenameWindow : Window
    {
        public string EnteredText { get; private set; }

        public RenameWindow(string initialText)
        {
            InitializeComponent();
            ProductName.Text = initialText;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (ProductName.Text.Trim().Length != 0)
            {
                EnteredText = ProductName.Text;
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Имя не может быть пустым");
            }
        }
    }
}
