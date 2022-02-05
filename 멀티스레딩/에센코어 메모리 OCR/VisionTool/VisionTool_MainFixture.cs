using Cognex.VisionPro;
using Cognex.VisionPro.CalibFix;
using Cognex.VisionPro.Implementation;
using Cognex.VisionPro.PMAlign;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TIS_OCRVisionTool
{
    public partial class VisionTool
    {
        public static string MainPatternToolName => nameof(mainPatternFinder);
        public static string MainPatternFixtureName => nameof(mainPatternFixture);


        private const string mainFixtureName = "MainFixture";
        private CogPMAlignTool mainPatternFinder;
        private CogFixtureTool mainPatternFixture;
        private bool mainFixtureEnable;

        public CogToolBase mainTool => mainPatternFinder;
        public bool MainPatternTrained => mainPatternFinder.Pattern.Trained;
        public bool MainFixtureEnable { get=>mainFixtureEnable; set=>mainFixtureEnable = value; }
        public bool SubFixtureEnable { get=>subFixtureEnable; set=>subFixtureEnable = value; }

        private void InitializeMainFixture()
        {
            mainPatternFinder = new CogPMAlignTool { Name = nameof(mainPatternFinder) };
            mainPatternFixture = new CogFixtureTool { Name = nameof(mainPatternFixture) };
            mainFixtureEnable = true;

            mainPatternFinder.SearchRegion = null; // CreateInteractiveRectangleAffine("Main Pattern SearchRegion");
            mainPatternFinder.Pattern.TrainRegion = CreateInteractiveRectangleAffine("Main Pattern TrainRegion");
            mainPatternFinder.RunParams.ZoneAngle.Configuration = CogPMAlignZoneConstants.LowHigh;
            mainPatternFinder.RunParams.ZoneAngle.Low = CogMisc.DegToRad(-180);
            mainPatternFinder.RunParams.ZoneAngle.High = CogMisc.DegToRad(180);


            mainPatternFixture.RunParams.FixturedSpaceName = mainFixtureName;
            mainPatternFixture.RunParams.FixturedSpaceNameDuplicateHandling = CogFixturedSpaceNameDuplicateHandlingConstants.Compatibility;

            
            MainPatternEventLoad();
        }

        private void MainPatternEventLoad()
        {
            mainPatternFinder.Ran += MainPatternFinder_Ran;
            mainPatternFixture.Ran += MainPatternFixture_Ran;
        }

        //private void MainPatternEventUnload()
        //{
        //    mainPatternFinder.Ran -= MainPatternFinder_Ran;
        //    mainPatternFixture.Ran -= MainPatternFixture_Ran;
        //}

        public void TrainMainPattern(ICogImage inputImage)
        {
            patternTraining = true;

            //MainPatternEventLoad();
            mainPatternFinder.Pattern.TrainImage = inputImage;
            mainPatternFinder.Pattern.Origin.TranslationX = MainPatternTrainRegion.CenterX;
            mainPatternFinder.Pattern.Origin.TranslationY = MainPatternTrainRegion.CenterY;
            mainPatternFinder.Pattern.Train();

            if (mainPatternFinder.Pattern.Trained)
            {
                mainPatternFinder.InputImage = inputImage;

                mainPatternFinder.Run();
            }

            //MainPatternEventUnload();
            patternTraining = false;
        }

        private void MainPatternFixture_Ran(object sender, EventArgs e)
        {
            if(mainPatternFixture.RunStatus.Result == CogToolResultConstants.Accept)
            {
                lastFixturedName = mainFixtureName;
                capacityOCR.SpaceName = lastFixturedName;
            }
        }

        private void MainPatternFinder_Ran(object sender, EventArgs e)
        {
            if(mainPatternFinder.Results != null && mainPatternFinder.Results.Count > 0)
            {
                mainPatternFixture.RunParams.UnfixturedFromFixturedTransform = mainPatternFinder.Results[0].GetPose();

                mainPatternFixture.InputImage = mainPatternFinder.InputImage;

                mainPatternFixture.Run();
            }
        }

        private void Run_MainFixture(ICogImage inputImage)
        {
            if(mainPatternFinder.Pattern.Trained)
            {
                //MainPatternEventLoad();
                mainPatternFinder.InputImage = inputImage;

                mainPatternFinder.Run();

                //MainPatternEventUnload();
            }
        }
    }
}
