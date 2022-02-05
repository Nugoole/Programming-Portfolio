using ALT.CVL.GeneralCam.Enum;
using ALT.CVL.GeneralCam.Interface;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALT.CVL.GeneralCam.Model
{
    internal abstract class FrameGrabberBase : IAltFrameGrabber, IDisposable
    {
        protected ICam camera = null;

        public ICamInfo CamInfo { get; internal set; }


        protected FrameGrabberBase(ICamInfo caminfo)
        {
            CamInfo = caminfo;
        }

        public abstract ICam Create();
        

        public void Dispose()
        {
            if (camera != null && camera.Status.IsConnected)
                camera.Disconnect();
        }
    }
}
