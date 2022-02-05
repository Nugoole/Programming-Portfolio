using ALT.CVL.GeneralCam.CamBaseModel;
using ALT.CVL.GeneralCam.Interface;
using ALT.CVL.GeneralCam.Model.CamBaseModel;
using ALT.CVL.GeneralCam.Model.Matrox;
using Matrox.MatroxImagingLibrary;
using System.Text;

namespace ALT.CVL.GeneralCam.Model
{
    internal class MatroxGrabbers : FrameGrabbersBase
    {

        #region Constructor

        public MatroxGrabbers()
        {
            
        }


        #endregion

        #region Variables
        private MIL_ID mNull = MIL.M_NULL;
        private MIL_ID mDefault = MIL.M_DEFAULT;
        private MIL_ID mApplication;
        private MIL_ID numOfCamera;
        private MIL_ID mSystem;
        private MIL_ID mSystemType;

        public override void Initialize()
        {
            MIL.MappAlloc(mNull, mDefault, ref mApplication);
            MIL.MsysAlloc(mDefault, MIL.M_SYSTEM_DEFAULT, MIL.M_DEV0, mDefault, ref mSystem);
            MIL.MsysInquire(mSystem, MIL.M_SYSTEM_TYPE, ref mSystemType);
            MIL.MsysInquire(mSystem, MIL.M_NUM_CAMERA_PRESENT, ref numOfCamera);
            for (int i = 0; i < numOfCamera; i++)
            {
                MIL_ID mDigitizer = default;
                MIL.MdigAlloc(mSystem, MIL.M_DEV0 + i, "M_DEFAULT", mDefault, ref mDigitizer);
                Add(new MatroxGrabber(new CamInfo(), mDigitizer));
            }
        }
        #endregion

        #region Properties

        #endregion

        #region Functions

        #endregion

    }
}