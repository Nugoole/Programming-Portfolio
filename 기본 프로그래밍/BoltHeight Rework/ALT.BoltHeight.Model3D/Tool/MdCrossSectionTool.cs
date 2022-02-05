using ALT.DSCamera.Interface;

using Cognex.VisionPro;
using Cognex.VisionPro.Implementation;
using Cognex.VisionPro3D;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace ALT.DSCamera.Tool
{
    public enum OperatorType
    {
        ExtractPoint = 1,
        ExtractLineSegment,
        DistancePointLine
    }

    public class MdCrossSectionTool : MdToolBase, IAppliable<I3DCrossSectionRegion>
    {
        private static MdCrossSectionTool instance;
        private Cog3DRangeImageCrossSectionTool tool;

        private Dictionary<string, CrossSectionParams> Params;
        private ICogImage baseImage;


        private CrossSectionResult Result;

        public event EventHandler OnProfileMade;
        public event EventHandler<string> OnCrossSectionRegionChanged;
        public event EventHandler<MdCrossSectionTool> OnLoaded;


        public event EventHandler ReDrawProfile;

        public static MdCrossSectionTool Instance
        {
            get
            {
                if (instance is null)
                    instance = new MdCrossSectionTool();

                return instance;
            }
        }


        public IList<string> ParamKeys
        {
            get
            {
                return Params.Keys.ToList();
            }
        }

        public ObservableCollection<ICogRegion> CrossSectionRegions { get; set; }

        public CogGraphicCollection OperatorRegions { get; set; }
        public CogGraphicCollection ProfileGraphics { get; set; }
        public Dictionary<string, CogGraphicCollection> OutputGraphics { get; set; }

        public ICogRecord ProfileRecord
        {
            get
            {
                var record = new CogRecord("Profile", typeof(ICogImage), CogRecordUsageConstants.Diagnostic, true, baseImage, "");

                record.SubRecords.Add(new CogRecord("Graphic", typeof(CogGraphicCollection), CogRecordUsageConstants.Diagnostic, true, ProfileGraphics, ""));
                record.SubRecords[0].SubRecords.Add(new CogRecord("Regions", typeof(CogGraphicCollection), CogRecordUsageConstants.Diagnostic, true, OperatorRegions, ""));




                OutputGraphics.Values.ToList().ForEach(x => record.SubRecords[0].SubRecords.Add(new CogRecord("Outputs", typeof(CogGraphicCollection), CogRecordUsageConstants.Diagnostic, true, x, "")));


                return record;
            }
        }

        private void MdCrossSectionTool_DraggingStopped(object sender, CogDraggingEventArgs e)
        {
            tool.RunParams.ProfileParams.Region = e.DragGraphic as CogRectangleAffine;

            try
            {
                Params[e.DragGraphic.TipText].Profile = tool.RunParams.ProfileParams.Execute(InputImage);

                ProfileGraphics.Clear();
                ProfileGraphics = Params[e.DragGraphic.TipText].Profile.BuildProfileGraphics(false);
                baseImage = Params[e.DragGraphic.TipText].Profile.ImageUsedForProfileDisplay();
            }
            catch (Exception ex)
            {

            }

            //프로파일 생성 안될시 코드 넣기





            //End of Function
            OnProfileMade(sender, EventArgs.Empty);
            RaisePropertyChanged(nameof(ProfileRecord));
        }

        public void SetCurrentOPName(string regionName, string opName)
        {
            Params[regionName].SelectedOperatorName = opName;
        }

        public string GetCurrentOPName(string regionName)
        {
            return Params[regionName].SelectedOperatorName;
        }

        private MdCrossSectionTool()
        {
            tool = new Cog3DRangeImageCrossSectionTool();

            InitializeTool();

            Params = new Dictionary<string, CrossSectionParams>();
            Result = new CrossSectionResult();

            CrossSectionRegions = new ObservableCollection<ICogRegion>();
            OperatorRegions = new CogGraphicCollection();
            ProfileGraphics = new CogGraphicCollection();
            OutputGraphics = new Dictionary<string, CogGraphicCollection>();
            CrossSectionRegions.CollectionChanged += CrossSectionRegions_CollectionChanged;
        }

        private void InitializeTool()
        {
            tool.RunParams.UseInputProfile = true;

            tool.LastRunRecordDiagEnable = Cog3DRangeImageCrossSectionLastRunRecordDiagConstants.All;
            tool.LastRunRecordEnable = Cog3DRangeImageCrossSectionLastRunRecordConstants.All;
            tool.Ran += Tool_Ran;
        }

        private void Tool_Ran(object sender, EventArgs e)
        {
            LastRunRecords = tool.CreateLastRunRecord().SubRecords;
        }

        public void SetLineInDistancePointLine(string regionName, string obj)
        {
            var op = Params[regionName].Operators.Where(x => x.Name.Equals(Params[regionName].SelectedOperatorName)).First();
            if (op is Cog3DRangeImageCrossSectionDistancePointLine distancePointLine)
            {
                distancePointLine.LineSegment = obj;
            }
        }

        public void SetPointInDistancePointLine(string regionName, string obj)
        {
            var op = Params[regionName].Operators.Where(x => x.Name.Equals(Params[regionName].SelectedOperatorName)).First();
            if (op is Cog3DRangeImageCrossSectionDistancePointLine distancePointLine)
            {
                distancePointLine.Point = obj;
            }
        }

        public void ResetParams()
        {
            OperatorRegions.Clear();
            OutputGraphics.Clear();
        }

        private void CrossSectionRegions_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach (ICogGraphicInteractive region in e.NewItems)
                {
                    region.Changed += Region_Changed;
                    region.DraggingStopped += MdCrossSectionTool_DraggingStopped;
                }
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                foreach (ICogGraphicInteractive region in e.NewItems)
                {
                    region.DraggingStopped -= MdCrossSectionTool_DraggingStopped;
                }
            }
            else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
            {
                foreach (ICogGraphicInteractive region in CrossSectionRegions)
                {
                    region.DraggingStopped -= MdCrossSectionTool_DraggingStopped;
                }
            }
        }

        private void Region_Changed(object sender, CogChangedEventArgs e)
        {
            if ((e.StateFlags & CogRectangleAffine.SfSelected) > 0)
            {
                var rect = sender as CogRectangleAffine;
                if (rect.Selected)
                    OnCrossSectionRegionChanged?.Invoke(this, rect.TipText);
            }
        }

        public void Run(string regionName)
        {
            if (InputImage != null)
            {
                tool.InputImage = InputImage;
                tool.RunParams.ProfileParams.Region = CrossSectionRegions.Where(x => (x as CogRectangleAffine).TipText.Equals(regionName)).FirstOrDefault() as CogRectangleAffine;
                var profile = tool.RunParams.ProfileParams.Execute(InputImage);

                tool.RunParams.InputProfile = profile;
                tool.RunParams.OperatorsParams = Params[regionName].Operators;


                //tool.RunParams.InputProfile.BuildProfileGraphics(true);
                //LastRunRecords = tool.CreateLastRunRecord().SubRecords;
                tool.Run();
            }
        }

        public void ApplyParams(I3DCrossSectionRegion paramObject)
        {
            string dllName = string.Empty;
            string dllName3D = string.Empty;


            //Find Cognex Dll assembly Full Name
            foreach (var item in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (item.FullName.Contains("Cognex.VisionPro.Core"))
                {
                    dllName = item.FullName;
                }
                else if (item.FullName.Contains("Cognex.VisionPro3D"))
                {
                    dllName3D = item.FullName;
                }

                if (!string.IsNullOrEmpty(dllName) && !string.IsNullOrEmpty(dllName3D))
                    break;
            }


            if (string.IsNullOrEmpty(dllName) || string.IsNullOrEmpty(dllName3D))
                throw new Exception("Cognex Dll doesn't exist");


            var loadedObject = paramObject.CrossSectionRegions;

            CrossSectionRegions.Clear();
            Params.Clear();

            foreach (var item in loadedObject)
            {
                var newObject = Type.GetType(item.Value.Region.Type + ", " + dllName).GetConstructor(Array.Empty<Type>())?.Invoke(null);

                newObject?.GetType().GetProperty(nameof(item.Value.Region.CenterX))?.SetValue(newObject, item.Value.Region.CenterX);
                newObject?.GetType().GetProperty(nameof(item.Value.Region.CenterY))?.SetValue(newObject, item.Value.Region.CenterY);
                newObject?.GetType().GetProperty(nameof(item.Value.Region.TipText))?.SetValue(newObject, item.Value.Region.TipText);
                newObject?.GetType().GetProperty(nameof(item.Value.Region.SideXLength))?.SetValue(newObject, item.Value.Region.SideXLength);
                newObject?.GetType().GetProperty(nameof(item.Value.Region.SideYLength))?.SetValue(newObject, item.Value.Region.SideYLength);
                newObject?.GetType().GetProperty(nameof(item.Value.Region.Radius))?.SetValue(newObject, item.Value.Region.Radius);

                CrossSectionRegions.Add(newObject as ICogRegion);
                CreateNewParamList((newObject as ICogGraphicInteractive).TipText);

                foreach (var op in item.Value.Operators)
                {
                    OperatorType type = default;

                    foreach (var opType in Enum.GetNames(typeof(OperatorType)))
                    {
                        if (op.OPType.Contains(opType))
                        {
                            type = (OperatorType)Enum.Parse(typeof(OperatorType), opType);
                            break;
                        }
                    }


                    if (op.Region != null)
                    {
                        var newOPObject = Type.GetType(op.OPType + ", " + dllName3D).GetConstructor(Array.Empty<Type>()).Invoke(null) as Cog3DRangeImageCrossSectionOperatorBase;

                        Params[item.Value.Region.TipText].Operators.Add(newOPObject);


                        newOPObject.Regions[0]?.GetType().GetProperty(nameof(op.Region.CenterX))?.SetValue(newOPObject.Regions[0], op.Region.CenterX);
                        newOPObject.Regions[0]?.GetType().GetProperty(nameof(op.Region.CenterY))?.SetValue(newOPObject.Regions[0], op.Region.CenterY);
                        newOPObject.Regions[0]?.GetType().GetProperty(nameof(op.Region.TipText))?.SetValue(newOPObject.Regions[0], op.Region.TipText);
                        newOPObject.Regions[0]?.GetType().GetProperty(nameof(op.Region.SideXLength))?.SetValue(newOPObject.Regions[0], op.Region.SideXLength);
                        newOPObject.Regions[0]?.GetType().GetProperty(nameof(op.Region.SideYLength))?.SetValue(newOPObject.Regions[0], op.Region.SideYLength);
                        newOPObject.Regions[0]?.GetType().GetProperty(nameof(op.Region.Radius))?.SetValue(newOPObject.Regions[0], op.Region.Radius);
                    }
                    else
                    {
                        var newOPObject = Type.GetType(op.OPType + ", " + dllName3D).GetConstructor(Array.Empty<Type>()).Invoke(null);

                        Params[item.Value.Region.TipText].Operators.Add(newOPObject as Cog3DRangeImageCrossSectionOperatorBase);

                        newOPObject?.GetType().GetProperty("Point")?.SetValue(newOPObject, op.PointName);
                        newOPObject?.GetType().GetProperty("LineSegment")?.SetValue(newOPObject, op.LineSegmentName);
                    }
                }


            }

            OnLoaded?.Invoke(this, this);
        }

        public Dictionary<string, CrossSectionSaveLoadParam> Save()
        {
            Dictionary<string, CrossSectionSaveLoadParam> saveObject = new Dictionary<string, CrossSectionSaveLoadParam>();

            foreach (ICogGraphicInteractive region in CrossSectionRegions)
            {
                List<CrossSectionOPParam> OP_Regions = new List<CrossSectionOPParam>();

                foreach (var item in Params[region.TipText].Operators)
                {
                    if (item.Regions is null)
                    {
                        OP_Regions.Add(new CrossSectionOPParam()
                        {
                            OPType = item.GetType().ToString(),
                            PointName = item.GetType().GetProperty("Point").GetValue(item).ToString(),
                            LineSegmentName = item.GetType().GetProperty("LineSegment").GetValue(item).ToString()
                        });
                    }
                    else
                    {
                        if (item.Regions.Count > 0)
                        {
                            var x = item.Regions[0];
                            OP_Regions.Add(new CrossSectionOPParam()
                            {
                                OPType = item.GetType().ToString(),
                                Region = new RegionParams()
                                {
                                    Type = x.GetType().ToString(),
                                    TipText = x.GetType().GetProperty("TipText").GetValue(x).ToString(),
                                    CenterX = (double?)x.GetType().GetProperty("CenterX")?.GetValue(x),
                                    CenterY = (double?)x.GetType().GetProperty("CenterY")?.GetValue(x),
                                    SideXLength = (double?)x.GetType().GetProperty("SideXLength")?.GetValue(x),
                                    SideYLength = (double?)x.GetType().GetProperty("SideYLength")?.GetValue(x),
                                    Radius = (double?)x.GetType().GetProperty("Radius")?.GetValue(x)
                                }
                            });
                        }
                    }
                }

                var regionParam = new RegionParams()
                {
                    Type = region.GetType().ToString(),
                    TipText = region.GetType().GetProperty("TipText").GetValue(region).ToString(),
                    CenterX = (double?)region.GetType().GetProperty("CenterX")?.GetValue(region),
                    CenterY = (double?)region.GetType().GetProperty("CenterY")?.GetValue(region),
                    SideXLength = (double?)region.GetType().GetProperty("SideXLength")?.GetValue(region),
                    SideYLength = (double?)region.GetType().GetProperty("SideYLength")?.GetValue(region),
                    Radius = (double?)region.GetType().GetProperty("Radius")?.GetValue(region)
                };

                saveObject.Add(regionParam.TipText, new CrossSectionSaveLoadParam() { Region = regionParam, Operators = OP_Regions });
            }

            return saveObject;
        }

        public void CreateNewParamList(string regionName)
        {
            Params.Add(regionName, new CrossSectionParams());
            Params[regionName].OnOperatorRan += MdCrossSectionTool_OnOperatorRan;
        }



        private void MdCrossSectionTool_OnOperatorRan(Cog3DRangeImageCrossSectionProfile profile, Cog3DRangeImageCrossSectionOperatorsParams operators)
        {
            if (!operators.All(x =>
            {
                if (x is Cog3DRangeImageCrossSectionDistancePointLine op)
                    return !string.IsNullOrEmpty(op.Point) && !string.IsNullOrEmpty(op.LineSegment);

                return true;
            }))
                return;


            RunOperators(profile, operators);

            ReDrawProfile?.Invoke(this, EventArgs.Empty);
        }

        private void CreateResultGraphic()
        {
            foreach (var item in tool.RunParams.OperatorsParams)
            {
                CogGraphicCollection collection = new CogGraphicCollection();
                item.CreateResultGraphics(collection, false);


                foreach (ICogGraphic graphic in collection)
                {
                    if (graphic is CogRectangle)
                        collection.RemoveAt(collection.IndexOf(graphic));
                }


                OutputGraphics.Add(item.Name, collection);


                if (item is Cog3DRangeImageCrossSectionDistancePointLine)
                    continue;

                OperatorRegions.Add(item.Regions[0] as ICogGraphic);
            }
        }

        public void RunOperators(string regionName)
        {
            OutputGraphics.Clear();
            OperatorRegions.Clear();

            tool.RunParams.OperatorsParams = Params[regionName].Operators;
            tool.RunParams.OperatorsParams.Execute(Params[regionName].Profile);

            CreateResultGraphic();
        }

        public void RunOperators(Cog3DRangeImageCrossSectionProfile profile, Cog3DRangeImageCrossSectionOperatorsParams operators)
        {
            OutputGraphics.Clear();
            OperatorRegions.Clear();

            tool.RunParams.OperatorsParams = operators;
            tool.RunParams.OperatorsParams.Execute(profile);

            CreateResultGraphic();
        }
        public ObservableCollection<object> GetOperators(string regionName)
        {
            return new ObservableCollection<object>(Params[regionName].Operators);
        }

        public object GetOperator(string regionName, string opName)
        {
            return Params[regionName].Operators.Where(x => x.Name.Equals(opName)).FirstOrDefault();
        }

        public object AddNewOperator(string regionName, OperatorType operatorType)
        {
            if (operatorType == OperatorType.ExtractPoint)
            {
                Params[regionName].Operators.Add(new Cog3DRangeImageCrossSectionExtractPoint());
                OperatorRegions.Add(Params[regionName].Operators.Last().Regions[0] as ICogGraphic);
            }
            else if (operatorType == OperatorType.ExtractLineSegment)
            {
                Params[regionName].Operators.Add(new Cog3DRangeImageCrossSectionExtractLineSegment());
                OperatorRegions.Add(Params[regionName].Operators.Last().Regions[0] as ICogGraphic);
            }
            else if (operatorType == OperatorType.DistancePointLine)
            {
                Params[regionName].Operators.Add(new Cog3DRangeImageCrossSectionDistancePointLine());
            }

            var addedOP = Params[regionName].Operators.Last();

            Params[regionName].Operators.Where(x => x is Cog3DRangeImageCrossSectionDistancePointLine).ToList().ForEach(y => Params[regionName].Operators.Move(Params[regionName].Operators.IndexOf(y), Params[regionName].Operators.Count - 1));


            return addedOP;
        }

        public void RemoveSelectedGraphic(string regionName, string toolName)
        {
            foreach (ICogGraphicInteractive graphic in OperatorRegions)
            {
                if (graphic.TipText.Equals(toolName))
                {
                    OperatorRegions.RemoveAt(OperatorRegions.IndexOf(graphic));
                }
            }

            var regionsToRemove = Params[regionName].Operators.Where(x => x.Name.Equals(toolName)).ToList();
            regionsToRemove.ForEach(x => Params[regionName].Operators.Remove(x));

            if (OutputGraphics.Keys.Contains(toolName))
                OutputGraphics.Remove(toolName);
        }

        internal void GetResultGraphic(string regionName)
        {
            tool.RunParams.OperatorsParams = Params[regionName].Operators;
            tool.RunParams.InputProfile = Params[regionName].Profile;

        }
    }
}
