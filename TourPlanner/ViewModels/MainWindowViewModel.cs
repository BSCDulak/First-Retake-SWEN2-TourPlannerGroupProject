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
        private static readonly ILoggerWrapper log = LoggerFactory.GetLogger();
        public ToursListViewModel ToursListView { get; }
        public TourLogsViewModel TourLogs { get; }
        public SubTabButtonsViewModel SubTabButtonsForToursListView { get; }
        public SubTabButtonsViewModel SubTabButtonsForTourLogsView { get; }
        public TourDetailsWrapPanelViewModel TourDetailsWrapPanelView { get; }
        public MapViewModel MapViewModel { get; }

        public MainWindowViewModel()
        {
            try
            {
                log.Info("MainWindowViewModel constructor called. Initializing viewmodels...");
                // this makes a new TourListViewModel called ToursListView
                ToursListView = new ToursListViewModel();
                // this makes a new TourLogsViewModel called TourLogs
                log.Info("ToursListView initialized.");
                TourLogs = new TourLogsViewModel(ToursListView);
                log.Info("TourLogs initialized.");
                // this binds the subtab buttons to the tours viewmodel, it is super important that the commands are passed in the correct order
                SubTabButtonsForToursListView = new SubTabButtonsViewModel(
                    ToursListView.AddCommand,
                    ToursListView.DeleteCommand,
                    ToursListView.UpdateCommand
                );
                log.Info("SubTabButtonsForToursListView initialized.");
                // this binds the subtab buttons to the tour logs viewmodel, and don´t you forget to add new buttons here if you add them to the TourLogsViewModel
                SubTabButtonsForTourLogsView = new SubTabButtonsViewModel(
                    TourLogs.AddTourLogCommand,
                    TourLogs.DeleteTourLogCommand,
                    TourLogs.UpdateTourLogCommand
                );
                log.Info("SubTabButtonsForTourLogsView initialized.");
                TourDetailsWrapPanelView = new TourDetailsWrapPanelViewModel(ToursListView);
                log.Info("TourDetailsWrapPanelView initialized.");
                MapViewModel = new MapViewModel();
                log.Info("MapViewModel initialized.");
                // Connect the selected tour to the map view model
                ToursListView.PropertyChanged += (sender, e) =>
                {
                    if (e.PropertyName == nameof(ToursListViewModel.SelectedTour))
                    {
                        MapViewModel.SetSelectedTour(ToursListView.SelectedTour);
                    }
                };
            }
            catch (Exception ex)
            {
                log.Fatal("Error initializing MainWindowViewModel", ex);
                throw;
            }
            log.Info("MainWindowViewModel initialized successfully.");
        }
    }
}