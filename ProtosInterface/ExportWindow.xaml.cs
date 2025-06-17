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
        private ListBox equipments;


        public ExportWindow(List<MenuItem> products, ListBox operations, ListBox equipments)
        {
            InitializeComponent();
            this.products = products;
            this.operations = operations;
            this.equipments = equipments;
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var saveFileDialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "Excel Files|*.xlsx",
                    FileName = "DynamicExport"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    using (var package = new ExcelPackage())
                    {
                        var worksheet = package.Workbook.Worksheets.Add("ExportedData");
                        int currentColumn = 1;

                        // Всегда экспортируем первый ListBox
                        ExportListToExcel(worksheet, products, currentColumn++);
                        worksheet.Cells[1, 1].Value = "Основные данные";

                        // Проверяем чекбоксы
                        if (OperationCheck.IsChecked == true)
                        {
                            ExportListBoxToColumn(worksheet, operations, currentColumn);
                            worksheet.Cells[1, currentColumn].Value = "Доп. данные 1";
                            currentColumn++;
                        }

                        if (EquipmentCheck.IsChecked == true)
                        {
                            //ExportListBoxToColumn(worksheet, equipments, currentColumn);
                            //worksheet.Cells[1, currentColumn].Value = "Доп. данные 2";
                            currentColumn++;
                        }

                        // Автоподбор ширины для всех использованных колонок
                        for (int i = 1; i < currentColumn; i++)
                        {
                            worksheet.Column(i).AutoFit();
                        }

                        package.SaveAs(new FileInfo(saveFileDialog.FileName));
                        MessageBox.Show("Экспорт завершен успешно!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка экспорта: {ex.Message}");
            }
        }

        private void ExportListBoxToColumn(ExcelWorksheet worksheet, ListBox listBox, int column)
        {
            for (int i = 0; i < listBox.Items.Count; i++)
            {
                worksheet.Cells[i + 2, column].Value = listBox.Items[i]?.ToString();
            }
        }

        private void ExportListToExcel(ExcelWorksheet worksheet, List<MenuItem> listBox, int column)
        {
            for (int i = 0; i < listBox.Count; i++)
            {
                worksheet.Cells[i + 2, column].Value = listBox[i].Title;
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
