using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWEN2_TourPlannerGroupProject.Models;

namespace SWEN2_TourPlannerGroupProject.ViewModels
{
    // This is the main viewmodel for the MainWindow, it contains the viewmodels for
    // the ToursList, SubTabButtons and TourDetailsWrapPanel.
    // every time a new viewmodel is created it needs to be added here and initialized in the constructor.
    internal class MainWindowViewModel : ViewModelBase
    {
        public ToursListViewModel ToursListViewModel { get; }
        public SubTabButtonsViewModel SubTabButtonsViewModel { get; }
        public SubTabButtonsViewModel SubTabButtonsViewModelTourLogs { get; }
        public TourDetailsWrapPanelViewModel TourDetailsWrapPanelViewModel { get; }

        public MainWindowViewModel()
        {
            var tours = new ObservableCollection<Tour>();
            for (var i = 1; i < 10; i++)
            {
                tours.Add(new Tour
                {
                    Name = $"Tour{i}"
                });
            }

            //this makes a new TourListViewModel and binds it to the tours list
            ToursListViewModel = new ToursListViewModel(tours);
            //todo create a TourListTourLogsViewModel -> this makes a new TourListViewModel and binds it to the tour logs list
            //ToursListTourLogsViewModel = new ToursListViewModel(not sure if we gotta do tours or tourlogs here);
            // this binds the subtab buttons to the tours viewmodel
            SubTabButtonsViewModel = new SubTabButtonsViewModel(
                ToursListViewModel.AddCommand,
                ToursListViewModel.DeleteCommand
            );
            // this binds the subtab buttons to the tour logs viewmodel
            /*SubTabButtonsViewModelTourLogs = new SubTabButtonsViewModel(
                TourLogsViewModel.AddCommand,
                TourLogsViewModel.DeleteCommand
            );*/
            TourDetailsWrapPanelViewModel = new TourDetailsWrapPanelViewModel(ToursListViewModel);
        }
    }
}