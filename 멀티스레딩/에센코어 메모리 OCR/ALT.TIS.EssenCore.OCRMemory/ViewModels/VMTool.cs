
using ALT.CVL;
using ALT.Serialize;
using ALT.TIS.EssenCore.OCRMemory.Interfaces;
using ALT.TIS.EssenCore.OCRMemory.Messenger;
using ALT.TIS.EssenCore.OCRMemory.Views;

using Cognex.VisionPro;
using Cognex.VisionPro.Dimensioning;
using Cognex.VisionPro.Implementation;
using Cognex.VisionPro.PMAlign;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

using TIS_OCRVisionTool;

namespace ALT.TIS.EssenCore.OCRMemory.ViewModels
{
    public partial class VMTool : ViewModelBase, IVMTool
    {
        private VisionTool tool = new VisionTool();
        private ICogImage currentImage;
        private int cameraIndex;

        public WindowsFormsHost wfDisplay { get; }
        public IEnumerable<PatternTool> PMTools => tool.PatternList;
        private CogRecordDisplay Display => (wfDisplay.Child as UCCrDisplay).Display;

        public RelayCommand CmdRun { get; set; }
        public RelayCommand CmdAddPMTool { get; set; }
        public RelayCommand CmdRemovePMTool { get; set; }
        public RelayCommand CmdPMToolSearchRegion { get; set; }
        public RelayCommand CmdPMToolTrainRegion { get; set; }
        public RelayCommand CmdTrainPMTool { get; set; }
        public RelayCommand CmdRunPMTools { get; set; }
        public RelayCommand CmdSave { get; set; }
        public RelayCommand CmdLoad { get; set; }
        public RelayCommand CmdInitialize { get; set; }
        public CogPMAlignTool SelectedTool { get; set; }
        public IEnumerable Spaces => currentImage?.CoordinateSpaceTree.AllChildNames("@",CogSpaceTreePathnameConstants.Full,false);
        public long TacTime => tool.LastRunElapsedTime;
        public bool FirstFixtureEnable
        {
            get
            {
                return tool.MainFixtureEnable;
            }
            set
            {
                tool.MainFixtureEnable = value;
                RaisePropertyChanged();
            }
        }
        public bool SecondFixtureEnable
        {
            get
            {
                return tool.SubFixtureEnable;
            }
            set
            {
                tool.SubFixtureEnable = value;
                RaisePropertyChanged();
            }
        }

        public VMTool(int camIndex) : this()
        {
            cameraIndex = camIndex;
            wfDisplay = MdConfigData.Getinstance().ConfigParam.OCWfhDisplay[camIndex];

            (Display as Control).DragDrop += CogDisplay_DoDrop;
        }

        public VMTool()
        {
            CmdRun = new RelayCommand(Run);
            CmdAddPMTool = new RelayCommand(AddPMTool);
            CmdRemovePMTool = new RelayCommand(RemovePMTool);
            CmdPMToolSearchRegion = new RelayCommand(PMToolSearchRegion);
            CmdPMToolTrainRegion = new RelayCommand(PMToolTrainRegion);
            CmdTrainPMTool = new RelayCommand(TrainPMTool);
            CmdRunPMTools = new RelayCommand(RunPMTools);
            CmdInitialize = new RelayCommand(Initialize);
            CmdSave = new RelayCommand(Save);
            CmdLoad = new RelayCommand(Load);

            MessengerInstance.Register<ICogImage>(this, OnImageAcquired);
            MessengerInstance.Register<string>(this, "CamName1", OnSaveCommandAction);
            MessengerInstance.Register<ObservableCollection<MdTotalParamInfo>>(this, OnLoadCommandAction);

            InitializeCapacityTool();
            InitializeMemStickInspectTool();
            InitializeMainNSubPattern();
        }

        private void Load()
        {
            ///로드하고 다시 세이브하면 이상이 생김
            
            tool = Serializer.Load(@"C:\Users\MSI\Desktop\hi3.sv") as VisionTool;

            tool.ToolRan += Tool_ToolRan;
            tool.ToolRan += Tool_ToolRan1;
            RaisePropertyChanged(nameof(PMTools));
        }

        
        private void Save()
        {
            //CogCreateLineTool lineTool = new CogCreateLineTool();
            //lineTool.InputImage = currentImage;
            //lineTool.Line.X = 100;
            //lineTool.Line.Y = 100;
            //lineTool.Line.Rotation = CogMisc.DegToRad(40);
            //lineTool.Run();

            //SetImageAndAddRegion(lineTool.GetOutputLine());

            //tool.Save(@"C:\Users\MSI\Desktop\hi.sv");

            Serializer.Save(tool, @"C:\Users\MSI\Desktop\hi3.sv");
        }

        private void Initialize()
        {
            tool.Initialize();
            RaisePropertyChanged(nameof(PMTools));
        }

        private void OnLoadCommandAction(ObservableCollection<MdTotalParamInfo> loadParams)
        {
            Load(loadParams);
        }

        private void OnSaveCommandAction(string path)
        {
            Save(path);
        }

        private void Load(ObservableCollection<MdTotalParamInfo> loadParams)
        {
            tool.LoadTools(loadParams[0].ToolParamInfo.OCToolBase);

            if (currentImage != null)
            {
                TrainMainPattern();
                TrainSubPattern();
                TrainKLEVVPattern();
            }
        }

        private void Save(string saveFilePath)
        {
            MdFileIO.Getinstance().SaveTools(tool.GetToolsToSave(), saveFilePath);
        }

        private void RunPMTools()
        {
            tool.Run_PatternList(currentImage);
        }

        private void TrainPMTool()
        {             
            if (SelectedTool != null && Display.Image != null)
            {
                SelectedTool.Pattern.TrainImage = currentImage;
                SelectedTool.Pattern.Train();
            }
        }

        private void PMToolTrainRegion()
        {
            SetImageAndAddRegion(SelectedTool.Pattern.TrainRegion as ICogGraphicInteractive, !SelectedTool.Pattern.Trained);
            //if (SelectedTool != null && Display.Image != null)
            //{
            //    Display.InteractiveGraphics.Clear();
            //    Display.InteractiveGraphics.Add(SelectedTool.Pattern.TrainRegion as ICogGraphicInteractive, "", false);
            //}
        }

        private void PMToolSearchRegion()
        {
            SetImageAndAddRegion(SelectedTool.SearchRegion as ICogGraphicInteractive);
            //if (SelectedTool != null && Display.Image != null)
            //{
            //    Display.InteractiveGraphics.Clear();
            //    Display.InteractiveGraphics.Add(SelectedTool.SearchRegion as ICogGraphicInteractive, "", false);
            //}
        }

        private void RemovePMTool()
        {
            if (SelectedTool != null)
            {
                tool.RemovePattern(SelectedTool);
            }
        }

        private void AddPMTool()
        {
            tool.AddPattern();
        }

        private void OnImageAcquired(ICogImage msg)
        {
            var image = msg;

            if (msg is CogImage24PlanarColor)
                image = CogImageConvert.GetIntensityImage(msg, 0, 0, msg.Width, msg.Height);
                
            currentImage = image;
            Display.Image = currentImage;
            Run();
        }

        private void Run()
        {
            tool.Run(currentImage);
            RaisePropertyChanged(nameof(TacTime));
            RaisePropertyChanged(nameof(Spaces));
        }

        private void CogDisplay_DoDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            CogRecordDisplay Display = sender as CogRecordDisplay;

            if (Display.Image is CogImage24PlanarColor)
            {
                Display.Image = CogImageConvert.GetIntensityImage(Display.Image, 0, 0, Display.Image.Width, Display.Image.Height);
            }

            currentImage = Display.Image;
        }
    }


    
}
