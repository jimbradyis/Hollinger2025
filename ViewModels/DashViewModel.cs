using CommunityToolkit.Mvvm.ComponentModel;
using Hollinger2025.Models;

namespace Hollinger2025.ViewModels
{
    public partial class DashViewModel : ObservableObject
    {
        // The archivist that was logged in
        [ObservableProperty]
        private Archivist currentArchivist;

        public DashViewModel(Archivist archivist)
        {
            currentArchivist = archivist;
        }

        // TODO: add logic for your dashboard
    }
}
