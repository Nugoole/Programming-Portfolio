using ALT.Serialize;
using Cognex.VisionPro;
using Cognex.VisionPro.CalibFix;
using Cognex.VisionPro.Implementation;
using Cognex.VisionPro.OCRMax;
using Cognex.VisionPro.PMAlign;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace TIS_OCRVisionTool
{
    [Serializable]
    public class CapacityOCR : SerializableObjectBase
    {
        private object lockObject = new object();
        private const string fixtureName = "capacityFixture";
        private CogPMAlignTool patternTool;
        private CogFixtureTool fixtureTool;
        private CogOCRMaxTool ocrTool;
        private IList<CogOCRMaxSegmenterPositionResult> currentCharacters;
        private string ocrResult;
        private bool PatternTraining = false;
        private string spaceName;
        private bool enable;

        public bool Enable { get => enable; set => enable = value; }
        public string SpaceName
        {
            get => spaceName;
            set
            {
                spaceName = value;
                PatternRegion.SelectedSpaceName = value;
                OCRRegion.SelectedSpaceName = value;
            }
        }
        public CogRectangleAffine PatternRegion
        {
            get
            {
                return patternTool.SearchRegion as CogRectangleAffine;
            }
            set
            {
                patternTool.SearchRegion = value;
            }
        }
        public CogRectangleAffine OCRRegion
        {
            get
            {
                return ocrTool.Region;
            }
            set
            {
                ocrTool.Region = value;
            }
        }
        public CogRectangleAffine TrainRegion
        {
            get
            {
                return patternTool.Pattern.TrainRegion as CogRectangleAffine;
            }
            set
            {
                patternTool.Pattern.TrainRegion = value;
            }
        }



        public string OCRResult => ocrResult;
        public bool PatternTrained => patternTool.Pattern.Trained;


        internal CapacityOCR()
        {
            patternTool = new CogPMAlignTool { Name = nameof(patternTool) };
            fixtureTool = new CogFixtureTool { Name = nameof(fixtureTool) };
            ocrTool = new CogOCRMaxTool { Name = nameof(ocrTool) };


            ToolInitialize();
        }

        private CapacityOCR(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            ToolEventLoad();
        }

        private void ToolInitialize()
        {
            patternTool.SearchRegion = new CogRectangleAffine
            {
                Interactive = true,
                GraphicDOFEnable = CogRectangleAffineDOFConstants.All,
                TipText = "Pattern SearchRegion"
            };

            patternTool.Pattern.TrainRegion = new CogRectangleAffine
            {
                Interactive = true,
                GraphicDOFEnable = CogRectangleAffineDOFConstants.All,
                TipText = "Pattern TrainRegion"
            };

            ocrTool.Region = new CogRectangleAffine
            {
                Interactive = true,
                GraphicDOFEnable = CogRectangleAffineDOFConstants.All,
                TipText = "OCR Region"
            };

            patternTool.RunParams.ZoneAngle.Configuration = CogPMAlignZoneConstants.LowHigh;
            patternTool.RunParams.ZoneAngle.Low = CogMisc.DegToRad(-180);
            patternTool.RunParams.ZoneAngle.High = CogMisc.DegToRad(180);

            fixtureTool.UserData.Add(this.GetType().ToString(), this.GetType().ToString());
            ocrTool.UserData.Add(this.GetType().ToString(), this.GetType().ToString());
            ToolEventLoad();
        }

        private void ToolEventLoad()
        {
            patternTool.Ran += PatternTool_Ran;
            fixtureTool.Ran += FixtureTool_Ran;
            ocrTool.Ran += OcrTool_Ran;
        }

        //private void ToolEventUnload()
        //{
        //    patternTool.Ran -= PatternTool_Ran;
        //    fixtureTool.Ran -= FixtureTool_Ran;
        //    ocrTool.Ran -= OcrTool_Ran;
        //}

        private void OcrTool_Ran(object sender, EventArgs e)
        {
            if (ocrTool.Classifier.Trained)
                ocrResult = ocrTool.LineResult.ResultString;
        }

        private void FixtureTool_Ran(object sender, EventArgs e)
        {
            ocrTool.InputImage = fixtureTool.InputImage;
            ocrTool.Region.SelectedSpaceName = fixtureTool.RunParams.FixturedSpaceName;

            if (!PatternTraining)
                ocrTool.Run();
        }

        private void PatternTool_Ran(object sender, EventArgs e)
        {
            if (patternTool.Results != null && patternTool.Results.Count > 0)
            {
                fixtureTool.RunParams.FixturedSpaceName = fixtureName;
                fixtureTool.RunParams.UnfixturedFromFixturedTransform = patternTool.Results[0].GetPose();

                fixtureTool.Run();
            }
        }

        public void TrainPattern(ICogImage trainImage)
        {
            PatternTraining = true;
            //ToolEventLoad();
            patternTool.Pattern.TrainImage = trainImage;
            patternTool.Pattern.Origin.TranslationX = TrainRegion.CenterX;
            patternTool.Pattern.Origin.TranslationY = TrainRegion.CenterY;

            patternTool.Pattern.Train();

            if (patternTool.Pattern.Trained)
            {
                patternTool.InputImage = trainImage;
                fixtureTool.InputImage = trainImage;
                patternTool.Run();

                ocrTool.Region.SelectedSpaceName = fixtureName;
            }
            //ToolEventUnload();
            PatternTraining = false;
        }

        internal IList<CogOCRMaxSegmenterPositionResult> OCRSegmentation(ICogImage inputImage)
        {
            var result = ocrTool.Segmenter.Execute(inputImage as CogImage8Grey, ocrTool.Region);

            if (result.Count > 0)
            {
                currentCharacters = result[0];
                return result[0];
            }
            else
                return null;
        }

        public void LoadOCRFont(string ocrFilePath)
        {
            ocrTool.Classifier.Font.Import(ocrFilePath);
        }

        public void SaveOCRFont(string saveFilePath)
        {
            ocrTool.Classifier.Font.Export(saveFilePath);
        }

        public void TrainOCR(string characters)
        {
            int[] charCodes = CogOCRMaxChar.GetCharCodesFromString(characters, "?");

            foreach (var cogCharacter in currentCharacters)
            {
                int idx = currentCharacters.IndexOf(cogCharacter);

                cogCharacter.Character.CharacterCode = charCodes[idx];
                ocrTool.Classifier.Font.Add(cogCharacter.Character);
            }

            ocrTool.Classifier.Train();
        }

        public void Run(ICogImage inputImage)
        {
            if (!Enable)
                return;

            lock (this)
            {
                //ToolEventLoad();
                patternTool.InputImage = inputImage;
                fixtureTool.InputImage = inputImage;


                if (patternTool.Pattern.Trained)
                    patternTool.Run();

                //ToolEventUnload();
            }
        }

        private void Save(FileStream fStream)
        {
            using (MemoryStream patternMs = new MemoryStream())
            using (MemoryStream fixtureMs = new MemoryStream())
            using (MemoryStream ocrMs = new MemoryStream())
            {
                CogSerializer.SaveObjectToStream(patternTool, patternMs);
                CogSerializer.SaveObjectToStream(fixtureTool, fixtureMs);
                CogSerializer.SaveObjectToStream(ocrTool, ocrMs);

                var strBytes = Encoding.ASCII.GetBytes(patternMs.Length + "/" + fixtureMs.Length + "/" + ocrMs.Length);



                fStream.WriteByte(Convert.ToByte(strBytes.Length));

                fStream.Write(strBytes, 0, strBytes.Length);

                fStream.Write(patternMs.GetBuffer(), 0, (int)patternMs.Length);
                fStream.Write(fixtureMs.GetBuffer(), 0, (int)fixtureMs.Length);
                fStream.Write(ocrMs.GetBuffer(), 0, (int)ocrMs.Length);
            }
        }

        public List<CogToolBase> GetToolsToSave()
        {
            return new List<CogToolBase>(3) { patternTool, fixtureTool, ocrTool };
        }

        public void LoadTools(CogToolBase tool)
        {
            if (tool.Name.Equals(nameof(patternTool)) && tool is CogPMAlignTool pmTool)
            {
                patternTool = pmTool;
                patternTool.Ran += PatternTool_Ran;
            }
            else if (tool.Name.Equals(nameof(fixtureTool)) && tool is CogFixtureTool fxTool)
            {
                fixtureTool = fxTool;
                fixtureTool.Ran += FixtureTool_Ran;
            }
            else if (tool.Name.Equals(nameof(ocrTool)) && tool is CogOCRMaxTool ocrtool)
            {
                ocrTool = ocrtool;
                ocrtool.Ran += OcrTool_Ran;
            }
        }

        private void Load(FileStream fStream)
        {
            var len = fStream.ReadByte();

            //길이 읽고 그 다음 읽기

            var readBytes = new byte[Convert.ToInt32(len)];
            fStream.Read(readBytes, 0, readBytes.Length);


            var bufferLengths = Encoding.ASCII.GetString(readBytes).Replace("\0", string.Empty).Split('/');

            byte[] patternBytes = new byte[Convert.ToInt32(bufferLengths[0])];
            byte[] fixtureBytes = new byte[Convert.ToInt32(bufferLengths[1])];
            byte[] ocrBytes = new byte[Convert.ToInt32(bufferLengths[2])];


            fStream.Read(patternBytes, 0, patternBytes.Length);
            fStream.Read(fixtureBytes, 0, fixtureBytes.Length);
            fStream.Read(ocrBytes, 0, ocrBytes.Length);

            using (MemoryStream patternMS = new MemoryStream(patternBytes))
            using (MemoryStream fixtureMS = new MemoryStream(fixtureBytes))
            using (MemoryStream ocrMS = new MemoryStream(ocrBytes))
            {
                var patTool = CogSerializer.LoadObjectFromStream(patternMS) as CogPMAlignTool;
                var fixTool = CogSerializer.LoadObjectFromStream(fixtureMS) as CogFixtureTool;
                var ocrTol = CogSerializer.LoadObjectFromStream(ocrMS) as CogOCRMaxTool;

                patternTool = patTool;
                fixtureTool = fixTool;
                ocrTool = ocrTol;

                ToolEventLoad();
            }
        }
    }
}

