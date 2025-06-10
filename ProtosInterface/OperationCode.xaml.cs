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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ProtosInterface
{
    /// <summary>
    /// Логика взаимодействия для OperationCount.xaml
    /// </summary>
    public partial class OperationCode: Window
    {
        public int EnteredNumber { get; private set; }
        public OperationCode()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {           
            if (textBox.Text.Trim().Length != 0)
            {
                if (!int.TryParse(textBox.Text, out int number))
                {
                    MessageBox.Show("Номер должен быть числом");
                }
                else
                {
                    EnteredNumber = number;
                    DialogResult = true;
                    Close();
                }
            }
            else
            {
                MessageBox.Show("Номер не может быть пустым");
            }
        }
    }
}
