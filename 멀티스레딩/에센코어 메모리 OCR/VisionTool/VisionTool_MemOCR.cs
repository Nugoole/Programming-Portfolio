using Cognex.VisionPro;
using Cognex.VisionPro.CalibFix;
using Cognex.VisionPro.PMAlign;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TIS_OCRVisionTool
{
    public partial class VisionTool
    {
        public static string KLEVVPatternToolName => nameof(KLEVVPatternFinder);
        public static string KLEVVPatternFixtureToolName => "KLEVV FixtureTool";

        private ICogImage inputImage = null;
        private const string fixtureName1 = "Upper KLEVV";
        private const string fixtureName2 = "Lower KLEVV";
        private CogPMAlignTool KLEVVPatternFinder;
        private CogFixtureTool[] fixtureTool;


        private MemOCR[] memoryOCRTools;
        private bool memStickEnable = true;

        public bool MemStickEnable
        {
            get => memStickEnable; set
            {
                memStickEnable = value;
                RaisePropertyChanged();
            }
        }
        private void InitializeMemOCRTool()
        {
            memoryOCRTools = new MemOCR[2] { new MemOCR(), new MemOCR() };
            memoryOCRTools[0].ToolChanged += VisionTool_ToolChanged;
            KLEVVPatternFinder = new CogPMAlignTool { Name = nameof(KLEVVPatternFinder) };
            KLEVVPatternFinder.SearchRegion = CreateInteractiveRectangleAffine("KLEVV Pattern SearchRegion");
            KLEVVPatternFinder.Pattern.TrainRegion = CreateInteractiveRectangleAffine("KLEVV Pattern TrainRegion");
            KLEVVPatternFinder.RunParams.ZoneAngle.Configuration = CogPMAlignZoneConstants.LowHigh;
            KLEVVPatternFinder.RunParams.ZoneAngle.Low = CogMisc.DegToRad(-180);
            KLEVVPatternFinder.RunParams.ZoneAngle.High = CogMisc.DegToRad(180);
            KLEVVPatternFinder.RunParams.ZoneScale.Configuration = CogPMAlignZoneConstants.LowHigh;

            fixtureTool = new CogFixtureTool[2] { new CogFixtureTool(), new CogFixtureTool() };
            fixtureTool[0].Name = "KLEVV FixtureTool1";
            fixtureTool[1].Name = "KLEVV FixtureTool2";
            fixtureTool[0].RunParams.FixturedSpaceName = fixtureName1;
            fixtureTool[1].RunParams.FixturedSpaceName = fixtureName2;
            fixtureTool[0].RunParams.FixturedSpaceNameDuplicateHandling = CogFixturedSpaceNameDuplicateHandlingConstants.Compatibility;
            fixtureTool[1].RunParams.FixturedSpaceNameDuplicateHandling = CogFixturedSpaceNameDuplicateHandlingConstants.Compatibility;

            KLEVVPatternEventLoad();
        }

        private void KLEVVPatternEventLoad()
        {
            KLEVVPatternFinder.Ran += KLEVVPatternFinder_Ran;
            foreach (var fixtureTool in fixtureTool)
            {
                fixtureTool.Ran += FixtureTool_Ran;
            }
        }

        private void FixtureTool_Ran(object sender, EventArgs e)
        {
            var tool = sender as CogFixtureTool;
            var index = fixtureTool.ToList().IndexOf(tool);

            if (index == 0)
            {
                memoryOCRTools[0].SpaceName = fixtureName1;
                inputImage?.CoordinateSpaceTree.AddSpace("@", fixtureName1, tool.RunParams.UnfixturedFromFixturedTransform, true, CogAddSpaceConstants.ReplaceDuplicate);
                if (!patternTraining)
                    memoryOCRTools[0].Run(tool.InputImage);
            }
            else
            {
                memoryOCRTools[1].SpaceName = fixtureName2;
                inputImage?.CoordinateSpaceTree.AddSpace("@", fixtureName2, tool.RunParams.UnfixturedFromFixturedTransform, true, CogAddSpaceConstants.ReplaceDuplicate);
                if (!patternTraining)
                    memoryOCRTools[1].Run(tool.InputImage);
            }
        }

        private void KLEVVPatternFinder_Ran(object sender, EventArgs e)
        {
            if (KLEVVPatternFinder.Results != null && KLEVVPatternFinder.Results.Count > 0)
            {
                var maxCount = KLEVVPatternFinder.Results.Count >= 2 ? 2 : 1;

                List<Task> taskList = new List<Task>();

                for (int i = 0; i < maxCount; i++)
                {
                    fixtureTool[i].InputImage = KLEVVPatternFinder.InputImage;
                    fixtureTool[i].RunParams.UnfixturedFromFixturedTransform = KLEVVPatternFinder.Results[i].GetPose();
                    var curfixtureTool = fixtureTool[i];



                    taskList.Add(new Task(() =>
                    {
                        curfixtureTool.Run();
                    }));
                }

                var tasks = taskList.ToArray();

                Parallel.ForEach(tasks, x => x.Start());

                try
                {
                    Task.WaitAll(tasks);
                }
                catch (AggregateException ae)
                {


                }
            }
        }

        private void VisionTool_ToolChanged(object sender, EventArgs e)
        {
            memoryOCRTools[1].SyncTool(memoryOCRTools[0]);
        }

        public void TrainKLEVVPattern(ICogImage image)
        {
            patternTraining = true;

            KLEVVPatternFinder.Pattern.TrainImage = image;
            KLEVVPatternFinder.Pattern.Origin.TranslationX = KLEVVPatternTrainRegion.CenterX;
            KLEVVPatternFinder.Pattern.Origin.TranslationY = KLEVVPatternTrainRegion.CenterY;
            KLEVVPatternFinder.Pattern.Train();



            if (KLEVVPatternFinder.Pattern.Trained)
            {
                KLEVVPatternFinder.InputImage = image;

                KLEVVPatternFinder.Run();

                memoryOCRTools[0].SpaceName = fixtureName1;
            }

            patternTraining = false;
        }
    }
}
