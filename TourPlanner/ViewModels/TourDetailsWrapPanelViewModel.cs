using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWEN2_TourPlannerGroupProject.Models;

namespace SWEN2_TourPlannerGroupProject.ViewModels
{
    // This class is the viewmodel for the TourDetailsWrapPanel, which contains the details of a selected Tour from the ToursListView
    internal class TourDetailsWrapPanelViewModel : ViewModelBase
    {
        private readonly ToursListViewModel _toursListViewModel;

        public Tour? SelectedTour
        {
            get => _toursListViewModel.SelectedTour;
            set
            {
                _toursListViewModel.SelectedTour = value;
                OnPropertyChanged();
            }
        }

        public TourDetailsWrapPanelViewModel(ToursListViewModel toursListViewModel)
        {
            _toursListViewModel = toursListViewModel;
            _toursListViewModel.PropertyChanged += ToursListViewModelPropertyChanged;
        }
        // This constructor is used for the design view, there shall be no errors!
        public TourDetailsWrapPanelViewModel()
        {
            _toursListViewModel = null;
        }



        private void ToursListViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_toursListViewModel.SelectedTour))
            {
                OnPropertyChanged(nameof(SelectedTour));
            }
        }
    }
}