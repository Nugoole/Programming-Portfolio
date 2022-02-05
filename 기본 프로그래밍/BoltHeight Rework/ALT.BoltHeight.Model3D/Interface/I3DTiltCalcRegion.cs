using System.Collections.Generic;

namespace ALT.DSCamera.Interface
{
    public interface I3DTiltCalcRegion : IAppliable<I3DTiltCalcRegion>
    {
        List<List<RegionParams>> TiltCalcRegions { get; set; }
    }
}
