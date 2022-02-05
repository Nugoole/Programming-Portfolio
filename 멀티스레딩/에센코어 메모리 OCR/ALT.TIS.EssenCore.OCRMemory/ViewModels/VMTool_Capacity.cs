using Cognex.VisionPro.OCRMax;

using GalaSoft.MvvmLight.Command;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ALT.TIS.EssenCore.OCRMemory.ViewModels
{
    public partial class VMTool
    {
        private string capacityCharacters;
        private IList<CogOCRMaxSegmenterPositionResult> currentOCRs;
        private string capacityResult;

        public RelayCommand CmdTrainPattern { get; set; }
        public RelayCommand CmdCapacityRegion { get; set; }
        public RelayCommand CmdPatternRegion { get; set; }
        public RelayCommand CmdRunOCR { get; set; }
        public RelayCommand CmdPatternTrainRegion { get; set; }
        public string CapacityResult { get => capacityResult; set => Set(ref capacityResult, value); }
        public string CapacityCharacters
        {
            get => capacityCharacters; set
            {
                capacityCharacters = value;
                if (currentOCRs != null)
                {
                    if (currentOCRs.Count <= value.Length)
                        Display.Image = null;
                    else
                        Display.Image = currentOCRs[value.Length].Character.Image;
                }

                RaisePropertyChanged();
            }
        }
        private void InitializeCapacityTool()
        {
            CmdCapacityRegion = new RelayCommand(CapacityRegion);
            CmdPatternRegion = new RelayCommand(PatternRegion);
            CmdRunOCR = new RelayCommand(RunOCR);
            CmdTrainPattern = new RelayCommand(TrainPattern);
            CmdPatternTrainRegion = new RelayCommand(PatternTrainRegion);

            tool.ToolRan += Tool_ToolRan1;
        }

        private void Tool_ToolRan1(TIS_OCRVisionTool.VisionTool tool)
        {
            CapacityResult = tool.CapacityOCRTool.OCRResult;
        }

        private void PatternTrainRegion()
        {
            SetImageAndAddRegion(tool.CapacityOCRTool.TrainRegion, !tool.CapacityOCRTool.PatternTrained);
        }

        private void TrainPattern()
        {
            tool.CapacityOCRTool.TrainPattern(currentImage);
        }

        private void RunOCR()
        {
            tool.CapacityOCRTool.Run(Display.Image);
        }

        private void PatternRegion()
        {
            SetImageAndAddRegion(tool.CapacityOCRTool.PatternRegion);
        }

        private void CapacityRegion()
        {
            SetImageAndAddRegion(tool.CapacityOCRTool.OCRRegion);
            CurrentOCRType = TIS_OCRVisionTool.OCRType.Logo_Capacity;
        }

        
    }
}
