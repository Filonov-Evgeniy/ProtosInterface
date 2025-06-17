using ProtosInterface.Extensions;
using ProtosInterface.Models;
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
    /// Логика взаимодействия для ProductCreator.xaml
    /// </summary>
    public partial class ProductCreator : Window
    {
        public ProductCreator()
        {
            dbDataLoader dbloader = new dbDataLoader();
            InitializeComponent();
            detailTypeComboBox.ItemsSource = dbloader.getProductTypeData();
            detailTypeComboBox.SelectedIndex = 0;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (nameTextBox.Text.Trim().Length != 0)
            {
                Product product = new Product();
                product.Name = String.Join(" ", [cipherTextBox.Text.Trim(), nameTextBox.Text.Trim()]).Trim();
                if (detailTypeComboBox.SelectedItem is ProductType selected)
                {
                    product.TypeId = selected.Id;
                }
                AppDbContext _context = new AppDbContext();
                product.Id = _context.Products.GetLastId() + 1;
                if (_context.Products.Where(x => x.Name == product.Name).Count() > 0)
                {
                    MessageBox.Show("Уже существует элемент с таким названием (Добавьте или измените шифр или название)");
                }
                else
                {
                    _context.Products.Add(product);
                    if (_context.SaveChanges() > 0)
                    {
                        MessageBox.Show($"Изделие {product.Name} сохранено успешно");
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Не удалось сохранить изделие");
                    }
                }
            }
            else
            {
                MessageBox.Show("Укажите название изделия");
            }
        }
    }
}
