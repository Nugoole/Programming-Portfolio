using ALT.CVL.GeneralCam.Enum;
using ALT.CVL.GeneralCam.Model;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALT.CVL.GeneralCam.Interface
{
    public interface IAltFrameGrabbers : IReadOnlyList<IAltFrameGrabber>
    {
        CameraType CameraType { get; }
        bool IsInitialized { get; }

        void Initialize();
        void Refresh();
    }
}
