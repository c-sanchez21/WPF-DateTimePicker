using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPF_DateTimePickerControl
{
    /// <summary>
    /// Interaction logic for DateTimePicker.xaml
    /// </summary>
    public partial class DateTimePicker : UserControl
    {
        private const string DateFormat = "yyyy-MMM-dd ddd HH:mm:ss";

        #region Constructor(s)
        public DateTimePicker()
        {
            InitializeComponent();
        }
        #endregion

        #region Properties

        public static readonly DependencyProperty SelectedDateProperty =
            DependencyProperty.Register(
                nameof(SelectedDate),
                typeof(DateTime?),
                typeof(DateTimePicker),
                new PropertyMetadata(DateTime.Now, new PropertyChangedCallback(OnSelectedDateChanged),CoerceDate));

        public DateTime? SelectedDate
        {
            get => (DateTime?)GetValue(SelectedDateProperty);
            set => SetValue(SelectedDateProperty, value);            
        }        

        public static void OnSelectedDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not DateTimePicker dtp) return;

            if (e.NewValue is not DateTime date)
                dtp.txtDateTime.Text = string.Empty;
            else
            {
                dtp.txtDateTime.Text = date.ToString(DateFormat);
                dtp.calView.SelectedDate = date;
                dtp.calView.DisplayDate = date;
            }
        }
        #endregion
        private static object CoerceDate(DependencyObject d, object value)
        {
            //Method reserved for future use in validating date (i.e restricing date to a Min/Max)
            return value;
        }

        private void calView_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            CalendarButton.IsChecked = false;
            
            //Syntax for:
            //if(!calView.SelectedDate.HasVaule) return;
             //DateTime selectedDate = calView.SelectedDate.Value 
            if (calView.SelectedDate is not { } selectedDate) return;


            //Preserve time component - Default to midnight if SelectedDate is null
            DateTime currentTime = SelectedDate ?? DateTime.Today;            

            SelectedDate = new DateTime(
                selectedDate.Year,
                selectedDate.Month,
                selectedDate.Day,
                currentTime.Hour,
                currentTime.Minute,
                currentTime.Second
                );            
        }

        private void txtDateTime_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            //Focuses on the date component that was clicked on
            SelectDateComponent(txtDateTime.SelectionStart);            
        }

        /// <summary>
        /// Selects the date/time component user at idx.
        /// </summary>
        /// <param name="idx"></param>
        private void SelectDateComponent(int idx)
        {
            //Check for out of bounds
            if (idx >= DateFormat.Length) idx = DateFormat.Length - 1;

            //Find first letter of the DateFormat that is being selected
            //char c = DateFormat.Substring(idx, 1)[0];
            char c = DateFormat[idx];
            int first = DateFormat.IndexOf(c); 

            //Get the length of the component (i.e HH or yyyy)
            int last = DateFormat.LastIndexOf(c) + 1;
            
            //Select the date component
            txtDateTime.Focus();
            txtDateTime.Select(first,last - first);
        }

        private void txtDateTime_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            int idx = txtDateTime.SelectionStart;
            e.Handled = true; //Prevents advancing cursor
            switch(e.Key)
            {
                case Key.Up:
                    SelectedDate = AddToDate(idx, 1);//Increment
                    SelectDateComponent(idx);
                    break;
                case Key.Down:
                    SelectedDate = AddToDate(idx, -1);//Decrement
                    SelectDateComponent(idx);
                    break;
                case Key.Left:
                    MoveLeft(idx);
                    break;
                case Key.Tab:
                case Key.Right:
                    MoveRight(idx);
                    break;
                //In case user inputs a digit
                case >= Key.D0 and <= Key.D9:
                case >= Key.NumPad0 and <= Key.NumPad9:
                    e.Handled = false; //Allow user to input digits                
                    break;
            }
        }

        private void MoveLeft(int idx)
        {
            //Check out of bounds
            if (idx <= 0 || idx >= DateFormat.Length) return;

            //DateFormat char that we start with
            char first = DateFormat[idx];

            //Previous character index
            int prev = idx - 1;

            //Keep moving left until a new DateFormat letter is found or prev < 0
            while (prev >= 0 && (DateFormat[prev] == first || !char.IsLetter(DateFormat[prev])))
                prev--;

            //Select previous control if prev < 0
            if (prev < 0)
                this.MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
            //Otherwise select the previous Date/Time component
            else SelectDateComponent(prev); 
        }

        private void MoveRight(int idx)
        {
            //Check out of bounds
            if (idx < 0 || idx >= DateFormat.Length) return;

            //DateFormat char that we start with
            char first = DateFormat[idx];

            //Next character index
            int next = idx + 1;

            //Keep moving right until a new DateFormat letter is found or next is out of bounds
            int max = DateFormat.Length; 
            while (next < max && (DateFormat[next] == first || !char.IsLetter(DateFormat[next])))
                next++;

            if (next == max)
                CalendarButton.Focus();
            else SelectDateComponent(next);
        }

        private DateTime? AddToDate(int idx, int val)
        {            
            //If parsing fails return revert to existing SelectedDate
            if (!DateTime.TryParse(txtDateTime.Text, out DateTime d))
                return SelectedDate;

            //First letter of the DateFormat selected
            char c = DateFormat[idx];

            return c switch
            {
                'y' => d.AddYears(val),
                'M' => d.AddMonths(val),
                'd' => d.AddDays(val),
                'h' or 'H' => d.AddHours(val),
                'm' => d.AddMinutes(val),
                's' => d.AddSeconds(val),
                'f' => d.AddMilliseconds(val),
                _ => d
            };
        }

        private void txtDateTime_LostFocus(object sender, RoutedEventArgs e)
        {            
            if (DateTime.TryParse(txtDateTime.Text, out DateTime d))
                SelectedDate = d;
            else txtDateTime.Undo();
        }
    }
}