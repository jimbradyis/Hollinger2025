using System;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Hollinger2025.Models;
using Hollinger2025.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Hollinger2025.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly EthicsContext _context;
        private readonly IServiceProvider _serviceProvider;

        // The username the user enters (maps to Archivist.Ric in DB)
        [ObservableProperty]
        private string? username;

        // The password the user enters (maps to Archivist.Password)
        [ObservableProperty]
        private string? password;

        // Whether user wants to save credentials in Settings
        [ObservableProperty]
        private bool rememberMe;

        // For displaying login errors (if any) in the UI
        [ObservableProperty]
        private string? errorMessage;

        // If true, we show the password as plain text.
        // The Window handles toggling between a PasswordBox and a TextBox.
        [ObservableProperty]
        private bool showPassword;

        // (Optional) If you want logic to fire whenever showPassword changes:
        partial void OnShowPasswordChanged(bool value)
        {
            // Force a re-binding or special logic if you want
            // e.g. if (value) ...
        }

        public LoginViewModel(EthicsContext context, IServiceProvider serviceProvider)
        {
            // Basic null checks
            ArgumentNullException.ThrowIfNull(context);
            ArgumentNullException.ThrowIfNull(serviceProvider);

            _context = context;
            _serviceProvider = serviceProvider;

            // Load saved credentials (if user previously chose "Remember Me")
            LoadSavedCredentials();
        }

        private void LoadSavedCredentials()
        {
            var savedUsername = Properties.Settings.Default.SavedUsername;
            var savedPassword = Properties.Settings.Default.SavedPassword;
            var savedRememberMe = Properties.Settings.Default.RememberMe;

            if (!string.IsNullOrEmpty(savedUsername) &&
                !string.IsNullOrEmpty(savedPassword) &&
                savedRememberMe)
            {
                Username = savedUsername;
                Password = savedPassword; // If you like, you can decrypt it here
                RememberMe = true;
            }
        }

        [RelayCommand]
        private async Task LoginAsync()
        {
            ErrorMessage = string.Empty;

            // 1) Basic validation
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Please enter both username and password";
                return;
            }

            try
            {
                // 2) Attempt to authenticate Archivist
                var archivist = await _context.Archivists
                    .FirstOrDefaultAsync(a => a.Ric == Username && a.Password == Password);

                if (archivist == null)
                {
                    ErrorMessage = "Invalid login credentials";
                    return;
                }

                // 3) Mark them as logged in, if you wish
                archivist.LoggedIn = 1;
                await _context.SaveChangesAsync();

                // 4) Store or clear credentials
                if (RememberMe)
                {
                    Properties.Settings.Default.SavedUsername = Username;
                    Properties.Settings.Default.SavedPassword = Password; // or encrypt
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

                // 5) Show the DashWindow
                var dashViewModel = _serviceProvider.GetRequiredService<DashViewModel>();
                dashViewModel.Initialize(archivist); // pass the Archivist

                var dashWindow = _serviceProvider.GetRequiredService<DashWindow>();
                dashWindow.DataContext = dashViewModel;
                dashWindow.Show();

                // 6) Close the current LoginWindow
                CloseLoginWindow();
            }
            catch (Exception ex)
            {
                ErrorMessage = "An error occurred during login: " + ex.Message;
            }
        }

        private void CloseLoginWindow()
        {
            // Closes the Window whose DataContext == this
            foreach (Window window in Application.Current.Windows)
            {
                if (window.DataContext == this)
                {
                    window.Close();
                    break;
                }
            }
        }
    }
}
