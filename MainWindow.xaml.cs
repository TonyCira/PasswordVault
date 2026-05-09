using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using PasswordVault.Services;

namespace PasswordVault;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        var services = new ServiceCollection();
        services.AddWpfBlazorWebView();
        services.AddSingleton<VaultService>();
        services.AddSingleton<PdfExportService>();

#if DEBUG
        services.AddBlazorWebViewDeveloperTools();
#endif

        Resources.Add("services", services.BuildServiceProvider());
    }
}
