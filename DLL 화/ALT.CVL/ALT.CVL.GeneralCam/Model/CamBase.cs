using ALT.CVL.GeneralCam.CamBaseModel;
using ALT.CVL.GeneralCam.Enum;
using ALT.CVL.GeneralCam.Interface;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ALT.CVL.GeneralCam.Model
{
    /// <summary>
    /// 카메라의 기본기능이 들어있는 베이스 클래스 입니다.
    /// </summary>
    internal abstract class CamBase : ICam
    {
        private double fps;

        protected CamParameters camParameters;
        protected CamInfo camInfo;
        protected CamPixelFormat format;
        protected ICamStatus status;
        private System.Timers.Timer fpsTimer;
        private int frameCountBuffer;

        protected event EventHandler OnConnected;
        public event EventHandler<dynamic> OnImageGrabbed;
        public double FPS
        {
            get => fps;
        }
        public int FrameCount { get; internal set; }
        public OutputImageFormat OutputImageFormat { get; set; }
        /// <summary>
        /// 카메라의 파라미터들의 리스트
        /// </summary>
        public virtual ICamParameters Parameters => camParameters;
        /// <summary>
        /// 카메라 정보 인터페이스
        /// </summary>
        public virtual ICamInfo CamInfo => camInfo;
        /// <summary>
        /// 카메라의 픽셀 포맷 정보
        /// </summary>
        public virtual ICamPixelFormat Format => format;
        /// <summary>
        /// 카메라의 상태 정보 인터페이스
        /// </summary>
        public virtual ICamStatus Status => status;

        protected CamBase()
        {
            OnImageGrabbed += ImageGrabber_OnImageGrabbed;
            
            InitializeTimer();
        }
        

        private void InitializeTimer()
        {
            fpsTimer = new System.Timers.Timer(500);
            frameCountBuffer = 0;
            fpsTimer.Elapsed += FpsTimer_Elapsed;
            fpsTimer.Start();
        }

        protected void RaiseOnConnected()
        {
            OnConnected?.Invoke(this, EventArgs.Empty);
        }

        private void ImageGrabber_OnImageGrabbed(object sender, dynamic e)
        {
            FrameCount++;
            if (FrameCount % 4 == 0)
                GC.Collect();
        }

        private void FpsTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            fps = (FrameCount - frameCountBuffer) * (1000 / fpsTimer.Interval);
            frameCountBuffer = FrameCount;
        }

        internal abstract void WhiteBalance();
        ///// <summary>
        ///// 카메라를 연결합니다.
        ///// </summary>
        ///// <param name="camId">
        ///// <see cref="MBasler.AvailableCameras"/>에서 가져온 카메라 정보
        ///// </param>
        //public abstract void Connect(string id);
        public abstract void Connect();
        public abstract void Grab();
        public abstract void StartLive();
        public abstract void StopLive();


        public virtual void Disconnect()
        {
            OnImageGrabbed -= ImageGrabber_OnImageGrabbed;
        }

        protected void RaiseOnImageGrabbed(dynamic image)
        {
            OnImageGrabbed?.Invoke(this, image);
        }

    }
}
