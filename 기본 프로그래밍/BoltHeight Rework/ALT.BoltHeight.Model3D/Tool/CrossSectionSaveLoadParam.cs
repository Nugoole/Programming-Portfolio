using System.Collections.Generic;

namespace ALT.DSCamera.Tool
{
    public class CrossSectionSaveLoadParam
    {
        public RegionParams Region { get; set; }
        public List<CrossSectionOPParam> Operators { get; set; }
        internal CrossSectionSaveLoadParam()
        {

        }
    }
}
