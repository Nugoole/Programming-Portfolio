using Cognex.VisionPro;

namespace ALT.DSCamera.Interface
{
    public interface I3DTool
    {
        ICogImage InputImage { get; set; }
        ICogRecords LastRunRecords { get; set; }
        CogGraphicCollection GetGraphicFromRecord(ICogRecord record);

        void Run();
    }
}
