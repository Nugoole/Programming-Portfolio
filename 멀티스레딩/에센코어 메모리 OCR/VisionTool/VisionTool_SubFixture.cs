using Cognex.VisionPro;
using Cognex.VisionPro.CalibFix;
using Cognex.VisionPro.Implementation;
using Cognex.VisionPro.PMAlign;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TIS_OCRVisionTool
{
    public partial class VisionTool
    {
        public static string SubPatternToolName => nameof(subPatternFinder);
        public static string SubPatternFixtureName => nameof(subPatternFixture);



        private const string subFixtureName = "SubFixture";
        private CogPMAlignTool subPatternFinder;
        private CogFixtureTool subPatternFixture;
        private bool subFixtureEnable;

        public bool SubPatternTrained => subPatternFinder.Pattern.Trained;
        private void InitializeSubFixture()
        {
            subPatternFinder = new CogPMAlignTool { Name = nameof(subPatternFinder) };
            subPatternFixture = new CogFixtureTool { Name = nameof(subPatternFixture) };
            subFixtureEnable = true;


            subPatternFinder.SearchRegion = CreateInteractiveRectangleAffine("Sub Pattern SearchRegion");
            subPatternFinder.Pattern.TrainRegion = CreateInteractiveRectangleAffine("Sub Pattern TrainRegion");
            subPatternFinder.RunParams.ZoneAngle.Configuration = CogPMAlignZoneConstants.LowHigh;
            subPatternFinder.RunParams.ZoneAngle.Low = CogMisc.DegToRad(-180);
            subPatternFinder.RunParams.ZoneAngle.High = CogMisc.DegToRad(180);

            subPatternFixture.RunParams.FixturedSpaceName = subFixtureName;
            subPatternFixture.RunParams.FixturedSpaceNameDuplicateHandling = CogFixturedSpaceNameDuplicateHandlingConstants.Compatibility;

            SubPatternEventLoad();
        }

        private void SubPatternEventLoad()
        {
            subPatternFinder.Ran += SubPatternFinder_Ran;
            subPatternFixture.Ran += SubPatternFixture_Ran;
        }

        //private void SubPatternEventUnload()
        //{
        //    subPatternFinder.Ran -= SubPatternFinder_Ran;
        //    subPatternFixture.Ran -= SubPatternFixture_Ran;
        //}

        public void TrainSubPattern(ICogImage inputImage)
        {
            patternTraining = true;

            //SubPatternEventLoad();

            subPatternFinder.Pattern.TrainImage = inputImage;
            subPatternFinder.Pattern.Origin.TranslationX = SubPatternTrainRegion.CenterX;
            subPatternFinder.Pattern.Origin.TranslationY = SubPatternTrainRegion.CenterY;
            subPatternFinder.Pattern.Train();

            if (subPatternFinder.Pattern.Trained)
            {
                subPatternFinder.InputImage = inputImage;

                subPatternFinder.Run();
            }

            //SubPatternEventUnload();
            patternTraining = false;
        }

        private void SubPatternFixture_Ran(object sender, EventArgs e)
        {
            if (subPatternFixture.RunStatus.Result == CogToolResultConstants.Accept)
            {
                lastFixturedName = subFixtureName;
                capacityOCR.SpaceName = lastFixturedName;
            }
        }

        private void SubPatternFinder_Ran(object sender, EventArgs e)
        {
            if (subPatternFinder.Results != null && subPatternFinder.Results.Count > 0)
            {
                subPatternFixture.RunParams.UnfixturedFromFixturedTransform = subPatternFinder.Results[0].GetPose();

                subPatternFixture.InputImage = subPatternFinder.InputImage;

                subPatternFixture.Run();
            }
        }

        private void Run_SubFixture(ICogImage inputImage)
        {
            if(subPatternFinder.Pattern.Trained)
            {
                //SubPatternEventLoad();

                subPatternFinder.InputImage = inputImage;

                subPatternFinder.Run();

                //SubPatternEventUnload();
            }
        }

        
    }
}
