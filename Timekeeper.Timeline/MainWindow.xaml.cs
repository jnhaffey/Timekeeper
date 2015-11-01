using Microsoft.ALMRangers.Samples.MyHistory;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var recs = new List<TimeRecord>();
            var baseDate = new DateTime(2015, 10, 26);
            recs.Add(new Microsoft.ALMRangers.Samples.MyHistory.TimeRecord() { StartTime = baseDate.Date.AddHours(2), EndTime = baseDate.Date.AddHours(4) });
            recs.Add(new Microsoft.ALMRangers.Samples.MyHistory.TimeRecord() { StartTime = baseDate.Date.AddHours(1), EndTime = baseDate.Date.AddHours(4) });
            recs.Add(new Microsoft.ALMRangers.Samples.MyHistory.TimeRecord() { StartTime = baseDate.Date.AddHours(4), EndTime = baseDate.Date.AddHours(6) });
            recs.Add(new Microsoft.ALMRangers.Samples.MyHistory.TimeRecord() { StartTime = baseDate.Date.AddHours(8), EndTime = baseDate.Date.AddHours(12) });
            recs.Add(new Microsoft.ALMRangers.Samples.MyHistory.TimeRecord() { StartTime = baseDate.Date.AddDays(1).AddHours(1), EndTime = baseDate.Date.AddDays(1).AddHours(3) });
            recs.Add(new Microsoft.ALMRangers.Samples.MyHistory.TimeRecord() { StartTime = baseDate.Date.AddDays(1), EndTime = baseDate.Date.AddDays(1).AddHours(3) });
            recs.Add(new Microsoft.ALMRangers.Samples.MyHistory.TimeRecord() { StartTime = baseDate.Date.AddDays(1).AddHours(3), EndTime = baseDate.Date.AddDays(1).AddHours(5) });
            recs.Add(new Microsoft.ALMRangers.Samples.MyHistory.TimeRecord() { StartTime = baseDate.Date.AddDays(1).AddHours(8), EndTime = baseDate.Date.AddDays(1).AddHours(12) });
            Model = new TimesheetModel(recs);
        }

        public MainWindow(IEnumerable<TimeRecord> records)
        {
            InitializeComponent();
            Model = new TimesheetModel(records);
        }

        public TimesheetModel Model
        {
            get
            {
                return (TimesheetModel)DataContext;
            }
            set
            {
                DataContext = value;
            }
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var t = this.FindVisualChildren<Button>().ToList();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Timesheet.ShowOutOfHours = !Timesheet.ShowOutOfHours;
        }
    }

}
