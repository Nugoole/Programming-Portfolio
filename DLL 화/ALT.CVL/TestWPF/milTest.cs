using Matrox.MatroxImagingLibrary;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


delegate void MBlobControl(MIL_ID id, long controlType, double controlvalue);



namespace TestWPF
{
    struct UserValStruct
    {
        private MIL_ID milDisplay;
        private MIL_ID milBlob;
        private MIL_ID milROI;
        private MIL_ID milGraphicList;
        private MIL_ID blobResult;
    }

    public class milTest
    {
        private const int DEFAULT = MIL.M_DEFAULT;

        private MBlobControl blobControl;
        private MIL_ID milApplication;
        private MIL_ID milSystem;
        private MIL_ID milDigitizer;
        private MIL_ID[] milImage;
        private MIL_ID milDisplay;
        private MIL_ID milBlob;
        private MIL_ID milMonoImage;
        private MIL_ID milBlobImage;
        private MIL_ID milBinImage;
        private MIL_ID milROI;
        private MIL_ID milGraphicList;
        private MIL_ID milBlobResultGraphicList;
        private MIL_INT x = default, y = default, band = default;
        private MIL_ID blobResult;
        private IntPtr imageOuputHandle;
        private MIL_GRA_HOOK_FUNCTION_PTR hookHandler;
        private MIL_DIG_HOOK_FUNCTION_PTR hookFunction;

        public IntPtr ImageOuputHandle { get => imageOuputHandle; set => imageOuputHandle = value; }

        public MIL_ID DisplayId => milDisplay;
        public milTest()
        {
            blobControl = new MBlobControl(MIL.MblobControl);
            hookHandler = new MIL_GRA_HOOK_FUNCTION_PTR(GraphicModifiedHookFunction);

            MIL.MappAlloc(MIL.M_NULL, MIL.M_DEFAULT, ref milApplication);
            MIL.MsysAlloc(MIL.M_DEFAULT, MIL.M_SYSTEM_GIGE_VISION, MIL.M_DEV0, MIL.M_DEFAULT, ref milSystem);
            MIL.MdigAlloc(milSystem, MIL.M_DEV0, "M_DEFAULT", MIL.M_DEFAULT, ref milDigitizer);
            MIL.MdispAlloc(milSystem, MIL.M_DEFAULT, "M_DEFAULT", MIL.M_WPF, ref milDisplay);



            MIL.MdigInquire(milDigitizer, MIL.M_SIZE_X, ref x);
            MIL.MdigInquire(milDigitizer, MIL.M_SIZE_Y, ref y);
            MIL.MdigInquire(milDigitizer, MIL.M_SIZE_BAND, ref band);

            MIL.MblobAlloc(milSystem, DEFAULT, DEFAULT, ref milBlob);
            MIL.MblobAllocResult(milSystem, DEFAULT, DEFAULT, ref blobResult);
            //blobControl(milBlob, MIL.M_NUMBER_OF_HOLES, MIL.M_ENABLE);
            blobControl(milBlob, MIL.M_CONVEX_HULL, MIL.M_ENABLE);
            //blobControl(milBlob, MIL.M_CENTER_OF_GRAVITY, MIL.M_ENABLE);
            //blobControl(milBlob, MIL.M_FERETS, MIL.M_ENABLE);

            long attribute = MIL.M_IMAGE + MIL.M_GRAB + MIL.M_DISP + MIL.M_PROC;

            milImage = new MIL_ID[3];

            for (int i = 0; i < milImage.Length; i++)
            {
                if (band == 1)
                    MIL.MbufAlloc2d(milSystem, x, y, 8 + MIL.M_UNSIGNED, attribute, ref milImage[i]);
                else if (band == 3)
                    MIL.MbufAllocColor(milSystem, band, x, y, 8 + MIL.M_UNSIGNED, attribute, ref milImage[i]);
            }


            MIL.MbufAlloc2d(milSystem, x, y, 8 + MIL.M_UNSIGNED, attribute, ref milMonoImage);
            MIL.MbufAlloc2d(milSystem, x, y, 8 + MIL.M_UNSIGNED, attribute, ref milBlobImage);


            MIL.MdispSelect(milDisplay, milImage[0]);

            MIL.MgraAllocList(milSystem, DEFAULT, ref milGraphicList);
            MIL.MgraAllocList(milSystem, DEFAULT, ref milBlobResultGraphicList);

            MIL.MdispControl(milDisplay, MIL.M_ASSOCIATED_GRAPHIC_LIST_ID, milGraphicList);
            MIL.MgraAlloc(milSystem, ref milROI);

            MIL.MdispControl(milDisplay, MIL.M_SCALE_DISPLAY, MIL.M_ENABLE);
            MIL.MdispControl(milDisplay, MIL.M_GRAPHIC_LIST_INTERACTIVE, MIL.M_ENABLE);


            int dx, dy;
            dx = 500;
            dy = 500;


            MIL.MgraControlList(milGraphicList, MIL.M_LIST, MIL.M_DEFAULT, MIL.M_ACTION_KEYS, MIL.M_ENABLE);
            MIL.MgraRect(milROI, milGraphicList, dx, dy, x - dx, y - dy);
            MIL.MgraControl(milROI, MIL.M_SELECTABLE, MIL.M_DISABLE);
            //MIL.MgraInteractive(milROI, milGraphicList, MIL.M_GRAPHIC_TYPE_RECT, DEFAULT, MIL.M_AXIS_ALIGNED_RECT);

            UserValStruct a = new UserValStruct();
            IntPtr ptr = GCHandle.ToIntPtr(GCHandle.Alloc(a));
            MIL.MgraHookFunction(milGraphicList, MIL.M_GRAPHIC_MODIFIED, hookHandler, ptr);

            hookFunction = new MIL_DIG_HOOK_FUNCTION_PTR(HookFunction);
        }

        private MIL_INT GraphicModifiedHookFunction(MIL_INT hookType, MIL_ID eventId, IntPtr usrPtr)
        {
            DoBlob();

            return 0;
        }
        private MIL_INT HookFunction(MIL_INT HookType, MIL_ID EventId, IntPtr UserDataPtr)
        {
            return 0;
        }
        public void Grab()
        {
            MIL.MdigProcess(milDigitizer, milImage, 3, MIL.M_START, MIL.M_DEFAULT, hookFunction, MIL.M_NULL);
            //var threadStopper = true;
            //new Thread(() =>
            //{

            //    MIL.MdigProcess(milDigitizer, milImage, 3, MIL.M_START, MIL.M_DEFAULT, hookFunction, MIL.M_NULL);
            //    //while (threadStopper) { };
            //    //MIL.MdigProcess(milDigitizer, milImage, 3, MIL.M_STOP, MIL.M_DEFAULT, hookFunction, MIL.M_NULL);


            //}).Start();



            //IntPtr ptr = default;

            //MIL.MdigGrab(milDigitizer, milImage);
            //MIL.MdigGrabWait(milDigitizer, MIL.M_GRAB_FRAME_END);



            //MIL.MdispSelect(milDisplay, milImage);
        }

        private void DoBlob()
        {
            MIL.MdispControl(milDisplay, MIL.M_UPDATE, MIL.M_DISABLE);

            MIL.MimConvert(milImage[0], milMonoImage, MIL.M_RGB_TO_L);



            int numOfGraphics = 0;
            MIL.MgraInquireList(milGraphicList, MIL.M_LIST, DEFAULT, MIL.M_NUMBER_OF_GRAPHICS, ref numOfGraphics);

            for (int i = numOfGraphics - 1; i > 0; i--)
            {
                MIL.MgraControlList(milGraphicList, MIL.M_GRAPHIC_INDEX(i), DEFAULT, MIL.M_DELETE, DEFAULT);
            }



            long attribute = MIL.M_IMAGE + MIL.M_GRAB + MIL.M_DISP + MIL.M_PROC;

            if (milBinImage != MIL.M_NULL)
                MIL.MbufFree(milBinImage);

            MIL.MbufAlloc2d(milSystem, x, y, 1 + MIL.M_UNSIGNED, MIL.M_IMAGE + MIL.M_PROC + MIL.M_DISP, ref milBinImage);
            MIL.MimBinarize(milMonoImage, milBinImage, MIL.M_FIXED + MIL.M_LESS_OR_EQUAL, 80, MIL.M_NULL);
            MIL.MimOpen(milBinImage, milBinImage, 3, MIL.M_BINARY);
            MIL.MimClose(milBinImage, milBinImage, 3, MIL.M_BINARY);










            /******** SET PARAMETERS TO ENABLE *************/


            //            blobControl(milBlob, MIL.M_CONVEX_HULL_AREA,MIL.M_DISABLE);


            MIL.MbufSetRegion(milBinImage, milGraphicList, DEFAULT, MIL.M_RASTERIZE + MIL.M_FILL_REGION, DEFAULT);

            /******** END OF SETTING PARAMETERS ************/






            //Run Blob
            MIL.MblobCalculate(milBlob, milBinImage, MIL.M_NULL, blobResult);


            //Get Results




            //int totalBlobs = default;
            //MIL.MblobGetResult(blobResult, DEFAULT, MIL.M_NUMBER + MIL.M_TYPE_MIL_INT, ref totalBlobs);
            //double[] CogX = new double[totalBlobs];
            //double[] CogY = new double[totalBlobs];




            //MIL.MblobGetResult(blobResult, DEFAULT, MIL.M_CENTER_OF_GRAVITY_X + MIL.M_BINARY, CogX);
            //MIL.MblobGetResult(blobResult, DEFAULT, MIL.M_CENTER_OF_GRAVITY_Y + MIL.M_BINARY, CogY);

            MIL.MgraColor(DEFAULT, MIL.M_COLOR_RED);
            MIL.MblobDraw(DEFAULT, blobResult, milGraphicList, MIL.M_DRAW_CONVEX_HULL_CONTOUR, MIL.M_INCLUDED_BLOBS, DEFAULT);


            MIL.MdispControl(milDisplay, MIL.M_UPDATE, MIL.M_ENABLE);
            //MIL.MdispSelect(milDisplay, milBinImage);
        }
    }
}
