using ALT.BoltHeight.Messenger;
using ALT.DSCamera.Tool;
using ALT.BoltHeight.Views;
using Cognex.VisionPro;
using Cognex.VisionPro.Implementation;

using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace ALT.BoltHeight.UI.ViewModel
{
    enum RegionType
    {
        Circle = 0,
        Rectangle
    };

    public partial class VMToolSetView : GalaSoft.MvvmLight.ViewModelBase
    {
        private readonly char StringSplitter = '_';

        private readonly string BoltToolHeaderName = "Bolt";
        private readonly string TiltToolHeaderName = "Tilt";
        private readonly string CrossSectionToolHeaderName = "CrossSection";
        private readonly string TiltBaseTailName = "Base";
        private readonly string TiltTargetTailName = "Target";
        private readonly string BasePlaneRegionName = "HeightBasePlane";
        private readonly string SpaceName_Pixel = "#";
        private readonly string ImageDateStringFormat = "hh-mm-ss";
        private readonly string ImageSaveFileFormat = "bmp";
        private readonly string HeightResultGraphicCollectionKey = "height";

        private readonly IMessenger Messenger = GalaSoft.MvvmLight.Messaging.Messenger.Default;
        private readonly string JsonFileName = "Params.json";
        private ICogImage OrgImage;
        private Dictionary<string, ICogRecord> HeightResultProfileRecord;
        private Dictionary<string, CogGraphicCollection> ResultGraphicCollection;
        private RegionType regionType;
        private string ModelFolderPath;
        private string ImageFolderPath;
        private bool colorMapEnable;
        private ICogImage displayImage;

        internal event Action<bool, string> SaveImageEvent;

        public ICogGraphicInteractive SelectedGraphic { get; set; }
        public string Trained
        {
            get
            {
                if (MdPatMax.Instance.Trained)
                    return "Trained";
                else
                    return "UnTrained";

                
            }
        }
        public ICogImage DisplayImage
        {
            get => displayImage; set
            {
                displayImage = value;
                MdCrossSectionTool.Instance.InputImage = value;
                MdHeightCalcTool.Instance.InputImage = value;
                MdTiltCalcTool.Instance.InputImage = value;
                RaisePropertyChanged();
            }
        }
        public ICogVisionData RangeImage { get; set; }
        public object ToolResultSet { get; set; }
        public ICogRecord DisplayRecord { get; set; }
        public ObservableCollection<ICogGraphicInteractive> AllRegions { get; set; }
        public bool OverlayGraphic { get; set; }
        public bool ColorMapEnable
        {
            get => colorMapEnable; set
            {
                colorMapEnable = value;
                RaisePropertyChanged();
            }
        }



        public VMToolSetView()
        {
            InitializeRelayCommands();
            InitializeMessengerListner();

            AllRegions = new ObservableCollection<ICogGraphicInteractive>();
            AllRegions.CollectionChanged += AllRegions_CollectionChanged;

            MdPatMax.Instance.RaiseAllPropertyChanged();
            MdCrossSectionTool.Instance.OnProfileMade += Instance_OnProfileMade;
            MdCrossSectionTool.Instance.OnCrossSectionRegionChanged += Instance_OnCrossSectionRegionChanged;
            MdCrossSectionTool.Instance.OnLoaded += CrossSection_OnLoaded;
            MdHeightCalcTool.Instance.OnLoaded += HeightCalc_OnLoaded;
            MdPatMax.Instance.OnPatternNotFound += PatMax_OnPatternNotFound;
            MdPatMax.Instance.OnFixtured += PatMax_OnFixtured;
            MdTiltCalcTool.Instance.OnLoaded += TiltCalc_OnLoaded;
            
            ResultGraphicCollection = new Dictionary<string, CogGraphicCollection>();
        }

        private void TiltCalc_OnLoaded(object sender, MdTiltCalcTool e)
        {
            Messenger.Send(new ParamSaveLoadMessenger<MdTiltCalcTool>()
            {
                SaveOrLoad = SaveLoadConstancts.Load,
                ParamContainer = e
            });
        }

        private void PatMax_OnFixtured(object sender, PatMaxLoadedArgs e)
        {
            Messenger.Send(new FixturedImageMessenger()
            {
                ViewOnly = e.ViewOnly,
                FixturedImage = e.FixturedImage,
                LastRunRecord = e.LastRunRecord,
            });
        }

        private void PatMax_OnPatternNotFound(object sender, ICogImage e)
        {
            Messenger.Send(new AcquiredImageMessenger()
            {
                Image = e,
                Is2DPreview = false,
                IsOnline = false,
                Purpose = ImagePurposeConstants.View
            });
        }

        private void HeightCalc_OnLoaded(object sender, MdHeightCalcTool tool)
        {
            Messenger.Send(new ParamSaveLoadMessenger<MdHeightCalcTool>() { SaveOrLoad = SaveLoadConstancts.Load, ParamContainer = tool });
        }

        private void CrossSection_OnLoaded(object sender, MdCrossSectionTool tool)
        {
            Messenger.Send(new ParamSaveLoadMessenger<MdCrossSectionTool>(){ SaveOrLoad = SaveLoadConstancts.Load, ParamContainer = tool });
        }

        private void Instance_OnCrossSectionRegionChanged(object sender, string regionName)
        {
            Messenger.Send(new CrossSectionRegionNameSetMessenger() { RegionName = regionName });
        }

        private void Instance_OnProfileMade(object sender, EventArgs e)
        {
            if (MessageBox.Show("이 영역으로 진행하시겠습니까?", "CrossSection", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Window crossSectionToolSetView = new CrossSectionToolSetWindow();

                crossSectionToolSetView.ShowDialog();
            }
        }

        private void AllRegions_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(AllRegions));
        }

    }
}
