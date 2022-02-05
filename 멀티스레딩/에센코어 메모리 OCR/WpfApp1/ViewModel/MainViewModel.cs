using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using LiveCharts;
using LiveCharts.Defaults;

using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using LiveCharts.Wpf;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using LiveCharts.Configurations;
using LiveCharts.Helpers;
using System.Threading.Tasks;

namespace WpfApp1.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private ChartValues<DateTimePoint> points;
        private double left;
        private double right;
        private PeriodUnits period = PeriodUnits.Days;
        private DateTime initDateTime;
        private int oK;
        private int nG;

        public PeriodUnits Period
        {
            get => period; set
            {
                Set(ref period, value);
            }
        }

        public DateTime InitDateTime { get => initDateTime; set => Set(ref initDateTime, value); }
        public int OK { get => oK; set => Set(ref oK, value); }
        public int NG { get => nG; set => Set(ref nG, value); }
        public double Left { get => left; set => Set(ref left, value); }
        public double Right { get => right; set => Set(ref right, value); }

        public RelayCommand AddOK { get; set; }
        public RelayCommand AddNG { get; set; }
        public RelayCommand Add { get; set; }
        
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            InitDateTime = DateTime.Today;
            Period = PeriodUnits.Milliseconds;

            OK = 0;
            NG = 0;

            Add = new RelayCommand(AddValue);
            AddOK = new RelayCommand(ADDOK);
            AddNG = new RelayCommand(ADDNG);
            



            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}
        }

        private void ADDNG()
        {
            var tasks = new List<Task>();
            for (int i = 0; i < 100; i++)
            {
                tasks.Add(new Task(() =>
                {
                    NG++;
                }));
            }

            var taskArr = tasks.ToArray();

            Parallel.ForEach(taskArr, x => x.Start());
            Task.WaitAll(taskArr);
        }

        private void ADDOK()
        {
            OK++;
        }

        private void AddValue()
        {
            //Random random = new Random(DateTime.Now.Millisecond);
            //points.Add(new DateTimePoint(DateTime.Now, random.Next(0, 50)));

            //Left = points.Last().X - 100;
            //Right = points.Last().X;
            //AnimationTime = new TimeSpan(0, 0, 0, 0, 000);
        }
    }
}