using ALT.CVL.GeneralCam.Enum;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALT.CVL.GeneralCam.Interface
{
    public interface IAltFrameGrabber : IFactory<ICam>
    {
        ICamInfo CamInfo { get; }
    }
}
