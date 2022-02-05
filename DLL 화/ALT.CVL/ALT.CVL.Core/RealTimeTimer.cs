using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALT.CVL.Core
{
    public class RealTimeTimer
    {
        private System.Timers.Timer initializeTimer;
        private System.Timers.Timer timer;
        private bool doingThings = false;

        public bool Enabled => timer.Enabled;

        /// <summary>
        /// 타이머의 초단위 경과 시 일어나는 이벤트 입니다.
        /// </summary>
        public event EventHandler<DateTime> RealTimeSecondTick;
        public RealTimeTimer()
        {
          
            timer = new System.Timers.Timer
            {
                Interval = 10
            };
            timer.Elapsed += Timer_Tick;
        }

        

        private void Timer_Tick(object sender, EventArgs e)
        {
            var dateTime = DateTime.Now;


            //정 시간 출력
            if (doingThings && dateTime.Millisecond < 100)
            {
                doingThings = false;

                RealTimeSecondTick?.Invoke(this, dateTime);
                return;
            }
            else if (dateTime.Millisecond >= 100)
                doingThings = true;
        }

        public void Start()
        {
            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
        }
    }
}
