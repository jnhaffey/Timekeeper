using Timekeeper.VsExtension.Properties;
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
using Timekeeper.Entities;
using Timekeeper.Crm;

// <copyright file="WorkItemsSection.cs" company="Microsoft Corporation">Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.</copyright>
namespace Timekeeper.VsExtension
{
    [TeamExplorerSection(TimeSlotsSection.SectionId, MyTimePage.PageId, 30)]
    public class TimeSlotsSection : TeamExplorerBaseSection
    {
        public const string SectionId = "5A59685E-AAB8-4F65-8A29-D67ED0CD845E";
        private bool _showExported = false;
        private bool _showIgnored = false;
        private ObservableCollection<TimeRecordBase> timeRecords = new ObservableCollection<TimeRecordBase>();
        //TODO invert control
        private ITimekeeperExporter _exporter = new CrmExporter();

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

        public ObservableCollection<TimeRecordBase> TimeRecords
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
        protected override async void ContextChanged(object sender, ContextChangedEventArgs e)
        {
            base.ContextChanged(sender, e);

            // If the team project collection or team project changed, refresh the data for this section
            if (e.TeamProjectCollectionChanged || e.TeamProjectChanged)
            {
                await this.RefreshAsync();
            }
        }

        public async void Export(List<WorkItemTimeRecord> records)
        {
            IsBusy = true;
            await _exporter.Export(records);
            await System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    records.ForEach(x =>
                    {
                        x.SetExported();
                    });

                });
            Refresh();
        }

        public async void Ignore(List<WorkItemTimeRecord> records)
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
                var timeRecords = new List<TimeRecordBase>();

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
                                if (!Properties.Settings.Default.SettingsCollection.IsIncluded(wi.Project.Name))
                                {
                                    continue;
                                }

                                var innerTimeRecords = new List<TimeRecordBase>();

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

                                WorkItemTimeRecord current = null;
                                foreach (var rev in revs)
                                {
                                    if (rev.State == Properties.Settings.Default.SettingsCollection.GetActiveState(wi.Project.Name))
                                    {
                                        current = new WorkItemTimeRecord() { Item = rev.Item, StartTime = rev.Timestamp, StartRevision = rev.Revision };
                                    }
                                    else if (current != null && current.EndTime == DateTime.MaxValue)
                                    {
                                        current.EndTime = rev.Timestamp;
                                        current.EndRevision = rev.Revision;
                                        innerTimeRecords.AddRange(current.SplitToRecordPerDay());
                                        current = null;
                                    }
                                }

                                if (current != null && current.EndTime == DateTime.MaxValue)
                                {
                                    current.EndTime = DateTime.Now;
                                    current.EndRevision = -1;
                                    innerTimeRecords.AddRange(current.SplitToRecordPerDay());
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
                this.TimeRecords = new ObservableCollection<TimeRecordBase>(timeRecords);
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

        public void Crop(List<WorkItemTimeRecord> records, TimeSpan startTime, TimeSpan endTime)
        {
            records.ForEach(x =>
            {
                x.Crop(startTime, endTime);
            });

            //TODO use this after setting up a way to persist time adjustments
            //IsBusy = true;
            //await System.Threading.Tasks.Task.Factory.StartNew(() =>
            //{
            //    records.ForEach(x =>
            //    {
            //        x.Crop(startTime, endTime);
            //    });

            //});
            //Refresh();
        }

        internal void Munge(List<WorkItemTimeRecord> records)
        {
            if (!records.All(x => records.First().Item == x.Item))
            {
                throw new InvalidOperationException("Records are not all from the same work item & cannot be munged");
            }
            var earliest = records.OrderBy(x => x.StartTime).First();
            var latest = records.OrderBy(x => x.EndTime).Last();
            var newRecord = new WorkItemTimeRecord()
            {
                StartTime = earliest.StartTime,
                StartRevision = earliest.StartRevision,
                EndTime = latest.EndTime,
                EndRevision = earliest.EndRevision,
                Item = earliest.Item
            };
            records.ForEach(x => TimeRecords.Remove(x));
            TimeRecords.Add(newRecord);
            TimeRecords = new ObservableCollection<TimeRecordBase>(TimeRecords.OrderBy(x => x.StartTime));
        }
    }
}