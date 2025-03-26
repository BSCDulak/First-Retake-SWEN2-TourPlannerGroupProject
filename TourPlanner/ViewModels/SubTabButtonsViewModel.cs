using System.Windows.Input;
using SWEN2_TourPlannerGroupProject.MVVM;
using System.Diagnostics;
using System.Windows.Input;

namespace SWEN2_TourPlannerGroupProject.ViewModels
{
    // This class is the viewmodel for the SubTabButtons, it contains the logic for the buttons in the subtab.
    // there is some tab selection logic too but we might not need it, it looks like this can be done with bindings.
    // after we got the tour logs logic working we can remove the tab selection logic.
    internal class SubTabButtonsViewModel : ViewModelBase
    {
        private string _newAddText;
        public string NewAddText
        {
            get => _newAddText;
            set
            {
                if (_newAddText != value)
                {
                    _newAddText = value;
                    OnPropertyChanged(nameof(NewAddText));  // Notify the UI that the value has changed
                }
            }
        }


        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }

        public SubTabButtonsViewModel(Action<string> addAction, ICommand deleteCommand)
        {
            AddCommand = new RelayCommand(param =>
            {
                Debug.WriteLine($"Command param: {param}");  // Debug here
                addAction(param as string);
            });
            DeleteCommand = deleteCommand;
        }

        // This constructor is only used for design-time data, if we do not have this
        // we get a somewhat misleading error since design time does matters for the compiled code.
        public SubTabButtonsViewModel()
        {
            // Provide mock commands for design-time
            AddCommand = new RelayCommand(_ => { /* Mock add action */ });
            DeleteCommand = new RelayCommand(_ => { /* Mock delete action */ });
        }

    }
}
