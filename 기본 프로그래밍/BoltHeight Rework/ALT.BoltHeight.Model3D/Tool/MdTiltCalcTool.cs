using ALT.DSCamera.Interface;

using Cognex.VisionPro;
using Cognex.VisionPro3D;

using System;
using System.Collections.Generic;
using System.Linq;

namespace ALT.DSCamera.Tool
{
    public class MdTiltCalcTool : MdToolBase, IAppliable<I3DTiltCalcRegion>
    {
        private readonly string BasePlaneName = "BasePlane";
        private readonly string TargetPlaneName = "TargetPlane";
        private TiltCalcParams currentParameter;
        private TiltCalcResult result;

        private static MdTiltCalcTool instance;

        public event EventHandler<MdTiltCalcTool> OnLoaded;

        public static MdTiltCalcTool Instance
        {
            get
            {
                if (instance is null)
                    instance = new MdTiltCalcTool();

                return instance;
            }
        }

        private Cog3DRangeImagePlaneEstimatorTool[] Tools_3D { get; set; }
        public List<List<ICogRegion>> TiltCalcRegions { get; set; }
        public ICogRegion BasePlaneRegion
        {
            get
            {
                return currentParameter.BasePlaneRegion;
            }
            set
            {
                currentParameter.BasePlaneRegion = value;
                RaisePropertyChanged();
            }
        }
        public ITiltCalcResult Result => result;
        public ICogRegion TargetPlaneRegion
        {
            get
            {
                return currentParameter.TargetPlaneRegion;
            }
            set
            {
                currentParameter.TargetPlaneRegion = value;
                RaisePropertyChanged();
            }
        }

        public event EventHandler LastToolRan;

        private MdTiltCalcTool()
        {
            if (Tools_3D is null)
                Tools_3D = new Cog3DRangeImagePlaneEstimatorTool[2]
                {
                new Cog3DRangeImagePlaneEstimatorTool() { Name = BasePlaneName },
                new Cog3DRangeImagePlaneEstimatorTool() { Name = TargetPlaneName }
                };

            currentParameter = new TiltCalcParams();
            TiltCalcRegions = new List<List<ICogRegion>>();
        }

        public List<List<RegionParams>> Save()
        {
            var jsonObject = from x in TiltCalcRegions
                             select new List<RegionParams>(
                                 from y in x
                                 select new RegionParams()
                                 {
                                     Type = y.GetType().ToString(),
                                     TipText = y.GetType().GetProperty("TipText").GetValue(y).ToString(),
                                     CenterX = (double?)y.GetType().GetProperty("CenterX")?.GetValue(y),
                                     CenterY = (double?)y.GetType().GetProperty("CenterY")?.GetValue(y),
                                     SideXLength = (double?)y.GetType().GetProperty("SideXLength")?.GetValue(y),
                                     SideYLength = (double?)y.GetType().GetProperty("SideYLength")?.GetValue(y),
                                     Radius = (double?)y.GetType().GetProperty("Radius")?.GetValue(y),
                                 });

            return jsonObject.ToList();
        }


        public void ApplyParams(I3DTiltCalcRegion paramObject)
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





            List<List<RegionParams>> LoadedObject = paramObject.TiltCalcRegions;
            TiltCalcRegions.Clear();

            //add each region to regionCollection
            foreach (var items in LoadedObject)
            {
                List<ICogRegion> temp = new List<ICogRegion>();

                foreach (var item in items)
                {
                    var newObject = Type.GetType(item.Type + ", " + dllName).GetConstructor(Array.Empty<Type>())?.Invoke(null);

                    newObject?.GetType().GetProperty(nameof(item.CenterX))?.SetValue(newObject, item.CenterX);
                    newObject?.GetType().GetProperty(nameof(item.TipText))?.SetValue(newObject, item.TipText);
                    newObject?.GetType().GetProperty(nameof(item.CenterY))?.SetValue(newObject, item.CenterY);
                    newObject?.GetType().GetProperty(nameof(item.SideXLength))?.SetValue(newObject, item.SideXLength);
                    newObject?.GetType().GetProperty(nameof(item.SideYLength))?.SetValue(newObject, item.SideYLength);
                    newObject?.GetType().GetProperty(nameof(item.Radius))?.SetValue(newObject, item.Radius);

                    temp.Add(newObject as ICogRegion);
                }

                temp.ForEach(x => (x as ICogGraphicInteractive).Interactive = true);
                TiltCalcRegions.Add(temp);
            }


            OnLoaded?.Invoke(this, this);
        }

        public override void Run()
        {
            result = null;
            Cog3DPlane[] planes = new Cog3DPlane[2] { new Cog3DPlane(), new Cog3DPlane() };


            ICogRegion[] regions = new ICogRegion[2] { currentParameter.BasePlaneRegion, currentParameter.TargetPlaneRegion };

            for (int i = 0; i < Tools_3D.Length; i++)
            {
                Tools_3D[i].InputImage = InputImage;
                Tools_3D[i].Region = regions[i];
                Tools_3D[i].RunParams.FitMethod = Cog3DRangeImagePlaneEstimatorFitMethodConstants.Area;
                Tools_3D[i].Run();

                if (Tools_3D[i].RunStatus.Result == CogToolResultConstants.Accept)
                    planes[i] = Tools_3D[i].Result.Plane;
                else
                    throw Tools_3D[i].RunStatus.Exception;
            }



            double dotProduct = planes[0].Normal.ComputeDotProduct(planes[1].Normal);

            if (dotProduct > 1.0) dotProduct = 1.0;
            if (dotProduct < -1.0) dotProduct = -1.0;

            result = new TiltCalcResult();
            result.Tilt = Math.Acos(dotProduct) * (180 / Math.PI);
        }
    }
}
