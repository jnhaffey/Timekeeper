using System;
using System.Collections.Generic;
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

namespace Timekeeper.Timeline
{
    /// <summary>
    /// Interaction logic for TimesheetControl.xaml
    /// </summary>
    public partial class TimesheetControl : UserControl
    {
        private const int _startOfWorkingDayMinutes = 420;
        private const int _endOfWorkingDayMinutes = 1140;
        private bool? _showingOoh;

        // Dependency Property
        public static readonly DependencyProperty ShowOutOfHoursProperty =
             DependencyProperty.Register("ShowOutOfHours", typeof(bool),
             typeof(TimesheetControl), new FrameworkPropertyMetadata(false));

        // .NET Property wrapper
        public bool ShowOutOfHours
        {
            get { return (bool)GetValue(ShowOutOfHoursProperty); }
            set 
            { 
                if (!_showingOoh.HasValue || value != _showingOoh.Value)
                {
                    if (value)
                    {
                        UnhideOutOfHours();
                    }
                    else
                    {
                        HideOutOfHours();
                    }
                    _showingOoh = value;
                    SetValue(ShowOutOfHoursProperty, value);
                }
            }
        }

        public TimesheetControl()
        {
            InitializeComponent();
            this.Loaded += TimesheetControl_Loaded;
        }

        void TimesheetControl_Loaded(object sender, RoutedEventArgs e)
        {
            HideOutOfHours();
        }

        public void UnhideOutOfHours()
        {
            var rows = this.FindVisualChildren<TimesheetGrid>().SelectMany(x => x.RowDefinitions).Where(x =>
            {
                var day = (x.Parent as TimesheetGrid).RowDefinitions.IndexOf(x);
                return day == 0 || day == 6;
            }).ToList();
            rows.ForEach(x => x.Height = new GridLength(1, GridUnitType.Star));

            var cols = this.FindVisualChildren<TimesheetHourMarkersGrid>().SelectMany(x => x.ColumnDefinitions).Where(x =>
            {
                var hour = (x.Parent as TimesheetHourMarkersGrid).ColumnDefinitions.IndexOf(x);
                return hour < (_startOfWorkingDayMinutes / 60) || hour > (_endOfWorkingDayMinutes / 60);
            }).ToList();
            cols.ForEach(x => x.Width = new GridLength(1, GridUnitType.Star));

            this.FindVisualChildren<TimelineControl>().ToList().ForEach(x => x.ShowOutOfHours());
        }

        public void HideOutOfHours()
        {
            var rows = this.FindVisualChildren<TimesheetGrid>().SelectMany(x => x.RowDefinitions).Where(x =>
            {
                var day = (x.Parent as TimesheetGrid).RowDefinitions.IndexOf(x);
                return day == 0 || day == 6;
            }).ToList();
            rows.ForEach(x => x.Height = new GridLength(0));

            var cols = this.FindVisualChildren<TimesheetHourMarkersGrid>().SelectMany(x => x.ColumnDefinitions).Where(x =>
            {
                var hour = (x.Parent as TimesheetHourMarkersGrid).ColumnDefinitions.IndexOf(x);
                return hour < (_startOfWorkingDayMinutes / 60) || hour > (_endOfWorkingDayMinutes / 60);
            }).ToList();
            cols.ForEach(x => x.Width = new GridLength(0));

            this.FindVisualChildren<TimelineControl>().ToList().ForEach(x => x.HideOutOfHours());
        }
    }

    public class TimesheetGrid : Grid { }

    public class TimesheetHourMarkersGrid : Grid { }
}
