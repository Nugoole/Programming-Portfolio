namespace ALT.BoltHeight.Messenger
{
    public enum SaveLoadConstancts
    {
        Save = 0,
        Load = 1
    }
    public class ParamSaveLoadMessenger<T> : GalaSoft.MvvmLight.Messaging.MessageBase
    {
        public SaveLoadConstancts SaveOrLoad { get; set; }
        public T ParamContainer { get; set; }
        public string Modelname { get; set; }
    }
}
