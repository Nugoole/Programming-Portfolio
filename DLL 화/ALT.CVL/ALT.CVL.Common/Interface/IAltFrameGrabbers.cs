using ALT.CVL.Common.Enum;
using ALT.CVL.Common.Model;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALT.CVL.Common.Interface
{
    public interface IAltFrameGrabbers : IReadOnlyList<IAltFrameGrabber>
    {
        CameraType CameraType { get; }
        bool IsInitialized { get; }

        void Initialize();
        void Refresh();
    }
}
