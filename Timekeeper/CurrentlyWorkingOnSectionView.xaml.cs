// <copyright file="WorkItemsSectionView.xaml.cs" company="Microsoft Corporation">Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.</copyright>
namespace Microsoft.ALMRangers.Samples.MyHistory
{
    using System.Windows;
    using System.Windows.Input;
    using Microsoft.TeamFoundation.WorkItemTracking.Client;
    using System.ComponentModel;
    using System.Collections.ObjectModel;
    using System;
    using Xceed.Wpf.Toolkit;
    using Company.Timekeeper.Properties;

    /// <summary>
    /// WorkItemsSectionView
    /// </summary>
    public partial class CurrentlyWorkingOnSectionView
    {
        public static readonly DependencyProperty ParentSectionProperty = DependencyProperty.Register("ParentSection", typeof(CurrentlyWorkingOnSection), typeof(CurrentlyWorkingOnSectionView));

        public CurrentlyWorkingOnSectionView()
        {
            this.InitializeComponent();
        }

        public int SelectedIndex
        {
            get
            {
                return workItemList.SelectedIndex;
            }

            set
            {
                workItemList.SelectedIndex = value;
                workItemList.ScrollIntoView(workItemList.SelectedItem);
            }
        }

        public CurrentlyWorkingOnSection ParentSection
        {
            get
            {
                return (CurrentlyWorkingOnSection)GetValue(ParentSectionProperty);
            }

            set
            {
                SetValue(ParentSectionProperty, value);
            }
        }

        private void workItemList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ParentSection.WorkItems[SelectedIndex].State == Settings.Default.StateNameConfiguration.GetActiveState(Global.ProjectName))
            {
                ParentSection.WorkItems[SelectedIndex].PartialOpen();
                ParentSection.WorkItems[SelectedIndex].State = Settings.Default.StateNameConfiguration.GetPausedState(Global.ProjectName);
                ParentSection.WorkItems[SelectedIndex].Save();
            }
            else
            {
                ParentSection.WorkItems[SelectedIndex].PartialOpen();
                ParentSection.WorkItems[SelectedIndex].State = Settings.Default.StateNameConfiguration.GetActiveState(Global.ProjectName);
                ParentSection.WorkItems[SelectedIndex].Save();
            }
            ParentSection.Refresh();
        }
    }
}
