using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Hollinger2025.Models;
using Microsoft.EntityFrameworkCore;

namespace Hollinger2025.ViewModels
{
    public partial class DashViewModel : ObservableObject
    {
        private readonly EthicsContext _context;

        // The archivist that was logged in
        [ObservableProperty]
        private Archivist? currentArchivist;

        // Maybe you want to display a list of Archives
        [ObservableProperty]
        private ObservableCollection<Archive> archives = new();

        // Example property to show the “current user” name
        // (In your DB, you have `Archivist.First` and `Archivist.Last`, or similar.)
        [ObservableProperty]
        private string userName = string.Empty;

        // If you want to filter or group by Congress, you could store them
        // in a ComboBox. For example:
        [ObservableProperty]
        private ObservableCollection<Congress> congresses = new();

        // If you want to filter or group by Subcommittee (Inquiry),
        // you could store them in a ComboBox:
        [ObservableProperty]
        private ObservableCollection<Inquiry> inquiries = new();

        // The user’s choice for “Select All” or a single congress, etc.
        [ObservableProperty]
        private bool selectAll;

        // The selected congress from a combobox, if any
        private Congress? _selectedCongress;
        public Congress? SelectedCongress
        {
            get => _selectedCongress;
            set
            {
                if (SetProperty(ref _selectedCongress, value))
                {
                    // Reload or refresh Archives for the chosen congress
                    _ = LoadDataAsync();
                }
            }
        }

        // The selected inquiry (subcommittee), optional
        private Inquiry? _selectedInquiry;
        public Inquiry? SelectedInquiry
        {
            get => _selectedInquiry;
            set
            {
                if (SetProperty(ref _selectedInquiry, value))
                {
                    // Reload or refresh Archives for the chosen subcommittee
                    _ = LoadDataAsync();
                }
            }
        }

        public DashViewModel(EthicsContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Initialize after successful login
        public void Initialize(Archivist archivistData)
        {
            CurrentArchivist = archivistData;
            // Example: build userName from archivist’s first/last fields
            UserName = ((archivistData.First ?? "") + " " + (archivistData.Last ?? "")).Trim();
        }

        // Called by DashWindow to load data
        public async Task LoadDataAsync()
        {
            // 1) Load all congresses, inquiries, etc. for combos
            var cList = await _context.Congresses
                                      .OrderByDescending(c => c.CongressNo)
                                      .ToListAsync();
            Congresses = new ObservableCollection<Congress>(cList);

            var iList = await _context.Inquiries
                                      .OrderBy(i => i.Subcommittee)
                                      .ToListAsync();
            Inquiries = new ObservableCollection<Inquiry>(iList);

            // 2) Query Archives depending on user selection
            IQueryable<Archive> query = _context.Archives
                .Include(a => a.SubcommitteeNavigation)
                .Include(a => a.CongressNavigation);

            if (!SelectAll)
            {
                if (SelectedCongress != null)
                {
                    query = query.Where(a => a.Congress == SelectedCongress.CongressNo);
                }
                if (SelectedInquiry != null)
                {
                    query = query.Where(a => a.Subcommittee == SelectedInquiry.Subcommittee);
                }
            }

            var archiveList = await query.ToListAsync();
            Archives = new ObservableCollection<Archive>(archiveList);
        }
    }
}
