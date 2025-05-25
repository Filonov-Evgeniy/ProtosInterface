using System;
using System.Collections.Generic;
using System.Data.Common;
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
    /// Логика взаимодействия для ItemList.xaml
    /// </summary>
    public partial class ItemList : Window
    {
        List<MenuItem> itemList;
        public ItemList(List<MenuItem> itemList)
        {
            InitializeComponent();
            dbDataLoader loader = new dbDataLoader();
            List<MenuItem> dataList = loader.getProductData();
            ItemListBox.ItemsSource = dataList;
            ItemListBox.DisplayMemberPath = "Title";
            this.itemList = itemList;
            itemList.Clear();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            MenuItem[] itemsToAdd = new MenuItem[ItemListBox.SelectedItems.Count];
            ItemListBox.SelectedItems.CopyTo(itemsToAdd, 0);
            itemList.AddRange(itemsToAdd);
            this.DialogResult = true;
        }
    }
}
