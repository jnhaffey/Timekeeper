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
    /// Interaction logic for TimelineControl.xaml
    /// </summary>
    public partial class TimelineControl : UserControl
    {
        private const int _startOfWorkingDayMinutes = 420;
        private const int _endOfWorkingDayMinutes = 1140;

        public TimelineControl()
        {
            InitializeComponent();
            ForceCursor = true;
        }

        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {
            
        }

        public void ShowOutOfHours()
        {
            var cols = this.FindVisualChildren<TimelineGrid>().SelectMany(x => x.ColumnDefinitions).Where(x =>
            {
                var minute = (x.Parent as TimelineGrid).ColumnDefinitions.IndexOf(x) + 1;
                return minute < _startOfWorkingDayMinutes || minute > _endOfWorkingDayMinutes;
            }).ToList();
            cols.ForEach(x => x.Width = new GridLength(1, GridUnitType.Star));

            cols = this.FindVisualChildren<TimelineHourMarkersGrid>().SelectMany(x => x.ColumnDefinitions).Where(x =>
            {
                var hour = (x.Parent as TimelineHourMarkersGrid).ColumnDefinitions.IndexOf(x) + 1;
                return hour < (_startOfWorkingDayMinutes / 60) || hour > (_endOfWorkingDayMinutes / 60);
            }).ToList();
            cols.ForEach(x => x.Width = new GridLength(1, GridUnitType.Star));
        }

        public void HideOutOfHours()
        {
            var cols = this.FindVisualChildren<TimelineGrid>().SelectMany(x => x.ColumnDefinitions).Where(x =>
                {
                    var minute = (x.Parent as TimelineGrid).ColumnDefinitions.IndexOf(x) + 1;
                    return minute < _startOfWorkingDayMinutes || minute > _endOfWorkingDayMinutes;
                }).ToList();
            cols.ForEach(x => x.Width = new GridLength(0));

            cols = this.FindVisualChildren<TimelineHourMarkersGrid>().SelectMany(x => x.ColumnDefinitions).Where(x =>
            {
                var hour = (x.Parent as TimelineHourMarkersGrid).ColumnDefinitions.IndexOf(x) + 1;
                return hour < (_startOfWorkingDayMinutes / 60) || hour > (_endOfWorkingDayMinutes / 60);
            }).ToList();
            cols.ForEach(x => x.Width = new GridLength(0));
        }
    }

    public class TimelineGrid : Grid { }

    public class TimelineHourMarkersGrid : Grid { }
}
