using Cognex.VisionPro;
using Cognex.VisionPro.CalibFix;
using Cognex.VisionPro.PMAlign;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace ALT.DSCamera.Tool
{
    public class PatMaxLoadedArgs : EventArgs
    {
        public bool ViewOnly { get; set; }
        public ICogImage FixturedImage { get; set; }
        public ICogRecord LastRunRecord { get; set; }

        internal PatMaxLoadedArgs() : base()
        {

        }
    }

    public class MdPatMax : MdToolBase
    {
        private readonly string PatternSaveFileName = "Pattern.pat";

        private ObservableCollection<ICogGraphicInteractive> regionGraphics;
        private CogPMAlignTool PMTool;
        private CogFixtureTool FixtureTool;
        private static MdPatMax instance;
        private CogRectangleAffine region;
        private CogCoordinateAxes fixturePoint;
        private bool drawFixturedAxes;
        private bool drawUnFixturedAxes;
        private bool viewOnlyFlag;
        private PMToolParams parameter;


        public event EventHandler<ICogImage> ToolRan;
        public event EventHandler<ICogImage> OnPatternNotFound;
        public event EventHandler<PatMaxLoadedArgs> OnFixtured;



        public bool DrawFixturedAxes
        {
            get => drawFixturedAxes; set
            {
                drawFixturedAxes = value;
                RaisePropertyChanged();
            }
        }
        public bool DrawUnFixturedAxes
        {
            get => drawUnFixturedAxes; set
            {
                drawUnFixturedAxes = value;
                RaisePropertyChanged();
            }
        }
        public static MdPatMax Instance
        {
            get
            {
                if (instance is null)
                    instance = new MdPatMax();

                return instance;
            }
        }



        public ObservableCollection<ICogGraphicInteractive> RegionGraphics
        {
            get
            {
                if (regionGraphics is null)
                {
                    regionGraphics = new ObservableCollection<ICogGraphicInteractive>() { Region, FixturePoint };
                }

                return regionGraphics;
            }
        }
        public ICogGraphicInteractive Region
        {
            get
            {
                if (region is null)
                    region = new CogRectangleAffine() { Interactive = true, GraphicDOFEnable = CogRectangleAffineDOFConstants.All, Visible = false };

                return region;
            }
            set
            {
                region = value as CogRectangleAffine;
            }
        }

        public ICogGraphicInteractive FixturePoint
        {
            get
            {
                if (fixturePoint is null)
                    fixturePoint = new CogCoordinateAxes() { Visible = false };

                return fixturePoint;
            }
            set
            {
                fixturePoint = value as CogCoordinateAxes;
                RaisePropertyChanged();
            }
        }

        public double ScoreThreshold
        {
            get
            {
                return PMTool.RunParams.AcceptThreshold * 100;
            }
            set
            {
                PMTool.RunParams.AcceptThreshold = value / 100.0;
                RaisePropertyChanged();
            }
        }

        public double Angle
        {
            get
            {
                if (PMTool != null)
                    return PMTool.RunParams.ZoneAngle.High;

                return 0;
            }
            set
            {
                if (PMTool != null)
                {
                    PMTool.RunParams.ZoneAngle.High = value;
                    PMTool.RunParams.ZoneAngle.Low = -value;
                }
                RaisePropertyChanged();
            }
        }

        public bool Trained
        {
            get
            {
                if (PMTool != null)
                    return PMTool.Pattern.Trained;

                return false;
            }
        }

        public ICogImage FixturedImage => FixtureTool.OutputImage;

        public ICogRegion SearchRegion
        {
            get
            {
                return parameter.SearchRegion;
            }
            set
            {
                parameter.SearchRegion = value;
            }
        }

        public bool viewOnly { get; set; }

        private MdPatMax()
        {
            PMTool = new CogPMAlignTool();
            FixtureTool = new CogFixtureTool();
            parameter = new PMToolParams();

            SetParams();

            PMTool.Ran += PMTool_Ran;
            FixtureTool.Ran += FixtureTool_Ran;
            Region.Changed += OnPMRegionChanged;
            RaiseAllPropertyChanged();
        }

        /// <summary>
        /// 패턴매칭 Region이 변할 때 중앙 점을 Origin 점으로 바꿈
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPMRegionChanged(object sender, CogChangedEventArgs e)
        {
            (FixturePoint as CogCoordinateAxes).OriginX = region.CenterX;
            (FixturePoint as CogCoordinateAxes).OriginY = region.CenterY;
        }

        /// <summary>
        /// Fixture Tool이 실행 된 후 실행 할 함수
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FixtureTool_Ran(object sender, EventArgs e)
        {
            RaisePropertyChanged(nameof(FixturedImage));

            OnFixtured?.Invoke(this, new PatMaxLoadedArgs()
            {
                ViewOnly = viewOnlyFlag,
                FixturedImage = FixturedImage,
                LastRunRecord = FixtureTool.CreateLastRunRecord().SubRecords[0]
            });
        }

        private void PMTool_Ran(object sender, EventArgs e)
        {
            //패턴이 검출되면 픽스처 툴 실행
            if (PMTool.Results?.Count > 0)
            {
                if (PMTool.Results[0].Score * 100 >= ScoreThreshold)
                {
                    FixtureTool.InputImage = PMTool.InputImage;
                    FixtureTool.RunParams.UnfixturedFromFixturedTransform = PMTool.Results[0].GetPose();

                    FixtureTool.Run();

                    return;
                }
            }
            //패턴이 검출되지 않았을 때

            OnPatternNotFound?.Invoke(this, PMTool.InputImage);


#if DEBUG
            throw PMTool.RunStatus.Result != CogToolResultConstants.Accept
                ? PMTool.RunStatus.Exception
                : new Exception("패턴을 찾지 못했습니다.");
#endif
        }

        private void SetParams()
        {
            PMTool.RunParams.ZoneAngle.Configuration = CogPMAlignZoneConstants.LowHigh;

            FixtureTool.RunParams.FixturedSpaceNameDuplicateHandling = CogFixturedSpaceNameDuplicateHandlingConstants.Compatibility;
            FixtureTool.RunParams.FixturedSpaceName = "FixturedSpace";
        }

        public void Train(ICogImage inputImage)
        {
            PMTool.Pattern.TrainMode = CogPMAlignTrainModeConstants.Image;

            PMTool.Pattern.TrainImage = inputImage;
            PMTool.Pattern.TrainRegion = region;
            PMTool.Pattern.Origin.Rotation = region.Rotation;
            PMTool.Pattern.Origin.Skew = fixturePoint.Skew;
            PMTool.Pattern.Origin.TranslationX = fixturePoint.OriginX;
            PMTool.Pattern.Origin.TranslationY = fixturePoint.OriginY;
            PMTool.Pattern.Train();

            if (Trained)
                InputImage = inputImage;

            RegionGraphics.All(x => x.Visible = false);
            RaisePropertyChanged(nameof(Trained));

            viewOnly = true;


            Run();
        }

        public void UnTrain()
        {
            PMTool.Pattern.Untrain();
            RaisePropertyChanged(nameof(Trained));
        }

        public override void Run()
        {
            if (InputImage != null && Trained)
            {
                viewOnlyFlag = viewOnly;
                PMTool.InputImage = InputImage;
                PMTool.SearchRegion = parameter.SearchRegion;
                PMTool.Run();
            }
        }

        public void RaiseAllPropertyChanged()
        {
            GetType().GetProperties().ToList().ForEach(x => RaisePropertyChanged(x.Name));
        }

        public void OnFixtureRecordConditionChanged()
        {
            FixtureTool.LastRunRecordDiagEnable = (CogFixtureLastRunRecordDiagConstants)((int)CogFixtureLastRunRecordDiagConstants.FixturedAxes * Convert.ToInt32(DrawFixturedAxes)) + ((int)CogFixtureLastRunRecordDiagConstants.UnfixturedAxes * Convert.ToInt32(DrawUnFixturedAxes));

            FixtureTool.Run();
        }

        public void SavePattern(string modelFolderPath)
        {
            if (PMTool != null && PMTool.Pattern.Trained)
            {
                if (!Directory.Exists(modelFolderPath))
                    Directory.CreateDirectory(modelFolderPath);
                CogSerializer.SaveObjectToFile(PMTool.Pattern, $@"{modelFolderPath}\{PatternSaveFileName}");
            }
        }

        public void LoadPattern(string modelFolderPath)
        {
            if (PMTool != null)
            {
                if (!File.Exists($@"{modelFolderPath}\{PatternSaveFileName}"))
                {
                    throw new Exception("패턴 파일이 없습니다.");
                }

                PMTool.Pattern = CogSerializer.LoadObjectFromFile($@"{modelFolderPath}\{PatternSaveFileName}") as CogPMAlignPattern;
            }

        }
    }
}
