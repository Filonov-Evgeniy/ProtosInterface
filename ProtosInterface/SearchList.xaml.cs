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
    /// Логика взаимодействия для SearchList.xaml
    /// </summary>
    public partial class SearchList : Window
    {
        List<MenuItem> searchingItem;
        Dictionary<MenuItem, string> searchingItems;
        public SearchList(Dictionary<MenuItem, string> searchingItems, List<MenuItem> searchingItem)
        {
            InitializeComponent();
            this.searchingItems = searchingItems;
            ItemListBox.ItemsSource = searchingItems;
            this.searchingItem = searchingItem;
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            int itemIndex = ItemListBox.SelectedIndex;
            List<MenuItem> items = new List<MenuItem>(searchingItems.Keys);
            MenuItem item = items[itemIndex];
            searchingItem.Add(item);
            this.DialogResult = true;
        }
    }
}
