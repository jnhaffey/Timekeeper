using Company.Timekeeper.Properties;
using Microsoft.TeamFoundation.Controls;
using Microsoft.TeamFoundation.VersionControl.Controls.Extensibility;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.VisualStudio.Shell;
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
                _pendingChangesExt = GetService<IPendingChangesExt>();
                if (_pendingChangesExt == null)
                {
                    var dte = Package.GetGlobalService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
                    VersionControlExt ext = dte.GetObject("Microsoft.VisualStudio.TeamFoundation.VersionControl.VersionControlExt") as VersionControlExt;
                    _pendingChangesExt = ext.PendingChanges;
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

    public class StateToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                if ((value as string) == Settings.Default.StateNameConfiguration.GetActiveState(Global.ProjectName))
                {
                    return new SolidColorBrush(Colors.DarkGreen);
                }
                else
                {
                    return new SolidColorBrush(Colors.DarkRed);
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    public class StateToDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                if ((value as string) == Settings.Default.StateNameConfiguration.GetActiveState(Global.ProjectName))
                {
                    return "Currently Working";
                }
                else
                {
                    return Settings.Default.StateNameConfiguration.GetPausedState(Global.ProjectName);
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}