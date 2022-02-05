using Cognex.VisionPro;

namespace ALT.DSCamera.Interface
{
    interface ITiltCalcParams
    {
        ICogRegion BasePlaneRegion { get; set; }
        ICogRegion TargetPlaneRegion { get; set; }
    }
}
