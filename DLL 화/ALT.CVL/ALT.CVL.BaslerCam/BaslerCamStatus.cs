using ALT.CVL.Common.Interface;

using Basler.Pylon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALT.CVL.BaslerCam
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
