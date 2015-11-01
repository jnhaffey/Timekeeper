using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System;

namespace Timekeeper.VsExtension
{
    public class StateEvent
    {
        public WorkItem Item { get; set; }
        public string Reason { get; set; }
        public string State { get; set; }
        public DateTime Timestamp { get; set; }
        public int Revision { get; set; }

        public override string ToString()
        {
            return State + " (Reason: " + Reason + ")";
        }
    }
}