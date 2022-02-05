using ALT.CVL.GeneralCam.Interface;

using Basler.Pylon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALT.CVL.GeneralCam.Model.Basler
{
    internal class BaslerCamStatus : ICamStatus
    {
        private readonly Camera cam;

        public bool IsConnected => cam.IsConnected;
        public bool IsLive { get; set; }

        public BaslerCamStatus(Camera camera)
        {
            cam = camera;
        }
    }
}
