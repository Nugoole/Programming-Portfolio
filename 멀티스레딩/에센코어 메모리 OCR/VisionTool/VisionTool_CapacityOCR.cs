using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TIS_OCRVisionTool
{
    public partial class VisionTool
    {
        public static string CapacityOCRPatternToolName = "patternTool";
        public static string CapacityOCRToolName = "ocrTool";

        private CapacityOCR capacityOCR;

        private void InitializeCapacityOCR()
        {
            capacityOCR = new CapacityOCR();
        }
    }
}
