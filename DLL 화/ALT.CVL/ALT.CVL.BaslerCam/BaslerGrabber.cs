

using ALT.CVL.Common.CamBaseModel;
using ALT.CVL.Common.Interface;
using ALT.CVL.Common.Model;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALT.CVL.BaslerCam
{
    internal class BaslerGrabber : FrameGrabberBase
    {
        internal BaslerGrabber(CamInfo caminfo):base(caminfo)
        {

        }

        public override ICam Create()
        {
            camera = new MBasler(CamInfo.SerialNumber);
            (camera as MBasler).CamInfoSetter = CamInfo as CamInfo;
            camera.Connect();
            return camera;
        }
    }
}
