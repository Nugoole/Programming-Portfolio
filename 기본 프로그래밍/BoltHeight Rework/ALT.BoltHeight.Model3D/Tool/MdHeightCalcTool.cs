using ALT.DSCamera.Interface;

using Cognex.VisionPro;
using Cognex.VisionPro3D;

using System;
using System.Collections.Generic;
using System.Linq;

namespace ALT.DSCamera.Tool
{
    public class MdHeightCalcTool : MdToolBase, IAppliable<I3DHeightCalcRegion>
    {
        private readonly string ImpulseCutterToolName = "ImpulseCutter";
        private readonly string HeightCalcToolName = "HeightCalc";

        private double highTail, lowTail;
        private static MdHeightCalcTool instance;
        private HeightCalcParams parameter;
        private HeightCalcResult result;

        public event EventHandler<MdHeightCalcTool> OnLoaded;

        public List<ICogRegion> HeightCalcRegions { get; set; }


        private CogToolCollection Tools_3D { get; set; }
        public IHeightCalcResult Result => result;
        public static MdHeightCalcTool Instance
        {
            get
            {
                if (instance is null)
                    instance = new MdHeightCalcTool();

                return instance;
            }
        }
        public double BasePlaneHeight
        {
            get
            {
                return parameter.BasePlaneHeight;
            }
            set
            {
                parameter.BasePlaneHeight = value;
                RaisePropertyChanged();
            }
        }
        public ICogRegion BasePlaneRegion
        {
            get
            {
                return parameter.BasePlaneRegion;
            }
            set
            {
                parameter.BasePlaneRegion = value;
                RaisePropertyChanged();
            }
        }
        public ICogRegion Region
        {
            get
            {
                return parameter.Region;
            }
            set
            {
                parameter.Region = value;
                RaisePropertyChanged();
            }
        }
        public double HighTail
        {
            get
            {
                return highTail;
            }
            set
            {
                highTail = value;
                RaisePropertyChanged();
            }
        }
        public double LowTail
        {
            get
            {
                return lowTail;
            }
            set
            {
                lowTail = value;
                RaisePropertyChanged();
            }
        }

        public event EventHandler LastToolRan;

        private MdHeightCalcTool()
        {
            if (Tools_3D is null)
                Tools_3D = new CogToolCollection(new ICogTool[] {
                new Cog3DRangeImageHeightCalculatorTool() { Name = ImpulseCutterToolName },
                new Cog3DRangeImageHeightCalculatorTool() { Name = HeightCalcToolName }
            });

            InitializeTools();

            HeightCalcRegions = new List<ICogRegion>();
            parameter = new HeightCalcParams();

            HighTail = 0.1;
            LowTail = 0.1;

            Tools_3D[ImpulseCutterToolName].Ran += ImpulseCutter_Ran;
            Tools_3D[HeightCalcToolName].Ran += LastToolRan;
        }

        private void InitializeTools()
        {
            foreach (Cog3DRangeImageHeightCalculatorTool tool in Tools_3D)
            {
                tool.Region.SelectedSpaceName = ".";
                tool.LastRunRecordDiagEnable = Cog3DRangeImageHeightCalculatorLastRunRecordDiagConstants.All;

            }
        }

        private void ImpulseCutter_Ran(object sender, EventArgs e)
        {
            var HeightCalcTool = (Tools_3D[HeightCalcToolName] as Cog3DRangeImageHeightCalculatorTool);
            var ImpulseCutter = (Tools_3D[ImpulseCutterToolName] as Cog3DRangeImageHeightCalculatorTool);
            HeightCalcTool.RunParams.HeightRangeFilterEnabled = true;
            HeightCalcTool.RunParams.HeightRangeFilterMode = Cog3DHeightRangeFilterModeConstants.IncludeValuesInRange;
            HeightCalcTool.RunParams.HeightRangeFilterLow = ImpulseCutter.Result.LowTail;
            HeightCalcTool.RunParams.HeightRangeFilterHigh = ImpulseCutter.Result.HighTail;
        }

        public List<RegionParams> Save()
        {

            var jsonObject = from x in HeightCalcRegions
                             select new RegionParams()
                             {
                                 Type = x.GetType().ToString(),
                                 TipText = x.GetType().GetProperty("TipText").GetValue(x).ToString(),
                                 CenterX = (double?)x.GetType().GetProperty("CenterX")?.GetValue(x),
                                 CenterY = (double?)x.GetType().GetProperty("CenterY")?.GetValue(x),
                                 SideXLength = (double?)x.GetType().GetProperty("SideXLength")?.GetValue(x),
                                 SideYLength = (double?)x.GetType().GetProperty("SideYLength")?.GetValue(x),
                                 Radius = (double?)x.GetType().GetProperty("Radius")?.GetValue(x)
                             };

            return jsonObject.ToList();
        }

        public void ApplyParams(I3DHeightCalcRegion paramObject)
        {
            string dllName = string.Empty;


            //Find Cognex Dll assembly Full Name
            foreach (var item in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (item.FullName.Contains("Cognex.VisionPro.Core"))
                {
                    dllName = item.FullName;
                    break;
                }
            }

            if (string.IsNullOrEmpty(dllName))
                throw new Exception("Cognex Dll doesn't exist");

            //convert json to List<RegionParams>
            List<RegionParams> LoadedObject = paramObject.HeightCalcRegions;
            HeightCalcRegions.Clear();


            if (LoadedObject != null)
                //add each region to regionCollection
                foreach (var item in LoadedObject)
                {
                    var newObject = Type.GetType(item.Type + ", " + dllName).GetConstructor(Array.Empty<Type>())?.Invoke(null);

                    newObject?.GetType().GetProperty(nameof(item.CenterX))?.SetValue(newObject, item.CenterX);
                    newObject?.GetType().GetProperty(nameof(item.CenterY))?.SetValue(newObject, item.CenterY);
                    newObject?.GetType().GetProperty(nameof(item.TipText))?.SetValue(newObject, item.TipText);
                    newObject?.GetType().GetProperty(nameof(item.SideXLength))?.SetValue(newObject, item.SideXLength);
                    newObject?.GetType().GetProperty(nameof(item.SideYLength))?.SetValue(newObject, item.SideYLength);
                    newObject?.GetType().GetProperty(nameof(item.Radius))?.SetValue(newObject, item.Radius);

                    HeightCalcRegions.Add(newObject as ICogRegion);
                }

            HeightCalcRegions.ForEach(x => (x as ICogGraphicInteractive).Interactive = true);


            OnLoaded?.Invoke(this, this);
        }

        public override void Run()
        {
            if (BasePlaneRegion is null)
            {
                result = null;
                (Tools_3D[ImpulseCutterToolName] as Cog3DRangeImageHeightCalculatorTool).RunParams.HighTailFrac = HighTail;
                (Tools_3D[ImpulseCutterToolName] as Cog3DRangeImageHeightCalculatorTool).RunParams.LowTailFrac = LowTail;

                foreach (Cog3DRangeImageHeightCalculatorTool tool in Tools_3D)
                {
                    tool.InputImage = InputImage;
                    tool.Region = parameter.Region;
                    tool.Run();
                }


                result = new HeightCalcResult();

                LastRunRecords = Tools_3D[HeightCalcToolName].CreateLastRunRecord().SubRecords;
                result.Height = (Tools_3D[HeightCalcToolName] as Cog3DRangeImageHeightCalculatorTool).Result.Mean - parameter.BasePlaneHeight;
            }
            else
            {
                result = null;

                //Tool Set
                (Tools_3D[ImpulseCutterToolName] as Cog3DRangeImageHeightCalculatorTool).RunParams.HighTailFrac = HighTail;
                (Tools_3D[ImpulseCutterToolName] as Cog3DRangeImageHeightCalculatorTool).RunParams.LowTailFrac = LowTail;


                //Calc BasePlane
                Cog3DRangeImagePlaneEstimatorTool basePlaneTool = new Cog3DRangeImagePlaneEstimatorTool();
                basePlaneTool.InputImage = InputImage;
                basePlaneTool.RunParams.FitMethod = Cog3DRangeImagePlaneEstimatorFitMethodConstants.Area;
                basePlaneTool.Region = BasePlaneRegion;
                basePlaneTool.Run();

                var basePlane = basePlaneTool.Result.Plane;


                foreach (Cog3DRangeImageHeightCalculatorTool tool in Tools_3D)
                {
                    tool.InputImage = InputImage;
                    tool.BasePlane = basePlane;
                    tool.Region = parameter.Region;
                    tool.Run();
                }

                result = new HeightCalcResult();

                LastRunRecords = Tools_3D[HeightCalcToolName].CreateLastRunRecord().SubRecords;
                result.Height = (Tools_3D[HeightCalcToolName] as Cog3DRangeImageHeightCalculatorTool).Result.Mean;
            }
        }
    }
}
