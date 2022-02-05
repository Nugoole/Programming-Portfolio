using Matrox.MatroxImagingLibrary;

using System;
using System.Collections.Generic;
using System.Text;

namespace TestWPF
{
    public class milPMAlign
    {
        private const int DEFAULT = MIL.M_DEFAULT;
        private MIL_ID milSystem;
        private MIL_ID milApplication;
        private MIL_ID milDigitizer;
        private MIL_ID milDisplay;
        private MIL_INT x;
        private MIL_INT y;
        private MIL_INT band;
        private MIL_ID milImage;
        private MIL_ID milMonoImage;
        private MIL_ID milModelFinder;
        private MIL_ID milModelFinderResult;
        private MIL_ID milGraphicList;
        private MIL_ID milROI;

        public MIL_ID DisplayId => milDisplay;

        public milPMAlign()
        {
            MIL.MappAlloc(MIL.M_NULL, MIL.M_DEFAULT, ref milApplication);
            MIL.MsysAlloc(MIL.M_DEFAULT, MIL.M_SYSTEM_GIGE_VISION, MIL.M_DEV0, MIL.M_DEFAULT, ref milSystem);
            MIL.MdigAlloc(milSystem, MIL.M_DEV0, "M_DEFAULT", MIL.M_DEFAULT, ref milDigitizer);
            MIL.MdispAlloc(milSystem, MIL.M_DEFAULT, "M_DEFAULT", MIL.M_WPF, ref milDisplay);


            MIL.MdigInquire(milDigitizer, MIL.M_SIZE_X, ref x);
            MIL.MdigInquire(milDigitizer, MIL.M_SIZE_Y, ref y);
            MIL.MdigInquire(milDigitizer, MIL.M_SIZE_BAND, ref band);

            long attribute = MIL.M_IMAGE + MIL.M_GRAB + MIL.M_DISP + MIL.M_PROC;

            if (band == 1)
                MIL.MbufAlloc2d(milSystem, x, y, 8 + MIL.M_UNSIGNED, attribute, ref milImage);
            else if (band == 3)
                MIL.MbufAllocColor(milSystem, band, x, y, 8 + MIL.M_UNSIGNED, attribute, ref milImage);

            MIL.MbufAlloc2d(milSystem, x, y, 8 + MIL.M_UNSIGNED, attribute, ref milMonoImage);


            MIL.MmodAlloc(milSystem, MIL.M_GEOMETRIC, DEFAULT, ref milModelFinder);
            MIL.MmodAllocResult(milSystem, DEFAULT, ref milModelFinderResult);

            MIL.MgraAllocList(milSystem, DEFAULT, ref milGraphicList);

            MIL.MdispControl(milDisplay, MIL.M_ASSOCIATED_GRAPHIC_LIST_ID, milGraphicList);
            MIL.MgraAlloc(milSystem, ref milROI);


            MIL.MdispControl(milDisplay, MIL.M_GRAPHIC_LIST_INTERACTIVE, MIL.M_ENABLE);

            int dx, dy;
            dx = 500;
            dy = 500;


            MIL.MgraControlList(milGraphicList, MIL.M_LIST, MIL.M_DEFAULT, MIL.M_ACTION_KEYS, MIL.M_ENABLE);
            MIL.MgraRect(milROI, milGraphicList, dx, dy, x - dx, y - dy);
            MIL.MgraControl(milROI, MIL.M_SELECTABLE, MIL.M_DISABLE);
            //MIL.MgraInteractive(milROI, milGraphicList, MIL.M_GRAPHIC_TYPE_RECT, DEFAULT, MIL.M_AXIS_ALIGNED_RECT);

            
            
        }

        public void Grab()
        {
            MIL.MdigGrab(milDigitizer, milImage);
            MIL.MdigGrabWait(milDigitizer, MIL.M_GRAB_FRAME_END);


            

            

            

            MIL.MimConvert(milImage, milMonoImage, MIL.M_RGB_TO_L);

            MIL.MdispSelect(milDisplay, milMonoImage);
        }

        public void Train()
        {
            MIL.MbufSetRegion(milMonoImage, milGraphicList, DEFAULT, MIL.M_RASTERIZE + MIL.M_FILL_REGION, DEFAULT);
            MIL.MmodDefine(milModelFinder, MIL.M_IMAGE, milMonoImage, DEFAULT, DEFAULT, DEFAULT, DEFAULT);
            MIL.MmodControl(milModelFinder, MIL.M_CONTEXT, MIL.M_SPEED, MIL.M_VERY_HIGH);

            MIL.MmodPreprocess(milModelFinder, DEFAULT);

            MIL.MgraColor(DEFAULT, MIL.M_COLOR_RED);
            MIL.MmodDraw(DEFAULT, milModelFinder, milGraphicList, MIL.M_DRAW_BOX + MIL.M_DRAW_POSITION, 0, MIL.M_ORIGINAL);
            MIL.MbufSetRegion(milMonoImage, MIL.M_NULL, DEFAULT, MIL.M_DELETE, DEFAULT);
        }

        public void Find()
        {
            Grab();

            MIL.MgraClear(DEFAULT, milGraphicList);
            MIL.MmodFind(milModelFinder, milMonoImage, milModelFinderResult);
            MIL.MmodDraw(DEFAULT, milModelFinderResult, milGraphicList, MIL.M_DRAW_EDGES + MIL.M_DRAW_POSITION, MIL.M_ALL, DEFAULT);
        }
    }
}
