using ALT.DSCamera.Interface;

using Cognex.VisionPro;

namespace ALT.DSCamera.Tool
{
    internal class HeightCalcParams : IHeightCalcParams
    {
        private ICogRegion region;

        public ICogRegion Region
        {
            get
            {
                if (region is null)
                    region = new CogRectangle();
                return region;
            }
            set => region = value;
        }
        public ICogRegion BasePlaneRegion { get; set; }
        public double BasePlaneHeight { get; set; }
    }
}
