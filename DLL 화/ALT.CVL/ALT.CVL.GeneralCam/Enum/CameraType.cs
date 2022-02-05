using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALT.CVL.GeneralCam.Enum
{
    [Flags]
    public enum CameraType 
    {
        Basler = 1,
        Cognex = 2,
        Mil = 4 ,


        All = int.MaxValue
    }
}
