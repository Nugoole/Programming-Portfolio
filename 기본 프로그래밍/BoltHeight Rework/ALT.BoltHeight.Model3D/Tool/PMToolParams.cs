using ALT.DSCamera.Interface;

using Cognex.VisionPro;

namespace ALT.DSCamera.Tool
{
    internal class PMToolParams : IPatMaxParams
    {
        public ICogRegion SearchRegion { get; set; }
    }
}
