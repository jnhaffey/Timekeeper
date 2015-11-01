using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Timekeeper.Entities
{
    public class WorkItemTimeRecord : TimeRecordBase
    {
        private WorkItem _item;

        public WorkItemTimeRecord()
        {
            StartTime = DateTime.MinValue;
            EndTime = DateTime.MaxValue;
        }

        public override void SetExported()
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

        public override void SetIgnored()
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

        public override string Case
        {
            get
            {
                return Item.Fields.Contains("Felinesoft.CrmCase") ? Item.Fields["Felinesoft.CrmCase"].Value as string : string.Empty;
            }
            set
            {
                if (!Item.Fields.Contains("Felinesoft.CrmCase"))
                {
                    throw new NotSupportedException("The work item type does not contain the CRM Case field " + "Felinesoft.CrmCase");
                }
                Item.Fields["Felinesoft.CrmCase"].Value = value;
            }
        }
 
        public override bool IsExported
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

        public override bool CanExport
        {
            get
            {
                return EndRevision > 0 && Item != null && Item.Fields.Contains("Felinesoft.ExportedRecords");
            }
        }

        public override bool IsIgnored
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

        public override bool CanIgnore
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

        public override string ItemTitle
        {
            get
            {
                return Item == null ? null : Item.Title;
            }
            set
            {
                if (Item != null)
                {
                    Item.Title = value;
                }
            }
        }

        public override string Order
        {
            get
            {
                return Item.Fields.Contains("Felinesoft.CrmOrder") ? Item.Fields["Felinesoft.CrmOrder"].Value as string : string.Empty;
            }
            set
            {
                if (!Item.Fields.Contains("Felinesoft.CrmOrder"))
                {
                    throw new NotSupportedException("The work item type does not contain the CRM Order field " + "Felinesoft.CrmOrder");
                }
                Item.Fields["Felinesoft.CrmOrder"].Value = value;
            }
        }

        public override string ToString()
        {
            return ItemTitle + ", Start: " + StartTime + ", End: " + EndTime + ", Duration: " + Duration;
        }

        protected override TimeRecordBase Clone()
        {
            var rec = new WorkItemTimeRecord();
            rec.StartRevision = StartRevision;
            rec.EndRevision = EndRevision;
            rec.Item = Item;
            rec.StartTime = StartTime;
            rec.EndTime = EndTime;
            return rec;
        }
    }
}