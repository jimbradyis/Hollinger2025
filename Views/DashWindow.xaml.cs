using Hollinger2025.ViewModels;
using System.Windows;
using System.Threading.Tasks;

namespace Hollinger2025.Views
{
    public partial class DashWindow : Window
    {
        public DashWindow(DashViewModel dashViewModel)
        {
            InitializeComponent();

            // Set the DataContext to the VM we got from DI or from somewhere else
            this.DataContext = dashViewModel;
        }

        private async void DashWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Once the window is loaded, call LoadDataAsync on the ViewModel
            if (DataContext is DashViewModel vm)
            {
                await vm.LoadDataAsync();
            }
        }
    }
}
