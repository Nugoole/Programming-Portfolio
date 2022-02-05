using Cognex.VisionPro;

namespace ALT.BoltHeight.Messenger
{
    public class FixturedImageMessenger : GalaSoft.MvvmLight.Messaging.MessageBase
    {
        public bool ViewOnly { get; set; }
        public ICogImage FixturedImage { get; set; }
        public ICogRecord LastRunRecord { get; set; }
    }
}
