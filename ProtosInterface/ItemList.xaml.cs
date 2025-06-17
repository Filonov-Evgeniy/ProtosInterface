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
        private ObservableCollection<MenuItem> AllItems { get; } = new ObservableCollection<MenuItem>();

        private event PropertyChangedEventHandler? PropertyChanged;
        List<MenuItem> itemList;
        private AppDbContext _context;
        private bool type;
        public int enteredNumber;
        public ItemList(List<MenuItem> itemList, bool operation, bool type)
        {
            InitializeComponent();
            ItemListBox.Visibility = Visibility.Collapsed;
            Detail.IsChecked = true;
            Assembly_uint.IsChecked = true;
            Product.IsChecked = true;
            this.type = type;
            SelectButton.Content = operation ? "Добавить" : "Выбрать";
            
            if (type == true)
            {
                ColorChange("#c7f9f7", "ButtonStyle");
                ColorChange("#FF9696FF", "ProgressStyle");
            }
            else
            {
                ColorChange("#ccf6d4", "ButtonStyle");
                ColorChange("#FF96FF96", "ProgressStyle");
            }
            
            LoadDataAsync();
            this.itemList = itemList;
            itemList.Clear();

            //foreach (MenuItem item in dataList)
            //{
            //    AllItems.Add(item);
            //}

            // Настраиваем фильтрацию
            //FilteredItems = CollectionViewSource.GetDefaultView(AllItems);
            //FilteredItems.Filter = FilterItems;
        }

        private async void LoadDataAsync()
        {
            try
            {
                LoadingIndicator.Visibility = Visibility.Visible;
                List<MenuItem> dataList = new List<MenuItem>();
                dbDataLoader loader = new dbDataLoader();
                AllItems.Clear();
                if (type == true)
                {
                    dataList = await loader.getProductData();
                }
                else
                {
                    FilterField.Visibility = Visibility.Collapsed;
                    dataList = await loader.GetOperationDataAsync();
                }
                ItemListBox.ItemsSource = dataList;
                ItemListBox.DisplayMemberPath = "Title";

                AllItems.Clear();
                foreach (MenuItem item in dataList)
                {
                    AllItems.Add(item);
                }

                FilteredItems = CollectionViewSource.GetDefaultView(AllItems);
                FilteredItems.Filter = FilterItems;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки данных: {ex.Message}");
                AllItems.Clear();
                ItemListBox.ItemsSource = null;
            }
            finally
            {
                LoadingIndicator.Visibility = Visibility.Collapsed;
                ItemListBox.Visibility = Visibility.Visible;
            }
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            itemList.Clear();
            MenuItem[] itemsToAdd = new MenuItem[ItemListBox.SelectedItems.Count];
            OperationCode opCode = new OperationCode();

            ItemListBox.SelectedItems.CopyTo(itemsToAdd, 0);
            itemList.AddRange(itemsToAdd);
            if (type == true)
            {
                this.DialogResult = true;
            }
            else
            {
                if (opCode.ShowDialog() == true)
                {
                    enteredNumber = opCode.EnteredNumber;
                    this.DialogResult = true;
                }
                else
                {
                    this.DialogResult = false;
                }
            }
            this.Close();
        }

        // Отфильтрованная коллекция (привязана к ListBox)
        public ICollectionView FilteredItems { get; private set; }

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

            IQueryable product = _context.Products.Include(p => p.ProductType).Where(p => p.Id == item.Id);

            //Проверяем условия фильтрации

            //bool passesFilter1 = IsFilter1Enabled && product.;
            //bool passesFilter2 = IsFilter2Enabled && item.Category == "Категория 2";
            //bool passesFilter3 = IsFilter3Enabled && item.Name.Contains("3");

            //return passesFilter1 || passesFilter2 || passesFilter3;
            return true;
        }

        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void ColorChange(string hexColor, string resource)
        {
            var brush = (SolidColorBrush)new BrushConverter().ConvertFromString(hexColor)!;

            var oldStyle = this.Resources[resource] as Style;

            if (oldStyle != null)
            {
                Style newStyle;
                if(resource == "ButtonStyle")
                {
                    newStyle = new Style(typeof(Button), oldStyle);
                    newStyle.Setters.Add(new Setter(BackgroundProperty, brush));
                }
                else
                {
                    newStyle = new Style(typeof(ProgressBar), oldStyle);
                    newStyle.Setters.Add(new Setter(ForegroundProperty, brush));
                }

                this.Resources[resource] = newStyle;
            }
        }
    }
}
