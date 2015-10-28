using Company.Timekeeper.Properties;
using EnvDTE;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Controls;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.VersionControl.Controls.Extensibility;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TeamFoundation;
using Microsoft.VisualStudio.TeamFoundation.VersionControl;
using Microsoft.VisualStudio.TeamFoundation.WorkItemTracking;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
// <copyright file="WorkItemsSection.cs" company="Microsoft Corporation">Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.</copyright>
namespace Microsoft.ALMRangers.Samples.MyHistory
{
    public interface ITimekeeperExporter
    {
        void Export(IEnumerable<TimeRecord> records);
    }

    public class FlatFileExporter : ITimekeeperExporter
    {
        public void Export(IEnumerable<TimeRecord> records)
        {
            var myDocs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string filename;
            var originalFilename = filename = "Timekeeper Export " + DateTime.Now.ToString("yyyy-MM-dd");
            int i = 1;
            while(File.Exists(Path.Combine(myDocs, filename + ".csv")))
            {
                filename = originalFilename + "(" + i + ")";
                i++;
            }
            using (var stream = File.CreateText(Path.Combine(myDocs, filename + ".csv")))
            {
                stream.WriteLine("Case,Order,StartTime,EndTime,WorkItemTitle");
                foreach(var record in records)
                {
                    stream.WriteLine(string.Format("{0},{1},{2:yyyy-MM-dd HH\\:mm\\:ss\\.fffff},{3:yyyy-MM-dd HH\\:mm\\:ss\\.fffff},{4}", record.Case, record.Order, record.StartTime, record.EndTime, record.ItemTitle));
                }
            }
        }
    }

    [TeamExplorerSection(TimeSlotsSection.SectionId, MyTimePage.PageId, 30)]
    public class TimeSlotsSection : TeamExplorerBaseSection
    {
        public const string SectionId = "5A59685E-AAB8-4F65-8A29-D67ED0CD845E";
        private bool _showExported = false;
        private bool _showIgnored = false;
        private ObservableCollection<TimeRecord> timeRecords = new ObservableCollection<TimeRecord>();
        //TODO invert control
        private ITimekeeperExporter _exporter = new FlatFileExporter();

        public TimeSlotsSection()
        {
            EmbeddedAssemblyResolver.Ensure();
            this.Days = 3;
            this.MinimumTimeSpan = 1;
            this.WorkdayStart = new TimeSpan(0, 1, 0);
            this.WorkdayEnd = new TimeSpan(23, 59, 0);
            this.Title = "Time Slots";
            this.IsVisible = true;
            this.IsExpanded = true;
            this.IsBusy = false;
            this.SectionContent = new TimeSlotsSectionView();
            this.View.ParentSection = this;
        }

        /// <summary>
        /// The number of days old time records to get
        /// </summary>
        public int Days { get; set; }

        /// <summary>
        /// The minimum time record to keep - those below this length
        /// in minutes will be ignored.
        /// </summary>
        public double MinimumTimeSpan { get; set; }

        public bool ShowExported
        {
            get { return _showExported; }
            set
            {
                if (_showExported != value)
                {
                    _showExported = value;
                    Refresh();
                }
            }
        }

        public bool ShowIgnored
        {
            get { return _showIgnored; }
            set
            {
                if (_showIgnored != value)
                { 
                    _showIgnored = value; 
                    Refresh(); 
                }
            }
        }

        public ObservableCollection<TimeRecord> TimeRecords
        {
            get
            {
                return this.timeRecords;
            }

            protected set
            {
                this.timeRecords = value;
                this.RaisePropertyChanged("TimeRecords");
            }
        }

        public TimeSpan WorkdayEnd { get; set; }

        public TimeSpan WorkdayStart { get; set; }

        protected TimeSlotsSectionView View
        {
            get { return this.SectionContent as TimeSlotsSectionView; }
        }

        /// <summary>
        /// ContextChanged override.
        /// </summary>
        protected override async void ContextChanged(object sender, TeamFoundation.Client.ContextChangedEventArgs e)
        {
            base.ContextChanged(sender, e);

            // If the team project collection or team project changed, refresh the data for this section
            if (e.TeamProjectCollectionChanged || e.TeamProjectChanged)
            {
                await this.RefreshAsync();
            }
        }

        public async void Export(List<TimeRecord> records)
        {
            IsBusy = true;
            await System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    _exporter.Export(records);
                    records.ForEach(x =>
                    {
                        x.SetExported();
                    });

                });
            Refresh();
        }

        public async void Ignore(List<TimeRecord> records)
        {
            IsBusy = true;
            await System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                records.ForEach(x =>
                {
                    x.SetIgnored();
                });

            });
            Refresh();
        }

        public async override void Initialize(object sender, SectionInitializeEventArgs e)
        {
            base.Initialize(sender, e);
            // If the user navigated back to this page, there could be saved context information that is passed in
            var sectionContext = e.Context as TimeRecordsContext;
            if (sectionContext != null)
            {
                // Restore the context instead of refreshing
                TimeRecordsContext context = sectionContext;
                this.TimeRecords = context.TimeRecords;
            }
            else
            {
                // Kick off the refresh
                await this.RefreshAsync();
            }
        }

        /// <summary>
        /// Refresh override.
        /// </summary>
        public async override void Refresh()
        {
            base.Refresh();
            await this.RefreshAsync();
        }

        /// <summary>
        /// Save contextual information about the current section state.
        /// </summary>
        public override void SaveContext(object sender, SectionSaveContextEventArgs e)
        {
            base.SaveContext(sender, e);

            // Save our current so when the user navigates back to the page the content is restored rather than requeried
            TimeRecordsContext context = new TimeRecordsContext { TimeRecords = this.TimeRecords };
            e.Context = context;
        }

        public void ViewWorkItemDetails(int workItemId)
        {
            try
            {
                ITeamFoundationContext context = this.CurrentContext;
                EnvDTE80.DTE2 dte2 = Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(DTE)) as EnvDTE80.DTE2;
                if (dte2 != null)
                {
                    DocumentService witDocumentService = (DocumentService)dte2.DTE.GetObject("Microsoft.VisualStudio.TeamFoundation.WorkItemTracking.DocumentService");
                    var widoc = witDocumentService.GetWorkItem(context.TeamProjectCollection, workItemId, this);
                    witDocumentService.ShowWorkItem(widoc);
                }
            }
            catch (Exception ex)
            {
                this.ShowNotification(ex.Message, NotificationType.Error);
            }
        }

        private async System.Threading.Tasks.Task RefreshAsync()
        {
            try
            {
                // Set our busy flag and clear the previous data
                this.IsBusy = true;
                this.TimeRecords.Clear();
                var timeRecords = new List<TimeRecord>();

                // Make the server call asynchronously to avoid blocking the UI
                await System.Threading.Tasks.Task.Run(() =>
                {
                    ITeamFoundationContext context = this.CurrentContext;
                    TeamFoundationIdentity user;
                    context.TeamProjectCollection.GetAuthenticatedIdentity(out user);
                    var userName = user.DisplayName;

                    if (context != null && context.HasCollection && context.HasTeamProject)
                    {
                        WorkItemStore wis = context.TeamProjectCollection.GetService<WorkItemStore>();
                        if (wis != null)
                        {
                            WorkItemCollection wic = wis.Query("SELECT [System.Id], [System.Title], [System.State] FROM WorkItems WHERE [System.WorkItemType] <> ''  AND  [System.State] <> ''  AND  [System.ChangedDate] > @Today - " + Days + "  AND  [System.AssignedTo] EVER @Me ORDER BY [System.Id]");
                            foreach (WorkItem wi in wic)
                            {
                                var innerTimeRecords = new List<TimeRecord>();

                                var revs = wi.Revisions.Cast<Revision>()
                                      .Where(revision => revision.Fields["System.State"].IsChangedInRevision)
                                      .Where(revision => (string)revision.Fields["System.AssignedTo"].Value == userName)
                                      .Where(revision => Convert.ToDateTime(revision.Fields["Changed Date"].Value) > DateTime.Now.Subtract(TimeSpan.FromDays(Days)))
                                      .Select(revision => new StateEvent()
                                      {
                                          Timestamp = Convert.ToDateTime(revision.Fields["Changed Date"].Value),
                                          State = (string)revision.Fields["System.State"].Value,
                                          Item = revision.WorkItem,
                                          Reason = (string)revision.Fields["System.Reason"].Value,
                                          Revision = wi.Revisions.IndexOf(revision)
                                      })
                                      .OrderBy(revision => revision.Timestamp).ToList();

                                TimeRecord current = null;
                                foreach (var rev in revs)
                                {
                                    if (rev.State == Settings.Default.StateNameConfiguration.GetActiveState(wi.Project.Name))
                                    {
                                        current = new TimeRecord() { Item = rev.Item, StartTime = rev.Timestamp, StartRevision = rev.Revision };
                                    }
                                    else if (current != null && current.EndTime == DateTime.MaxValue)
                                    {
                                        current.EndTime = rev.Timestamp;
                                        current.EndRevision = rev.Revision;
                                        innerTimeRecords.AddRange(current.Split(WorkdayStart, WorkdayEnd));
                                        current = null;
                                    }
                                }

                                if (current != null && current.EndTime == DateTime.MaxValue)
                                {
                                    current.EndTime = DateTime.Now;
                                    current.EndRevision = -1;
                                    innerTimeRecords.AddRange(current.Split(WorkdayStart, WorkdayEnd));
                                }

                                timeRecords.AddRange(innerTimeRecords);
                            }
                            timeRecords = timeRecords
                                .Where(x => x.Duration > TimeSpan.FromMinutes(MinimumTimeSpan))
                                .Where(x => _showExported || !x.IsExported)
                                .Where(x => _showIgnored || !x.IsIgnored)
                                .OrderBy(x => x.StartTime)
                                .ToList();
                        }
                    }
                });

                // Now back on the UI thread, update the bound collection and section title
                this.TimeRecords = new ObservableCollection<TimeRecord>(timeRecords);
            }
            catch (Exception ex)
            {
                this.ShowNotification(ex.Message, NotificationType.Error);
            }
            finally
            {
                // Always clear our busy flag when done
                this.IsBusy = false;
            }
        }
    }
}