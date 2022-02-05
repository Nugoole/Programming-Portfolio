using ALT.CVL.GeneralCam.CamBaseModel;
using ALT.CVL.GeneralCam.Enum;
using ALT.CVL.GeneralCam.Interface;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALT.CVL.GeneralCam.Model
{
    internal class BaslerGrabber : FrameGrabberBase
    {
        internal BaslerGrabber(CamInfo caminfo):base(caminfo)
        {

        }

        public override ICam Create()
        {
            camera = new MBasler(CamInfo.SerialNumber);
            //camera.Connect();
            return camera;
        }
    }
}
