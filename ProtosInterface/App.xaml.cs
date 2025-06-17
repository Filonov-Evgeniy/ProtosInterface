using OfficeOpenXml;
using System.Configuration;
using System.Data;
using System.Windows;
using static OfficeOpenXml.ExcelErrorValue;

namespace ProtosInterface;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        // Установка лицензии EPPlus 8+
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        base.OnStartup(e);
    }
}

