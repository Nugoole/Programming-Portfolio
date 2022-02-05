using ALT.DS.UC.ViewModel;


namespace ALT.DS.UC.Messenger
{
    public class RegionNameSetCompleteMessenger : GalaSoft.MvvmLight.Messaging.MessageBase
    {
        public RegionSetType RegionType { get; set; }
        public string RegionName { get; set; }
    }
}
