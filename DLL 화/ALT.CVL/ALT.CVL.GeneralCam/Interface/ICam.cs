using ALT.CVL.GeneralCam.Enum;
using ALT.CVL.GeneralCam.Interface;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALT.CVL.GeneralCam.Interface
{
    public interface ICam : IConnectable
    {
        double FPS { get; }
        int FrameCount { get; }

        OutputImageFormat OutputImageFormat { get; set; }
        ICamInfo CamInfo { get; }
        ICamPixelFormat Format { get; }
        ICamStatus Status { get; }
        ICamParameters Parameters { get; }

        void Grab();
        void StartLive();
        void StopLive();

        event EventHandler<dynamic> OnImageGrabbed;
        //void Disconnect();
    }
}
