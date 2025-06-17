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
        private Dictionary<int, List<MenuItem>> TypeItem = new Dictionary<int, List<MenuItem>>();

        private List<MenuItem> itemList = new List<MenuItem>();
        private bool type;
        public int enteredNumber;
        AppDbContext _context = new AppDbContext();

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
                    FilterField.Visibility = Visibility.Collapsed;
                    dataList = await loader.GetProductDataAsync();
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

                foreach (MenuItem item in AllItems)
                {
                    int typeId = int.Parse(_context.Products.Where(x => x.Id == item.Id)
                                                            .Select(x => x.TypeId)
                                                            .FirstOrDefault().ToString()!);

                    if (!TypeItem.ContainsKey(typeId))
                    {
                        TypeItem[typeId] = new List<MenuItem>();
                    }
                    TypeItem[typeId].Add(item);
                }
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
