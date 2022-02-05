using ALT.DS.UC.Messenger;
using ALT.DSCamera;
using ALT.DSCamera.Camera3D;
using ALT.DSCamera.Tool;
using Cognex.VisionPro;
using Cognex.VisionPro.Implementation;
using JsonSaveLoader;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ALT.DS.UC.ViewModel
{
    public partial class VMToolSetView
    {
        public bool SaveImageEnable { get; set; }

        private void InitializeMessengerListner()
        {
            Messenger.Register<ExceptionMessenger>(this, OnExceptionOccurAction);
            Messenger.Register<AcquiredImageMessenger>(this, OnImageAcquiredAction);
            Messenger.Register<FixturedImageMessenger>(this, OnFixturedImageAcquiredAction);
            Messenger.Register<ParamSaveLoadMessenger<MdHeightCalcTool>>(this, OnHeightRegionsLoaded);
            Messenger.Register<ParamSaveLoadMessenger<MdTiltCalcTool>>(this, OnTiltRegionsLoaded);
            Messenger.Register<ParamSaveLoadMessenger<MdPatMax>>(this, OnPatternLoaded);
            Messenger.Register<ParamSaveLoadMessenger<MdCrossSectionTool>>(this, OnCrossSectionLoaded);
            Messenger.Register<ParamSaveLoadMessenger<string>>(this, OnModelParamsLoad);
            Messenger.Register<ParamSaveLoadMessenger<string>>(this, OnModelParamsSave);
            Messenger.Register<ConfigMessenger>(this, OnConfigChanged);
            Messenger.Register<RegionNameSetCompleteMessenger>(this, OnRegionNameSetComplete);
            Messenger.Register<_3DGraphicVisionDataAcquiredMessenger>(this, On3DGraphicVisionDataAcquired);
        }



        private void On3DGraphicVisionDataAcquired(_3DGraphicVisionDataAcquiredMessenger obj)
        {
            RangeImage = obj.visionData;
        }

        private void OnRegionNameSetComplete(RegionNameSetCompleteMessenger obj)
        {
            MdPatMax.Instance.viewOnly = true;
            MdPatMax.Instance.InputImage = DisplayImage;
            MdPatMax.Instance.Run();

            //남아있는 모든 Region 그래픽 제거
            foreach (var item in AllRegions)
            {
                item.Visible = false;
            }


            //높이 측정 Region일 때
            if (obj.RegionType == RegionSetType.Height)
            {
                if (regionType == RegionType.Rectangle)
                {
                    //Region 생성
                    var newRegion = new CogRectangleAffine() { Interactive = true, GraphicDOFEnable = CogRectangleAffineDOFConstants.All };
                    //해당 툴 모델에 추가
                    MdHeightCalcTool.Instance.HeightCalcRegions.Add(newRegion);


                    //Region 이름 짓기
                    StringBuilder builder = new StringBuilder();
                    //각 툴마다 헤더 네임이 있음, 시작은 항상 아래와 같이 "헤더 + 구분자"
                    builder.Append($"{BoltToolHeaderName}{StringSplitter}");


                    //입력한 문자열이 없다면 인덱스, 있으면 해당 문자열을 뒤에 추가
                    if (string.IsNullOrEmpty(obj.RegionName))
                        builder.Append(MdHeightCalcTool.Instance.HeightCalcRegions.Count.ToString());
                    else
                        builder.Append(obj.RegionName);


                    //이름 설정
                    newRegion.TipText = builder.ToString();


                    //나머지 처리
                    newRegion.FitToImage(DisplayImage, 0.5, 0.5);
                    AllRegions.Add(newRegion);
                    SelectedGraphic = newRegion;
                    RaisePropertyChanged(nameof(SelectedGraphic));
                }
                else
                {
                    var newRegion = new CogCircle() { Interactive = true, GraphicDOFEnable = CogCircleDOFConstants.All };
                    MdHeightCalcTool.Instance.HeightCalcRegions.Add(newRegion);

                    StringBuilder builder = new StringBuilder();
                    builder.Append($"{BoltToolHeaderName}{StringSplitter}");

                    if (string.IsNullOrEmpty(obj.RegionName))
                        builder.Append(MdHeightCalcTool.Instance.HeightCalcRegions.Count.ToString());
                    else
                        builder.Append(obj.RegionName);


                    newRegion.TipText = builder.ToString();

                    newRegion.FitToImage(DisplayImage, 0.5, 0.5);
                    AllRegions.Add(newRegion);
                    SelectedGraphic = newRegion;
                    RaisePropertyChanged(nameof(SelectedGraphic));
                }
            }





            //CrossSection툴 Region일 때
            else if (obj.RegionType == RegionSetType.CrossSection)
            {
                var newRegion = new CogRectangleAffine() { Interactive = true, GraphicDOFEnable = CogRectangleAffineDOFConstants.All };
                MdCrossSectionTool.Instance.CrossSectionRegions.Add(newRegion);

                StringBuilder builder = new StringBuilder();
                builder.Append($"{CrossSectionToolHeaderName}{StringSplitter}");

                if (string.IsNullOrEmpty(obj.RegionName))
                    builder.Append(MdCrossSectionTool.Instance.CrossSectionRegions.Count.ToString());
                else
                    builder.Append(obj.RegionName);

                newRegion.TipText = builder.ToString();

                MdCrossSectionTool.Instance.CreateNewParamList(builder.ToString());

                newRegion.FitToImage(DisplayImage, 0.5, 0.5);
                AllRegions.Add(newRegion);
                SelectedGraphic = newRegion;
                MdCrossSectionTool.Instance.InputImage = DisplayImage;
                Messenger.Send(new CrossSectionRegionNameSetMessenger() { RegionName = builder.ToString() });

                RaisePropertyChanged(nameof(SelectedGraphic));
            }





            //Tilt 툴 Region일 때
            else
            {
                if (Md3DFrameGrabber.Instance.Image != null)
                {
                    var baseRegion = new CogRectangleAffine() { Interactive = true, GraphicDOFEnable = CogRectangleAffineDOFConstants.All , SelectedSpaceName="#"};
                    var targetRegion = new CogRectangleAffine() { Interactive = true, GraphicDOFEnable = CogRectangleAffineDOFConstants.All, SelectedSpaceName = "#" };
                    MdTiltCalcTool.Instance.TiltCalcRegions.Add(new List<ICogRegion>() { baseRegion, targetRegion });

                    StringBuilder builder = new StringBuilder();
                    builder.Append($"{TiltToolHeaderName}{StringSplitter}");

                    if (string.IsNullOrEmpty(obj.RegionName))
                        builder.Append(MdTiltCalcTool.Instance.TiltCalcRegions.Count.ToString());
                    else
                        builder.Append(obj.RegionName);

                    baseRegion.TipText = builder.ToString() + $"{StringSplitter}{TiltBaseTailName}";
                    targetRegion.TipText = builder.ToString() + $"{StringSplitter}{TiltTargetTailName}";

                    baseRegion.FitToImage(DisplayImage, 0.2, 0.2);
                    targetRegion.FitToImage(DisplayImage, 0.2, 0.2);

                    baseRegion.CenterX = DisplayImage.Width * 0.2;
                    targetRegion.CenterX = DisplayImage.Width * 0.8;


                    AllRegions.Add(baseRegion);
                    AllRegions.Add(targetRegion);
                }
            }


        }

        private void NewRegion_Changed(object sender, CogChangedEventArgs e)
        {

        }

        private void OnExceptionOccurAction(ExceptionMessenger obj)
        {
#if DEBUG
            // MessageBox.Show(obj.Exception?.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
#endif
            ToolResultSet = null;
        }
        private void OnConfigChanged(ConfigMessenger obj)
        {
            if (string.IsNullOrEmpty(ModelFolderPath) || (!ModelFolderPath.Equals(obj.ModelFolderPath) && obj.ModelFolderPath != null))
                ModelFolderPath = obj.ModelFolderPath;

            if (string.IsNullOrEmpty(ImageFolderPath) || (!ImageFolderPath.Equals(obj.ImgFolderPath) && obj.ImgFolderPath != null))
                ImageFolderPath = obj.ImgFolderPath;
        }
        private void OnModelParamsSave(ParamSaveLoadMessenger<string> obj)
        {
            if (obj.SaveOrLoad == SaveLoadConstancts.Save)
            {
                Messenger.Send(new OnLoadingMessenger() { isLoading = true });

                MdPatMax.Instance.SavePattern($@"{ModelFolderPath}\{obj.Modelname}");

                _3DParamContainer.Instance.HeightCalcRegions = MdHeightCalcTool.Instance.Save();
                _3DParamContainer.Instance.TiltCalcRegions = MdTiltCalcTool.Instance.Save();
                _3DParamContainer.Instance.CrossSectionRegions = MdCrossSectionTool.Instance.Save();

                MdJsonSaveLoader.Save(_3DParamContainer.Instance, $@"{ModelFolderPath}\{obj.Modelname}", JsonFileName);

                Messenger.Send(new OnLoadingMessenger() { isLoading = false });
            }
        }
        private void OnModelParamsLoad(ParamSaveLoadMessenger<string> obj)
        {
            if (obj.SaveOrLoad == SaveLoadConstancts.Load)
            {
                Messenger.Send(new OnLoadingMessenger() { isLoading = true });

                //Load Json
                var paramContainer = MdJsonSaveLoader.Load<_3DParamContainer>($@"{ModelFolderPath}\{obj.Modelname}\{JsonFileName}");

                try
                {
                    _3DParamContainer.Instance.ApplyParams(paramContainer);

                    Md3DFrameGrabber.Instance.ApplyParams(paramContainer);
                    MdPatMax.Instance.LoadPattern($@"{ModelFolderPath}\{obj.Modelname}");
                    MdHeightCalcTool.Instance.ApplyParams(paramContainer);
                    MdTiltCalcTool.Instance.ApplyParams(paramContainer);
                    MdCrossSectionTool.Instance.ApplyParams(paramContainer);
                }
                catch (System.Exception ex)
                {
                    Messenger.Send(new ExceptionMessenger()
                    {
                        Exception = ex
                    });
                }
                

                RaisePropertyChanged(nameof(Trained));

                Messenger.Send(new OnLoadingMessenger() { isLoading = false });
            }
        }
        private void OnPatternLoaded(ParamSaveLoadMessenger<MdPatMax> obj)
        {
            if (obj.SaveOrLoad == SaveLoadConstancts.Load)
                RaisePropertyChanged(nameof(Trained));
        }
        private void OnTiltRegionsLoaded(ParamSaveLoadMessenger<MdTiltCalcTool> obj)
        {
            if (obj.SaveOrLoad == SaveLoadConstancts.Load)
                obj.ParamContainer.TiltCalcRegions.ForEach(x => x.ForEach(y =>
                {
                    (y as ICogGraphicInteractive).Visible = false;
                    AllRegions.Add(y as ICogGraphicInteractive);
                }));
        }
        private void OnHeightRegionsLoaded(ParamSaveLoadMessenger<MdHeightCalcTool> obj)
        {
            if (obj.SaveOrLoad == SaveLoadConstancts.Load)
                obj.ParamContainer.HeightCalcRegions.ForEach(x =>
                {
                    (x as ICogGraphicInteractive).Visible = false;
                    AllRegions.Add(x as ICogGraphicInteractive);
                });
        }

        private void OnCrossSectionLoaded(ParamSaveLoadMessenger<MdCrossSectionTool> obj)
        {
            if (obj.SaveOrLoad == SaveLoadConstancts.Load)
                foreach (var region in obj.ParamContainer.CrossSectionRegions)
                {
                    (region as ICogGraphicInteractive).Visible = false;
                    AllRegions.Add(region as ICogGraphicInteractive);
                }

            MdCrossSectionTool.Instance.InputImage = DisplayImage;
        }

        private void OnFixturedImageAcquiredAction(FixturedImageMessenger obj)
        {
            DisplayImage = obj.FixturedImage;
            DisplayRecord = obj.LastRunRecord;

            ResultGraphicCollection.Clear();

            if (!obj.ViewOnly)
                if (MdHeightCalcTool.Instance.HeightCalcRegions.Count > 0 && MdTiltCalcTool.Instance.TiltCalcRegions.Count > 0)
                    OnToolRunAction();

            ICogRecord tempRecord = DisplayRecord;


            if (ResultGraphicCollection != null)
                foreach (CogGraphicCollection collection in ResultGraphicCollection.Values)
                {
                    tempRecord.SubRecords.Add(new CogRecord("height", typeof(CogGraphicCollection), CogRecordUsageConstants.Diagnostic, true, collection, ""));
                }


            DisplayRecord = tempRecord;


            RaisePropertyChanged(nameof(DisplayRecord));
        }
        private void OnImageAcquiredAction(AcquiredImageMessenger obj)
        {
            OrgImage = obj.Image;

            if (!obj.Is2DPreview)
            {
                if (MdPatMax.Instance.Trained && obj.Purpose == ImagePurposeConstants.ToolRun)
                {
                    MdPatMax.Instance.InputImage = obj.Image;
                    MdPatMax.Instance.viewOnly = false;
                    MdPatMax.Instance.Run();
                }
                else
                {
                    DisplayImage = obj.Image;
                }
            }

            //저장 세팅 시점 옮기기
            SaveImageEnable = SaveImage;

            RaisePropertyChanged(nameof(SaveImageEnable));
            RaisePropertyChanged(nameof(RangeImage));
        }
    }
}
