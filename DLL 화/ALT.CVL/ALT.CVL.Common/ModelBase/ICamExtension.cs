using ALT.CVL.Common.Interface;
using ALT.CVL.Common.Model;


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALT.CVL.Common.Extension
{
    public static class ICamExtension
    {
        public static void DoWhiteBalance(this ICam cam)
        {
            if (cam is CamBase camBase)
            {
                camBase.WhiteBalance();
                return;
            }
            //cam.GetType().GetMethod("WhiteBalance").Invoke(cam,null);
        }
    }
}
