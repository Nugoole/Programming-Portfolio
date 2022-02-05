using ALT.CVL.GeneralCam.Interface;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALT.CVL.GeneralCam.Model
{
    public static class ICamExtension
    {
        public static void DoWhiteBalance(this ICam cam)
        {
            if (cam is MBasler basler)
            {
                basler.WhiteBalance();
                return;
            }
            //cam.GetType().GetMethod("WhiteBalance").Invoke(cam,null);
        }
    }
}
