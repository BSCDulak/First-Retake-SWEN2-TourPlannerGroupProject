using SWEN2_TourPlannerGroupProject.Models;
using SWEN2_TourPlannerGroupProject.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SWEN2_TourPlannerGroupProject.Views.UserControls
{
    /// <summary>
    /// Interaction logic for SubTabButtons.xaml
    /// </summary>
    public partial class SubTabButtons : UserControl
    {
        public SubTabButtons()
        {
            InitializeComponent();
            DataContext = new ToursListViewModel(new ObservableCollection<Tour>());
        }
    }
}
