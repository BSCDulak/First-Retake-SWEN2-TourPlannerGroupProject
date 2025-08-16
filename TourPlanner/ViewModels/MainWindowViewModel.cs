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
    // the whole application and is used to connect all the different parts of the application together.
    // Every time a new viewmodel is created it needs to be added here and initialized in the constructor.
    internal class MainWindowViewModel : ViewModelBase
    {
        public ToursListViewModel ToursListView { get; }
        public TourLogsViewModel TourLogs { get; }
        public SubTabButtonsViewModel SubTabButtonsForToursListView { get; }
        public SubTabButtonsViewModel SubTabButtonsForTourLogsView { get; }
        public TourDetailsWrapPanelViewModel TourDetailsWrapPanelView { get; }
        public MapViewModel MapViewModel { get; }

        public MainWindowViewModel()
        {

            // this makes a new TourListViewModel called ToursListView
            ToursListView = new ToursListViewModel();
            // this makes a new TourLogsViewModel called TourLogs
            TourLogs = new TourLogsViewModel(ToursListView);
            // this binds the subtab buttons to the tours viewmodel
            SubTabButtonsForToursListView = new SubTabButtonsViewModel(
                ToursListView.AddCommand,
                ToursListView.DeleteCommand,
                ToursListView.UpdateCommand
            );
            // this binds the subtab buttons to the tour logs viewmodel
            SubTabButtonsForTourLogsView = new SubTabButtonsViewModel(
                TourLogs.AddTourLogCommand,
                TourLogs.DeleteTourLogCommand
            );
            TourDetailsWrapPanelView = new TourDetailsWrapPanelViewModel(ToursListView);
            MapViewModel = new MapViewModel();

            // Connect the selected tour to the map view model
            ToursListView.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(ToursListViewModel.SelectedTour))
                {
                    MapViewModel.SetSelectedTour(ToursListView.SelectedTour);
                }
            };
        }
    }
}