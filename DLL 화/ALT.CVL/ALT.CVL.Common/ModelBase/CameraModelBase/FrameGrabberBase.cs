using ALT.CVL.Common.Enum;
using ALT.CVL.Common.Interface;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALT.CVL.Common.Model
{
    public abstract class FrameGrabberBase : IAltFrameGrabber, IDisposable
    {
        protected ICam camera = null;

        public ICamInfo CamInfo { get; protected set; }


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
