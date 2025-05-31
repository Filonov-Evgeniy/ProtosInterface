using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        public event PropertyChangedEventHandler? PropertyChanged;
        List<MenuItem> itemList;
        private AppDbContext _context;
        public ItemList(List<MenuItem> itemList, bool operation)
        {
            InitializeComponent();
            Detail.IsChecked = true;
            Assembly_uint.IsChecked = true;
            Product.IsChecked = true;
            if (operation == true)
            {
                SelectButton.Content = "Добавить";
            }
            else
            {
                SelectButton.Content = "Выбрать";
            }
            dbDataLoader loader = new dbDataLoader();
            List<MenuItem> dataList = loader.getProductData();
            ItemListBox.ItemsSource = dataList;
            ItemListBox.DisplayMemberPath = "Title";
            this.itemList = itemList;
            itemList.Clear();

            foreach (MenuItem item in dataList)
            {
                AllItems.Add(item);
            }

            // Настраиваем фильтрацию
            FilteredItems = CollectionViewSource.GetDefaultView(AllItems);
            FilteredItems.Filter = FilterItems;
        }       

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            MenuItem[] itemsToAdd = new MenuItem[ItemListBox.SelectedItems.Count];
            ItemListBox.SelectedItems.CopyTo(itemsToAdd, 0);
            itemList.AddRange(itemsToAdd);
            this.DialogResult = true;
        }

        public ObservableCollection<MenuItem> AllItems { get; } = new ObservableCollection<MenuItem>();

        // Отфильтрованная коллекция (привязана к ListBox)
        public ICollectionView FilteredItems { get; }

        // Свойства для CheckBox
        private bool _isFilter1Enabled;
        public bool IsFilter1Enabled
        {
            get => _isFilter1Enabled;
            set
            {
                _isFilter1Enabled = value;
                OnPropertyChanged(nameof(IsFilter1Enabled));
                FilteredItems.Refresh(); // Обновляем фильтрацию
            }
        }

        private bool _isFilter2Enabled;
        public bool IsFilter2Enabled
        {
            get => _isFilter2Enabled;
            set
            {
                _isFilter2Enabled = value;
                OnPropertyChanged(nameof(IsFilter2Enabled));
                FilteredItems.Refresh();
            }
        }

        private bool _isFilter3Enabled;
        public bool IsFilter3Enabled
        {
            get => _isFilter3Enabled;
            set
            {
                _isFilter3Enabled = value;
                OnPropertyChanged(nameof(IsFilter3Enabled));
                FilteredItems.Refresh();
            }
        }

        // Логика фильтрации
        private bool FilterItems(object obj)
        {
            if (obj is not MenuItem item)
                return false;

            // Если все CheckBox выключены - показываем все элементы
            if (!IsFilter1Enabled && !IsFilter2Enabled && !IsFilter3Enabled)
                return true;

            //IQueryable product = _context.Products.Include(p => p.ProductType).Where(p => p.Id == item.Id);

            // Проверяем условия фильтрации

            //bool passesFilter1 = IsFilter1Enabled && item.;
            //bool passesFilter2 = IsFilter2Enabled && item.Category == "Категория 2";
            //bool passesFilter3 = IsFilter3Enabled && item.Name.Contains("3");

            //return passesFilter1 || passesFilter2 || passesFilter3;
            return true;
        }

        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
