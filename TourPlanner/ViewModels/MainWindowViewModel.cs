using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SWEN2_TourPlannerGroupProject.Models;

namespace SWEN2_TourPlannerGroupProject.ViewModels
{
    internal class MainWindowViewModel : ViewModelBase
    {
        public ToursListViewModel ToursListViewModel { get; }
        public SubTabButtonsViewModel SubTabButtonsViewModel { get; }

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

            ToursListViewModel = new ToursListViewModel(tours);
            SubTabButtonsViewModel = new SubTabButtonsViewModel(
                ToursListViewModel.AddCommand,
                ToursListViewModel.DeleteCommand
            );
        }
    }
}