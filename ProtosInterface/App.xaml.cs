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
        if (DBConnection.useAuth == true)
        {
            this.StartupUri = new Uri("AuthorizationWindow.xaml", UriKind.Relative);
        }
        else
        {
            this.StartupUri = new Uri("MainWindow.xaml", UriKind.Relative);
        }
        base.OnStartup(e);
    }
}

