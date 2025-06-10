using Microsoft.EntityFrameworkCore;
using ProtosInterface.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ProtosInterface.Extensions;

namespace ProtosInterface;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    List<MenuItem> itemsToAdd = new List<MenuItem>();
    List<MenuItem> searchingItem = new List<MenuItem>();
    List<MenuItem> productsList = new List<MenuItem>();
    dbDataLoader dbLoader = new dbDataLoader();
    public Dictionary<MenuItem, string> EquipmentLsit { get; } = new Dictionary<MenuItem, string>();
    int itemid = -1;
    public MainWindow()
    {
        InitializeComponent();
    }

    static public MenuItem Menu_Create(int id)
    {
        TreeMenu menu = new TreeMenu(id);
        MenuItem item = menu.createMenu();
        return item;
    }

    private void SaveEdition_Click(object sender, RoutedEventArgs e)
    {
        SaveWindow message = new SaveWindow();
        message.Owner = this;
        message.ShowDialog();
        AppDbContext _context = new AppDbContext();
        MenuItem root = new MenuItem();
        List<ProductLink> links = new List<ProductLink>();
        List<Operation> operations = new List<Operation>();
        List<OperationVariant> operationVariants = new List<OperationVariant>();
        List<OperationVariantComponent> operationVariantComponents = new List<OperationVariantComponent>();
        switch (message.Result)
        {
            case SaveWindow.SaveOption.SaveAsNew:

                root = trvMenu.Items[0] as MenuItem;

                int lastItemId = _context.Products.GetLastId();

                var oldItem = _context.Products
                                      .FirstOrDefault(x => x.Id == root.Id);

                string name = root.Title;
                int count = NameCount(oldItem.Name, "product");

                RenameWindow rename = new RenameWindow(ReplaceNumberInBrackets(name, count));

                if(rename.ShowDialog() == true)
                {
                    name = rename.EnteredText;
                }
                else
                {
                    break;
                }

                count = NameCount(name, "product");
                
                var newProduct = new Product
                {
                    Id = lastItemId + 1,
                    Name = ReplaceNumberInBrackets(name, count),
                    TypeId = oldItem.TypeId,
                    CoopStatusId = oldItem.CoopStatusId,
                    Description = oldItem.Description,
                };

                _context.Products.Add(newProduct);

                foreach (MenuItem product in root.Items)
                {
                    Product productParent = _context.Products.Find(newProduct.Id)!;
                    Product includedProduct = _context.Products.Find(product.itemId)!;

                    if (includedProduct != null)
                    {
                        links.Add(new ProductLink
                        {
                            ParentProductId = newProduct.Id,
                            ParentProduct = productParent,
                            IncludedProductId = product.itemId,
                            IncludedProduct = includedProduct,
                            Amount = product.Amount
                        });
                    }
                }

                _context.ProductLinks.AddRange(links);

                var oldOperationList = _context.Operations.Where(o => o.ProductId == oldItem.Id).ToList();

                count = 0;

                foreach (MenuItem operation in OperationList.Items)
                {
                    lastItemId = _context.Operations.GetLastId();

                    int code = int.Parse(operation.Title.Split('|')[0].Trim());

                    //понадобиться ещё 1 похожий запрос
                    var equipments = _context.Equipment
                    .Join(
                        _context.OperationVariantComponents,
                        e => e.Id,
                        ovc => ovc.EquipmentId,
                        (e, ovc) => new { Equipment = e, OVC = ovc }
                    )
                    .Join(
                        _context.OperationVariants,
                        combined => combined.OVC.OperationVariantId,
                        ov => ov.Id,
                        (combined, ov) => new { combined.Equipment, OV = ov }
                        )
                    .Where(x => x.OV.OperationId == operation.Id)
                    .Select(x => new {
                        x.Equipment.Id,
                        x.Equipment.Name,
                        x.OV.Duration,
                        x.OV.Description,
                    })
                    .Distinct()
                    .ToList();
      
                    Operation oldOperation = _context.Operations.FirstOrDefault(x => x.Id == operation.Id)!;   
                        
                    if (oldOperation.TypeId == oldOperationList[count].TypeId)
                    {
                        operations.Add(new Operation
                        {
                            Id = lastItemId + 1 + count,
                            Code = oldOperation.Code,
                            TypeId = oldOperation.TypeId,
                            ProductId = newProduct.Id,
                            CoopStatusId = oldOperation.CoopStatusId,
                            Description = oldOperation.Description,
                        });
                    }
                    else
                    {
                        operations.Add(new Operation
                        {
                            Id = lastItemId + 1,
                            Code = oldOperation.Code,
                            TypeId = oldOperation.TypeId,
                            ProductId = newProduct.Id,
                            CoopStatusId = oldOperation.CoopStatusId,
                            Description = oldOperation.Description,
                        });
                    }

                    int operationcount = 0;

                    foreach (var equipment in equipments)
                    {
                        if (operationVariants.Count == 0)
                        {
                            lastItemId = _context.OperationVariants.GetLastId();
                        }
                        else
                        {
                            lastItemId = operationVariants[operationVariants.Count - 1].Id;
                        }

                        operationVariants.Add(new OperationVariant
                        {
                            Id = lastItemId + 1,
                            OperationId = operations[count].Id,
                            Duration = equipment.Duration,
                            Description = equipment.Description,
                        });

                        if (operationVariantComponents.Count == 0)
                        {
                            lastItemId = _context.OperationVariantComponents.GetLastId();
                        }
                        else
                        {
                            lastItemId = operationVariantComponents[operationVariantComponents.Count - 1].Id;
                        }

                        operationVariantComponents.Add(new OperationVariantComponent
                        {
                            Id = lastItemId + 1,
                            OperationVariantId = operationVariants[operationcount].Id,
                            EquipmentId = equipment.Id,
                            ProfessionId = equipment.Id,
                            WorkersAmount = 1,
                        });

                        operationcount++;
                    }
                    
                    count++;
                }

                _context.Operations.AddRange(operations);
                _context.OperationVariants.AddRange(operationVariants);
                _context.OperationVariantComponents.AddRange(operationVariantComponents);

                _context.SaveChanges();

                MessageBox.Show("Элемент был добавлен в БД");
                break;
            case SaveWindow.SaveOption.SaveChanges:
                root = trvMenu.Items[0] as MenuItem;
  
                _context.ProductLinks
                    .Where(pl => pl.ParentProductId == root.itemId)
                    .ExecuteDelete();

                foreach (MenuItem product in root.Items)
                {
                    Product productParent = _context.Products.Find(root.itemId)!;
                    Product includedProduct = _context.Products.Find(product.itemId)!;
                    links.Add(new ProductLink
                    {
                        ParentProductId = root.itemId,
                        ParentProduct = productParent,

                        IncludedProductId = product.itemId,
                        IncludedProduct = includedProduct,

                        Amount = product.Amount
                    });
                }

                _context.Operations.Where(o => o.ProductId == root.Id).ExecuteDelete();

                foreach(MenuItem operation in OperationList.Items)
                {
                    int typeId = 1;
                    operations.Add(new Operation
                    {
                        Id = operation.Id,
                        Code = int.Parse(operation.Title.Split('|')[0].Trim()),
                        TypeId = typeId,
                        ProductId = root.Id,
                        CoopStatusId = 1,
                        Description = "",
                    });
                }

                _context.ProductLinks.AddRange(links);
                _context.SaveChanges();
                MessageBox.Show("Сохранение завершено!");
                break;
            case SaveWindow.SaveOption.Cancel:
                break;
        }
    }

    public string ReplaceNumberInBrackets(string originalName, int newNumber)
    {
        string newName;
        string pattern = @"^\(\d+\)";

        if (originalName.Contains(pattern))
        {
            newName = Regex.Replace(originalName, pattern, $"({newNumber})");
        }
        else if (newNumber > 0)
        {
            newName = $"({newNumber}) " + originalName;
        }
        else
        {
            newName = originalName;
        }

        return newName;
    }

    public int NameCount(string name, string table)
    {
        int count = 0;
        AppDbContext _context = new AppDbContext();

        switch (table)
        {
            case "product":
                if (name.Split().Length > 1)
                {
                    count = _context.Products.Count(p => EF.Functions.Like(p.Name, $"%{name.Split()[1]}%"));
                }
                else
                {
                    count = _context.Products.Count(p => EF.Functions.Like(p.Name, $"%{name.Split()[0]}%"));
                }
            break;
        }

        return count;
    }

    private void CopyTreeItemButton_Click(object sender, RoutedEventArgs e)
    {
        var selectedItem = trvMenu.SelectedItem as MenuItem;

        TreeMenu.copyMenuItem(selectedItem);
    }

    private void InsertTreeItemButton_Click(object sender, RoutedEventArgs e)
    {
        
        var selectedItem = trvMenu.Items[0] as MenuItem;

        if (selectedItem != null)
        {
            var insertedItem = TreeMenu.InsertMenuItem();

            if (insertedItem.Id == selectedItem.Id)
            {
                MessageBox.Show("Нельзя копировать элемент в себя");
            }
            else
            {
                if (insertedItem != null)
                {
                    insertedItem.Parent = selectedItem;
                    selectedItem.Items.Add(insertedItem);
                }
            }
        }
        else
        {
            MessageBox.Show("Выберите элемент для вставки");
        }
    }

    private void AddNewTreeItemButton_Click(object sender, RoutedEventArgs e)
    {
        if (productsRadioButton.IsChecked == true)
        {
            ItemList list = new ItemList(itemsToAdd, true, (bool)productsRadioButton.IsChecked);

            if (list.ShowDialog() == true)
            {
                var selectedItem = trvMenu.Items[0] as MenuItem;

                if (selectedItem != null)
                {
                    foreach (MenuItem item in itemsToAdd)
                    {
                        item.Parent = selectedItem;

                        selectedItem.Items.Add(item);
                    }
                    
                    MessageBox.Show("Элементы добавлены");
                }
                else
                {
                    MessageBox.Show("Необходимо выбрать элемент, перед тем как что-то в него добавить");
                }
            }
        }
        else
        {
            ItemList list = new ItemList(itemsToAdd, true, (bool)productsRadioButton.IsChecked);

            if (list.ShowDialog() == true && trvMenu.Items[0] is MenuItem)
            {
                foreach (MenuItem item in itemsToAdd)
                {
                    string code = "";
                    if(list.enteredNumber < 10)
                    {
                        code += "0" + list.enteredNumber;
                    }
                    else
                    {
                        code += list.enteredNumber;
                    }
                    

                    OperationList.Items.Add(new MenuItem{ Title = code + " | " + item.Title, Id = item.Id });
                }
                ListSort(OperationList);

                MessageBox.Show("Операции добавлены");
            }
        }
        itemsToAdd.Clear();
    }

    private void DeleteTreeItemButton_Click(object sender, RoutedEventArgs e)
    {
        if (productsRadioButton.IsChecked == true)
        {
            var selectedItem = trvMenu.SelectedItem as MenuItem;

            if (selectedItem == null)
            {
                MessageBox.Show("Не выбран элемент для удаления");
            }
            else if (selectedItem.Parent != null)
            {
                selectedItem.Parent.Items.Remove(selectedItem);
            }
            else
            {
                MessageBox.Show("Нельзя удалять корневой элемент");
            }
        }
        else
        {
            var selectedItem = OperationList.SelectedItem as MenuItem;
            if (selectedItem == null)
            {
                MessageBox.Show("Не выбран элемент для удаления");
            }
            else
            {
                OperationList.Items.Remove(selectedItem);
            }
        }
    }

    private void SearchText_LostFocus(object sender, RoutedEventArgs e)
    {
        TextBox textBox = SearchTreeItem;

        if (string.IsNullOrWhiteSpace(textBox.Text))
        {
            textBox.Text = "Поиск";
            textBox.Foreground = Brushes.Gray;
        }
    }

    private void SearchText_GotFocus(object sender, RoutedEventArgs e)
    {
        TextBox textBox = SearchTreeItem;

        if (textBox.Text == "Поиск")
        {
            textBox.Text = "";
            textBox.Foreground = Brushes.Black;
        }
    }

    public static TreeViewItem GetTreeViewItem(ItemsControl parent, object item)
    {
        if (parent == null || item == null)
        {
            return null;
        }

        var container = parent.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
        if (container != null)
        {
            container.IsExpanded = true;
            return container;
        }

        foreach (var childItem in parent.Items)
        {
            var childContainer = parent.ItemContainerGenerator.ContainerFromItem(childItem) as ItemsControl;

            if (childContainer == null)
            {
                continue;
            }

            container = GetTreeViewItem(childContainer, item);

            if (container != null)
            {
                return container;
            }
        }

        return null;
    }

    private void trvMenu_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
        //var olditem = e.OldValue as MenuItem;
        //AppDbContext _context = new AppDbContext();
        ////возможно ещё минус 1
        //if (OperationList.Items.Count == _context.Operations.Select(x => x.ProductId == olditem.Id).Count())
        //{

        //}
        if (trvMenu.SelectedItem != null)
        {
            OperationList.ItemsSource = null;
            var selectedItem = trvMenu.SelectedItem as MenuItem;
            this.FillListItems("Operation", selectedItem.itemId);
            this.FillListItems("Equipment", (OperationList.Items[0] as MenuItem).Id);
            FullName.Text = ((MenuItem)trvMenu.SelectedItem).Title.ToString() + " полное название";
        }
    }

    private void OperationList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if(OperationList.SelectedItem != null)
        {
            EquipmentList.ItemsSource = null;
            var selectedItem = OperationList.SelectedItem as MenuItem;

            this.FillListItems("Equipment", selectedItem.Id);            
        }
    }

    private void FillListItems(string type, int request)
    {
        ListFill list = new ListFill();
        switch (type)
        {
            case "Operation":
                var newList = list.ItemOperations(request);
                OperationList.Items.Clear();

                foreach (MenuItem item in newList)
                {
                    OperationList.Items.Add(item as MenuItem);
                }

                //OperationList.DisplayMemberPath = "Title";

                break;
            case "Equipment":
                OnPropertyChanged(nameof(EquipmentList));
                EquipmentList.ItemsSource = list.OperationEquipment(request);
                break;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private void SearchPic_MouseDown(object sender, MouseButtonEventArgs e)
    {
        TextBox search = SearchTreeItem;

        if (search.Text.Length > 0 && !search.Text.Contains("Поиск"))
        {
            try
            {
                MenuItem root = trvMenu.Items[0] as MenuItem;
                ExpandAllTreeViewItems(trvMenu, true);
                ExpandAllTreeViewItems(trvMenu, false);
                var searchingItems = TreeMenu.MenuItemSearch(root, search.Text, "");
                SearchList searchItems = new SearchList(searchingItems, searchingItem);

                if (searchItems.ShowDialog() == true)
                {
                    var container = GetTreeViewItem(trvMenu, searchingItem[0]);
                    if (container != null)
                    {
                        container.IsExpanded = true;
                        container.BringIntoView();
                        container.IsSelected = true;
                        container.Focus();
                    }
                }
            }
            finally
            {
                searchingItem.Clear();
            }
        }
    }

    private void ExpandAllTreeViewItems(ItemsControl parent, bool expand)
    {
        if (parent == null) return;

        parent.UpdateLayout();
        
        foreach (var item in parent.Items)
        {
            if (parent.ItemContainerGenerator.ContainerFromItem(item) is TreeViewItem container)
            {
                container.IsExpanded = expand;

                if (container.Items.Count > 0)
                {
                    ExpandAllTreeViewItems(container, expand);
                }
            }
        }
    }

    private void productSelect_Click(object sender, RoutedEventArgs e)
    {
        ItemList list = new ItemList(itemsToAdd, false, productsRadioButton.IsChecked!.Value);

        if (list.ShowDialog() == true && itemsToAdd.Count != 0)
        {
            MenuItem item = itemsToAdd[0];

            if (item != null)
            {
                trvMenu.Items.Clear();
                trvMenu.Items.Add(Menu_Create(item.Id));
                itemid = item.Id;
                FullName.Text = ((MenuItem)trvMenu.Items[0]).Title.ToString() + " полное название";
                this.FillListItems("Operation", (trvMenu.Items[0] as MenuItem).itemId);
                this.FillListItems("Equipment", (OperationList.Items[0] as MenuItem).Id);
            }
        }
    }

    private void EditRadioButton_Checked(object sender, RoutedEventArgs e)
    {
        if (sender is RadioButton radioButton && radioButton.IsChecked == true && radioButton.Tag != null)
        {
            string colorString;
            if(radioButton.Tag?.ToString() == "Product")
            {
                colorString = "#c7f9f7";
                productSelect.IsEnabled = true;
                CopyTreeItemButton.IsEnabled = true;
                InsertTreeItemButton.IsEnabled = true;
            }
            else
            {
                colorString = "#ccf6d4";
                productSelect.IsEnabled = false;
                CopyTreeItemButton.IsEnabled = false;
                InsertTreeItemButton.IsEnabled = false;
            }
            if (!string.IsNullOrEmpty(colorString))
            {
                ColorChange(colorString);
            }
        }
    }

    public void ColorChange(string hexColor)
    {
        var brush = (SolidColorBrush)new BrushConverter().ConvertFromString(hexColor);

        var oldStyle = this.Resources["ButtonStyle"] as Style;

        if (oldStyle != null)
        {
            var newStyle = new Style(typeof(Button), oldStyle);

            newStyle.Setters.Add(new Setter(Control.BackgroundProperty, brush));

            this.Resources["ButtonStyle"] = newStyle;
        }
    }

    private void UpButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is MenuItem item)
        {
            // Логика перемещения вверх
            int index = OperationList.Items.IndexOf(item);

            if (index > 0)
            {
                if (index == 1)
                {
                    (OperationList.Items[index] as MenuItem).Title = $"0{index * 5} | " + (OperationList.Items[index] as MenuItem).Title.Split('|')[1].Trim();
                }
                else
                {
                    (OperationList.Items[index] as MenuItem).Title = $"{index * 5} | " + (OperationList.Items[index] as MenuItem).Title.Split('|')[1].Trim();
                }
                (OperationList.Items[index - 1] as MenuItem).Title = $"{(index + 1) * 5} | " + (OperationList.Items[index - 1] as MenuItem).Title.Split('|')[1].Trim();
                var buffer = OperationList.Items[index];
                OperationList.Items[index] = OperationList.Items[index - 1];
                OperationList.Items[index - 1] = buffer;
            }
        }
    }

    private void DownButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is MenuItem item)
        {
            // Логика перемещения вниз
            int index = OperationList.Items.IndexOf(item);

            if (index < OperationList.Items.Count - 1)
            {
                if (index == 0)
                {
                    (OperationList.Items[index + 1] as MenuItem).Title = $"0{(index + 1) * 5} | " + (OperationList.Items[index + 1] as MenuItem).Title.Split('|')[1].Trim();
                }
                else
                {
                    (OperationList.Items[index + 1] as MenuItem).Title = $"{(index + 1) * 5} | " + (OperationList.Items[index + 1] as MenuItem).Title.Split('|')[1].Trim();
                }
                (OperationList.Items[index] as MenuItem).Title = $"{(index + 2) * 5} | " + (OperationList.Items[index] as MenuItem).Title.Split('|')[1].Trim();
                var buffer = OperationList.Items[index];
                OperationList.Items[index] = OperationList.Items[index + 1];
                OperationList.Items[index + 1] = buffer;
            }
        }
    }

    private void ListSort(ListBox listBox)
    {
        var items = listBox.Items.Cast<MenuItem>()
            .OrderBy(item =>
            {
                // Разбираем номер из формата "XX | название"
                string numberPart = item.Title.Split('|').First().Trim();
                return int.TryParse(numberPart, out int num) ? num : int.MaxValue;
            })
            .ToList();

        // Очищаем и добавляем отсортированные элементы
        listBox.Items.Clear();
        foreach (var item in items)
        {
            listBox.Items.Add(item);
        }
    }
}