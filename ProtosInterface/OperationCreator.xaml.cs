using ProtosInterface.Extensions;
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
using ProtosInterface.Models;

namespace ProtosInterface
{
    /// <summary>
    /// Логика взаимодействия для OperationCreator.xaml
    /// </summary>
    public partial class OperationCreator : Window
    {
        AppDbContext _context = new AppDbContext();
        public OperationCreator()
        {
            InitializeComponent();
        }

        private void SaveOperation_Click(object sender, RoutedEventArgs e)
        {
            if (operationName.Text != "".Trim())
            {
                if (_context.OperationTypes.Where(x=>x.Name == operationName.Text).ToList().Count == 0) {
                    int maxId = _context.OperationTypes.GetLastId();
                    OperationType operation = new OperationType();
                    operation.Id = maxId + 1;
                    operation.Name = operationName.Text;

                    _context.OperationTypes.Add(operation);
                    _context.SaveChanges();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Операция с таким названием уже существует");
                }
            }
            else
            {
                MessageBox.Show("Заполните название операции");
            }
        }
    }
}
