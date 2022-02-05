using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TIS_OCRVisionTool
{
    public class ToolResult
    {
        public bool GBMatched { get; }
        public bool PartNoMatched { get; }
        public bool StickDirectionMatched { get; }

        internal ToolResult(bool gbMatched, bool partNoMatched, bool stickDirectionMatched)
        {
            GBMatched = gbMatched;
            PartNoMatched = partNoMatched;
            StickDirectionMatched = stickDirectionMatched;
        }
    }
}
