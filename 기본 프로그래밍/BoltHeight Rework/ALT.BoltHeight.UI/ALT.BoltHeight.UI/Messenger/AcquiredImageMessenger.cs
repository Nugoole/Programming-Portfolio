using Cognex.VisionPro;

namespace ALT.BoltHeight.Messenger
{
    public enum ImagePurposeConstants
    {
        ToolRun,
        View
    }

    public class AcquiredImageMessenger : GalaSoft.MvvmLight.Messaging.MessageBase
    {
        public bool IsOnline { get; set; }
        public bool Is2DPreview { get; set; }
        public ICogImage Image { get; set; }
        public ImagePurposeConstants Purpose { get; set; }
    }
}
