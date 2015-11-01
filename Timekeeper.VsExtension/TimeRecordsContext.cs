using System.Collections.ObjectModel;
using Timekeeper.Entities;

namespace Timekeeper.VsExtension
{
    public class TimeRecordsContext
    {
        private ObservableCollection<TimeRecordBase> timeRecords = new ObservableCollection<TimeRecordBase>();

        public ObservableCollection<TimeRecordBase> TimeRecords
        {
            get
            {
                return timeRecords;
            }
            set
            {
                timeRecords = value;
            }
        }
    }
}