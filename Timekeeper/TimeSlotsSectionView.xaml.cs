// <copyright file="WorkItemsSectionView.xaml.cs" company="Microsoft Corporation">Copyright Microsoft Corporation. All Rights Reserved. This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) This is sample code only, do not use in production environments.</copyright>
namespace Timekeeper.VsExtension
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
    using Timekeeper.VsExtension;
    using Timekeeper.Timeline;
    using Timekeeper.Entities;

    /// <summary>
    /// WorkItemsSectionView
    /// </summary>
    public partial class TimeSlotsSectionView
    {
        private readonly TimeSpan _startOfDay = new TimeSpan(9, 0, 0);
        private readonly TimeSpan _endOfDay = new TimeSpan(18, 0, 0);

        public static readonly DependencyProperty ParentSectionProperty = DependencyProperty.Register("ParentSection", typeof(TimeSlotsSection), typeof(TimeSlotsSectionView));

        public TimeSlotsSectionView()
        {
            this.InitializeComponent();
            CropStart.Value = DateTime.Now.Date.Add(_startOfDay);
            CropEnd.Value = DateTime.Now.Date.Add(_endOfDay);
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
                WorkItemTimeRecord wi = this.workItemList.SelectedItems[0] as WorkItemTimeRecord;
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
                ParentSection.Export(records.Cast<WorkItemTimeRecord>().ToList());
            }
            else if (e.Source == IgnoreMenu)
            {
                ParentSection.Ignore(records.Cast<WorkItemTimeRecord>().ToList());
            }
            else if (e.Source == MungeMenu)
            {
                ParentSection.Munge(records.Cast<WorkItemTimeRecord>().ToList());
            }
            else if (e.Source == CropMenu)
            {
                ParentSection.Crop(records.Cast<WorkItemTimeRecord>().ToList(), CropStart.Value.Value.TimeOfDay, CropEnd.Value.Value.TimeOfDay);
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
                if (records.Count > 1 && records.All(x => records.First() is WorkItemTimeRecord && x is WorkItemTimeRecord && (records.First() as WorkItemTimeRecord).Item == (x as WorkItemTimeRecord).Item))
                {
                    MungeMenu.IsEnabled = true;
                }
            }
        }

        private List<TimeRecordBase> GetSelectedTimeRecords()
        {
            var indices = GetSelectedIndices();
            return new List<TimeRecordBase>(ParentSection.TimeRecords.Where(x => indices.Contains(ParentSection.TimeRecords.IndexOf(x))));
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var _settings = new Settings();
            _settings.ShowDialog();
        }

        private void Label_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (SettingsGrid.Visibility == System.Windows.Visibility.Visible)
            {
                SettingsGrid.Visibility = System.Windows.Visibility.Collapsed;
                HideSettings.Content = "Show Settings";
            }
            else
            {
                SettingsGrid.Visibility = System.Windows.Visibility.Visible;
                HideSettings.Content = "Hide Settings";
            }
        }

        private void ViewTimeline_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var tl = new TimesheetWindow(ParentSection.TimeRecords);
            tl.Show();
        }
    }
}
