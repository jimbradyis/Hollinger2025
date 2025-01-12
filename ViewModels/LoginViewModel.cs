using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hollinger2025.Models;
using Hollinger2025.Views;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace Hollinger2025.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly EthicsContext _context;

        [ObservableProperty]
        private string? username;

        [ObservableProperty]
        private string? password;

        [ObservableProperty]
        private string? errorMessage;

        [ObservableProperty]
        private bool rememberMe;

        [ObservableProperty]
        private bool showPassword = false ;

        public LoginViewModel(EthicsContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

            // 1) Load saved credentials from Properties.Settings if they exist
            LoadSavedCredentials();
        }

        private void LoadSavedCredentials()
        {
            // These are hypothetical settings you create:
            // Properties.Settings.Default.SavedUsername
            // Properties.Settings.Default.SavedPassword (base64 or plain text)
            // Adjust the property names to your actual settings.

            var savedUsername = Properties.Settings.Default.SavedUsername;
            var savedPassword = Properties.Settings.Default.SavedPassword;
            var savedRememberMe = Properties.Settings.Default.RememberMe;

            if (!string.IsNullOrEmpty(savedUsername) &&
                !string.IsNullOrEmpty(savedPassword) &&
                savedRememberMe)
            {
                Username = savedUsername;
                Password = savedPassword; // or decrypt if you prefer
                RememberMe = true;
            }
        }

        [RelayCommand]
        private async Task Login()
        {
            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Please enter both username and password";
                return;
            }

            // Attempt to authenticate
            var archivist = await _context.Archivists
                .FirstOrDefaultAsync(a => a.Ric == Username && a.Password == Password);

            if (archivist == null)
            {
                ErrorMessage = "Invalid login credentials";
                return;
            }

            // Mark them as logged in, if desired
            archivist.LoggedIn = 1;
            await _context.SaveChangesAsync();

            // 2) If RememberMe is checked, store credentials in user settings
            if (RememberMe)
            {
                Properties.Settings.Default.SavedUsername = Username;
                Properties.Settings.Default.SavedPassword = Password; // or encrypt if you'd like
                Properties.Settings.Default.RememberMe = true;
                Properties.Settings.Default.Save();
            }
            else
            {
                // Clear them if user unchecks
                Properties.Settings.Default.SavedUsername = string.Empty;
                Properties.Settings.Default.SavedPassword = string.Empty;
                Properties.Settings.Default.RememberMe = false;
                Properties.Settings.Default.Save();
            }

            // 3) Show the DashWindow
            var dashVm = new DashViewModel(archivist); // We’ll create a stub below
            var dashWindow = new DashWindow(dashVm);   // Also a stub

            dashWindow.Show();

            // 4) Close the LoginWindow
            CloseLoginWindow();
        }

        private void CloseLoginWindow()
        {
            // This is a quick trick to find the window whose DataContext == this
            foreach (Window w in Application.Current.Windows)
            {
                if (w.DataContext == this)
                {
                    w.Close();
                    break;
                }
            }
        }
    }
}
