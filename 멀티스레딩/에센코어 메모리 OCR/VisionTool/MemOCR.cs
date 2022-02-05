using ALT.Serialize;
using Cognex.VisionPro;
using Cognex.VisionPro.CalibFix;
using Cognex.VisionPro.ID;
using Cognex.VisionPro.Implementation;
using Cognex.VisionPro.OCRMax;
using Cognex.VisionPro.PMAlign;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TIS_OCRVisionTool
{
    public enum OCRType
    {
        Logo_Capacity,
        MemStick_PartNo,
        MemStick_Capacity,
        MemStick_Speed
    }

    [Serializable]
    public class MemOCR : SerializableObjectBase
    {
        public static string PartNoOCRToolName => nameof(partNoOCRTool);
        public static string CapacityOCRToolName => nameof(capacityOCRTool);
        public static string SpeedOCRToolName => nameof(speedOCRTool);
        public static string BarcodeToolName => nameof(barcodeReader);
        public static string StickDirectionPatternToolName => nameof(stickDirectionPatternTool);




        private object lockObject = new object();
        private string partNoResult;
        private string capacityResult;
        private string speedResult;
        private string barcodeResult;
        private bool stickDirection;





        private CogOCRMaxTool partNoOCRTool;
        private CogOCRMaxTool capacityOCRTool;
        private CogOCRMaxTool speedOCRTool;
        private CogIDTool barcodeReader;
        private CogPMAlignTool stickDirectionPatternTool;
        private string spaceName;


        



        public string SpaceName
        {
            get => spaceName;
            set
            {
                spaceName = value;
                PartNoOCRRegion.SelectedSpaceName = value;
                CapacityOCRRegion.SelectedSpaceName = value;
                SpeedOCRRegion.SelectedSpaceName = value;
                BarcodeRegion.SelectedSpaceName = value;
                StickDirectionSearchRegion.SelectedSpaceName = value;
            }
        }

        public CogRectangleAffine PartNoOCRRegion
        {
            get
            {
                return partNoOCRTool.Region;
            }
            set
            {
                partNoOCRTool.Region = value;
            }
        }

        public CogRectangleAffine CapacityOCRRegion
        {
            get
            {
                return capacityOCRTool.Region;
            }
            set
            {
                capacityOCRTool.Region = value;
            }
        }

        public CogRectangleAffine SpeedOCRRegion
        {
            get
            {
                //if (!canModify)
                //    throw new InvalidOperationException("변경 불가능한 툴입니다.");

                return speedOCRTool.Region;
            }
            set
            {
                speedOCRTool.Region = value;
            }
        }

        public CogRectangleAffine StickDirectionSearchRegion
        {
            get
            {
                return stickDirectionPatternTool.SearchRegion as CogRectangleAffine;
            }
            set
            {
                stickDirectionPatternTool.SearchRegion = value;
            }
        }

        public CogRectangleAffine StickDirectionPatternRegion
        {
            get
            {
                return stickDirectionPatternTool.Pattern.TrainRegion as CogRectangleAffine;
            }
            set
            {
                stickDirectionPatternTool.Pattern.TrainRegion = value;
            }
        }

        public CogRectangleAffine BarcodeRegion
        {
            get
            {
                return barcodeReader.Region as CogRectangleAffine;
            }
            set
            {
                barcodeReader.Region = value;
            }
        }

        public bool StickDirectionPatternTrained => stickDirectionPatternTool.Pattern.Trained;
        public string PartNoResult => partNoResult;
        public string CapacityResult => capacityResult;
        public string SpeedResult => speedResult;
        public string BarcodeResult => barcodeResult;
        public bool StickDirection => stickDirection;


        public event EventHandler ToolChanged;
        public event EventHandler ToolRan;

        internal MemOCR()
        {
            partNoOCRTool = new CogOCRMaxTool { Name = nameof(partNoOCRTool) };
            capacityOCRTool = new CogOCRMaxTool { Name = nameof(capacityOCRTool) };
            speedOCRTool = new CogOCRMaxTool { Name = nameof(speedOCRTool) };
            barcodeReader = new CogIDTool { Name = nameof(barcodeReader) };
            stickDirectionPatternTool = new CogPMAlignTool { Name = nameof(stickDirectionPatternTool) };



            ToolInitialize();
        }

        private MemOCR(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            
            //ToolEventLoad();
        }

        private void ToolInitialize()
        {
            partNoOCRTool.Region = new CogRectangleAffine
            {
                Interactive = true,
                GraphicDOFEnable = CogRectangleAffineDOFConstants.All,
                TipText = "PartNoOCR Region"
            };

            capacityOCRTool.Region = new CogRectangleAffine
            {
                Interactive = true,
                GraphicDOFEnable = CogRectangleAffineDOFConstants.All,
                TipText = "CapacityOCR Region"
            };

            speedOCRTool.Region = new CogRectangleAffine
            {
                Interactive = true,
                GraphicDOFEnable = CogRectangleAffineDOFConstants.All,
                TipText = "SpeedOCR Region"
            };

            barcodeReader.Region = new CogRectangleAffine
            {
                Interactive = true,
                GraphicDOFEnable = CogRectangleAffineDOFConstants.All,
                TipText = "Barcode Region"
            };


            stickDirectionPatternTool.SearchRegion = new CogRectangleAffine
            {
                Interactive = true,
                GraphicDOFEnable = CogRectangleAffineDOFConstants.All,
                TipText = "Stick Direction SearchRegion"
            };

            stickDirectionPatternTool.Pattern.TrainRegion = new CogRectangleAffine
            {
                Interactive = true,
                GraphicDOFEnable = CogRectangleAffineDOFConstants.All,
                TipText = "Stick Direction PatternRegion"
            };



            barcodeReader.RunParams.DecodedStringCodePage = CogIDCodePageConstants.ANSILatin1;
            barcodeReader.RunParams.Code128.Enabled = true;
            barcodeReader.RunParams.Code128.LengthMax = 40;
            barcodeReader.RunParams.Code128.LengthMin = 3;

            stickDirectionPatternTool.RunParams.ZoneAngle.Configuration = CogPMAlignZoneConstants.LowHigh;
            stickDirectionPatternTool.RunParams.ZoneAngle.Low = CogMisc.DegToRad(-180);
            stickDirectionPatternTool.RunParams.ZoneAngle.High = CogMisc.DegToRad(180);

            ToolEventLoad();
        }

        internal IList<CogOCRMaxSegmenterPositionResult> OCRSegmentation(ICogImage inputImage, OCRType memOCRType)
        {
            CogOCRMaxTool ocrTool = null;

            if (memOCRType == OCRType.MemStick_PartNo)
                ocrTool = partNoOCRTool;
            else if (memOCRType == OCRType.MemStick_Capacity)
                ocrTool = capacityOCRTool;
            else if (memOCRType == OCRType.MemStick_Speed)
                ocrTool = speedOCRTool;

            var result = ocrTool.Segmenter.Execute(inputImage as CogImage8Grey, ocrTool.Region);

            if (result.Count > 0)
            {
                return result[0];
            }
            else
                return null;
        }

        public void TrainOCR(IList<CogOCRMaxSegmenterPositionResult> ocrs, string characters, OCRType memOCRType)
        {
            if (string.IsNullOrEmpty(characters))
                return;

            CogOCRMaxTool ocrTool = null;

            if (memOCRType == OCRType.MemStick_PartNo)
                ocrTool = partNoOCRTool;
            else if (memOCRType == OCRType.MemStick_Capacity)
                ocrTool = capacityOCRTool;
            else if (memOCRType == OCRType.MemStick_Speed)
                ocrTool = speedOCRTool;


            int[] charCodes = CogOCRMaxChar.GetCharCodesFromString(characters, "?");

            foreach (var cogCharacter in ocrs)
            {
                int idx = ocrs.IndexOf(cogCharacter);

                cogCharacter.Character.CharacterCode = charCodes[idx];
                ocrTool.Classifier.Font.Add(cogCharacter.Character);
            }

            ocrTool.Classifier.Train();

            ToolChanged?.Invoke(this, EventArgs.Empty);
        }

        internal void Load(string filePath)
        {
            partNoOCRTool = CogSerializer.LoadObjectFromFile(filePath) as CogOCRMaxTool;
        }

        internal void Save(string filePath)
        {
            CogSerializer.SaveObjectToFile(partNoOCRTool, filePath);
        }

        public void TrainStickPattern(ICogImage trainImage)
        {
            stickDirectionPatternTool.Pattern.TrainImage = trainImage;
            stickDirectionPatternTool.Pattern.Origin.TranslationX = StickDirectionPatternRegion.CenterX;
            stickDirectionPatternTool.Pattern.Origin.TranslationY = StickDirectionPatternRegion.CenterY;

            stickDirectionPatternTool.Pattern.Train();

            ToolChanged?.Invoke(this, EventArgs.Empty);
        }

        public void SyncTool(MemOCR anotherOCRTool)
        {
            stickDirectionPatternTool = new CogPMAlignTool(anotherOCRTool.stickDirectionPatternTool);


            partNoOCRTool = new CogOCRMaxTool(anotherOCRTool.partNoOCRTool);
            
            capacityOCRTool = new CogOCRMaxTool(anotherOCRTool.capacityOCRTool);
            
            speedOCRTool = new CogOCRMaxTool(anotherOCRTool.speedOCRTool);

            

            var anotherBarcodeRegion = anotherOCRTool.BarcodeRegion;

            BarcodeRegion.SetCenterLengthsRotationSkew(anotherBarcodeRegion.CenterX, anotherBarcodeRegion.CenterY, anotherBarcodeRegion.SideXLength, anotherBarcodeRegion.SideYLength, anotherBarcodeRegion.Rotation, anotherBarcodeRegion.Skew);

            ToolEventLoad();
        }

        

        private void StickDirectionPatternTool_Ran(object sender, EventArgs e)
        {
            if (stickDirectionPatternTool.Results != null && stickDirectionPatternTool.Results.Count > 0)
                stickDirection = true;
            else
                stickDirection = false;
        }

        private void BarcodeReader_Ran(object sender, EventArgs e)
        {
            if (barcodeReader.Results != null && barcodeReader.Results.Count > 0)
                barcodeResult = barcodeReader.Results[0].DecodedData.DecodedString;
            else
                barcodeResult = string.Empty;
        }

        private void SpeedOCRTool_Ran(object sender, EventArgs e)
        {
            speedResult = speedOCRTool.LineResult.ResultString;
        }

        private void CapacityOCRTool_Ran(object sender, EventArgs e)
        {
            capacityResult = capacityOCRTool.LineResult.ResultString;
        }

        private void PartNoOCRTool_Ran(object sender, EventArgs e)
        {
            partNoResult = partNoOCRTool.LineResult.ResultString;
        }

        public void Run(ICogImage inputImage)
        {
            lock (lockObject)
            {
                //ToolEventLoad();

                partNoOCRTool.InputImage = new CogImage8Grey(inputImage as CogImage8Grey);
                capacityOCRTool.InputImage = new CogImage8Grey(inputImage as CogImage8Grey);
                speedOCRTool.InputImage = new CogImage8Grey(inputImage as CogImage8Grey);
                barcodeReader.InputImage = new CogImage8Grey(inputImage as CogImage8Grey);
                stickDirectionPatternTool.InputImage = new CogImage8Grey(inputImage as CogImage8Grey);


                List<Task> taskList = new List<Task>();

                taskList.Add(new Task(() =>
                {
                    if (partNoOCRTool.Classifier.Trained)
                        partNoOCRTool.Run();
                }));
                taskList.Add(new Task(() =>
                {
                    if (capacityOCRTool.Classifier.Trained)
                        capacityOCRTool.Run();
                }));
                taskList.Add(new Task(() =>
                {
                    if (speedOCRTool.Classifier.Trained)
                        speedOCRTool.Run();
                }));
                taskList.Add(new Task(() =>
                {
                    barcodeReader.Run();
                }));
                taskList.Add(new Task(() =>
                {
                    if (stickDirectionPatternTool.Pattern.Trained)
                        stickDirectionPatternTool.Run();
                }));



                var tasks = taskList.ToArray();

                Parallel.ForEach(tasks, x => x.Start());
                Task.WaitAll(tasks);

                ToolRan?.Invoke(this, EventArgs.Empty);
                //ToolEventUnload();
            }
        }

        private void Save(FileStream fStream)
        {
            using (MemoryStream pnMS = new MemoryStream())
            using (MemoryStream capMS = new MemoryStream())
            using (MemoryStream spdMS = new MemoryStream())
            using (MemoryStream bcdMS = new MemoryStream())
            using (MemoryStream stkMS = new MemoryStream())
            {
                CogSerializer.SaveObjectToStream(partNoOCRTool, pnMS);
                CogSerializer.SaveObjectToStream(capacityOCRTool, capMS);
                CogSerializer.SaveObjectToStream(speedOCRTool, spdMS);
                CogSerializer.SaveObjectToStream(barcodeReader, bcdMS);
                CogSerializer.SaveObjectToStream(stickDirectionPatternTool, stkMS);


                var strBytes = Encoding.ASCII.GetBytes(pnMS.Length + "/" + capMS.Length + "/" + spdMS.Length + "/" + bcdMS.Length + "/" + stkMS.Length);

                fStream.WriteByte(Convert.ToByte(strBytes.Length));

                fStream.Write(strBytes, 0, strBytes.Length);

                fStream.Write(pnMS.GetBuffer(), 0, (int)pnMS.Length);
                fStream.Write(capMS.GetBuffer(), 0, (int)capMS.Length);
                fStream.Write(spdMS.GetBuffer(), 0, (int)spdMS.Length);
                fStream.Write(bcdMS.GetBuffer(), 0, (int)bcdMS.Length);
                fStream.Write(stkMS.GetBuffer(), 0, (int)stkMS.Length);
            }
        }

        private void Load(FileStream fStream)
        {
            var len = fStream.ReadByte();

            //길이 읽고 그 다음 읽기

            var readBytes = new byte[Convert.ToInt32(len)];
            fStream.Read(readBytes, 0, readBytes.Length);


            var bufferLengths = Encoding.ASCII.GetString(readBytes).Replace("\0", string.Empty).Split('/');

            byte[] partnoBytes = new byte[Convert.ToInt32(bufferLengths[0])];
            byte[] capacityBytes = new byte[Convert.ToInt32(bufferLengths[1])];
            byte[] speedBytes = new byte[Convert.ToInt32(bufferLengths[2])];
            byte[] barcodeBytes = new byte[Convert.ToInt32(bufferLengths[3])];
            byte[] stickDirectionBytes = new byte[Convert.ToInt32(bufferLengths[4])];


            fStream.Read(partnoBytes, 0, partnoBytes.Length);
            fStream.Read(capacityBytes, 0, capacityBytes.Length);
            fStream.Read(speedBytes, 0, speedBytes.Length);
            fStream.Read(barcodeBytes, 0, barcodeBytes.Length);
            fStream.Read(stickDirectionBytes, 0, stickDirectionBytes.Length);

            using (MemoryStream pnMS = new MemoryStream(partnoBytes))
            using (MemoryStream capMS = new MemoryStream(capacityBytes))
            using (MemoryStream spdMS = new MemoryStream(speedBytes))
            using (MemoryStream bcdMS = new MemoryStream(barcodeBytes))
            using (MemoryStream stkMS = new MemoryStream(stickDirectionBytes))
            {
                var pnTool = CogSerializer.LoadObjectFromStream(pnMS) as CogOCRMaxTool;
                var capTool = CogSerializer.LoadObjectFromStream(capMS) as CogOCRMaxTool;
                var spdTool = CogSerializer.LoadObjectFromStream(spdMS) as CogOCRMaxTool;
                var bcdTool = CogSerializer.LoadObjectFromStream(bcdMS) as CogIDTool;
                var stkTool = CogSerializer.LoadObjectFromStream(stkMS) as CogPMAlignTool;

                partNoOCRTool = pnTool;
                capacityOCRTool = capTool;
                speedOCRTool = spdTool;
                barcodeReader = bcdTool;
                stickDirectionPatternTool = stkTool;
            }
        }

        public List<CogToolBase> GetToolsToSave()
        {
            return new List<CogToolBase>(5) { partNoOCRTool, capacityOCRTool, speedOCRTool, barcodeReader, stickDirectionPatternTool };
        }

        public void LoadTool(CogToolBase tool)
        {
            if (tool.Name.Equals(nameof(partNoOCRTool)) && tool is CogOCRMaxTool pnocrTool)
            {
                partNoOCRTool = pnocrTool;
                partNoOCRTool.Ran += PartNoOCRTool_Ran;
            }
            else if (tool.Name.Equals(nameof(capacityOCRTool)) && tool is CogOCRMaxTool capacityOcrTool)
            {
                capacityOCRTool = capacityOcrTool;
                capacityOcrTool.Ran += CapacityOCRTool_Ran;
            }
            else if (tool.Name.Equals(nameof(speedOCRTool)) && tool is CogOCRMaxTool speedOcrTool)
            {
                speedOCRTool = speedOcrTool;
                speedOcrTool.Ran += SpeedOCRTool_Ran;
            }
            else if (tool.Name.Equals(nameof(barcodeReader)) && tool is CogIDTool idTool)
            {
                barcodeReader = idTool;
                barcodeReader.Ran += BarcodeReader_Ran;
            }
            else if (tool.Name.Equals(nameof(stickDirectionPatternTool)) && tool is CogPMAlignTool pmTool)
            {
                stickDirectionPatternTool = pmTool;
                stickDirectionPatternTool.Ran += StickDirectionPatternTool_Ran;
            }
        }

        public void ToolEventLoad()
        {
            partNoOCRTool.Ran += PartNoOCRTool_Ran;
            capacityOCRTool.Ran += CapacityOCRTool_Ran;
            speedOCRTool.Ran += SpeedOCRTool_Ran;
            barcodeReader.Ran += BarcodeReader_Ran;
            stickDirectionPatternTool.Ran += StickDirectionPatternTool_Ran;

            barcodeReader.Region.Changed += (sender, args) => ToolChanged?.Invoke(this, EventArgs.Empty);
        }

        //private void ToolEventUnload()
        //{
        //    partNoOCRTool.Ran -= PartNoOCRTool_Ran;
        //    capacityOCRTool.Ran -= CapacityOCRTool_Ran;
        //    speedOCRTool.Ran -= SpeedOCRTool_Ran;
        //    barcodeReader.Ran -= BarcodeReader_Ran;
        //    stickDirectionPatternTool.Ran -= StickDirectionPatternTool_Ran;
        //}

        internal void FlushResult()
        {
            partNoResult = string.Empty;
            capacityResult = string.Empty;
            speedResult = string.Empty;
            barcodeResult = string.Empty;
        }

        
    }


    public class CogDictionaryExtension : CogDictionary
    {
        public CogDictionaryExtension()
        {
            
        }
    }

}
