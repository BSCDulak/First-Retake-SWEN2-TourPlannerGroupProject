using SWEN2_TourPlannerGroupProject.ViewModels;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SWEN2_TourPlannerGroupProject;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private MainWindowViewModel vm;

    public MainWindow()
    {
        InitializeComponent();
        Loaded += MainWindow_Loaded; // subscribe to Loaded event
    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        vm = new MainWindowViewModel();
        DataContext = vm;

        try
        {
            await vm.InitializeAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error initializing application: {ex.Message}", "Initialization Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}