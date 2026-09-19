using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPF_DateTimePicker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// Solution to experiment with Date/Time Controls
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        DateTime myDate = new DateTime(2021, 3, 21, 12, 30, 0);
        public DateTime MyDate
        {
            get
            {
                return myDate;
            }
            set
            {
                myDate = value;
                NotifyPropertyChanged();
            }
        }
        public DateTime? SelectedDate
        {
            get
            {
                return dtpDate.SelectedDate;
            }
            set
            {
                dtpDate.SelectedDate = value;
                NotifyPropertyChanged();
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;

        }
        private void btnDisplayDate_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(String.Format("MyDate:= {0:yyyy-MMM-dd ddd, HH:mm:ss}", MyDate));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
