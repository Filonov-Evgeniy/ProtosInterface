using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Логика взаимодействия для ExportWindow.xaml
    /// </summary>
    public partial class ExportWindow : Window
    {
        private List<MenuItem> products;
        private ListBox operations;
        private MenuItem productsRoot;

        public ExportWindow(List<MenuItem> products, ListBox operations, MenuItem productsRoot)
        {
            InitializeComponent();
            this.products = products;
            this.operations = operations;
            this.productsRoot = productsRoot;
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<MenuItem> equipments = new List<MenuItem>();

                var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "Excel Files|*.xlsx",
                    FileName = $"Изделие {productsRoot.ShortName}"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    using (var package = new ExcelPackage())
                    {
                        var worksheet = package.Workbook.Worksheets.Add("ExportedData");

                        ExportListToExcel(worksheet, products, 6, 2);
                        worksheet.Cells[2, 2].Value = "Изделие";
                        worksheet.Cells[3, 2].Value = productsRoot.Title.ToString();
                        worksheet.Cells[5, 2].Value = "Список детали";

                        if (OperationCheck.IsChecked == true)
                        {
                            ExportListBoxToColumn(worksheet, operations, 6, 4);
                            worksheet.Cells[5, 4].Value = "Операции";
                        }

                        if (EquipmentCheck.IsChecked == true)
                        {
                            //foreach (var operation in operations.Items)
                            //{
                            //    var equipment = _context.EquipmentAdd
                            //                            .Join(
                            //                                _context.OperationVariantComponents,
                            //                                e => e.Id,
                            //                                ovc => ovc.EquipmentId,
                            //                                (e, ovc) => new { Equipment = e, OVC = ovc }
                            //                            )
                            //                            .Join(
                            //                                _context.OperationVariants,
                            //                                combined => combined.OVC.OperationVariantId,
                            //                                ov => ov.Id,
                            //                                (combined, ov) => new { combined.Equipment, OV = ov }
                            //                                )
                            //                            .Where(x => x.OV.OperationId == operation.Id)
                            //                            .Select(x => new {
                            //                                x.Equipment.Id,
                            //                                x.Equipment.Name,
                            //                                x.OV.Duration,
                            //                                x.OV.Description,
                            //                            })
                            //                            .Distinct()
                            //                            .ToList();
                            //}
                           // ExportListBoxToColumn(worksheet, equipments, 6, 6);
                            worksheet.Cells[5, 6].Value = "Оборудование";
                        }

                        for (int j = 1; j < 10; j++)
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

        private void ExportListBoxToColumn(ExcelWorksheet worksheet, ListBox listBox, int row, int column)
        {
            for (int i = 0; i < listBox.Items.Count; i++)
            {
                if (listBox.Items[i] == products[0])
                {
                    continue;
                }
                worksheet.Cells[i + row, column].Value = (listBox.Items[i] as MenuItem)!.Title.ToString();
            }
        }

        private void ExportListToExcel(ExcelWorksheet worksheet, List<MenuItem> listBox, int row, int column)
        {
            for (int i = 0; i < listBox.Count; i++)
            {
                worksheet.Cells[i + row, column].Value = listBox[i].Title;
            }
        }

        private void CheckOperation_Checked(object sender, RoutedEventArgs e)
        {
            //OperationExport.IsEnabled = true;
        }

        private void CheckEquipment_Checked(object sender, RoutedEventArgs e)
        {
            //EquipmentExport.IsEnabled = true;
        }
    }
}
