using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWEN2_TourPlannerGroupProject.MVVM;
using SWEN2_TourPlannerGroupProject.Models;
using System.Windows.Input;

namespace SWEN2_TourPlannerGroupProject.ViewModels
{
    internal class TourLogsViewModel : ViewModelBase
    {
        private TourLog _selectedTourLog;

        public TourLog SelectedTourLog
        {
            get => _selectedTourLog;
            set
            {
                SetField(ref _selectedTourLog, value);
                OnPropertyChanged(nameof(SelectedTourLog));
                CommandManager.InvalidateRequerySuggested();
            }
        }

    }
}
