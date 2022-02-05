using ALT.CVL.GeneralCam.Interface;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALT.CVL.GeneralCam.CamBaseModel
{
    internal class CamPixelFormat : ICamPixelFormat
    {
        public bool IsMono { get; internal set; }
    }
}
