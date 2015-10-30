using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.ALMRangers.Samples.MyHistory
{
    public class TimeRecord : INotifyPropertyChanged
    {
        private int _endRevision;
        private DateTime _endTime;
        private bool _isExported;
        private bool _isIgnored;
        private WorkItem _item;
        private int _startRevision;
        private DateTime _startTime;
        private int _splitNumber;

        public int SplitNumber
        {
            get { return _splitNumber; }
            set { _splitNumber = value; Raise("SplitNumber", "IsExported", "IsIgnored", "IsIgnoredOrExported"); }
        }

        public TimeRecord()
        {
            StartTime = DateTime.MinValue;
            EndTime = DateTime.MaxValue;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string Case
        {
            get
            {
                return Item.Fields.Contains("Felinesoft.CrmCase") ? Item.Fields["Felinesoft.CrmCase"].Value as string : string.Empty;
            }
        }

        public TimeSpan Duration
        {
            get
            {
                return EndTime - StartTime;
            }
        }

        public int EndRevision
        {
            get { return _endRevision; }
            set { _endRevision = value; Raise("EndRevision", "IsExported", "IsIgnored", "IsIgnoredOrExported"); }
        }

        public DateTime EndTime
        {
            get { return _endTime; }
            set { if (_endTime != value) { _endTime = value; Raise("EndTime", "Duration"); } }
        }

        public bool IsExported
        {
            get
            {
                if (Item == null)
                {
                    return false;
                }
                var str = Item.Fields.Contains("Felinesoft.ExportedRecords") ? (string)Item.Fields["Felinesoft.ExportedRecords"].Value : string.Empty;
                var entries = str.Split(',');
                var current = string.Format("{0}-{1}({2})", StartRevision, EndRevision, SplitNumber);
                return entries.Any(x => x == current);
            }
        }

        public void SetExported()
        {
            if (EndRevision == -1)
            {
                throw new InvalidOperationException("Cannot ignore or export a time record which is currently accrueing time");
            }
            if (Item == null || !Item.Fields.Contains("Felinesoft.ExportedRecords"))
            {
                throw new InvalidOperationException(Item == null ? "Work Item is not set" : "Work Item does not have field Felinesoft.ExportedRecords");
            }
            var str = (string)Item.Fields["Felinesoft.ExportedRecords"].Value;
            str += string.Format(string.IsNullOrWhiteSpace(str) ? "{0}-{1}({2})" : ",{0}-{1}({2})", StartRevision, EndRevision, SplitNumber);
            Item.PartialOpen();
            Item.Fields["Felinesoft.ExportedRecords"].Value = str;
            Item.Save();
            Raise("IsExported");
        }

        public bool CanExport
        {
            get
            {
                return EndRevision > 0 && Item != null && Item.Fields.Contains("Felinesoft.ExportedRecords");
            }
        }

        public bool IsIgnored
        {
            get
            {
                if (Item == null)
                {
                    return false;
                }
                var str = Item.Fields.Contains("Felinesoft.IgnoredRecords") ? (string)Item.Fields["Felinesoft.IgnoredRecords"].Value : string.Empty;
                var entries = str.Split(',');
                var current = string.Format("{0}-{1}({2})", StartRevision, EndRevision, SplitNumber);
                return entries.Any(x => x == current);
            }
        }

        public void SetIgnored()
        {
            if (EndRevision == -1)
            {
                throw new InvalidOperationException("Cannot ignore or export a time record which is currently accrueing time");
            }
            if (Item == null || !Item.Fields.Contains("Felinesoft.IgnoredRecords"))
            {
                throw new InvalidOperationException(Item == null ? "Work Item is not set" : "Work Item does not have field Felinesoft.IgnoredRecords");
            }
            var str = (string)Item.Fields["Felinesoft.IgnoredRecords"].Value;
            str += string.Format(string.IsNullOrWhiteSpace(str) ? "{0}-{1}({2})" : ",{0}-{1}({2})", StartRevision, EndRevision, SplitNumber);
            Item.PartialOpen();
            Item.Fields["Felinesoft.IgnoredRecords"].Value = str;
            Item.Save();
            Raise("IsIgnored");
        }

        public bool CanIgnore
        {
            get
            {
                return EndRevision > 0 && Item != null && Item.Fields.Contains("Felinesoft.IgnoredRecords");
            }
        }

        public bool IsIgnoredOrExported
        {
            get
            {
                return IsIgnored || IsExported;
            }
        }

        public WorkItem Item
        {
            get { return _item; }
            set { _item = value; Raise("Item", "ItemTitle", "Order", "Case", "IsExported", "IsIgnored", "IsIgnoredOrExported"); }
        }

        public string ItemTitle
        {
            get
            {
                return Item == null ? null : Item.Title;
            }
        }

        public string Order
        {
            get
            {
                return Item.Fields.Contains("Felinesoft.CrmOrder") ? Item.Fields["Felinesoft.CrmOrder"].Value as string : string.Empty;
            }
        }

        public int StartRevision
        {
            get { return _startRevision; }
            set { _startRevision = value; Raise("StartRevision", "IsExported", "IsIgnored", "IsIgnoredOrExported"); }
        }
        public DateTime StartTime
        {
            get { return _startTime; }
            set { if (_startTime != value) { _startTime = value; Raise("StartTime", "Duration"); } }
        }

        public List<TimeRecord> SplitToRecordPerDay()
        {
            TimeSpan startTime = new TimeSpan(0,0,0);
            TimeSpan endTime = new TimeSpan(23, 59, 59);
            var recs = new List<TimeRecord>();
            var finalTime = _endTime.TimeOfDay > endTime ? _endTime.Date.Add(endTime) : _endTime;
            var currentTime = _startTime.TimeOfDay < startTime ? _startTime.Date.Add(startTime) : _startTime;
            TimeRecord rec;
            int splitNumber = 0;

            do
            {
                rec = new TimeRecord();
                rec.StartRevision = StartRevision;
                rec.EndRevision = EndRevision;
                rec.SplitNumber = splitNumber;
                splitNumber++;
                rec._item = _item;
                if (currentTime.TimeOfDay < startTime) //Crop start time
                {
                    rec._startTime = currentTime.Date.Add(startTime);
                }
                else //Use the start time as-is
                {
                    rec._startTime = currentTime;
                }

                if (_endTime.Date > rec._startTime.Date) //Split the record
                {
                    currentTime = rec._startTime.Date.AddDays(1).Add(startTime);
                    rec._endTime = rec._startTime.Date.Add(endTime);
                }
                else //Complete the record
                {
                    rec._endTime = finalTime;
                }
                recs.Add(rec);
            }
            while (rec.EndTime != finalTime);

            return recs;
        }

        public override string ToString()
        {
            return ItemTitle + ", Start: " + StartTime + ", End: " + EndTime + ", Duration: " + Duration;
        }

        private void Raise(params string[] p)
        {
            if (PropertyChanged != null)
            {
                foreach (var prop in p)
                {
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs(prop));
                }
            }
        }

        public void Crop(TimeSpan startTime, TimeSpan endTime)
        {
            if (StartTime.TimeOfDay < startTime && EndTime.TimeOfDay > startTime)
            {
                StartTime = StartTime.Date.Add(startTime);
            }
            if (EndTime.TimeOfDay > endTime && StartTime.TimeOfDay < endTime)
            {
                EndTime = EndTime.Date.Add(endTime);
            }
        }
    }
}