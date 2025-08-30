using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace SWEN2_TourPlannerGroupProject.Models
{
    // This class is a model for a Tour, it contains all the information that is needed for a Tour and is used
    // by toursList(ViewModel) and TourDetailsWrapPanel(ViewModel) to display the information in the GUI.
    public class Tour : INotifyPropertyChanged
    {
        private string? _name;
        private string? _description;
        private string? _startLocation;
        private string? _endLocation;
        private string? _transportType;
        private string? _distance;
        private string? _estimatedTime;
        private string? _routeInformation;
        private string? _routeImagePath;
        private int? _tourId;
        public Guid BackupId { get; set; } = Guid.NewGuid();
        private string? _popularity;
        private string? _childFriendliness;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string? Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public string? Description
        {
            get => _description;
            set
            {
                if (_description != value)
                {
                    _description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }

        public string? StartLocation
        {
            get => _startLocation;
            set
            {
                if (_startLocation != value)
                {
                    _startLocation = value;
                    OnPropertyChanged(nameof(StartLocation));
                }
            }
        }

        public string? EndLocation
        {
            get => _endLocation;
            set
            {
                if (_endLocation != value)
                {
                    _endLocation = value;
                    OnPropertyChanged(nameof(EndLocation));
                }
            }
        }

        public string? TransportType
        {
            get => _transportType;
            set
            {
                if (_transportType != value)
                {
                    _transportType = value;
                    OnPropertyChanged(nameof(TransportType));
                }
            }
        }

        public string? Distance
        {
            get => _distance;
            set
            {
                if (_distance != value)
                {
                    _distance = value;
                    OnPropertyChanged(nameof(Distance));
                }
            }
        }

        public string? EstimatedTime
        {
            get => _estimatedTime;
            set
            {
                if (_estimatedTime != value)
                {
                    _estimatedTime = value;
                    OnPropertyChanged(nameof(EstimatedTime));
                }
            }
        }

        public string? RouteInformation
        {
            get => _routeInformation;
            set
            {
                if (_routeInformation != value)
                {
                    _routeInformation = value;
                    OnPropertyChanged(nameof(RouteInformation));
                }
            }
        }

        public string? RouteImagePath
        {
            get => _routeImagePath;
            set
            {
                if (_routeImagePath != value)
                {
                    _routeImagePath = value;
                    OnPropertyChanged(nameof(RouteImagePath));
                }
            }
        }

        public int? TourId
        {
            get => _tourId;
            set
            {
                if (_tourId != value)
                {
                    _tourId = value;
                    OnPropertyChanged(nameof(TourId));
                }
            }
        }

        /* todo This is a list of TourLogs that are associated with this Tour, needs a model, viewmodel and usercontrol.*/
        //public ObservableCollection<TourLog> TourLogs { get; set; } = new ObservableCollection<TourLog>();
        public ObservableCollection<TourLog> TourLogs { get; set; } = new();

        public string? Popularity
        {
            get => _popularity;
            set
            {
                if (_popularity != value)
                {
                    _popularity = value;
                    OnPropertyChanged(nameof(Popularity));
                }
            }
        }

        public string? ChildFriendliness
        {
            get => _childFriendliness;
            set
            {
                if (_childFriendliness != value)
                {
                    _childFriendliness = value;
                    OnPropertyChanged(nameof(ChildFriendliness));
                }
            }
        }
    }
}
