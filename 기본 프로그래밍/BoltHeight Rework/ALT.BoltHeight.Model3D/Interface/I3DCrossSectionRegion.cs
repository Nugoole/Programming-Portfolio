using ALT.DSCamera.Tool;

using System.Collections.Generic;

namespace ALT.DSCamera.Interface
{
    public interface I3DCrossSectionRegion : IAppliable<I3DCrossSectionRegion>
    {
        Dictionary<string, CrossSectionSaveLoadParam> CrossSectionRegions { get; set; }
    }
}
