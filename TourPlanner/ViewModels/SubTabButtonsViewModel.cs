using System.Windows.Input;
using SWEN2_TourPlannerGroupProject.MVVM;


namespace SWEN2_TourPlannerGroupProject.ViewModels
{
    // This class is the viewmodel for the SubTabButtons, it contains the logic for the buttons in the subtab.
    // there is some tab selection logic too but we might not need it, it looks like this can be done with bindings.
    // after we got the tour logs logic working we can remove the tab selection logic.
    internal class SubTabButtonsViewModel : ViewModelBase
    {
  

        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand ReportCommand { get; }
        public ICommand ExportCommand { get; }
        public ICommand ImportCommand { get; }

        // The ICommand? updateCommand parameter is optional and can be null because I haven´t set it yet for the TourLogsViewModel. TODO later today maybe?
        public SubTabButtonsViewModel(ICommand addCommand, ICommand deleteCommand, ICommand? updateCommand = null, ICommand? reportCommand = null, ICommand? exportCommand = null, ICommand? importCommand = null)
        {
            AddCommand = addCommand;
            DeleteCommand = deleteCommand;
            UpdateCommand = updateCommand ?? new AsyncRelayCommand(_ => Task.CompletedTask); // Provide a default no-op command if null
            ReportCommand = reportCommand ?? new AsyncRelayCommand(_ => Task.CompletedTask); // Provide a default no-op command if null
            ExportCommand = exportCommand ?? new AsyncRelayCommand(_ => Task.CompletedTask); // Provide a default no-op command if null
            ImportCommand = importCommand ?? new AsyncRelayCommand(_ => Task.CompletedTask); // Provide a default no-op command if null
        }

        // This constructor is only used for design-time data, if we do not have this
        // we get a somewhat misleading error since design time does matters for the compiled code.
        public SubTabButtonsViewModel()
        {
            // Provide mock commands for design-time
            AddCommand = new RelayCommand(_ => { /* Mock add action */ });
            DeleteCommand = new RelayCommand(_ => { /* Mock delete action */ });
            UpdateCommand = new RelayCommand(_ => { /* Mock update action */ });
            ReportCommand = new RelayCommand(_ => { /* Mock report action */ });
            ExportCommand = new RelayCommand(_ => { /* Mock export action */ });
            ImportCommand = new RelayCommand(_ => { /* Mock import action */ });
        }

    }
}
