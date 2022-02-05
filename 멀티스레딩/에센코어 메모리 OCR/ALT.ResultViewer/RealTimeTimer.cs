using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALT.ResultViewer
{
    public enum ElapsedTimeType
    {
        Second,
        Minute,
        Hour
    }

    public class RealTimeTimer
    {
        private Stopwatch stopwatch;
        private int tick = 1000;
        private Task currentTask = default;
        private ElapsedTimeType timeType = ElapsedTimeType.Second;
        private bool isRunning = false;


        public bool IsRunning => isRunning;
        public ElapsedTimeType TimeType
        {
            get => timeType;
            set
            {
                timeType = value;

                if (value == ElapsedTimeType.Second)
                    tick = 1000;
                else if (value == ElapsedTimeType.Minute)
                    tick = 1000 * 60;
                else if (value == ElapsedTimeType.Hour)
                    tick = 1000 * 60 * 60;
            }
        }
        public event EventHandler Elapsed;
        public RealTimeTimer()
        {
            stopwatch = new Stopwatch();
        }


        private void TimeCount()
        {
            stopwatch.Start();

            while (isRunning)
            {
                if (stopwatch.ElapsedMilliseconds > tick)
                {
                    Elapsed?.BeginInvoke(this, EventArgs.Empty, null, null);
                    stopwatch.Restart();
                }
            }

            stopwatch.Stop();
            stopwatch.Reset();
        }


        public void Start()
        {
            if (!isRunning)
            {
                currentTask = Task.Run(TimeCount);
                isRunning = true;
            }
        }

        public void Stop()
        {
            if(isRunning)
            {
                isRunning = false;
                Task.WaitAll(currentTask);
                currentTask = null;
            }
        }
    }
}
