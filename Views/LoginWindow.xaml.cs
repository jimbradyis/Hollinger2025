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


        private void LoginWindow_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext is LoginViewModel viewModel)
            {
                // Set initial password if it exists
                if (!string.IsNullOrEmpty(viewModel.Password))
                {
                    PasswordBox.Password = viewModel.Password;
                }

                // Subscribe to ShowPassword changes
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
            // Only update the ViewModel if the user is typing in the PasswordBox 
            // and ShowPassword == false
            if (DataContext is LoginViewModel viewModel && viewModel.ShowPassword == false)
            {
                viewModel.Password = PasswordBox.Password;
            }
        }

        private void SyncPasswordVisibility(LoginViewModel viewModel)
        {
            if (viewModel.ShowPassword)
            {
                // Move the current PasswordBox value into the PlainTextPasswordBox
                PlainTextPasswordBox.Text = PasswordBox.Password;
            }
            else
            {
                // Move the current PlainTextPasswordBox value into the PasswordBox
                PasswordBox.Password = viewModel.Password;
            }
        }
    }
}
