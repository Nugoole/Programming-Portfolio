using ALT.TIS.DB;
using ALT.TIS.EssenCore.OCRMemory.Messenger;

using Cognex.VisionPro;

using GalaSoft.MvvmLight.CommandWpf;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TIS_OCRVisionTool;

namespace ALT.TIS.EssenCore.OCRMemory.ViewModels
{
    public partial class VMTool
    {
        private List<string> partNo = new List<string>();
        private List<string> capacity = new List<string>();
        private List<string> speed = new List<string>();
        private List<string> barcode = new List<string>();
        private List<bool> stickDirection = new List<bool>();
        private OCRType? currentMemOCRType;
        private string partnoString;
        private string mstickCapacityString;
        private string speedString;
        private string logoCapacityString;

        public RelayCommand CmdKLEVVSearchRegion { get; set; }
        public RelayCommand CmdKLEVVTrainRegion { get; set; }
        public RelayCommand CmdTrainKLEVVPattern { get; set; }
        public RelayCommand CmdPartNoRegion { get; set; }
        public RelayCommand CmdStickCapacityRegion { get; set; }
        public RelayCommand CmdSpeedRegion { get; set; }
        public RelayCommand CmdBarcodeRegion { get; set; }
        public RelayCommand CmdSegmentOCRChars { get; set; }
        public RelayCommand CmdTrainSegmentedOCR { get; set; }
        public RelayCommand CmdStickDirectionTrainRegion { get; set; }
        public RelayCommand CmdStickDirectionSearchRegion { get; set; }
        public RelayCommand CmdTrainStickDirection { get; set; }

        public IEnumerable<string> PartNo => partNo;
        public IEnumerable<string> Capacity => capacity;
        public IEnumerable<string> Speed => speed;
        public IEnumerable<string> Barcode => barcode;
        public IEnumerable<bool> StickDirection => stickDirection;
        public OCRType? CurrentOCRType
        {
            get
            {
                return currentMemOCRType;
            }
            set
            {
                Set(ref currentMemOCRType, value);
            }
        }

        private void InitializeMemStickInspectTool()
        {
            CmdKLEVVTrainRegion = new RelayCommand(KLEVVTrainRegion);
            CmdKLEVVSearchRegion = new RelayCommand(KLEVVSearchRegion);
            CmdTrainKLEVVPattern = new RelayCommand(TrainKLEVVPattern);
            CmdPartNoRegion = new RelayCommand(PartNoRegion);
            CmdStickCapacityRegion = new RelayCommand(StickCapacityRegion);
            CmdSpeedRegion = new RelayCommand(SpeedRegion);
            CmdBarcodeRegion = new RelayCommand(BarcodeRegion);
            CmdSegmentOCRChars = new RelayCommand(SegmentOCRChars);
            CmdTrainSegmentedOCR = new RelayCommand(TrainSegmentedOCR);
            CmdStickDirectionSearchRegion = new RelayCommand(StickDirectionSearchRegion);
            CmdStickDirectionTrainRegion = new RelayCommand(StickDirectionTrainRegion);
            CmdTrainStickDirection = new RelayCommand(TrainStickDirection);

            tool.ToolRan += Tool_ToolRan;
        }

        private void TrainStickDirection()
        {
            tool.MemoryOCRTool.TrainStickPattern(currentImage);
        }

        private void StickDirectionTrainRegion()
        {
            SetImageAndAddRegion(tool.MemoryOCRTool.StickDirectionPatternRegion, !tool.MemoryOCRTool.StickDirectionPatternTrained);
        }

        private void StickDirectionSearchRegion()
        {
            SetImageAndAddRegion(tool.MemoryOCRTool.StickDirectionSearchRegion);
        }

        private void TrainSegmentedOCR()
        {
            tool.TrainOCR(currentOCRs, CapacityCharacters, currentMemOCRType);

            switch (currentMemOCRType.Value)
            {
                case OCRType.MemStick_PartNo:
                    partnoString = CapacityCharacters;
                    break;
                case OCRType.MemStick_Capacity:
                    mstickCapacityString = CapacityCharacters;
                    break;
                case OCRType.MemStick_Speed:
                    speedString = CapacityCharacters;
                    break;
                case OCRType.Logo_Capacity:
                    logoCapacityString = CapacityCharacters;
                    break;
            }

            CapacityCharacters = string.Empty;
            Display.Image = currentImage;
            Display.Fit();
        }

        private void SegmentOCRChars()
        {
            CapacityCharacters = string.Empty;
            currentOCRs = tool.OCRSegmentation(currentImage, currentMemOCRType);
            Display.Image = currentOCRs[0].Character.Image;
            Display.Fit();
        }

        private void Tool_ToolRan(VisionTool tool)
        {
            partNo.Clear();
            partNo.Add(tool.MemoryOCRTool.PartNoResult);
            partNo.Add(tool.MemoryOCRTool2.PartNoResult);
            RaisePropertyChanged(nameof(PartNo));

            bool partnoPassFail = partNo.All(x => x.Equals(partnoString));


            capacity.Clear();
            capacity.Add(tool.MemoryOCRTool.CapacityResult);
            capacity.Add(tool.MemoryOCRTool2.CapacityResult);
            RaisePropertyChanged(nameof(Capacity));

            bool capacityPassFail = capacity.All(x => x.Equals(mstickCapacityString));

            speed.Clear();
            speed.Add(tool.MemoryOCRTool.SpeedResult);
            speed.Add(tool.MemoryOCRTool2.SpeedResult);
            RaisePropertyChanged(nameof(Speed));

            bool speedPassFail = speed.All(x => x.Equals(speedString));


            barcode.Clear();
            barcode.Add(tool.MemoryOCRTool.BarcodeResult);
            barcode.Add(tool.MemoryOCRTool2.BarcodeResult);
            RaisePropertyChanged(nameof(Barcode));

            bool barcodePassFail = barcode.All(x => !string.IsNullOrEmpty(x));

            stickDirection.Clear();
            stickDirection.Add(tool.MemoryOCRTool.StickDirection);
            stickDirection.Add(tool.MemoryOCRTool2.StickDirection);
            RaisePropertyChanged(nameof(StickDirection));


            using (TISResultEntities context = new TISResultEntities())
            {

                TIS_Result result = new TIS_Result
                {
                    PN = partNo[0],
                    Speed = speed[0],
                    Capacity = capacity[0],
                    Barcode = barcode[0]
                };

                DbManager.Insert(context, result);
            }



            bool finalResult = partnoPassFail && capacityPassFail && speedPassFail && barcodePassFail;

            MessengerInstance.Send(new ResultMessenger(cameraIndex, finalResult));

        }

        private void BarcodeRegion()
        {
            SetImageAndAddRegion(tool.MemoryOCRTool.BarcodeRegion);
        }

        private void SpeedRegion()
        {
            SetImageAndAddRegion(tool.MemoryOCRTool.SpeedOCRRegion);
            CurrentOCRType = OCRType.MemStick_Speed;
        }

        private void StickCapacityRegion()
        {
            SetImageAndAddRegion(tool.MemoryOCRTool.CapacityOCRRegion);
            CurrentOCRType = OCRType.MemStick_Capacity;
        }

        private void PartNoRegion()
        {
            SetImageAndAddRegion(tool.MemoryOCRTool.PartNoOCRRegion);
            CurrentOCRType = OCRType.MemStick_PartNo;
        }

        private void TrainKLEVVPattern()
        {
            tool.KLEVVPatternTrain(currentImage);
        }

        private void KLEVVSearchRegion()
        {
            SetImageAndAddRegion(tool.KLEVVPatternSearchRegion);
        }



        private void KLEVVTrainRegion()
        {
            SetImageAndAddRegion(tool.KLEVVPatternTrainRegion, !tool.KLEVVPatternTrained);
        }

        private void SetImageAndAddRegion(ICogGraphicInteractive interactiveGraphic, bool fitToImage = false)
        {
            Display.Image = currentImage;

            if (Display.Image != null)
            {
                Display.InteractiveGraphics.Clear();
                if (fitToImage)
                    (interactiveGraphic as ICogShape).FitToImage(Display.Image, 0.5, 0.5);

                Display.InteractiveGraphics.Add(interactiveGraphic, "", false);
            }

            CurrentOCRType = null;
        }


    }
}
