using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ALT.ResultViewer
{
    

    public class DateValue : INotifyPropertyChanged
    {

        private int value;
        private DateTime time;

        public DateTime Time
        {
            get => time; set
            {
                time = value;
                RaisePropertyChanged();
            }
        }
        public int Value
        {
            get => value; set
            {
                this.value = value;
                RaisePropertyChanged();
            }
        }

        public DateValue(DateTime time, int value)
        {
            Time = time;
            Value = value;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public partial class ResultViewer : UserControl, INotifyPropertyChanged
    {
        private static object lockObject = new object();
        private DateRangeCollection ngs;
        private RealTimeTimer timer;
        public int TotalCount { get; private set; }
        [Browsable(false)]
        public int InnerOKCount
        {
            get => innerOKCount; set
            {
                innerOKCount = value;
                RaisePropertyChanged();
            }
        }

        [Browsable(false)]
        public int InnerNGCount
        {
            get => innerNGCount;
            set
            {
                innerNGCount = value;
                RaisePropertyChanged();
            }
        }



        [Category("ResultViewer")]
        public int OKCount
        {
            get { return (int)GetValue(OKCountProperty); }
            set { SetValue(OKCountProperty, value); }
        }


        [Category("ResultViewer")]
        public int NGCount
        {
            get { return (int)GetValue(NGCountProperty); }
            set { SetValue(NGCountProperty, value); }
        }


        [Category("ResultViewer")]
        public string CameraName
        {
            get { return (string)GetValue(CameraNameProperty); }
            set { SetValue(CameraNameProperty, value); }
        }


        [Category("ResultViewer")]
        public ElapsedTimeType TimeType
        {
            get { return (ElapsedTimeType)GetValue(TimeTypeProperty); }
            set { SetValue(TimeTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TimeType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TimeTypeProperty =
            DependencyProperty.Register("TimeType", typeof(ElapsedTimeType), typeof(ResultViewer), new PropertyMetadata(ElapsedTimeType.Second,OnTimeTypeChanged));

        private static void OnTimeTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ResultViewer;
            DateTime timeNow = DateTime.Now;
            DateTime timeNowSec = new DateTime(timeNow.Year, timeNow.Month, timeNow.Day, timeNow.Hour, timeNow.Minute, timeNow.Second);
            var timeSpan = default(TimeSpan);
            control.timer.TimeType = (ElapsedTimeType)e.NewValue;
            switch (control.timer.TimeType)
            {
                case ElapsedTimeType.Second:
                    timeSpan = new TimeSpan(0, 0, 9);
                    control.dateTimeAxis.IntervalType = DateTimeIntervalType.Seconds;
                    break;
                case ElapsedTimeType.Minute:
                    timeSpan = new TimeSpan(0, 29, 0);
                    control.dateTimeAxis.IntervalType = DateTimeIntervalType.Minutes;
                    break;
                case ElapsedTimeType.Hour:
                    timeSpan = new TimeSpan(23, 0, 0);
                    control.dateTimeAxis.IntervalType = DateTimeIntervalType.Hours;
                    break;
                
            }

            control.dateTimeAxis.Maximum = timeNowSec;
            control.dateTimeAxis.Minimum = timeNowSec - timeSpan;
        }



        // Using a DependencyProperty as the backing store for CameraName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CameraNameProperty =
            DependencyProperty.Register("CameraName", typeof(string), typeof(ResultViewer), new PropertyMetadata(string.Empty, OnCameraNameChanged));

        private static void OnCameraNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ResultViewer;
            control.camNameTextBlock.Text = e.NewValue.ToString();
        }


        // Using a DependencyProperty as the backing store for NGCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NGCountProperty =
            DependencyProperty.Register("NGCount", typeof(int), typeof(ResultViewer), new PropertyMetadata(0, OnNGCountChanged));


        // Using a DependencyProperty as the backing store for OKCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OKCountProperty =
            DependencyProperty.Register("OKCount", typeof(int), typeof(ResultViewer), new PropertyMetadata(0, OnOKCountChanged));



        private static void OnNGCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ResultViewer;

            DateTime timeNow = DateTime.Now;
            DateTime timeNowSec = new DateTime(timeNow.Year, timeNow.Month, timeNow.Day, timeNow.Hour, timeNow.Minute, timeNow.Second);

            var dValue = (int)e.NewValue - (int)e.OldValue;

            while (true)
            {
                if (Monitor.TryEnter(lockObject, 10))
                {
                    if (control.ngs.Count > 0 && control.ngs.Last().Time.Equals(timeNowSec))
                    {
                        control.ngs.Last().Value += dValue;
                    }
                    else
                        control.ngs.Add(new DateValue(timeNowSec, dValue));

                    Monitor.Exit(lockObject);
                    break;
                }
            }


            control.TotalCount += dValue;
            control.totalTextBlock.Text = "Total : " + control.TotalCount;

            control.InnerNGCount = (int)e.NewValue;
            control.ngTextBlock.Text = "NG : " + control.InnerNGCount;
        }




        private static void OnOKCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ResultViewer;

            var dValue = (int)e.NewValue - (int)e.OldValue;

            control.TotalCount += dValue;
            control.totalTextBlock.Text = "Total : " + control.TotalCount;
            control.InnerOKCount = (int)e.NewValue;
            control.okTextBlock.Text = "OK : " + control.InnerOKCount;
        }


        // Using a DependencyProperty as the backing store for TotalCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TotalCountProperty =
            DependencyProperty.Register("TotalCount", typeof(int), typeof(ResultViewer), new PropertyMetadata(0));
        private int innerOKCount;
        private int innerNGCount;

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ResultViewer()
        {
            ngs = new DateRangeCollection
            {
                TimeSpanForShow = new TimeSpan(0, 0, 10)
            };
            InitializeComponent();
            totalTextBlock.Text = "Total : 0";
            okTextBlock.Text = "OK : 0";
            ngTextBlock.Text = "NG : 0";
            lineSeries.ItemsSource = ngs;
            lineSeries.DependentValuePath = "Value";
            lineSeries.IndependentValuePath = "Time";


            dateTimeAxis.IntervalType = DateTimeIntervalType.Seconds;
            timer = new RealTimeTimer();
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
        }

        private void Timer_Elapsed(object sender, EventArgs e)
        {
            DateTime timeNow = DateTime.Now;
            DateTime timeNowSec = new DateTime(timeNow.Year, timeNow.Month, timeNow.Day, timeNow.Hour, timeNow.Minute, timeNow.Second);


            while (true)
            {
                if (Monitor.TryEnter(lockObject, 10))
                {
                    if (ngs.Count > 0 && ngs.Last().Time < timeNowSec)
                        Task.Run(() =>
                        {
                            lineSeries.Dispatcher.Invoke(() =>
            {
                ngs.Add(new DateValue(timeNowSec, 0));
            });
                        }).Wait(10);

                    Monitor.Exit(lockObject);
                    break;
                }
            }

            Dispatcher.Invoke(() =>
            {
                var timeSpan = default(TimeSpan);

                switch (timer.TimeType)
                {
                    case ElapsedTimeType.Second:
                        timeSpan = new TimeSpan(0, 0, 9);
                        break;
                    case ElapsedTimeType.Minute:
                        timeSpan = new TimeSpan(0, 29, 0);
                        break;
                    case ElapsedTimeType.Hour:
                        timeSpan = new TimeSpan(23, 0, 0);
                        break;
                    
                }
                dateTimeAxis.Maximum = timeNowSec;
                dateTimeAxis.Minimum = timeNowSec - timeSpan;
            });
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
