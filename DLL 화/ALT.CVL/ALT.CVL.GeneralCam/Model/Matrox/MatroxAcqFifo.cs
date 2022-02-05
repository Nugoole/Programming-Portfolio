using ALT.CVL.GeneralCam.CamBaseModel;
using ALT.CVL.GeneralCam.Enum;
using ALT.CVL.GeneralCam.Interface;
using Matrox.MatroxImagingLibrary;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ALT.CVL.GeneralCam.Model.Matrox
{
    internal class MatroxAcqFifo : CamBase
    {

        #region Constructor

        public MatroxAcqFifo(MIL_ID digitizer)
        {
            mDigitizer = digitizer;
            
        }

        private void InitStatus()
        {
        }

        private void InitMIL()
        {
            try
            {
                MIL.MdigInquire(mDigitizer, MIL.M_OWNER_SYSTEM, ref mSystem);
                width = MIL.MdigInquire(mDigitizer, MIL.M_SIZE_X);
                height = MIL.MdigInquire(mDigitizer, MIL.M_SIZE_Y);
                camType = 8 + MIL.M_UNSIGNED;
                long attribute = MIL.M_IMAGE + MIL.M_GRAB + MIL.M_DISP;
                MIL.MbufAlloc2d(mSystem, width, height, camType, attribute, ref mImage);
                MIL.MdigHookFunction(mDigitizer, MIL.M_GRAB_FRAME_END, DeleGrab, mNull);

                
            }
            catch (Exception)
            {
            }
            
        }

        private void InitCurrentImageBuffer()
        {
            //Init byte array
            array = new byte[width * height];
            currentImage = new Bitmap((int)width, (int)height, PixelFormat.Format8bppIndexed);
            //writeableBmp = new WriteableBitmap((int)width, (int)height, 96,96, System.Windows.Media.PixelFormats.Gray8, null);
            //roiRect = new Int32Rect(0, 0, (int)width, (int)height);
            var pallete = currentImage.Palette;
            for (int i = 0; i < 256; i++)
            {
                pallete.Entries[i] = Color.FromArgb(i, i, i);
            }
            currentImage.Palette = pallete;
        }


        #endregion

        #region Variables

        byte[] array;
        private MIL_ID mSystem;
        private MIL_ID mDigitizer;
        private MIL_ID mImage;
        private MIL_ID mNull = MIL.M_NULL;
        private Bitmap currentImage;
        //private WriteableBitmap writeableBmp;
        //private Int32Rect roiRect;
        private MIL_INT width;
        private MIL_INT height;
        private MIL_INT camType;

        #endregion

        #region Properties

        #endregion

        #region Functions
        private MIL_INT DeleGrab(MIL_INT HookType, MIL_ID EventId, IntPtr UserDataPtr)
        {
            MIL.MbufGet2d(mImage, 0, 0, width, height, array);
            BitmapData bitmapData = currentImage.LockBits(new Rectangle(0, 0, currentImage.Width, currentImage.Height), ImageLockMode.ReadWrite, currentImage.PixelFormat);
            Marshal.Copy(array, 0, bitmapData.Scan0, array.Length);
            currentImage.UnlockBits(bitmapData);

            //writeableBmp.Lock();
            //Marshal.Copy(array, 0, writeableBmp.BackBuffer, array.Length);
            //writeableBmp.AddDirtyRect(roiRect);
            //writeableBmp.Unlock();
            RaiseOnImageGrabbed(currentImage);
            return 0;
        }

        public override void Connect()
        {
            InitMIL();
            //CamParams = new CamParameters(mDigitizer);
            InitCurrentImageBuffer();
            InitStatus();
        }

        public override void Disconnect()
        {
            MILFree();
        }

        private void MILFree()
        {
            MIL.MbufFree(mImage);
        }

        internal override void WhiteBalance()
        {
            
        }

        public override void Grab()
        {
            MIL.MdigGrab(mDigitizer, mImage);
        }

        public override void StartLive()
        {
        }

        public override void StopLive()
        {
            
        }

        #endregion

        #region Events


        #endregion

    }
}
