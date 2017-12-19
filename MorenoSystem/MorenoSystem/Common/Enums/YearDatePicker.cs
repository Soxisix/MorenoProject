using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace MorenoSystem.Common.Enums
{
    public class YearDatePicker : DatePicker
    {
        protected override void OnCalendarOpened( RoutedEventArgs e)
        {
            //var popup = this.Template.FindName(
            //    "PART_Popup", this) as Popup;
            //if (popup != null && popup.Child is System.Windows.Controls.Calendar)
            //{
            //    ((Calendar)popup.Child).DisplayMode = CalendarMode.Decade;
            //}

            //if (popup != null)
            //    ((Calendar) popup.Child).DisplayModeChanged +=
            //        new EventHandler<CalendarModeChangedEventArgs>(DatePickerCo_DisplayModeChanged);
            //var calendar = GetDatePickerCalendar(sender);
            ////var calendar = GetDatePickerCalendar();
            //calendar.DisplayMode = CalendarMode.Year;

            //calendar.DisplayModeChanged += CalendarOnDisplayModeChanged;
        }

        //void DatePickerCo_DisplayModeChanged(object sender, CalendarModeChangedEventArgs e)
        //{
        //    var popup = this.Template.FindName(
        //        "PART_Popup", this) as Popup;
        //    if (popup != null && popup.Child is System.Windows.Controls.Calendar)
        //    {
        //        var calendar = popup.Child as System.Windows.Controls.Calendar;

        //        calendar.DisplayMode = CalendarMode.Year;

        //        if (IsDropDownOpen)
        //        {
        //            this.SelectedDate = new DateTime(calendar.DisplayDate.Year, 1, 1);
        //            this.IsDropDownOpen = false;
        //            ((Calendar)popup.Child).DisplayModeChanged -= new EventHandler<CalendarModeChangedEventArgs>(DatePickerCo_DisplayModeChanged);
        //        }

        //    }
        //}

        private static void DatePickerOnCalendarClosed(object sender, RoutedEventArgs routedEventArgs)
        {
            var datePicker = (DatePicker)sender;
            var calendar = GetDatePickerCalendar(sender);
            datePicker.SelectedDate = calendar.SelectedDate;

            calendar.DisplayModeChanged -= CalendarOnDisplayModeChanged;
        }

        private static Calendar GetDatePickerCalendar(object sender)
        {
            var datePicker = (DatePicker)sender;
            var popup = (Popup)datePicker.Template.FindName("PART_Popup", datePicker);
            return ((Calendar)popup.Child);
        }

        private static void CalendarOnDisplayModeChanged(object sender, CalendarModeChangedEventArgs e)
        {
            var calendar = (Calendar)sender;
            if (calendar.DisplayMode != CalendarMode.Year)
                return;

            calendar.SelectedDate = GetSelectedCalendarDate(calendar.DisplayDate);

            var datePicker = GetCalendarsDatePicker(calendar);
            datePicker.IsDropDownOpen = false;
        }

        private static DatePicker GetCalendarsDatePicker(FrameworkElement child)
        {
            var parent = (FrameworkElement)child.Parent;
            if (parent.Name == "PART_Root")
                return (DatePicker)parent.TemplatedParent;
            return GetCalendarsDatePicker(parent);
        }

        private static DateTime? GetSelectedCalendarDate(DateTime? selectedDate)
        {
            if (!selectedDate.HasValue)
                return null;
            return new DateTime(selectedDate.Value.Year, 0, 0);
        }

    }
}