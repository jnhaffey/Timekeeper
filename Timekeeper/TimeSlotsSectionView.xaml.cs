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
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Controls;

    /// <summary>
    /// WorkItemsSectionView
    /// </summary>
    public partial class TimeSlotsSectionView
    {
        public static readonly DependencyProperty ParentSectionProperty = DependencyProperty.Register("ParentSection", typeof(TimeSlotsSection), typeof(TimeSlotsSectionView));

        public TimeSlotsSectionView()
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

        public TimeSlotsSection ParentSection
        {
            get
            {
                return (TimeSlotsSection)GetValue(ParentSectionProperty);
            }

            set
            {
                SetValue(ParentSectionProperty, value);
            }
        }

        private void WorkItemList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && this.workItemList.SelectedItems.Count == 1)
            {
                this.ViewWorkItemDetails();
            }
        }

        private void WorkItemList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && this.workItemList.SelectedItems.Count == 1)
            {
                this.ViewWorkItemDetails();
            }
        }

        private void ViewWorkItemDetails()
        {
            if (this.workItemList.SelectedItems.Count == 1)
            {
                TimeRecord wi = this.workItemList.SelectedItems[0] as TimeRecord;
                if (wi != null)
                {
                    this.ParentSection.ViewWorkItemDetails(wi.Item.Id);
                }
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var records = GetSelectedTimeRecords();

            if (e.Source == ExportMenu)
            {
                ParentSection.Export(records);
            }
            else if (e.Source == IgnoreMenu)
            {
                ParentSection.Ignore(records);
            }
            else if (e.Source == MungeMenu)
            {

            }
        }

        private void workItemList_ContextMenuOpening(object sender, System.Windows.Controls.ContextMenuEventArgs e)
        {
            var records = GetSelectedTimeRecords();
            MungeMenu.IsEnabled = false;
            ExportMenu.IsEnabled = false;
            IgnoreMenu.IsEnabled = false;

            if (records.Count > 0)
            {
                if (records.All(x => x.CanExport && !x.IsExported))
                {
                    ExportMenu.IsEnabled = true;
                }
                if (records.All(x => x.CanIgnore && !x.IsIgnored))
                {
                    IgnoreMenu.IsEnabled = true;
                }
            }
        }

        private List<TimeRecord> GetSelectedTimeRecords()
        {
            var indices = GetSelectedIndices();
            return new List<TimeRecord>(ParentSection.TimeRecords.Where(x => indices.Contains(ParentSection.TimeRecords.IndexOf(x))));
        }

        private List<int> GetSelectedIndices()
        {
            var indices = new List<int>();
            foreach (var item in workItemList.SelectedItems)
            {
                indices.Add(workItemList.Items.IndexOf(item));
            }
            return indices;
        }

        private void IgnoredCheck_Checked(object sender, RoutedEventArgs e)
        {
            ParentSection.ShowIgnored = true;
        }

        private void IgnoredCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            ParentSection.ShowIgnored = false;
        }

        private void ExportedCheck_Checked(object sender, RoutedEventArgs e)
        {
            ParentSection.ShowExported = true;
        }

        private void ExportedCheck_Unchecked(object sender, RoutedEventArgs e)
        {
            ParentSection.ShowExported = false;
        }

        private void PeriodCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ParentSection != null)
            {
                var str = ((PeriodCombo.SelectedItem as ComboBoxItem).Tag as string);
                if (str == null)
                {
                    str = "7";
                }
                var val = int.Parse(str);
                if (ParentSection.Days == 0)
                {
                    //When initialising don't refresh as the parent already does that.
                    ParentSection.Days = val;
                }
                else if (ParentSection.Days != val)
                {
                    ParentSection.Days = val;
                    ParentSection.Refresh();
                }
            }
        }
    }
}
