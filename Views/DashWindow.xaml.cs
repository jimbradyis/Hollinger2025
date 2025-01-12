using Hollinger2025.ViewModels;
using System.Windows;

namespace Hollinger2025.Views
{
    public partial class DashWindow : Window
    {
        public DashWindow(DashViewModel dashViewModel)
        {
            InitializeComponent();
            this.DataContext = dashViewModel;
        }
    }
}
