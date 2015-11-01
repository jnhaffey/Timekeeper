using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Timekeeper.Entities;

namespace Timekeeper.Crm
{
    public class TimeRecordSync
    {
        private Xrm.XrmServiceContext _context;
        private Xrm.SystemUser _currentUser;

        public TimeRecordSync(string connectionString)
        {
            var connection = Microsoft.Xrm.Client.CrmConnection.Parse(connectionString);
            _context = new Xrm.XrmServiceContext(connection);
            _currentUser = GetIdentity();
            var item = _context.IncidentSet.FirstOrDefault();
        }

        public void Sync(IEnumerable<TimeRecordBase> records, Category category)
        {
            foreach(var record in records)
            {
                var newSlot = new Xrm.New_TimeSlot();
                var ord = _context.SalesOrderSet.FirstOrDefault(x => x.Name == record.Order);
                var cas = string.IsNullOrWhiteSpace(record.Case) ? null : _context.IncidentSet.FirstOrDefault(x => x.TicketNumber == record.Case);
                newSlot.SalesOrder_New_TimeSlots = ord;
                newSlot.new_incident_new_timeslot_Case = cas;
                newSlot.New_name = record.ItemTitle;
                newSlot.New_Category = category.Id;
                newSlot.New_StartTime = record.StartTime;
                newSlot.New_EndTime = record.EndTime;
                newSlot.New_Hours = record.Duration.TotalHours;
                newSlot.fso_TimeSpent = (int)Math.Floor(record.Duration.TotalMinutes);
                _context.AddObject(newSlot);
            }
            _context.SaveChanges();
        }

        public IEnumerable<Category> GetCategories()
        {
            RetrieveAttributeRequest attr = new RetrieveAttributeRequest();
            attr.LogicalName = "new_category";
            attr.EntityLogicalName = "new_timeslot";
            RetrieveAttributeResponse attributeResponse = (RetrieveAttributeResponse)_context.Execute(attr);
            return (attributeResponse.AttributeMetadata as Microsoft.Xrm.Sdk.Metadata.PicklistAttributeMetadata).OptionSet.Options.Select(x => new Category() { Label = x.Label.LocalizedLabels.First().Label, Id = x.Value.Value });
        }

        public Xrm.SystemUser GetIdentity()
        {
            WhoAmIRequest wai = new WhoAmIRequest();
            var resp = (WhoAmIResponse)_context.Execute(wai);
            var me = _context.SystemUserSet.Where(x => x.Id == resp.UserId).SingleOrDefault();
            return me;
        }
    }

    public class Category
    {
        public int Id { get; set; }
        public string Label { get; set; }
    }
}
