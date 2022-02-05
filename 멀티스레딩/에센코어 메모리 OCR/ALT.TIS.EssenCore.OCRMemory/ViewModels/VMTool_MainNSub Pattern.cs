using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALT.TIS.EssenCore.OCRMemory.ViewModels
{
    public partial class VMTool
    {
        public RelayCommand CmdMainTrainRegion { get; set; }
        public RelayCommand CmdTrainMainPattern { get; set; }
        public RelayCommand CmdSubTrainRegion { get; set; }
        public RelayCommand CmdSubSearchRegion { get; set; }
        public RelayCommand CmdTrainSubPattern { get; set; }

        private void InitializeMainNSubPattern()
        {
            CmdMainTrainRegion = new RelayCommand(MainTrainRegion);
            CmdTrainMainPattern = new RelayCommand(TrainMainPattern);
            CmdSubTrainRegion = new RelayCommand(SubTrainRegion);
            CmdSubSearchRegion = new RelayCommand(SubSearchRegion);
            CmdTrainSubPattern = new RelayCommand(TrainSubPattern);
        }

        private void TrainSubPattern()
        {
            tool.TrainSubPattern(currentImage);
        }

        private void SubSearchRegion()
        {
            SetImageAndAddRegion(tool.SubPatternSearchRegion);
        }

        private void SubTrainRegion()
        {
            SetImageAndAddRegion(tool.SubPatternTrainRegion, !tool.SubPatternTrained);
        }

        private void TrainMainPattern()
        {
            tool.TrainMainPattern(currentImage);
        }

        private void MainTrainRegion()
        {
            SetImageAndAddRegion(tool.MainPatternTrainRegion, !tool.MainPatternTrained);
        }
    }
}
