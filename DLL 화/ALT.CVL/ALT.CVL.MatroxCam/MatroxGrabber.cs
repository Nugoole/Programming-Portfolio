using ALT.CVL.Common.CamBaseModel;
using ALT.CVL.Common.Interface;
using ALT.CVL.Common.Model;
using Matrox.MatroxImagingLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALT.CVL.MatroxCam
{
    internal class MatroxGrabber : FrameGrabberBase
    {

        #region Constructor

        public MatroxGrabber(ICamInfo caminfo, MIL_ID digitizer) : base(caminfo)
        {
            mDigitizer = digitizer;
            CamInfo = GetCamInfo(mDigitizer);
        }

        #endregion

        #region Variables

        private MIL_ID mDigitizer;

        #endregion

        #region Properties

        #endregion

        #region Functions

        #endregion

        public void Disconnect()
        {
            
        }

        public override ICam Create()
        {
            ICam result = new MatroxAcqFifo(mDigitizer);
            return result;
        }

        private static ICamInfo GetCamInfo(MIL_ID mDigitizer)
        {
            StringBuilder modelName = new StringBuilder();
            StringBuilder serialNumber = new StringBuilder();
            MIL.MdigInquire(mDigitizer, MIL.M_CAMERA_MODEL, modelName);
            MIL.MdigInquire(mDigitizer, MIL.M_GC_SERIAL_NUMBER, serialNumber);
            ICamInfo camInfo = new CamInfo { CamName = modelName.ToString(), SerialNumber = serialNumber.ToString() };
            return camInfo;
        }
    }
}
