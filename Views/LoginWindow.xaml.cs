using Hollinger2025.ViewModels; // <== Use your new namespace
using System.Windows;

namespace Hollinger2025.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow(LoginViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
            PasswordBox.PasswordChanged += PasswordBox_PasswordChanged;
            this.DataContextChanged += LoginWindow_DataContextChanged;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel vm)
            {
                if (!string.IsNullOrEmpty(vm.Password))
                {
                    PasswordBox.Password = vm.Password;
                }
            }
        }


        private void LoginWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext is LoginViewModel viewModel)
            {
                // 1) If the VM already has a password from saved settings,
                //    set the masked PasswordBox property:
                if (!string.IsNullOrEmpty(viewModel.Password))
                {
                    PasswordBox.Password = viewModel.Password;
                }

                // 2) Subscribe to ShowPassword changes to swap
                //    between masked and plain text:
                viewModel.PropertyChanged += (s, args) =>
                {
                    if (args.PropertyName == nameof(LoginViewModel.ShowPassword))
                    {
                        SyncPasswordVisibility(viewModel);
                    }
                };
            }
        }


        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is LoginViewModel viewModel)
            {
                // Always move the typed password into the ViewModel’s Password,
                // but only if the PasswordBox is actually visible (i.e. user can type).
                if (PasswordBox.Visibility == Visibility.Visible)
                {
                    viewModel.Password = PasswordBox.Password;
                }
            }
        }

        private void SyncPasswordVisibility(LoginViewModel viewModel)
        {
            if (viewModel.ShowPassword)
            {
                // The user wants to see it in plain text:
                // So we copy whatever is in the masked PasswordBox into the plain TextBox:
                PlainTextPasswordBox.Text = PasswordBox.Password;
                PlainTextPasswordBox.Focus(); // optional
            }
            else
            {
                // The user wants it masked:
                // So we copy the plain text into the masked PasswordBox:
                PasswordBox.Password = PlainTextPasswordBox.Text;
                PasswordBox.Focus(); // optional
            }
        }

    }
}
