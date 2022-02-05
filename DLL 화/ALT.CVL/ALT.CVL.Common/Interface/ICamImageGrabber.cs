using System;
using System.Drawing;

namespace ALT.CVL.Common.Interface
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
