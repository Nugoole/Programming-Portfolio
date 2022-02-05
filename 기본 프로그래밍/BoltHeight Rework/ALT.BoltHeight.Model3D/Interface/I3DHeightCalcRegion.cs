using System.Collections.Generic;

namespace ALT.DSCamera.Interface
{
    public interface I3DHeightCalcRegion : IAppliable<I3DHeightCalcRegion>
    {
        List<RegionParams> HeightCalcRegions { get; set; }
    }
}
