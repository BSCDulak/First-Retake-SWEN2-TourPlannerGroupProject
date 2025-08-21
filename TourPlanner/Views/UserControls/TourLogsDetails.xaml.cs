using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading; // Needed for Dispatcher

namespace SWEN2_TourPlannerGroupProject.Views.UserControls
{
    public partial class TourLogsDetails : UserControl
    {
        // Regex to match time format: h:mm:ss or hh:mm:ss (e.g., 5:30:45 or 05:30:45)
        private static readonly Regex _timeRegex = new Regex(@"^([0-9]{1,2}):([0-5][0-9]):([0-5][0-9])$");

        public TourLogsDetails()
        {
            InitializeComponent();
        }

        // Allow only digits and colons to be typed into the textbox
        private void TimeTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, @"[\d:]");
        }

        // Validate time format when the user leaves the textbox
        private void TimeTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var tb = sender as TextBox;
            if (tb != null && !string.IsNullOrWhiteSpace(tb.Text))
            {
                if (!_timeRegex.IsMatch(tb.Text))
                {
                    MessageBox.Show("Please enter time in format hh:mm:ss (e.g., 05:30:45)", "Invalid Time Format", MessageBoxButton.OK, MessageBoxImage.Warning);

                    // Use Dispatcher to avoid infinite focus loop
                    Dispatcher.BeginInvoke(new Action(() => tb.Focus()), DispatcherPriority.ApplicationIdle);
                }
            }
        }
    }
}
