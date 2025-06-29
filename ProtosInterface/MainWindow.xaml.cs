using Microsoft.EntityFrameworkCore;
using ProtosInterface.Extensions;
using ProtosInterface.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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
using OfficeOpenXml;

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
    AppDbContext _context;
    public Dictionary<MenuItem, string> EquipmentLsit { get; } = new Dictionary<MenuItem, string>();
    int itemid = -1;
    public MainWindow()
    {
        InitializeComponent();
        _context = new AppDbContext();
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
        MenuItem root = new MenuItem();
        List<ProductLink> links = new List<ProductLink>();
        List<Operation> operations = new List<Operation>();
        List<OperationVariant> operationVariants = new List<OperationVariant>();
        List<OperationVariantComponent> operationVariantComponents = new List<OperationVariantComponent>();
        switch (message.Result)
        {
            case SaveWindow.SaveOption.SaveAsNew:

                if (trvMenu.Items[0] is not MenuItem)
                {
                    MessageBox.Show("Перед сохранением выберите изделие");
                    break;
                }
                root = trvMenu.Items[0] as MenuItem;

                int lastItemId = _context.Products.GetLastId();

                var rootItem = _context.Products
                                      .FirstOrDefault(x => x.Id == root.Id);

                string name = root.Title;
                int count = NameCount(rootItem.Name, "product");

                RenameWindow rename = new RenameWindow(ReplaceNumberInBrackets(name, count));

                if (rename.ShowDialog() == true)
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
                    TypeId = rootItem.TypeId,
                    CoopStatusId = rootItem.CoopStatusId,
                    Description = rootItem.Description,
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

                var container = GetTreeViewItem(trvMenu, trvMenu.Items[0]);
                if (container != null)
                {
                    container.IsExpanded = true;
                    container.BringIntoView();
                    container.IsSelected = true;
                    container.Focus();
                }

                List<int> operationsTypeId = new List<int>();
                List<int> operationsCode = new List<int>();

                MenuItem firstOperation = OperationList.Items[0] as MenuItem;
                if (firstOperation.Title != "Операций нет")
                {
                    foreach (MenuItem operation in OperationList.Items)
                    {
                        string[] operationSplitted = operation.Title.Split('|', StringSplitOptions.RemoveEmptyEntries);
                        operationsCode.Add(Convert.ToInt32(operationSplitted[0].Trim()));
                        operationsTypeId.Add(getOperationTypeIdByName(operationSplitted[1].Trim()));
                    }

                    int operationsId = _context.Operations.GetLastId();
                    Dictionary<int, List<Operation>> originalCopyOperations = new Dictionary<int, List<Operation>>();

                    for (int i = 0; i < operationsTypeId.Count; i++)
                    {
                        int operationId = getOperationByTypeId(operationsTypeId[i]);
                        Operation newOperation = getOperationFromReference(operationId, operationsCode[i], operationsId, _context.Products.GetLastId() + 1);

                        if (originalCopyOperations.ContainsKey(operationId))
                        {
                            originalCopyOperations[operationId].Add(newOperation);
                        }
                        else
                        {
                            originalCopyOperations.Add(operationId, new List<Operation> { newOperation });
                        }

                        operationsId++;
                    }

                    foreach (var operation in originalCopyOperations.Values)
                    {
                        _context.Operations.AddRange(operation);
                    }

                    Dictionary<int, Dictionary<OperationVariant, List<OperationVariant>>> variantIdAndVariantForCopy = getOperationVariantListFromReference(originalCopyOperations);

                    foreach (var outerPair in variantIdAndVariantForCopy)
                    {
                        foreach (var innerPair in outerPair.Value)
                        {
                            _context.OperationVariants.AddRange(innerPair.Value);
                        }
                    }

                    List<OperationVariantComponent> newOpVarComp = getOperationVariantComponentFromReference(variantIdAndVariantForCopy);
                    _context.OperationVariantComponents.AddRange(newOpVarComp);
                }

                _context.SaveChanges();

                MessageBox.Show("Сохранение завершено!");
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

                //List<Operation> operationsList = _context.Operations.Where(x=>x.ProductId == root.itemId).ToList();

                //Dictionary<int, List<OperationVariant>> operationVariantList = new Dictionary<int, List<OperationVariant>>();

                //for (int i = 0; i < operationsList.Count; i++)
                //{

                //    operationVariantList.Add(operationsList[i].Id, _context.OperationVariants.Where(x => x.OperationId == operationsList[i].Id).ToList());

                //    //operationVariantList[i] = _context.OperationVariants.Where(x => x.OperationId == operationsList[i].Id).ToList();
                //}

                //Dictionary<int, List<OperationVariantComponent>> operationVariantComponentList = new Dictionary<int, List<OperationVariantComponent>>();

                //foreach (var item in operationVariantList)
                //{
                //    for (int i = 0; i < item.Value.Count; i++)
                //    {
                //        operationVariantComponentList.Add(item.Value[i].Id, _context.OperationVariantComponents.Where(x => x.OperationVariantId == item.Value[i].Id).ToList());
                //    }
                //    //operationVariantComponentList[i] = _context.OperationVariantComponents.Where(x => x.OperationVariantId == operationVariantList[i]).ToList();
                //}

                //var treeContainer = GetTreeViewItem(trvMenu, trvMenu.Items[0]);
                //if (treeContainer != null)
                //{
                //    treeContainer.IsExpanded = true;
                //    treeContainer.BringIntoView();
                //    treeContainer.IsSelected = true;
                //    treeContainer.Focus();
                //}

                //List<Operation> operationsToAdd = new List<Operation>();

                //foreach (MenuItem operation in OperationList.Items)
                //{
                //    Operation operationOld = _context.Operations.Where(x => x.Id == operation.Id).FirstOrDefault();
                //    Operation newOperation = new Operation();

                //    if (operationOld == null)
                //    {
                //        int opId = _context.OperationTypes.Where(l => l.Name == operation.itemName).FirstOrDefault().Id;
                //        operationOld = _context.Operations.Where(x => x.TypeId == opId).FirstOrDefault();
                //        newOperation.Id = _context.Operations.GetLastId() + 1;
                //        newOperation.ProductId = root.Id;
                //        newOperation.Code = int.Parse(operation.Title.Split('|')[0].Trim());
                //        newOperation.OperationType = operationOld.OperationType;
                //        newOperation.CoopStatusId = operationOld.CoopStatusId;
                //        newOperation.TypeId = operationOld.TypeId;
                //        newOperation.Description = operationOld.Description;
                //    }
                //    else
                //    {
                //        newOperation.Id = operation.Id;
                //        newOperation.ProductId = root.Id;
                //        newOperation.Code = int.Parse(operation.Title.Split('|')[0].Trim());
                //        newOperation.OperationType = operationOld.OperationType;
                //        newOperation.CoopStatusId = operationOld.CoopStatusId;
                //        newOperation.TypeId = operationOld.TypeId;
                //        newOperation.Description = operationOld.Description;
                //    }
                //    operationsToAdd.Add(newOperation);
                //}

                //_context.Operations.Where(o => o.ProductId == root.Id).ExecuteDelete();

                //foreach (MenuItem operation in OperationList.Items)
                //{
                //    if (operation.Title == "Операций нет")
                //    {
                //        continue;
                //    }
                //    int typeId = 1;
                //    operations.Add(new Operation
                //    {
                //        Id = operation.Id,
                //        Code = int.Parse(operation.Title.Split('|')[0].Trim()),
                //        TypeId = typeId,
                //        ProductId = root.Id,
                //        CoopStatusId = 1,
                //        Description = "",
                //    });
                //}

                _context.ProductLinks.AddRange(links);

                SaveOperation(((MenuItem)trvMenu.Items[0]).Id);

                //_context.Operations.AddRange(operations);

                _context.SaveChanges();
                MessageBox.Show("Сохранение завершено!");
                break;
            case SaveWindow.SaveOption.Cancel:
                break;
        }
    }

    public void SaveOperation(int productId)
    {
        List<int> operationsTypeId = new List<int>();
        List<int> operationsCode = new List<int>();
        List<Operation> writtenOperations = new List<Operation>();

        MenuItem firstOperation = OperationList.Items[0] as MenuItem;
        if (firstOperation.Title != "Операций нет")
        {
            foreach (MenuItem operation in OperationList.Items)
            {
                string[] operationSplitted = operation.Title.Split('|', StringSplitOptions.RemoveEmptyEntries);

                if (operation.Id != 0)
                {
                    Operation operationWritten = getWrittenOperation(operationSplitted[0], operation.Id);
                    //вот тут список операций
                    writtenOperations.Add(operationWritten);
                }
                else
                {
                    operationsCode.Add(Convert.ToInt32(operationSplitted[0].Trim()));
                    operationsTypeId.Add(getOperationTypeIdByName(operationSplitted[1].Trim()));
                }
            }

            int operationsId = _context.Operations.GetLastId();
            Dictionary<int, List<Operation>> originalCopyOperations = new Dictionary<int, List<Operation>>();

            for (int i = 0; i < operationsTypeId.Count; i++)
            {
                int operationId = getOperationByTypeId(operationsTypeId[i]);
                Operation newOperation = getOperationFromReference(operationId, operationsCode[i], operationsId, productId);

                if (originalCopyOperations.ContainsKey(operationId))
                {
                    originalCopyOperations[operationId].Add(newOperation);
                }
                else
                {
                    originalCopyOperations.Add(operationId, new List<Operation> { newOperation });
                }

                operationsId++;
            }

            Dictionary<int, Dictionary<OperationVariant, List<OperationVariant>>> variantIdAndVariantForCopy = getOperationVariantListFromReference(originalCopyOperations);
            List<OperationVariantComponent> newOpVarComp = getOperationVariantComponentFromReference(variantIdAndVariantForCopy);

            List<OperationVariant> oldOV = new List<OperationVariant>();
            List<OperationVariantComponent> oldOVC = new List<OperationVariantComponent>();

            foreach (var operation in writtenOperations)
            {
                oldOV.AddRange(_context.OperationVariants.Where(ov => ov.OperationId == operation.Id).ToList());
            }

            foreach (var variant in oldOV)
            {
                oldOVC.AddRange(_context.OperationVariantComponents.Where(ovc => ovc.OperationVariantId == variant.Id));
            }

            DeleteOldDataFromDB();

            foreach (var operation in originalCopyOperations.Values)
            {
                _context.Operations.AddRange(operation);
            }

            _context.Operations.AddRange(writtenOperations);

            foreach (var outerPair in variantIdAndVariantForCopy)
            {
                foreach (var innerPair in outerPair.Value)
                {
                    _context.OperationVariants.AddRange(innerPair.Value);
                }
            }

            _context.OperationVariants.AddRange(oldOV);

            _context.OperationVariantComponents.AddRange(newOpVarComp);

            _context.OperationVariantComponents.AddRange(oldOVC);
        }
    }

    private void DeleteOldDataFromDB()
    {
        int rootId = (trvMenu.Items[0] as MenuItem).Id;
        List<Operation> operationsToDelete = _context.Operations.Where(x => x.ProductId == rootId).ToList();
        List<OperationVariant> operationVariantsToDelete = getOpVariantsToDelete(operationsToDelete);
        List<OperationVariantComponent> operationVariantComponentsToDelete = getOperationVariantComponentToDelete(operationVariantsToDelete);

        _context.OperationVariantComponents.RemoveRange(operationVariantComponentsToDelete);
        _context.OperationVariants.RemoveRange(operationVariantsToDelete);
        _context.Operations.RemoveRange(operationsToDelete);

        _context.SaveChanges();
    }

    private List<OperationVariantComponent> getOperationVariantComponentToDelete(List<OperationVariant> operationVariantComponentsToDelete)
    {
        List<OperationVariantComponent> operationVariantComponentToDelete = new List<OperationVariantComponent>();
        foreach (OperationVariant operationVariant in operationVariantComponentsToDelete)
        {
            operationVariantComponentToDelete.AddRange(_context.OperationVariantComponents.Where(x => x.OperationVariantId == operationVariant.Id).ToList());
        }
        return operationVariantComponentToDelete;
    }

    private List<OperationVariant> getOpVariantsToDelete(List<Operation> operationsToDelete)
    {
        List<OperationVariant> operationVariantsToDelete = new List<OperationVariant>();
        foreach (Operation operation in operationsToDelete)
        {
            operationVariantsToDelete.AddRange(_context.OperationVariants.Where(x=>x.OperationId==operation.Id).ToList());
        }
        return operationVariantsToDelete;
    }

    private Operation getWrittenOperation(string code, int id)
    {
        Operation operation = _context.Operations.Where(x=>x.Id==id).FirstOrDefault();
        if (int.TryParse(code, out int intCode))
        {
            operation.Code = intCode;
        }
        return operation;
    }

    private List<OperationVariantComponent> getOperationVariantComponentFromReference(Dictionary<int, Dictionary<OperationVariant, List<OperationVariant>>> variantIdAndVariantForCopy)
    {
        int opVarCompId = _context.OperationVariantComponents.GetLastId();
        List<OperationVariantComponent> operationVariantComponentsCopy = new List<OperationVariantComponent>();

        foreach (var variants in variantIdAndVariantForCopy.Values)
        {
            foreach (var dict in variants)
            {
                List<OperationVariantComponent> oldComp = new List<OperationVariantComponent>();
                oldComp = _context.OperationVariantComponents.Where(x => x.OperationVariantId == dict.Key.Id).Select(x => new OperationVariantComponent { Id = x.Id, EquipmentId = x.EquipmentId }).ToList();
                foreach (OperationVariantComponent comp in oldComp)
                {
                    foreach (var item in dict.Value)
                    {
                        OperationVariantComponent newComp = new OperationVariantComponent();
                        newComp.ProfessionId = comp.ProfessionId;
                        newComp.EquipmentId = comp.EquipmentId;
                        newComp.Equipment = comp.Equipment;
                        newComp.OperationVariantId = item.Id;
                        newComp.OperationVariant = item;
                        operationVariantComponentsCopy.Add(newComp);
                        newComp.Id = opVarCompId + 1;
                        opVarCompId++;
                    }
                }
                oldComp.Clear();
            }
        }

        return operationVariantComponentsCopy;
    }

    private Dictionary<int, Dictionary<OperationVariant, List<OperationVariant>>> getOperationVariantListFromReference(Dictionary<int, List<Operation>> originalCopyOperations)
    {
        int opVarId = _context.OperationVariants.GetLastId();
        Dictionary<int, Dictionary<OperationVariant, List<OperationVariant>>> modifiedVariants = new Dictionary<int, Dictionary<OperationVariant, List<OperationVariant>>>();

        foreach (int operationId in originalCopyOperations.Keys)
        {
            List<OperationVariant> variantForCopy = _context.OperationVariants.Where(x => x.OperationId == operationId).ToList();
            Dictionary<OperationVariant, List<OperationVariant>> oldNewOpVariants = new Dictionary<OperationVariant, List<OperationVariant>>();

            foreach (OperationVariant variant in variantForCopy)
            {
                for (int i = 0; i < originalCopyOperations[operationId].Count; i++)
                {
                    OperationVariant newVariant = new OperationVariant();
                    newVariant.Duration = variant.Duration;
                    newVariant.OperationId = originalCopyOperations[variant.OperationId][i].Id;
                    newVariant.Id = opVarId + 1;

                    if (oldNewOpVariants.ContainsKey(variant))
                    {
                        oldNewOpVariants[variant].Add(newVariant);
                    }
                    else
                    {
                        oldNewOpVariants.Add(variant, new List<OperationVariant> { newVariant });
                    }

                    opVarId++;
                }
            }

            modifiedVariants.Add(operationId, oldNewOpVariants);
        }

        return modifiedVariants;
    }

    private Operation getOperationFromReference(int referenceId, int code, int id, int productId)
    {
        Operation operation = new Operation();
        Operation referenceOperation = _context.Operations.FirstOrDefault(x => x.Id == referenceId);
        operation.OperationType = referenceOperation.OperationType;
        operation.TypeId = referenceOperation.TypeId;
        operation.Code = code;
        operation.ProductId = productId;
        operation.CoopStatusId = referenceOperation.CoopStatusId;
        operation.Id = id + 1;
        return operation;
    }

    private int getOperationByTypeId(int typeId)
    {
        int operationId = _context.Operations.FirstOrDefault(x => x.TypeId == typeId).Id;
        return operationId;
    }

    private int getOperationTypeIdByName(string name)
    {
        int operationId = _context.OperationTypes.FirstOrDefault(x => x.Name == name).Id;
        return operationId;
    }

    public string ReplaceNumberInBrackets(string originalName, int newNumber)
    {
        string pattern = @"^\(\d+\)";

        if (Regex.IsMatch(originalName, pattern))
        {
            return Regex.Replace(originalName, pattern, $"({newNumber})");
        }
        else if (newNumber > 0)
        {
            return $"({newNumber}) {originalName}";
        }

        return originalName;
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
            if (trvMenu.SelectedItem == trvMenu.Items[0]) 
            {
                if (list.ShowDialog() == true && trvMenu.Items[0] is MenuItem)
                {
                    foreach (MenuItem item in itemsToAdd)
                    {
                        string code = "";
                        if (list.enteredNumber < 10)
                        {
                            code += "0" + list.enteredNumber;
                        }
                        else
                        {
                            code += list.enteredNumber;
                        }

                        bool duplicatedCode = false;

                        foreach (MenuItem operationItem in OperationList.Items)
                        {
                            string[] opName = operationItem.Title.Trim().Split('|', StringSplitOptions.RemoveEmptyEntries);
                            if (code == opName[0].Trim())
                            {
                                duplicatedCode = true;
                                break;
                            }
                        }

                        if (!duplicatedCode)
                        {
                            OperationList.Items.Add(new MenuItem { Title = code + " | " + item.Title, Id = 0 });
                            MessageBox.Show("Операции добавлены");
                        }
                        else
                        {
                            MessageBox.Show("На указанной позиции уже есть операция");
                        }
                    }
                    ListSort(OperationList);

                    if (OperationList.Items.Count == 2)
                    {
                        for (int i = OperationList.Items.Count - 1; i >= 0; i--)
                        {
                            if (OperationList.Items[i] is MenuItem item && item.Title == "Операций нет")
                            {
                                OperationList.Items.RemoveAt(i);
                            }
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Нельзя добавить операции не в корневой элемент");
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
            int index = OperationList.Items.IndexOf(selectedItem);
            if (selectedItem == null)
            {
                MessageBox.Show("Не выбран элемент для удаления");
            }
            else
            {
                OperationList.Items.Remove(selectedItem);
                for (int i = index; i < OperationList.Items.Count; i++)
                {
                    int code = int.Parse((OperationList.Items[i] as MenuItem).Title.ToString().Split('|')[0]);
                    if (code <= 5)
                    {
                        code = index + 1;
                    }
                    else
                    {
                        code -= 5;
                    }
                    if (code < 10)
                    {
                        (OperationList.Items[i] as MenuItem).Title = "0" + code + " | " + (OperationList.Items[i] as MenuItem).Title.ToString().Split('|')[1].Trim();
                    }
                    else
                    {
                        (OperationList.Items[i] as MenuItem).Title = code + " | " + (OperationList.Items[i] as MenuItem).Title.ToString().Split('|')[1].Trim();
                    }
                }
                OperationList.Items.Refresh();
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
            EquipmentList.ItemsSource = null;
            var selectedItem = trvMenu.SelectedItem as MenuItem;
            this.FillListItems("Operation", selectedItem.itemId);
            if (OperationList.Items.Count > 0)
            {
                this.FillListItems("Equipment", (OperationList.Items[0] as MenuItem).Id);
            }
            else
            {
                OperationList.Items.Add(new MenuItem { Title = "Операций нет" });
            }
            if (selectedItem.Parent == null)
            {
                FullName.Text = selectedItem.Title.ToString();
            }
            else
            {
                FullName.Text = selectedItem.Title.ToString() + " | " + selectedItem.Amount.ToString() + " ед.";
            }
        }
    }

    private void OperationList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (OperationList.SelectedItem != null)
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
                OperationList.ItemsSource = null;
                EquipmentList.ItemsSource = null;
                EquipmentList.Items.Refresh();
                trvMenu.Items.Add(Menu_Create(item.Id));
                itemid = item.Id;
                FullName.Text = ((MenuItem)trvMenu.Items[0]).Title.ToString();
                this.FillListItems("Operation", (trvMenu.Items[0] as MenuItem).itemId);
                if (OperationList.Items.Count > 0)
                {
                    this.FillListItems("Equipment", (OperationList.Items[0] as MenuItem).Id);
                }
                else
                {
                    OperationList.Items.Add(new MenuItem { Title = "Операций нет" });
                }
            }
        }
    }

    private void EditRadioButton_Checked(object sender, RoutedEventArgs e)
    {
        if (sender is RadioButton radioButton && radioButton.IsChecked == true && radioButton.Tag != null)
        {
            string colorString;
            if (radioButton.Tag?.ToString() == "Product")
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
                string numberPart = item.Title.Split('|').First().Trim();
                return int.TryParse(numberPart, out int num) ? num : int.MaxValue;
            })
            .ToList();

        listBox.Items.Clear();
        foreach (var item in items)
        {
            listBox.Items.Add(item);
        }
    }

    private void ExportButton_Click(object sender, RoutedEventArgs e)
    {
        if (trvMenu.Items.Count == 0)
        {
            MessageBox.Show("Выберите изделие для дерева");
            return;
        }
        if (trvMenu.Items[0] is MenuItem)
        {
            productsList.Clear();
            foreach (MenuItem item in (trvMenu.Items[0] as MenuItem)!.Items)
            {
                productsList.Add(item);
            }

            ExportWindow export = new ExportWindow((MenuItem)trvMenu.Items[0], productsList);
            export.ShowDialog();
        }
        else
        {
            MessageBox.Show("Выберите элемент для экспорта");
        }
    }

    private void createProductButton_Click(object sender, RoutedEventArgs e)
    {
        ProductCreator productCreator = new ProductCreator();
        productCreator.ShowDialog();
    }
}