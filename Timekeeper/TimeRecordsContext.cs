using System.Collections.ObjectModel;

namespace Microsoft.ALMRangers.Samples.MyHistory
{
    public class TimeRecordsContext
    {
        private ObservableCollection<TimeRecord> timeRecords = new ObservableCollection<TimeRecord>();

        public ObservableCollection<TimeRecord> TimeRecords
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