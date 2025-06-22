using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
//using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ProtosInterface
{
    /// <summary>
    /// Логика взаимодействия для ExportWindow.xaml
    /// </summary>
    public partial class ExportWindow : Window
    {
        private List<MenuItem> products;
        private ListBox operations;
        private MenuItem productsRoot;
        

        public ExportWindow(MenuItem productsRoot, List<MenuItem> products)
        {
            InitializeComponent();
            this.products = products;
            this.productsRoot = productsRoot;
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<string> operations = new List<string>();

                AppDbContext _context = new AppDbContext();

                var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "Excel Files|*.xlsx",
                    FileName = $"Изделие {productsRoot.ShortName}"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    using (var package = new ExcelPackage())
                    {
                        int row = 2, column = 2;

                        var worksheet = package.Workbook.Worksheets.Add("ExportedData");

                        worksheet.Cells[row, column].Value = "Изделие:";

                        FillList(worksheet, productsRoot.Title, ++row, column);

                        if (OperationCheck.IsChecked == false && EquipmentCheck.IsChecked == false)
                        {
                            row += 2;
                            worksheet.Cells[row, column].Value = "Список деталей:";

                            foreach (var item in products)
                            {
                                FillList(worksheet, item.Title, ++row, column);
                            }
                        }
                        else if (OperationCheck.IsChecked == true && EquipmentCheck.IsChecked == false)
                        {
                            row++;
                            operations = _context.Operations
                                .Where(o => o.ProductId == productsRoot.Id)
                                .Select(o => o.OperationType.Name).ToList();
                            FillList(worksheet, operations, row, column);

                            row += operations.Count + 3;

                            worksheet.Cells[row, column].Value = "Список деталей:";

                            foreach (var item in products)
                            {
                                operations = _context.Operations
                                .Where(o => o.ProductId == item.Id)
                                .Select(o => o.OperationType.Name).ToList();

                                FillList(worksheet, item.Title, ++row, column);

                                FillList(worksheet, operations, ++row, column);

                                row += operations.Count + 1;
                            }
                        }
                        else
                        {
                            row++;

                            operations = _context.Operations
                                .Where(o => o.ProductId == productsRoot.Id)
                                .Select(o => o.OperationType.Name).ToList();
                            FillList(worksheet, operations, row, column);

                            row += operations.Count + 3;

                            worksheet.Cells[row, column].Value = "Список деталей:";

                            foreach (var item in products)
                            {
                                operations = _context.Operations
                                .Where(o => o.ProductId == item.Id)
                                .Select(o => o.OperationType.Name).ToList();

                                FillList(worksheet, item.Title, ++row, column);

                                FillList(worksheet, operations, ++row, column);

                                List<int> operationsId = _context.Operations
                                .Where(o => o.ProductId == item.Id)
                                .Select(o => o.Id).ToList();

                                List<List<string>> equipments = new List<List<string>>();

                                foreach (var id in operationsId)
                                {
                                    equipments.Add(_context.Equipment
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
                                    .Where(x => x.OV.OperationId == id)
                                    .Select(x => x.Equipment.Name.ToString())
                                    .Distinct()
                                    .ToList());
                                }

                                FillList(worksheet, equipments, row, ++column);

                                column--;
                                row += operations.Count + 2;
                            }
                        }

                        for (int j = 1; j < 20; j++)
                        {
                            worksheet.Column(j).AutoFit();
                        }

                        package.SaveAs(new FileInfo(saveFileDialog.FileName));
                        MessageBox.Show("Экспорт завершен успешно!");
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка экспорта: {ex.Message}");
            }
        }

        private void FillList(ExcelWorksheet worksheet, string product, int row, int column)
        {
            worksheet.Cells[row, column].Value = product;
            worksheet.Cells[row, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells[row, column].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(200,200,255));
        }

        private void FillList(ExcelWorksheet worksheet, List<string> productOperation, int row, int column)
        {
            worksheet.Cells[row, column].Value = "Операции:";

            for (int i = 0; i < productOperation.Count; i++)
            {
                worksheet.Cells[row + i + 1, column].Value = productOperation[i].ToString();
                worksheet.Cells[row + i + 1, column].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[row + i + 1, column].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(200, 255, 200));
            }
        }

        private void FillList(ExcelWorksheet worksheet, List<List<string>> operationEquipment, int row, int column)
        {
            if (EquipmentCheck.IsChecked == true)
            {
                worksheet.Cells[row, column].Value = "Оборудование:";

                for (int i = 0; i < operationEquipment.Count; i++)
                {
                    for (int j = 0; j < operationEquipment[i].Count; j++)
                    {
                        worksheet.Cells[row + 1 + i, column + j].Value = operationEquipment[i][j].ToString();
                    }
                }
            }
        }

        private void OperationCheck_Checked(object sender, RoutedEventArgs e)
        {
            EquipmentCheck.IsChecked = true;
            EquipmentCheck.IsEnabled = true;
        }

        private void OperationCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            EquipmentCheck.IsChecked = false;
            EquipmentCheck.IsEnabled = false;
        }
    }
}
