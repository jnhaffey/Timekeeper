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
using System.Windows.Shapes;
using TimelineLibrary;

namespace Company.Timekeeper
{
    /// <summary>
    /// Interaction logic for Timeline.xaml
    /// </summary>
    public partial class Timeline : Window
    {
        private List<TimelineEvent> items = new List<TimelineEvent>();
        public Timeline()
        {
            InitializeComponent();
            for (int i = 1; i < 10; i++)
            {
                items.Add(new TimelineEvent()
                {
                    Title = i.ToString(),
                    Description = i.ToString(),
                    StartDate = DateTime.Now.AddHours(-(i * 2)),
                    EndDate = DateTime.Now.AddHours(-i)
                });
            }
            lineControl.ResetEvents(items);
            lineControl.MinDateTime = DateTime.Now.AddDays(-1);
            lineControl.MaxDateTime = DateTime.Now.AddDays(1);
        }

        public Timeline(IEnumerable<TimeRecord> records)
        {
            InitializeComponent();
            lineControl.ClearEvents();
            if (records.Count() == 0)
            {
                return;
            }
            lineControl.MinDateTime = records.OrderBy(x => x.StartTime).First().StartTime.AddDays(-1);
            lineControl.MaxDateTime = records.OrderBy(x => x.EndTime).Last().EndTime.AddDays(1);
            lineControl.ResetEvents(records.Select(x => new TimelineEvent()
            {
                Title = x.ItemTitle,
                Description = x.Item.Description,
                StartDate = x.StartTime,
                EndDate = x.EndTime,
                IsDuration = true,
                HeightOverride = 100,
                WidthOverride = 100,
                Tag = x
            }).ToList());
            lineControl.MinDateTime = records.OrderBy(x => x.StartTime).First().StartTime.AddDays(-1);
            lineControl.MaxDateTime = records.OrderBy(x => x.EndTime).Last().EndTime.AddDays(1);
        }
    }
}
