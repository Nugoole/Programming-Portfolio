using ALT.DSCamera.Interface;

using Cognex.VisionPro;

namespace ALT.DSCamera.Tool
{
    internal class TiltCalcParams : ITiltCalcParams
    {
        private ICogRegion basePlaneRegion;
        private ICogRegion targetPlaneRegion;

        public ICogRegion BasePlaneRegion
        {
            get
            {
                if (basePlaneRegion is null)
                    basePlaneRegion = new CogRectangle();
                return basePlaneRegion;
            }

            set => basePlaneRegion = value;
        }

        public ICogRegion TargetPlaneRegion
        {
            get
            {
                if (targetPlaneRegion is null)
                    targetPlaneRegion = new CogRectangle();
                return targetPlaneRegion;
            }
            set => targetPlaneRegion = value;
        }
    }
}
