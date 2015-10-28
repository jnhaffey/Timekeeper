using Company.Timekeeper.Properties;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Controls;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.VersionControl.Controls.Extensibility;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.TeamFoundation;
using Microsoft.VisualStudio.TeamFoundation.VersionControl;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;

namespace Microsoft.ALMRangers.Samples.MyHistory
{
    [TeamExplorerSection(CurrentlyWorkingOnSection.SectionId, Microsoft.TeamFoundation.Controls.TeamExplorerPageIds.PendingChanges, 0)]
    public class CurrentlyWorkingOnSection : TeamExplorerBaseSection
    {
        public const string SectionId = "E17F37BC-A048-48E0-96EE-7E9E826FFA21";
        private IPendingChangesExt _pendingChangesExt;
        private ObservableCollection<WorkItem> _workItems = new ObservableCollection<WorkItem>();

        public ObservableCollection<WorkItem> WorkItems
        {
            get { return _workItems; }
            set { _workItems = value; RaisePropertyChanged("WorkItems"); }
        }

        public CurrentlyWorkingOnSection()
        {
            EmbeddedAssemblyResolver.Ensure();
            this.Title = "Currently Working On";
            this.IsVisible = true;
            this.IsExpanded = true;
            this.IsBusy = false;
            this.SectionContent = new CurrentlyWorkingOnSectionView();
            this.View.ParentSection = this;
        }

        protected CurrentlyWorkingOnSectionView View
        {
            get { return this.SectionContent as CurrentlyWorkingOnSectionView; }
        }

        /// <summary>
        /// ContextChanged override.
        /// </summary>
        protected override void ContextChanged(object sender, TeamFoundation.Client.ContextChangedEventArgs e)
        {
            base.ContextChanged(sender, e);

            // If the team project collection or team project changed, refresh the data for this section
            if (e.TeamProjectCollectionChanged || e.TeamProjectChanged)
            {
                this.Refresh();
            }
        }

        public override void Initialize(object sender, SectionInitializeEventArgs e)
        {
            base.Initialize(sender, e);
            Refresh();
        }

        private void EnsurePendingChangesService()
        {
            if (_pendingChangesExt == null)
            {
                var dte = Package.GetGlobalService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
                TeamFoundationServerExt ext = dte.GetObject("Microsoft.VisualStudio.TeamFoundation.TeamFoundationServerExt") as TeamFoundationServerExt;
                _pendingChangesExt = GetService<IPendingChangesExt>();
                if (_pendingChangesExt == null)
                {
                    //var dte = Package.GetGlobalService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
                    VersionControlExt ex = dte.GetObject("Microsoft.VisualStudio.TeamFoundation.VersionControl.VersionControlExt") as VersionControlExt;
                    _pendingChangesExt = ex.PendingChanges;
                }
                if (_pendingChangesExt != null)
                {
                    _pendingChangesExt.PropertyChanged += svc_PropertyChanged;
                }
            }
        }

        void svc_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "WorkItems")
            {
                Refresh();
            }
        }

        /// <summary>
        /// Refresh override.
        /// </summary>
        public override void Refresh()
        {
            base.Refresh();
            EnsurePendingChangesService();
            if (_pendingChangesExt != null)
            {
                WorkItems = new ObservableCollection<WorkItem>(_pendingChangesExt.WorkItems.Select(x => x.WorkItem));
            }
        }
    }

}