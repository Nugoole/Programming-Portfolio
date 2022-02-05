using Cognex.VisionPro;

namespace ALT.DSCamera.Interface
{
    public interface IHeightCalcParams
    {
        ICogRegion Region { get; set; }
        ICogRegion BasePlaneRegion { get; set; }
        double BasePlaneHeight { get; set; }
    }
}
