namespace ALT.BoltHeight.Messenger
{
    public class ConfigMessenger : GalaSoft.MvvmLight.Messaging.MessageBase
    {
        public string CamName { get; set; }
        public string ImgFolderPath { get; set; }
        public string ConfigFolderPath { get; set; }
        public string ModelFolderPath { get; set; }
    }
}
