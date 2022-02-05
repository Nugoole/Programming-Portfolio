using ALT.CVL.Core;

using Microsoft.Win32;

using Sick.GenIStream;

using SICKCalibCLR;


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ALT.CVL.SICK
{
    public enum CameraScanMode
    {
        LINESCAN_3D,
        AREASCAN
    }
    public enum Regions
    {
        Region0,
        Region1,
        Scan3dExtraction1
    }

    public enum UsersetID
    {
        DEFAULT = 0,
        USER_SET_1 = 1,
        USER_SET_2 = 2,
        USER_SET_3 = 3,
        USER_SET_4 = 4,
        USER_SET_5 = 5
    }
    public class Param
    {
        public string ParamName { get; set; }
        public string Value { get; set; }

    }

    public class SICKGrabber : INotifyPropertyChanged
    {
        [DllImport("kernel32.dll", EntryPoint = "CopyMemory", SetLastError = false)]
        public static extern void CopyMemory(IntPtr dest, IntPtr src, uint size);

        private CameraDiscovery CameraScanner;
        private IFrame currentImage = null;
        private FrameGrabber fg;
        private bool isConnected;

        public string CamName => Camera?.GetDevice().GetDisplayName();
        public bool IsConnected
        {
            get
            {
                if (Camera != null)
                    return Camera.IsConnected;

                return false;
            }
        }
        public Tree<Sick.GenIStream.Parameter> ParameterTree { get; set; }
        public ICamera Camera { get; private set; }
        public Regions Region
        {
            get
            {
                if (Camera != null && Camera.IsConnected)
                {
                    var paramValue = Camera.GetAnyParameters().GetEnum("RegionSelector");

                    if (paramValue.Equals(Regions.Region0.ToString()))
                        return Regions.Region0;
                    else if (paramValue.Equals(Regions.Region1.ToString()))
                        return Regions.Region1;
                    else if (paramValue.Equals(Regions.Scan3dExtraction1.ToString()))
                        return Regions.Scan3dExtraction1;
                }

                return 0;
            }

            set
            {
                if (Camera != null && Camera.IsConnected)
                {
                    Camera.GetAnyParameters().SetEnum("RegionSelector", value.ToString());

                    RaisePropertyChanged();
                }
            }
        }
        public CameraScanMode Scan3DMode
        {
            get
            {
                if (Camera != null && Camera.IsConnected)
                {
                    switch (Camera.GetCameraParameters().DeviceScanType.Get())
                    {
                        case DeviceScanType.AREASCAN:
                            return CameraScanMode.AREASCAN;
                        case DeviceScanType.LINESCAN_3D:
                            return CameraScanMode.LINESCAN_3D;
                    }
                }

                return 0;
            }

            set
            {
                if (Camera != null && Camera.IsConnected)
                {
                    switch (value)
                    {
                        case CameraScanMode.LINESCAN_3D:
                            Camera.GetCameraParameters().DeviceScanType.Set(DeviceScanType.LINESCAN_3D);
                            break;
                        case CameraScanMode.AREASCAN:
                            Camera.GetCameraParameters().DeviceScanType.Set(DeviceScanType.AREASCAN);
                            break;
                    }

                    RaisePropertyChanged();
                }
            }
        }
        public uint ScanLength
        {
            get
            {
                if (Camera != null && Camera.IsConnected)
                {
                    return Camera.GetCameraParameters().Region(RegionId.SCAN_3D_EXTRACTION_1).Height.Get();
                }

                return 0;
            }
            set
            {
                if (Camera != null && Camera.IsConnected)
                {
                    Camera.GetCameraParameters().Region(RegionId.SCAN_3D_EXTRACTION_1).Height.Set(value);
                    RaisePropertyChanged();
                }
            }
        }
        public float ExposureTime
        {
            get
            {
                if (Camera != null && Camera.IsConnected)
                {
                    return Camera.GetCameraParameters().Region(RegionId.REGION_1).ExposureTime.Get();
                }

                return -1;
            }
            set
            {
                if (Camera != null && Camera.IsConnected)
                {
                    Camera.GetCameraParameters().Region(RegionId.REGION_1).ExposureTime.Set(value);
                    RaisePropertyChanged();
                }
            }
        }
        public List<string> AvailableCameras => CameraScanner.ScanForCameras().Select(x => $@"{x.Model} : {x.SerialNumber}").ToList();
        public event EventHandler<IFrame> OnImageGrabbed;
        public event EventHandler<Exception> OnErrorOccured;
        public event PropertyChangedEventHandler PropertyChanged;

        public SICKGrabber()
        {
            try
            {
                CameraScanner = CameraDiscovery.CreateFromProducerFile("SICKGigEVisionTL.cti");
            }
            catch (Exception e)
            {
                if (e is System.IO.FileNotFoundException)
                    throw new System.IO.FileNotFoundException("SICKGigEVisionTL.cti 파일이 없습니다.");
                else
                    throw e;
            }
        }




        public bool Connect(string stringFromCameras)
        {
            var model = stringFromCameras.Split(':').First().Trim();
            var serialNumber = stringFromCameras.Split(':').Last().Trim();

            var cameraToConnect = CameraScanner.ScanForCameras().Where(x => x.Model.Equals(model) && x.SerialNumber.Equals(serialNumber)).FirstOrDefault();

            if (cameraToConnect != null)
            {
                Camera = CameraScanner.ConnectTo(cameraToConnect);

                InitCamera();
            }

            RaisePropertyChanged(nameof(IsConnected));
            RaisePropertyChanged(nameof(CamName));
            return Camera != null;
        }

        public void Disconnect()
        {
            if (Camera != null && Camera.IsConnected)
            {
                Camera.Disconnect();
                Camera.Dispose();
                Camera = null;

                RaisePropertyChanged(nameof(IsConnected));
                RaisePropertyChanged(nameof(CamName));
            }
        }

        public void SaveImage(string filePath)
        {
            if (currentImage != null)
            {
                currentImage.Save(filePath, IFrame.SaveCalibratedMode.RANGE_AS_FLOAT);
            }
        }

        private void InitCamera()
        {
            Camera.GetCameraParameters().DeviceScanType.Set(DeviceScanType.LINESCAN_3D);
            fg = Camera.CreateFrameGrabber();
            Camera.Disconnected += Camera_Disconnected;


            ParameterTree = new Tree<Parameter>();

        }

        private void Camera_Disconnected(string deviceId)
        {
            RaisePropertyChanged(nameof(IsConnected));
        }

        public void LoadParameter(string filePath, UsersetID id)
        {
            Camera.ImportParameters(filePath);
            var selectedUserset = (UserSetId)Enum.Parse(typeof(UserSetId), id.ToString());

            var userSet = Camera.OpenUserSet(selectedUserset);
            userSet.Save();
        }

        public void SaveParameter(string filePath)
        {
            Camera.ExportParameters(filePath);
        }

        public IFrame GrabOne(TimeSpan? timeout = null)
        {
            IFrame frametoReturn = default;

            if (Camera == null)
                return null;

            if (timeout == null)
                timeout = TimeSpan.FromMilliseconds(3000);

            using (FrameGrabber fg = Camera.CreateFrameGrabber())
            {
                if (fg.IsStarted)
                    return null;

                fg.Start();
                fg.Grab(timeout.Value)
                    .IfCompleted((frame) =>
                    {
                        frametoReturn = frame;
                        OnFrameReceived(frame);
                    })
                    .IfAborted(() => OnErrorOccured?.Invoke(this, new Exception("Grab작업이 취소되었습니다.")))
                    .IfTimedOut(() => OnErrorOccured?.Invoke(this, new TimeoutException("Grab작업이 시간 초과 되었습니다.")));

                fg.Stop();
            }


            return frametoReturn;
        }

        public void ContinueStart()
        {
            if (fg != null)
            {
                fg.Dispose();
                fg = null;
            }

            fg = Camera.CreateFrameGrabber();

            fg.Start();
            fg.FrameReceived += Fg_FrameReceived;
        }

        private void Fg_FrameReceived(IFrame frame)
        {
            OnFrameReceived(frame);
        }

        private void OnFrameReceived(IFrame frame)
        {
            if (frame.IsComplete())
            {
                currentImage = frame;

                OnImageGrabbed?.Invoke(this, frame);

            }
        }

        public void ContinueStop()
        {
            if (fg.IsStarted)
            {
                fg.FrameReceived -= Fg_FrameReceived;
                fg?.Stop();
            }
        }

        public WriteableBitmap GetWriteableBitmapFromIFrame(IFrame frame, CameraScanMode mode)
        {
            using (var grabbedImage = mode == CameraScanMode.AREASCAN ? frame.GetIntensity() : frame.GetRange())
            {
                var image = grabbedImage.CreateCopiedImage();

                byte[] pixelData = new byte[image.GetDataSize()];
                var width = image.GetWidth();
                var height = image.GetHeight();
                Marshal.Copy(image.GetData(), pixelData, 0, pixelData.Length);

                WriteableBitmap bitmapImage = new WriteableBitmap(width, height, 96, 96, Scan3DMode == CameraScanMode.AREASCAN ? PixelFormats.Gray8 : PixelFormats.Gray16, null);

                //Write image into bitmapImage
                bitmapImage.Lock();

                CopyMemory(bitmapImage.BackBuffer, image.GetData(), image.GetDataSize());

                bitmapImage.AddDirtyRect(new System.Windows.Int32Rect(0, 0, width, height));
                bitmapImage.Unlock();



                return bitmapImage;
            }
        }


        public UsersetID GetDefaultUserset()
        {
            foreach (UserSetId id in Enum.GetValues(typeof(UserSetId)))
            {
                if (Camera.OpenUserSet(id).IsUsedAtStartup())
                    return (UsersetID)Enum.Parse(typeof(UsersetID), id.ToString());
            }

            return 0;
        }

        public void SetDefaultUserSet(UsersetID id)
        {
            UserSetId internalId = (UserSetId)Enum.Parse(typeof(UserSetId), id.ToString());
            Camera.OpenUserSet(internalId).UseAtStartup();
        }


        public Tree<Param> GetCurrentParameterTree(UsersetID id)
        {
            Tree<Param> paramTree = new Tree<Param>();
            Node<Param> CurrentNode = null;
            UserSetId internalId = (UserSetId)Enum.Parse(typeof(UserSetId), id.ToString());
            string[] stringLines = null;
            var userSet = Camera.OpenUserSet(internalId);
            if (!userSet.Exists())
            {
                userSet.Save();
            }

            stringLines = userSet.RetrieveFromCamera().Trim().Split('\n');

            CurrentNode = paramTree.MainNode;

            foreach (var item in stringLines)
            {
                if (!item.Equals("#Root") && CurrentNode == null)
                    continue;

                if (item.StartsWith("#"))
                {
                    if (CurrentNode.ParentNode != null)
                        CurrentNode = CurrentNode.ParentNode;

                    CurrentNode.AddChild(new Param { ParamName = item.TrimStart(new char[] { '#' }), Value = null });
                    CurrentNode = CurrentNode.ChildNodes.Last();
                }
                else
                {
                    var splitString = item.Split(',');
                    if (splitString.Count() < 2)
                        continue;


                    var newParam = new Param { ParamName = splitString[0], Value = splitString[1] };
                    CurrentNode.AddChild(newParam);
                }

            }

            return paramTree;
        }


        public Image LoadImage(string filePath)
        {
            if (filePath.EndsWith(".xml") || filePath.EndsWith(".dat"))
            {
                var loadedFrame = IFrame.Load(filePath);
                currentImage = loadedFrame;
                loadedFrame.Release();
                return loadedFrame.GetRange().CreateCopiedImage();
            }

            return null;
        }

        public Image Load2DImage(string filePath)
        {
            if (filePath.EndsWith(".xml") || filePath.EndsWith(".dat"))
            {
                var loadedFrame = IFrame.Load(filePath);
                currentImage = loadedFrame;
                loadedFrame.Release();
                return loadedFrame.GetIntensity().CreateCopiedImage();
            }

            return null;
        }

        private void RaisePropertyChanged([CallerMemberName] string propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        public void Calibrate(string filePath = null, IFrame frame = null)
        {
            string inputFilePath = filePath;

            if (string.IsNullOrEmpty(filePath) || !System.IO.File.Exists(filePath))
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Title = "Open Calibration File";

                var dialogResult = dialog.ShowDialog();
                if (dialogResult.HasValue && dialogResult.Value)
                {
                    inputFilePath = dialog.FileName;
                }

                
            }


            SICK_Calibrator.LoadCalibrationFile(inputFilePath, Camera);


            IFrame inputFrame = frame == null ? currentImage : frame;

            if (inputFrame != null)
                SICK_Calibrator.ApplyCalibrationToImage(inputFrame);

        }
    }
}
