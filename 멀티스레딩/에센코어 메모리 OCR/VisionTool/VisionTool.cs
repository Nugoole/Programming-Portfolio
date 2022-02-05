using ALT.Serialize;
using Cognex.VisionPro;
using Cognex.VisionPro.CalibFix;
using Cognex.VisionPro.Implementation;
using Cognex.VisionPro.OCRMax;
using Cognex.VisionPro.PMAlign;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TIS_OCRVisionTool
{
    public delegate void VisionToolEventHandler(VisionTool tool);

    [Serializable]
    public partial class VisionTool : SerializableObjectBase, INotifyPropertyChanged
    {
        #region PropertyChangedEventConfig
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion


        [NonSerialized()]
        private long lastRunElapsedTime;
        [NonSerialized()]
        private Stopwatch tacTimeStopWatch = new Stopwatch();




        private bool patternTraining;
        private bool subPatternEnable;

        private string lastFixturedName = ".";
        private bool mainPatternEnable;

        [field:NonSerialized]
        public event VisionToolEventHandler ToolRan;

        public long LastRunElapsedTime => lastRunElapsedTime;
        public CapacityOCR CapacityOCRTool => capacityOCR;
        public MemOCR MemoryOCRTool => memoryOCRTools[0];
        public MemOCR MemoryOCRTool2 => memoryOCRTools[1];
        public CogRectangleAffine MainPatternSearchRegion
        {
            get
            {
                return mainPatternFinder.SearchRegion as CogRectangleAffine;
            }
            set
            {
                mainPatternFinder.SearchRegion = value;
            }
        }
        public CogRectangleAffine MainPatternTrainRegion
        {
            get
            {
                return mainPatternFinder.Pattern.TrainRegion as CogRectangleAffine;
            }
            set
            {
                mainPatternFinder.Pattern.TrainRegion = value;
            }
        }
        public CogRectangleAffine SubPatternSearchRegion
        {
            get
            {
                subPatternFinder.SearchRegion.SelectedSpaceName = mainFixtureEnable ? mainFixtureName : ".";
                return subPatternFinder.SearchRegion as CogRectangleAffine;
            }
            set
            {
                subPatternFinder.SearchRegion = value;
            }
        }
        public CogRectangleAffine SubPatternTrainRegion
        {
            get
            {
                return subPatternFinder.Pattern.TrainRegion as CogRectangleAffine;
            }
            set
            {
                subPatternFinder.Pattern.TrainRegion = value;
            }
        }
        public CogRectangleAffine KLEVVPatternSearchRegion
        {
            get
            {
                KLEVVPatternFinder.SearchRegion.SelectedSpaceName = lastFixturedName;
                return KLEVVPatternFinder.SearchRegion as CogRectangleAffine;
            }
            set
            {
                KLEVVPatternFinder.SearchRegion = value;
            }
        }
        public CogRectangleAffine KLEVVPatternTrainRegion
        {
            get
            {
                return KLEVVPatternFinder.Pattern.TrainRegion as CogRectangleAffine;
            }
            set
            {
                KLEVVPatternFinder.Pattern.TrainRegion = value;
            }
        }
        public bool KLEVVPatternTrained => KLEVVPatternFinder.Pattern.Trained;

        public bool SubPatternEnable
        {
            get => subPatternEnable; set
            {
                subPatternEnable = value;
                RaisePropertyChanged();
            }
        }
        public bool MainPatternEnable
        {
            get => mainPatternEnable; set
            {
                mainPatternEnable = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<PatternTool> PatternList
        {
            get => patternList;
            set
            {
                patternList = value;
                RaisePropertyChanged();
            }
        }
        public VisionTool()
        {
            Initialize();
        }

        protected VisionTool(SerializationInfo info, StreamingContext context) :base(info, context)
        {
            
        }

        public override void OnDeserialization(object sender)
        {
            base.OnDeserialization(sender);

            EventLoad();
        }
        
        public void Initialize()
        {
            InitializeMainFixture();
            InitializeSubFixture();
            InitializeCapacityOCR();
            InitializeMemOCRTool();
            InitializePatternList();
        }

        public List<CogToolBase> GetToolsToSave()
        {
            List<CogToolBase> toolsToSave = new List<CogToolBase>();
            toolsToSave.AddRange(capacityOCR.GetToolsToSave());
            toolsToSave.AddRange(memoryOCRTools[0].GetToolsToSave());
            toolsToSave.Add(mainPatternFinder);
            toolsToSave.Add(mainPatternFixture);
            toolsToSave.Add(subPatternFinder);
            toolsToSave.Add(subPatternFixture);
            toolsToSave.Add(KLEVVPatternFinder);
            toolsToSave.AddRange(fixtureTool);
            var patList = PatternList.Select(x => x.PMTool).ToList();
            patList.ForEach(x => x.Name = "patternList_" + x.Name);
            toolsToSave.AddRange(patList);
            

            return toolsToSave;
        }
        public void LoadTools(ObservableCollection<CogToolBase> oCToolBase)
        {
            patternList.Clear();
            foreach (var tool in oCToolBase)
            {
                if (tool.Name.Equals(MainPatternToolName) && tool is CogPMAlignTool mainPMTool)
                    mainPatternFinder = mainPMTool;
                else if (tool.Name.Equals(MainPatternFixtureName) && tool is CogFixtureTool mainPMFixtureTool)
                    mainPatternFixture = mainPMFixtureTool;
                else if (tool.Name.Equals(SubPatternToolName) && tool is CogPMAlignTool subPMTool)
                    subPatternFinder = subPMTool;
                else if (tool.Name.Equals(subPatternFixture) && tool is CogFixtureTool subPMFixtureTool)
                    subPatternFixture = subPMFixtureTool;
                else if (tool.Name.Equals(KLEVVPatternFinder.Name) && tool is CogPMAlignTool klevvPMTool)
                    KLEVVPatternFinder = klevvPMTool;
                else if (tool.Name.Equals(fixtureTool[0].Name) && tool is CogFixtureTool fxTool1)
                    fixtureTool[0] = fxTool1;
                else if (tool.Name.Equals(fixtureTool[1].Name) && tool is CogFixtureTool fxTool2)
                    fixtureTool[1] = fxTool2;
                else if (tool.Name.StartsWith("patternList_"))
                {
                    tool.Name = tool.Name.Split('_')[1];
                    PatternList.Add(new PatternTool(tool as CogPMAlignTool, false));
                }
                else
                {
                    capacityOCR.LoadTools(tool);
                    memoryOCRTools[0].LoadTool(tool);
                }
            }

            EventLoad();
        }

        public void EventLoad()
        {
            MainPatternEventLoad();
            SubPatternEventLoad();
            KLEVVPatternEventLoad();
            memoryOCRTools[0].ToolEventLoad();
            memoryOCRTools[1].ToolEventLoad();
        }

        public void Load(string filePath)
        {

            MemoryOCRTool.Load(filePath);

            //using (FileStream stream = new FileStream(filePath, FileMode.Open))
            //{
            //    //capacityOCR.Load(stream);
            //    ////KLEVVPattern Load
            //    //memoryOCRTools[0].Load(stream);
            //    //memoryOCRTools[1].SyncTool(memoryOCRTools[0]);
            //}
        }

        public void Save(string filePath)
        {
            MemoryOCRTool.Save(filePath);


            //using (FileStream stream = new FileStream(filePath, FileMode.Open))
            //{
            //    //capacityOCR.Load(stream);
            //    ////KLEVVPattern Load
            //    //memoryOCRTools[0].Load(stream);
            //    //memoryOCRTools[1].SyncTool(memoryOCRTools[0]);
            //}
        }

        public void TrainOCR(IList<CogOCRMaxSegmenterPositionResult> ocrs, string characters, OCRType? memOCRType)
        {
            if (!memOCRType.HasValue)
                return;

            if (memOCRType.Value == OCRType.Logo_Capacity)
                capacityOCR.TrainOCR(characters);
            else
            {
                MemoryOCRTool.TrainOCR(ocrs, characters, memOCRType.Value);
                MemoryOCRTool2.SyncTool(MemoryOCRTool);
            }
        }

        public IList<CogOCRMaxSegmenterPositionResult> OCRSegmentation(ICogImage inputImage, OCRType? memOCRType = OCRType.Logo_Capacity)
        {
            if (!memOCRType.HasValue)
                return null;

            if (memOCRType.Value == OCRType.Logo_Capacity)
                return capacityOCR.OCRSegmentation(inputImage);
            else
                return MemoryOCRTool.OCRSegmentation(inputImage, memOCRType.Value);
        }

        public void KLEVVPatternTrain(ICogImage trainImage)
        {
            patternTraining = true;


            KLEVVPatternFinder.Pattern.TrainImage = trainImage;
            KLEVVPatternFinder.Pattern.Origin.TranslationX = KLEVVPatternTrainRegion.CenterX;
            KLEVVPatternFinder.Pattern.Origin.TranslationY = KLEVVPatternTrainRegion.CenterY;
            KLEVVPatternFinder.Pattern.Train();

            if (KLEVVPatternFinder.Pattern.Trained)
            {
                KLEVVPatternFinder.InputImage = trainImage;

                KLEVVPatternFinder.Run();

                //MemoryOCRTool.SpaceName = lastFixturedName;
            }

            patternTraining = false;
        }

        public void Run(ICogImage inputImage)
        {
            tacTimeStopWatch.Reset();
            tacTimeStopWatch.Start();


            lastFixturedName = ".";
            this.inputImage = inputImage;

            foreach (var tool in memoryOCRTools)
            {
                tool.FlushResult();
            }

            if (mainFixtureEnable)
                Run_MainFixture(inputImage);

            if (subFixtureEnable)
                Run_SubFixture(inputImage);


            List<Task> taskList = new List<Task>();

            taskList.Add(new Task(() =>
            {
                capacityOCR.SpaceName = lastFixturedName;
                capacityOCR.Run(new CogImage8Grey(inputImage as CogImage8Grey));
            }));

            if (MemStickEnable)
                taskList.Add(new Task(() =>
                {
                //var image = inputImage.CopyBase(CogImageCopyModeConstants.CopyPixels);
                //image.SelectedSpaceName = KLEVVPatternTrainRegion.SelectedSpaceName;
                KLEVVPatternFinder.SearchRegion.SelectedSpaceName = lastFixturedName;
                    KLEVVPatternFinder.InputImage = new CogImage8Grey(inputImage as CogImage8Grey);

                    if (KLEVVPatternFinder.Pattern.Trained)
                        KLEVVPatternFinder.Run();
                }));

            taskList.Add(new Task(() =>
            {
                Run_PatternList(new CogImage8Grey(inputImage as CogImage8Grey));
            }));

            var tasks = taskList.ToArray();

            Parallel.ForEach(tasks, x => x.Start());
            Task.WaitAll(tasks);



            ToolRan?.Invoke(this);
            tacTimeStopWatch.Stop();
            lastRunElapsedTime = tacTimeStopWatch.ElapsedMilliseconds;
        }

        public void ToggleOnline(bool onoff)
        {
            if (onoff)
            {

            }
        }

        private CogRectangleAffine CreateInteractiveRectangleAffine(string tipText)
        {
            return new CogRectangleAffine
            {
                Interactive = true,
                GraphicDOFEnable = CogRectangleAffineDOFConstants.All,
                TipText = tipText
            };
        }
    }
}
