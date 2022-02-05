using ALT.CVL.Common.Interface;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALT.CVL.Common.CamBaseModel
{
    public class CamPixelFormat : ICamPixelFormat
    {
        internal CamPixelFormat()
        {

        }

        public bool IsMono { get; set; }
    }
}
