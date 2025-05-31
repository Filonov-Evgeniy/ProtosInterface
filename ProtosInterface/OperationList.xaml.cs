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
    /// Логика взаимодействия для OperationList.xaml
    /// </summary>
    public partial class OperationList : Window
    {
        List<MenuItem> items;
        public OperationList(List<MenuItem> items)
        {
            InitializeComponent();
            this.items = items;
            dbDataLoader loader = new dbDataLoader();
            List<MenuItem> dataList = loader.getOperationData();
            OperationListBox.ItemsSource = dataList;
            OperationListBox.DisplayMemberPath = "Title";
            items.Clear();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            MenuItem[] itemsToAdd = new MenuItem[OperationListBox.SelectedItems.Count];
            OperationListBox.SelectedItems.CopyTo(itemsToAdd, 0);
            items.AddRange(itemsToAdd);
            this.DialogResult = true;
        }
    }
}
