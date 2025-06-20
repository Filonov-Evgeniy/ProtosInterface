﻿using Microsoft.EntityFrameworkCore;
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
        private List<CheckBox> formCheckboxes = new List<CheckBox>();
        private List<MenuItem> dataList = new List<MenuItem>();
        private List<MenuItem> dataListFull = new List<MenuItem>();
        private Dictionary<CheckBox, List<MenuItem>> checkboxQueries = new Dictionary<CheckBox, List<MenuItem>>();
        private Dictionary<CheckBox, int> checkboxCode = new Dictionary<CheckBox, int>(); 
        dbDataLoader loader = new dbDataLoader();

        private List<MenuItem> itemList = new List<MenuItem>();
        private bool type;
        public int enteredNumber;
        AppDbContext _context = new AppDbContext();

        public ItemList(List<MenuItem> itemList, bool operation, bool type)
        {
            InitializeComponent();
            getCheckboxes();
            ItemListBox.Visibility = Visibility.Collapsed;
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

        private void getCheckboxes()
        {
            formCheckboxes.Add(StandartCheckBox);
            formCheckboxes.Add(ProductCheckBox);
            formCheckboxes.Add(DefaultCheckBox);
            formCheckboxes.Add(DetailCheckBox);
            formCheckboxes.Add(OtherCheckBox);

            checkboxCode.Add(StandartCheckBox, 2);
            checkboxCode.Add(ProductCheckBox, 0);
            checkboxCode.Add(DefaultCheckBox, -1);
            checkboxCode.Add(DetailCheckBox, 1);
            checkboxCode.Add(OtherCheckBox, 3);
        }

        private async void LoadDataAsync()
        {
            try
            {
                LoadingIndicator.Visibility = Visibility.Visible;
                AllItems.Clear();

                if (type == true)
                {
                    dataList = await loader.getProductData(dataListFull);
                    Grid.SetRowSpan(ItemListBox, 2);
                }
                else
                {
                    FilterField1.Visibility = Visibility.Collapsed;
                    FilterField2.Visibility = Visibility.Collapsed;
                    Grid.SetRowSpan(ItemListBox, 4);
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

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var checkbox = (CheckBox)sender;
            int checkedBoxes = 0;
            foreach (CheckBox check in formCheckboxes)
            {
                if (check.IsChecked == true)
                {
                    checkedBoxes++;
                }
            }
            
            if (checkedBoxes == 1)
            {
                dataList.Clear();
                ItemListBox.Items.Refresh();
            }

            if (checkboxQueries.ContainsKey(checkbox))
            {
                dataList.AddRange(checkboxQueries[checkbox]);
                ItemListBox.Items.Refresh();
            }
            else
            {
                List<MenuItem> item = loader.getProductByTypeId(checkboxCode[checkbox]);
                checkboxQueries.Add(checkbox, item);
                dataList.AddRange(item);
                ItemListBox.Items.Refresh();
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            int checkedCount = 0;
            dataList.Clear();
            foreach (CheckBox checkbox in formCheckboxes)
            {
                if (checkbox.IsChecked == true)
                {
                    checkedCount++;
                }
            }

            if (checkedCount == 0)
            {
                dataList.AddRange(dataListFull);
            }
            else
            {
                foreach (CheckBox checkbox in formCheckboxes)
                {
                    if (checkbox.IsChecked == true)
                    {
                        dataList.AddRange(checkboxQueries[checkbox]);
                    }
                }
            }
            ItemListBox.Items.Refresh();
        }

        private void SearchPic_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string searchingItem = SearchTextBox.Text;
            if (searchingItem.Trim().Length > 0)
            {
                List<MenuItem> searchItems = new List<MenuItem>();
                foreach (MenuItem item in dataList)
                {
                    if (item.Title.ToLower().Contains(searchingItem.ToLower()))
                    {
                        searchItems.Add(item);
                    }
                }
                ItemListBox.ItemsSource = searchItems;
                ItemListBox.Items.Refresh();
            }
        }

        private void ClearSearchButton_Click(object sender, RoutedEventArgs e)
        {
            ItemListBox.ItemsSource = dataList;
            ItemListBox.Items.Refresh();
        }
    }
}
