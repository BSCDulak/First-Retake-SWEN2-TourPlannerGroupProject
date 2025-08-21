using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace SWEN2_TourPlannerGroupProject.Views.UserControls
{
    public partial class TourLogsDetails : UserControl
    {
        // Regex to match time format: hh:mm:ss
        private static readonly Regex _timeRegex = new Regex(@"^([0-9]{1,2}):([0-5][0-9]):([0-5][0-9])$");

        // Regex to allow input like: 10.2, 115.6, etc.
        private static readonly Regex _distanceInputRegex = new Regex(@"^\d*\.?\d{0,1}$");
        private static readonly Regex _distanceExactFormatRegex = new Regex(@"^\d+\.\d$");

        public TourLogsDetails()
        {
            InitializeComponent();
        }

        // Allow only digits and colons in the Time textbox
        private void TimeTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, @"[\d:]");
        }

        // Validate time format on losing focus
        private void TimeTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var tb = sender as TextBox;
            if (tb != null && !string.IsNullOrWhiteSpace(tb.Text))
            {
                if (!_timeRegex.IsMatch(tb.Text))
                {
                    MessageBox.Show("Please enter time in format hh:mm:ss (e.g., 05:30:45)", "Invalid Time Format", MessageBoxButton.OK, MessageBoxImage.Warning);
                    Dispatcher.BeginInvoke(new Action(() => tb.Focus()), DispatcherPriority.ApplicationIdle);
                }
            }
        }

        // Restrict input to numbers and at most one decimal digit for Distance
        private void DistanceTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            string newText = textBox.Text.Insert(textBox.SelectionStart, e.Text);
            e.Handled = !_distanceInputRegex.IsMatch(newText);
        }

        // Validate exact format for Distance on losing focus
        private void DistanceTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (!string.IsNullOrWhiteSpace(textBox?.Text) && !_distanceExactFormatRegex.IsMatch(textBox.Text))
            {
                MessageBox.Show("Please enter a valid distance (e.g., 10.2, 115.6)", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                textBox.Text = string.Empty;
            }
        }
    }
}
