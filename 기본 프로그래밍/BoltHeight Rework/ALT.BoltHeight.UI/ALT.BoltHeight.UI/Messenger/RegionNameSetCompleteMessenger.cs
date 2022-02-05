using ALT.BoltHeight.UI.ViewModel;


namespace ALT.BoltHeight.Messenger
{
    public class RegionNameSetCompleteMessenger : GalaSoft.MvvmLight.Messaging.MessageBase
    {
        public RegionSetType RegionType { get; set; }
        public string RegionName { get; set; }
    }
}
