using Matrox.MatroxImagingLibrary;

using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LineScanViewer.Model
{
    public class CamNode
    {
        public string Key { get; set; }
        public string Node { get; set; }
        public int DEVn { get; set; }
    }

    public static class Enums
    {
        public enum ImageFormat
        {
            Mono,
            Color
        };
    }

    class HookDataStruct
    {

    };

    public class MCam
    {
        private Tree<CamNode> camNodes;

        private const int DEFAULT_IMAGE_SIZE_X = 0;
        private const int DEFAULT_IMAGE_SIZE_Y = 0;
        private const int DEFAULT_IMAGE_SIZE_BAND = 1;
        private readonly Enums.ImageFormat FormatColor = Enums.ImageFormat.Color;
        private readonly Enums.ImageFormat FormatMono = Enums.ImageFormat.Mono;


        MIL_ID MilApplication = MIL.M_NULL;  // MIL Application identifier.
        MIL_ID MilSystem = MIL.M_NULL;       // MIL System identifier.=
        MIL_ID MilDigitizer = MIL.M_NULL;    // MIL Digitizer identifier.

        MIL_INT BufSizeX = DEFAULT_IMAGE_SIZE_X;
        MIL_INT BufSizeY = DEFAULT_IMAGE_SIZE_Y;
        MIL_INT BufSizeBand = DEFAULT_IMAGE_SIZE_BAND;// MIL Image buffer identifier.

        private MIL_ID BufResultDisp;
        private MIL_ID[] BufGrabDisp;
        private MIL_ID BufGrabSubDisp;
        private MIL_DIG_HOOK_FUNCTION_PTR hookFunction;
        private HookDataStruct userHookData;

        private int Attributes;
        private int frameCount = 0;
        private byte[] byteImage;
        private byte[] tempByteArr;
        private bool threadStopper;
        private int scanLength;

        public int LiveYLength { get; set; }
        public string SaveImagePath { get; set; }
        public Enums.ImageFormat ImageFormat { get; set; }
        public bool IsDigitizerSet { get => MilDigitizer != MIL.M_NULL; }
        public Tree<CamNode> CamTree { get => camNodes; set => camNodes = value; }

        public event EventHandler<BitmapSource> OnGrab;

        public MCam()
        {
            InitCam();
        }
        private void InitCam()
        {
            MIL.MappAlloc(MIL.M_NULL, MIL.M_DEFAULT, ref MilApplication);

            MIL_INT numOfSys = 0;
            MIL.MappInquire(MilApplication, MIL.M_INSTALLED_SYSTEM_COUNT, ref numOfSys);

            if (numOfSys > 0)
            {
                camNodes = new Tree<CamNode>();

                int indexValue = 0;

                //연결된 보드 및 장치 검색
                while (numOfSys > 0)
                {
                    StringBuilder system = new StringBuilder();
                    MIL.MappInquire(MilApplication, MIL.M_INSTALLED_SYSTEM_DESCRIPTOR + indexValue++, system);

                    if (!string.IsNullOrEmpty(system.ToString()))
                    {
                        camNodes.AddChild(new CamNode { Node = system.ToString() });
                        numOfSys--;
                    }

                    if (indexValue > 16)
                        break;
                }
            }


            for (int i = 0; i < camNodes.ChildTrees.Count; i++)
            {
                MIL_ID sysTemp = MIL.M_NULL;
                MIL.MsysAlloc(MilApplication, camNodes.ChildTrees[i][0].Node, MIL.M_DEFAULT, MIL.M_DEFAULT, ref sysTemp);

                MIL_INT numOfDigitizer = 0;

                MIL.MsysInquire(sysTemp, MIL.M_DIGITIZER_NUM, ref numOfDigitizer);

                if (numOfDigitizer > 0)
                {
                    for (int j = 0; j < numOfDigitizer; j++)
                    {
                        MIL_ID digitizer = MIL.M_NULL;
                        StringBuilder modelName = new StringBuilder();
                        MIL.MdigAlloc(sysTemp, MIL.M_DEV0 + j, "M_DEFAULT", MIL.M_DEFAULT, ref digitizer);

                        if (digitizer == MIL.M_NULL)
                            break;
                        MIL.MdigInquire(digitizer, MIL.M_CAMERA_MODEL, modelName);
                        if (!string.IsNullOrEmpty(modelName.ToString()))
                            camNodes.ChildTrees[i].AddChild(new CamNode { Key = camNodes.ChildTrees[i][0].Node, Node = modelName.ToString(), DEVn = MIL.M_DEV0 + j});
                        MIL.MdigFree(digitizer);
                    }
                }

                MIL.MsysFree(sysTemp);
            }


            //MIL.MsysAlloc(MIL.M_SYSTEM_SOLIOS, MIL.M_DEFAULT, MIL.M_DEFAULT, ref MilSystem);

            BufGrabDisp = new MIL_ID[20];
            hookFunction = new MIL_DIG_HOOK_FUNCTION_PTR(HookFunction);
            userHookData = new HookDataStruct();

            LiveYLength = 100;
        }

        private MIL_INT HookFunction(MIL_INT HookType, MIL_ID EventId, IntPtr UserDataPtr)
        {
            if (frameCount >= (scanLength % BufSizeY == 0 ? scanLength / BufSizeY : scanLength / BufSizeY + 1))
                return MIL.M_NULL;

            MIL_ID ModifiedBufferId = MIL.M_NULL;

            if (!IntPtr.Zero.Equals(UserDataPtr))
            {
                MIL.MdigGetHookInfo(EventId, MIL.M_MODIFIED_BUFFER + MIL.M_BUFFER_ID, ref ModifiedBufferId);

                if (ImageFormat == FormatMono)
                {
                    MIL.MbufGet(ModifiedBufferId, tempByteArr);
                }
                else
                {
                    MIL.MbufGetColor(ModifiedBufferId, MIL.M_BGR24 + MIL.M_PACKED, MIL.M_ALL_BANDS, tempByteArr);
                }
                System.Buffer.BlockCopy(tempByteArr, 0, byteImage, tempByteArr.Length * frameCount++, tempByteArr.Length);
            }

            return 0;
        }

        public void CameraOpen(CamNode caminfo)
        {
            MIL.MsysAlloc(MilApplication, caminfo.Key, MIL.M_DEFAULT, MIL.M_DEFAULT, ref MilSystem);

            MIL.MdigAlloc(MilSystem, caminfo.DEVn, "M_DEFAULT", MIL.M_DEFAULT, ref MilDigitizer);


            

            InitDigitizer();

        }

        private void InitDigitizer()
        {
            MIL_INT scanMode = MIL.M_NULL;
            MIL.MdigInquire(MilDigitizer, MIL.M_SCAN_MODE, ref scanMode);

            if(scanMode == MIL.M_LINESCAN)
                MIL.MdigControl(MilDigitizer, MIL.M_SOURCE_SIZE_Y, 10);

            MIL.MdigInquire(MilDigitizer, MIL.M_SOURCE_SIZE_X, ref BufSizeX);
            MIL.MdigInquire(MilDigitizer, MIL.M_SOURCE_SIZE_Y, ref BufSizeY);
            MIL.MdigInquire(MilDigitizer, MIL.M_SIZE_BAND, ref BufSizeBand);

            if (BufSizeBand > 1)
                ImageFormat = Enums.ImageFormat.Color;
            else
                ImageFormat = Enums.ImageFormat.Mono;

            Attributes = MIL.M_IMAGE + MIL.M_PROC;

            tempByteArr = new byte[BufSizeX * BufSizeY * BufSizeBand];
            if (MilDigitizer != MIL.M_NULL)
            {
                Attributes |= MIL.M_GRAB;
            }
        }

        public void DCFFileSet(string dcfFileName)
        {
            if (MIL.MsysInquire(MilSystem, MIL.M_DIGITIZER_NUM, MIL.M_NULL) > 0)
            {
                if (MilDigitizer != MIL.M_NULL) MIL.MdigFree(MilDigitizer);
                MIL.MdigAlloc(MilSystem, MIL.M_DEFAULT, dcfFileName, MIL.M_DEFAULT, ref MilDigitizer);
                InitDigitizer();
            }
        }

        public void ToggleLive(bool OnOff)
        {
            if (OnOff)
            {
                threadStopper = true;
                new Thread(() =>
                {
                    while (threadStopper) { Grab(LiveYLength); }
                }).Start();
            }
            else
                threadStopper = false;
        }

        public void Grab(int v)
        {
            MIL_INT sizeY = Convert.ToInt32(v);
            MIL_INT subSizeY = Convert.ToInt32(sizeY % BufSizeY);

            frameCount = 0;
            scanLength = (int)sizeY;


            CreateBuffer(sizeY, subSizeY);

            GCHandle hUserData = GCHandle.Alloc(userHookData);

            MIL.MdigProcess(MilDigitizer, BufGrabDisp, BufGrabDisp.Count() - 2, MIL.M_START, MIL.M_DEFAULT, hookFunction, GCHandle.ToIntPtr(hUserData));

            while (frameCount < (subSizeY == 0 ? v / BufSizeY : v / BufSizeY + 1)) { };

            MIL.MdigProcess(MilDigitizer, BufGrabDisp, BufGrabDisp.Count() - 2, MIL.M_STOP, MIL.M_DEFAULT, hookFunction, GCHandle.ToIntPtr(hUserData));

            hUserData.Free();

            BitmapSource currentBitmapSource = CreateBitmapSource(sizeY);

            ClearBuffer();

            GC.Collect();

            OnGrab?.Invoke(this, currentBitmapSource);
        }

        private BitmapSource CreateBitmapSource(MIL_INT sizeY)
        {
            BitmapSource bmpSource;
            PixelFormat pxFormat;
            int bufSize;


            if (ImageFormat == FormatMono)
            {
                pxFormat = PixelFormats.Gray8;
                bufSize = (int)BufSizeX;
            }
            else
            {
                pxFormat = PixelFormats.Bgr24;
                bufSize = (int)(BufSizeX * 3);
            }

            bmpSource = BitmapSource.Create((int)BufSizeX, (int)sizeY, 1, 1, pxFormat, null, byteImage, bufSize);

            return bmpSource;
        }

        private void ClearBuffer()
        {
            if (BufGrabDisp != null)
            {
                if (BufGrabDisp.Count() != MIL.M_NULL)
                    foreach (var item in BufGrabDisp)
                    {
                        MIL.MbufFree(item);
                    }
            }
            if (BufResultDisp != MIL.M_NULL) MIL.MbufFree(BufResultDisp);
            if (BufGrabSubDisp != MIL.M_NULL) MIL.MbufFree(BufGrabSubDisp);
        }

        private void CreateBuffer(MIL_INT sizeY, MIL_INT subSizeY)
        {
            for (int i = 0; i < BufGrabDisp.Count(); i++)
            {
                MIL.MbufAllocColor(MilSystem, BufSizeBand, BufSizeX, BufSizeY, 8 + MIL.M_UNSIGNED, Attributes, ref BufGrabDisp[i]);
            }

            int attribute;
            if (ImageFormat == FormatMono)
            {
                byteImage = new byte[BufSizeX * sizeY];
                attribute = Attributes;
            }
            else
            {
                byteImage = new byte[BufSizeX * sizeY * 3];
                attribute = MIL.M_IMAGE + MIL.M_PACKED + MIL.M_BGR24 + MIL.M_PAGED;
            }

            MIL.MbufAllocColor(MilSystem, BufSizeBand, BufSizeX, sizeY, 8 + MIL.M_UNSIGNED, attribute, ref BufResultDisp);

            if (subSizeY > 0)
                MIL.MbufAllocColor(MilSystem, BufSizeBand, BufSizeX, subSizeY, 8 + MIL.M_UNSIGNED, attribute, ref BufGrabSubDisp);
        }

        public void CloseCam()
        {
            MIL.MappFreeDefault(MilApplication, MilSystem, MIL.M_NULL, MilDigitizer, MIL.M_NULL);
        }

    }
}
