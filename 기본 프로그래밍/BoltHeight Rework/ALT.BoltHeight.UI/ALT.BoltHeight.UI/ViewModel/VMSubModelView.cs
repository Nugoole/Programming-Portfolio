using ALT.BoltHeight.Messenger;
using ALT.DSCamera;
using ALT.BoltHeight.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace ALT.BoltHeight.UI.ViewModel
{
    public class VMSubModelView : ViewModelBase
    {
        private readonly MdDynamicUC dynamicUC;
        public readonly MdConfig mdConfig;
        public RelayCommand<string> CmdLoad { get; }
        public RelayCommand<string> CmdSave { get; }
        public RelayCommand<string> CmdBtnTextChange { get; }
        public VMSubModelView()
        {
            CmdLoad = new RelayCommand<string>(LoadFunc);
            CmdSave = new RelayCommand<string>(SaveFunc);
            CmdBtnTextChange = new RelayCommand<string>(BtnTextChangeFunc);
            dynamicUC = MdDynamicUC.GetDynamicCs();
            mdConfig = MdConfig.GetMdConfig();
        }
        private void LoadFunc(string btntextname)
        {
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Send(new ParamSaveLoadMessenger<string>() { Modelname = btntextname, SaveOrLoad = SaveLoadConstancts.Load });
        }

        private void SaveFunc(string btntextname)
        {
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Send(new ParamSaveLoadMessenger<string>() { Modelname = btntextname, SaveOrLoad = SaveLoadConstancts.Save });
        }
        private void BtnTextChangeFunc(string btntextname)
        {
            dynamicUC.UCModelNameChange(btntextname);
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Send<GridMessenger>(new GridMessenger() { subUC = dynamicUC.GridDraw() });
        }

    }
}
