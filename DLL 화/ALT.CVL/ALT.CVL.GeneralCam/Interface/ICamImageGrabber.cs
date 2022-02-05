using System;
using System.Drawing;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ALT.CVL.GeneralCam.Interface
{
    public interface ICamImageGrabber
    {
        double FPS { get; }
        int FrameCount { get; }
        void Grab();
        void StartLive();
        void StopLive();

        event EventHandler<dynamic> OnImageGrabbed;
    }
}
