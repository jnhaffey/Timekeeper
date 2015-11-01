using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Timekeeper.Crm;
using Timekeeper.Entities;

namespace Timekeeper.VsExtension
{
    public interface ITimekeeperExporter
    {
        Task Export(IEnumerable<TimeRecordBase> records);
    }

    public class CrmExporter : ITimekeeperExporter
    {

        public async Task Export(IEnumerable<TimeRecordBase> records)
        {
            TimeRecordSync trs = new TimeRecordSync(Properties.Settings.Default.SettingsCollection.CrmConnectionString);
            var categories = trs.GetCategories();
            CategorySelection cs = new CategorySelection(categories);
            cs.ShowDialog();
            var category = cs.Value;
            await Task.Factory.StartNew(() =>
                {
                    trs.Sync(records, category);
                });
        }
    }

    public class FlatFileExporter : ITimekeeperExporter
    {
        public async Task Export(IEnumerable<TimeRecordBase> records)
        {
            var myDocs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string filename;
            var originalFilename = filename = "Timekeeper Export " + DateTime.Now.ToString("yyyy-MM-dd");
            int i = 1;
            while (File.Exists(Path.Combine(myDocs, filename + ".csv")))
            {
                filename = originalFilename + "(" + i + ")";
                i++;
            }
            using (var stream = File.CreateText(Path.Combine(myDocs, filename + ".csv")))
            {
                stream.WriteLine("Case,Order,StartTime,EndTime,WorkItemTitle");
                foreach (var record in records)
                {
                    stream.WriteLine(string.Format("{0},{1},{2:yyyy-MM-dd HH\\:mm\\:ss\\.fffff},{3:yyyy-MM-dd HH\\:mm\\:ss\\.fffff},{4}", record.Case, record.Order, record.StartTime, record.EndTime, record.ItemTitle));
                }
            }
        }
    }
}