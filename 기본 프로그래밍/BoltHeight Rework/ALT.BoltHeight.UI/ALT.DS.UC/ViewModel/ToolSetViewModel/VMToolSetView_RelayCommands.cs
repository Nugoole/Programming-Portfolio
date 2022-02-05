using ALT.DS.UC.Messenger;
using ALT.DSCamera.Camera3D;
using ALT.DSCamera.Tool;
using ALT.DS.UC.View;
using Cognex.VisionPro;
using Cognex.VisionPro.Implementation;
using CommonServiceLocator;

using GalaSoft.MvvmLight.Command;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.ComponentModel;

namespace ALT.DS.UC.ViewModel
{
    public static class Extension
    {
        public static void MessageBox(this GalaSoft.MvvmLight.ViewModelBase vm, string message)
        {
            System.Windows.MessageBox.Show(message);
        }
    }
    public partial class VMToolSetView : GalaSoft.MvvmLight.ViewModelBase
    {
        public RelayCommand OnHeightToolCircleAdd { get; set; }
        public RelayCommand OnHeightToolRectangleAdd { get; set; }
        public RelayCommand OnTiltCalcToolAdd { get; set; }
        public RelayCommand OnPMRegionSet { get; set; }
        public RelayCommand OnSelectedGraphicRemoved { get; set; }
        public RelayCommand OnFixtureParamChanged { get; private set; }
        public RelayCommand OnRegionVisibleChanged { get; private set; }
        public RelayCommand OnSetHeightBasePlane { get; set; }
        public RelayCommand OnPMTrain { get; set; }
        public RelayCommand OnAcquisitionStart { get; set; }
        public RelayCommand OnRegionSave { get; set; }
        public RelayCommand<UIElement> OnToolSetViewLoaded { get; set; }
        public RelayCommand<object> OnDataGridItemDoubleClicked { get; set; }
        public RelayCommand<ICogImage> On3DViewClicked { get; set; }
        public RelayCommand OnUnTrain { get; set; }
        public RelayCommand<UIElement> OnSaveImageChecked { get; set; }
        public RelayCommand<bool> OnOnlineBtnClicked { get; set; }
        public RelayCommand OnImageSaving { get; set; }
        public RelayCommand OnCamSetBtnClicked { get; set; }
        public RelayCommand OnColorMapEnabled { get; set; }
        public RelayCommand OnColorMapDisabled { get; set; }
        public RelayCommand OnRemoveAllRegionClicked { get; set; }
        public RelayCommand OnPMROISet { get; set; }
        public RelayCommand OnCrossSectionRegionAddClicked { get; set; }
        public RelayCommand RunOnce { get; set; }
        public bool SaveImage { get; set; }

        private void InitializeRelayCommands()
        {
            OnDataGridItemDoubleClicked = new RelayCommand<object>(OnDataGridItemDoubleClickedAction);
            OnCrossSectionRegionAddClicked = new RelayCommand(OnCrossSectionRegionAddClickedAction);
            OnHeightToolCircleAdd = new RelayCommand(OnHeightToolCircleAddAction);
            OnHeightToolRectangleAdd = new RelayCommand(OnHeightToolRectangleAddAction);
            OnSelectedGraphicRemoved = new RelayCommand(OnSelectedGraphicRemoveAction);
            OnFixtureParamChanged = new RelayCommand(OnFixtureParamChangedAction);
            OnSetHeightBasePlane = new RelayCommand(OnSetHeightBasePlaneAction);
            OnRegionVisibleChanged = new RelayCommand(OnRegionVisibleChangedAction);
            OnAcquisitionStart = new RelayCommand(OnAcquisitionStartAction);
            OnToolSetViewLoaded = new RelayCommand<UIElement>(OnToolSetViewLoadedAction);
            OnTiltCalcToolAdd = new RelayCommand(OnTiltCalcToolAddAction);
            OnPMRegionSet = new RelayCommand(OnPMRegionSetAction);
            OnUnTrain = new RelayCommand(OnUnTrainAction);
            On3DViewClicked = new RelayCommand<ICogImage>(On3DViewClickedAction);
            OnSaveImageChecked = new RelayCommand<UIElement>(OnSaveImageCheckedAction);
            OnImageSaving = new RelayCommand(OnImageSavingAction);
            OnCamSetBtnClicked = new RelayCommand(OnCamSetBtnClickedAction);
            OnColorMapEnabled = new RelayCommand(OnColorMapEnableCheckChangedAction);
            OnRemoveAllRegionClicked = new RelayCommand(OnRemoveAllRegionClickedAction);
            OnColorMapDisabled = new RelayCommand(OnColorMapDisabledAction);
            OnPMROISet = new RelayCommand(OnPMROISetAction);
            OnOnlineBtnClicked = new RelayCommand<bool>(OnOnlineBtnClickedAction);



            OnPMTrain = new RelayCommand(OnPMTrainAction);
            RunOnce = new RelayCommand(RunOnceAction);
        }

        private void OnOnlineBtnClickedAction(bool isChecked)
        {
            Md3DFrameGrabber.Instance.Toggle2D3DCamera(!isChecked);


            Md3DFrameGrabber.Instance.IsOnline = isChecked;


            if (isChecked)
                Md3DFrameGrabber.Instance.StartAcquire();
        }

        private void OnCrossSectionRegionAddClickedAction()
        {
            if (Md3DFrameGrabber.Instance.Image != null)
            {
                regionType = RegionType.Rectangle;
                Window window = new RegionNameSetWindow();
                ServiceLocator.Current.GetInstance<VMRegionNameSet>().RegionType = RegionSetType.CrossSection;

                window.ShowDialog();
            }
        }

        private void RunOnceAction()
        {
            ICogImage nowImage;

            if (DisplayRecord is null)
                nowImage = DisplayImage;
            else
                nowImage = DisplayRecord.Content as ICogImage;

            MdPatMax.Instance.InputImage = nowImage;
            MdPatMax.Instance.viewOnly = false;
            MdPatMax.Instance.Run();
        }

        private void OnColorMapDisabledAction()
        {
            ColorMapEnable = false;
        }

        private void OnPMROISetAction()
        {
            if (DisplayRecord != null)
            {
                var displayRecord = DisplayRecord;

                DisplayImage.SelectedSpaceName = SpaceName_Pixel;
                displayRecord.Content = DisplayImage as ICogImage;
                DisplayRecord = displayRecord;
                RaisePropertyChanged(nameof(DisplayRecord));

            }
            else
            {
                DisplayImage = OrgImage;
            }

            MdHeightCalcTool.Instance.HeightCalcRegions.ForEach(x => (x as ICogGraphicInteractive).Visible = false);
            MdTiltCalcTool.Instance.TiltCalcRegions.ForEach(x => x.ForEach(y => (y as ICogGraphicInteractive).Visible = false));


            if (MdPatMax.Instance.SearchRegion is null)
                MdPatMax.Instance.SearchRegion = new CogRectangle();

            var ROI = MdPatMax.Instance.SearchRegion as ICogGraphicInteractive;

            ROI.SelectedSpaceName = SpaceName_Pixel;

            AllRegions.Clear();


            if (!ROI.Visible)
            {
                ROI.Interactive = true;
                ROI.GraphicDOFEnableBase = CogGraphicDOFConstants.All;
                ROI.Visible = true;
                AllRegions.Add(ROI);
            }
            else
            {
                ROI.Visible = false;
            }
        }

        private void OnRemoveAllRegionClickedAction()
        {
            MdHeightCalcTool.Instance.HeightCalcRegions.Clear();
            MdTiltCalcTool.Instance.TiltCalcRegions.Clear();
            AllRegions.Clear();
        }

        private void OnColorMapEnableCheckChangedAction()
        {
            ColorMapEnable = true;
        }

        private void OnCamSetBtnClickedAction()
        {
            Window camsetWindow = new CameraViewWindow();

            camsetWindow.ShowDialog();
        }

        internal void OnImageSavingAction()
        {
            if (SaveImageEnable)
            {
                var savePath = (string.IsNullOrEmpty(ImageFolderPath) ? Application.Current.StartupUri.LocalPath : ImageFolderPath);

                if (!Directory.Exists($@"{savePath}\Images"))
                    Directory.CreateDirectory($@"{savePath}\Images");

                SaveImageEvent(OverlayGraphic, $@"{savePath}\Images\{DateTime.Now.ToString(ImageDateStringFormat)}.{ImageSaveFileFormat}");
            }
        }

        private void OnSaveImageCheckedAction(UIElement obj)
        {

        }

        private void On3DViewClickedAction(ICogImage image)
        {


        }

        private void OnDataGridItemDoubleClickedAction(object obj)
        {
            string Name = obj.GetType().GetProperty("Name").GetValue(obj).ToString();

            if (HeightResultProfileRecord.Keys.Contains(Name))
            {
                DisplayRecord = HeightResultProfileRecord[Name];
                RaisePropertyChanged(nameof(DisplayRecord));
            }
        }

        private void OnHeightToolCircleAddAction()
        {
            if (Md3DFrameGrabber.Instance.Image != null)
            {
                regionType = RegionType.Circle;
                Window window = new RegionNameSetWindow();
                ServiceLocator.Current.GetInstance<VMRegionNameSet>().RegionType = RegionSetType.Height;

                window.ShowDialog();
            }
        }

        private void OnHeightToolRectangleAddAction()
        {
            if (Md3DFrameGrabber.Instance.Image != null)
            {
                regionType = RegionType.Rectangle;
                Window window = new RegionNameSetWindow();
                ServiceLocator.Current.GetInstance<VMRegionNameSet>().RegionType = RegionSetType.Height;

                window.ShowDialog();
            }
        }

        private void OnSelectedGraphicRemoveAction()
        {
            if (SelectedGraphic.TipText.Contains(BoltToolHeaderName))
            {
                MdHeightCalcTool.Instance.HeightCalcRegions.Remove(SelectedGraphic as ICogRegion);
                AllRegions.Remove(SelectedGraphic);
            }
            else if (SelectedGraphic.TipText.Equals(BasePlaneRegionName))
            {
                MdHeightCalcTool.Instance.BasePlaneRegion = null;
                AllRegions.Remove(SelectedGraphic);
            }
            else
            {
                var stringSeg = SelectedGraphic.TipText.Split(StringSplitter).First();
                var toRemove = new List<ICogRegion>();
                foreach (var x in MdTiltCalcTool.Instance.TiltCalcRegions)
                {
                    var found = false;
                    foreach (var y in x)
                    {
                        var region = y as ICogGraphicInteractive;

                        if (region.TipText.Split(StringSplitter).First().Equals(stringSeg))
                        {
                            found = true;
                            AllRegions.Remove(region);
                            toRemove = x;
                        }
                    }

                    if (found)
                    {
                        MdTiltCalcTool.Instance.TiltCalcRegions.Remove(x);
                        break;
                    }
                }
            }
        }


        private void OnFixtureParamChangedAction()
        {
            MdPatMax.Instance.OnFixtureRecordConditionChanged();
        }

        private void OnSetHeightBasePlaneAction()
        {
            if (MdHeightCalcTool.Instance.BasePlaneRegion is null)
            {

                MdHeightCalcTool.Instance.BasePlaneRegion = new CogRectangleAffine() { Interactive = true, GraphicDOFEnable = CogRectangleAffineDOFConstants.All };
                (MdHeightCalcTool.Instance.BasePlaneRegion as ICogGraphicInteractive).TipText = BasePlaneRegionName;
                MdHeightCalcTool.Instance.BasePlaneRegion.FitToImage(DisplayImage, 0.5, 0.5);
            }

            if (!AllRegions.Contains(MdHeightCalcTool.Instance.BasePlaneRegion as ICogGraphicInteractive))
                AllRegions.Add(MdHeightCalcTool.Instance.BasePlaneRegion as ICogGraphicInteractive);
        }

        private void OnRegionVisibleChangedAction()
        {
            if (AllRegions.Count > 0)
            {
                AllRegions.Clear();
            }
            else
            {
                MdHeightCalcTool.Instance.HeightCalcRegions.ForEach(x => (x as ICogGraphicInteractive).Visible = true);
                MdTiltCalcTool.Instance.TiltCalcRegions.ForEach(x => x.ForEach(y => (y as ICogGraphicInteractive).Visible = true));

                AllRegions.Clear();

                MdHeightCalcTool.Instance.HeightCalcRegions.ForEach(x =>
                {
                    (x as ICogGraphicInteractive).Interactive = true;
                    (x as ICogGraphicInteractive).GraphicDOFEnableBase = CogGraphicDOFConstants.All;
                    AllRegions.Add(x as ICogGraphicInteractive);
                }
                );
                MdTiltCalcTool.Instance.TiltCalcRegions.ForEach(x => x.ForEach(y =>
                {
                    (y as ICogGraphicInteractive).Interactive = true;
                    (y as ICogGraphicInteractive).GraphicDOFEnableBase = CogGraphicDOFConstants.All;
                    AllRegions.Add(y as ICogGraphicInteractive);
                }));

                foreach (var item in MdCrossSectionTool.Instance.CrossSectionRegions)
                {
                    (item as ICogGraphicInteractive).Visible = true;
                    (item as ICogGraphicInteractive).Interactive = true;
                    (item as ICogGraphicInteractive).GraphicDOFEnableBase = CogGraphicDOFConstants.All;
                    AllRegions.Add(item as ICogGraphicInteractive);
                }
            }
        }

        private void OnAcquisitionStartAction()
        {
            MdPatMax.Instance.viewOnly = false;
            Md3DFrameGrabber.Instance.Toggle2D3DCamera(false);

            Md3DFrameGrabber.Instance.StartAcquire();
        }

        private void OnToolSetViewLoadedAction(UIElement cogdisplay)
        {

        }

        private void OnTiltCalcToolAddAction()
        {
            if (Md3DFrameGrabber.Instance.Image != null)
            {
                regionType = RegionType.Rectangle;
                Window window = new RegionNameSetWindow();
                ServiceLocator.Current.GetInstance<VMRegionNameSet>().RegionType = RegionSetType.Tilt;

                window.ShowDialog();
            }
        }

        private void OnPMRegionSetAction()
        {
            if (DisplayRecord != null)
            {
                DisplayRecord.Content = OrgImage;
                RaisePropertyChanged(nameof(DisplayRecord));
            }
            else
            {
                DisplayImage = OrgImage;
            }

            MdHeightCalcTool.Instance.HeightCalcRegions.ForEach(x => (x as ICogGraphicInteractive).Visible = false);
            MdTiltCalcTool.Instance.TiltCalcRegions.ForEach(x => x.ForEach(y => (y as ICogGraphicInteractive).Visible = false));

            AllRegions.Clear();

            MdPatMax.Instance.RegionGraphics.ToList().ForEach(x =>
            {
                if (!x.Visible)
                {
                    x.Visible = true;
                    (x as ICogShape)?.FitToImage(DisplayImage, 0.5, 0.5);
                    AllRegions.Add(x);
                }
                else
                    x.Visible = false;
            });
        }

        private void OnUnTrainAction()
        {
            MdPatMax.Instance.UnTrain();
            RaisePropertyChanged(nameof(Trained));
        }

        private void OnPMTrainAction()
        {
            try
            {
                if (!MdPatMax.Instance.Trained)
                    MdPatMax.Instance.Train(DisplayImage);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            RaisePropertyChanged(nameof(Trained));
        }

        private void OnToolRunAction()
        {
            //Initialize variables
            Dictionary<string, double> heightResults = new Dictionary<string, double>();
            Dictionary<string, double> tiltResults = new Dictionary<string, double>();
            HeightResultProfileRecord = new Dictionary<string, ICogRecord>();


            CogGraphicCollection heightToolGraphics = new CogGraphicCollection();


            //HeightCalcTool Run & Collect Results
            MdHeightCalcTool.Instance.HeightCalcRegions.ForEach((region) =>
            {
                ICogRecords records = null;
                try
                {
                    if (MdHeightCalcTool.Instance.BasePlaneRegion is null)
                    {
                        MdHeightCalcTool.Instance.InputImage = DisplayImage;
                        MdHeightCalcTool.Instance.BasePlaneHeight = 0;
                        MdHeightCalcTool.Instance.Region = region;
                        MdHeightCalcTool.Instance.Run();
                    }
                    else
                    {
                        MdHeightCalcTool.Instance.InputImage = DisplayImage;
                        MdHeightCalcTool.Instance.Region = region;
                        MdHeightCalcTool.Instance.Run();
                    }

                    foreach (ICogGraphic graphic in MdHeightCalcTool.Instance.GetGraphicFromRecord(MdHeightCalcTool.Instance.LastRunRecords[0]))
                    {
                        graphic.Visible = true;
                        heightToolGraphics.Add(graphic);
                    }

                    HeightResultProfileRecord.Add((region as ICogGraphicInteractive).TipText, MdHeightCalcTool.Instance.LastRunRecords[1]);
                    heightResults.Add((region as ICogGraphicInteractive).TipText, MdHeightCalcTool.Instance.Result.Height);
                }
                catch (Exception ex)
                {
#if DEBUG
                    var messenger = new ExceptionMessenger() { Exception = ex };
                    Messenger.Send(messenger + $" {(region as ICogGraphicInteractive).TipText}");
#endif
                }
            });


            ResultGraphicCollection.Add(HeightResultGraphicCollectionKey, heightToolGraphics);

            //TiltCalcTool Run & Collect Results
            MdTiltCalcTool.Instance.TiltCalcRegions.ForEach((region) =>
            {
                MdTiltCalcTool.Instance.InputImage = DisplayImage;
                MdTiltCalcTool.Instance.BasePlaneRegion = region[0];
                MdTiltCalcTool.Instance.TargetPlaneRegion = region[1];
                try
                {
                    MdTiltCalcTool.Instance.Run();

                    tiltResults.Add((region[0] as ICogGraphicInteractive).TipText.Split(StringSplitter).First(), MdTiltCalcTool.Instance.Result.Tilt);
                }
                catch (Exception)
                {

                }

            });

            foreach (ICogGraphicInteractive region in MdCrossSectionTool.Instance.CrossSectionRegions)
            {
                MdCrossSectionTool.Instance.Run(region.TipText);

                var resultGraphics = MdCrossSectionTool.Instance.LastRunRecords[0].SubRecords[0].Content as CogGraphicCollection;
                DisplayRecord.SubRecords.Add(new CogRecord("CrossSectionGraphics", typeof(CogGraphicCollection), CogRecordUsageConstants.Diagnostic,
                    true, resultGraphics, ""));
            }


            var FinalResult_Height = from x in heightResults
                                     select new
                                     {
                                         Name = x.Key,
                                         Height = x.Value.ToString("F4"),
                                         Tilt = string.Empty
                                     };

            var FinalResult_Regions = from x in tiltResults
                                      select new
                                      {
                                          Name = x.Key,
                                          Height = string.Empty,
                                          Tilt = x.Value.ToString("F4")
                                      };

            var FinalResult = FinalResult_Height.Union(FinalResult_Regions);

            ToolResultSet = FinalResult;
            RaisePropertyChanged(nameof(ToolResultSet));

            GC.Collect();
        }
    }
}
