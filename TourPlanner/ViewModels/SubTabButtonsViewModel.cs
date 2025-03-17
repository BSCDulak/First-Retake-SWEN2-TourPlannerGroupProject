using System.Windows.Input;


namespace SWEN2_TourPlannerGroupProject.ViewModels
{
    internal class SubTabButtonsViewModel : ViewModelBase
    {
        private bool _isTourTabSelected;
        private bool _isLogTabSelected;
        public bool IsTourTabSelected
        {
            get => _isTourTabSelected;
            set => SetField(ref _isTourTabSelected, value);
        }
        public bool IsLogTabSelected
        {
            get => _isLogTabSelected;
            set => SetField(ref _isLogTabSelected, value);
        }

        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }

        public SubTabButtonsViewModel(ICommand addCommand, ICommand deleteCommand)
        {
            AddCommand = addCommand;
            DeleteCommand = deleteCommand;
        }

    }
}
