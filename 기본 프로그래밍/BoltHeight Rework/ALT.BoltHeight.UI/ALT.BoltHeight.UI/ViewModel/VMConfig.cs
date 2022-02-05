using ALT.BoltHeight.Messenger;
using ALT.DSCamera;
using ALT.DSCamera.Camera3D;
using GalaSoft.MvvmLight.Command;
using JsonSaveLoader;
using System.Collections.Generic;

namespace ALT.BoltHeight.UI.ViewModel
{
    public class VMConfigView : GalaSoft.MvvmLight.ViewModelBase
    {
        private string saveImgFolderPath;
        private string configFilePath;
        private string saveModelForderPath;
        private List<string> camNumList;
        private readonly MdConfig mdConfig;

        public struct PathData
        {
            public string ImgFolderPath;
            public string ConfigFolderPath;
            public string ModelFolderPath;
            public PathData(string imgpath, string configpath, string modelpath)
            {
                this.ImgFolderPath = imgpath;
                this.ConfigFolderPath = configpath;
                this.ModelFolderPath = modelpath;
            }
        }

        public RelayCommand CmdSaveImgFolderPathSet { get; }
        public RelayCommand CmdConfigFilePathSet { get; }
        public RelayCommand CmdSaveModelForderPathSet { get; }
        public RelayCommand CmdSelectCamNameChange { get; }
        public RelayCommand CmdConFigSave { get; }
        public List<string> CamNameList
        {
            get => camNumList;
            set
            {
                camNumList = value;
                RaisePropertyChanged();
            }
        }
        public string SelectedCamName { get; set; }
        public string SaveImgFolderPath
        {
            get => saveImgFolderPath;
            set
            {
                saveImgFolderPath = value;
                GalaSoft.MvvmLight.Messaging.Messenger.Default.Send(new ConfigMessenger() { ImgFolderPath = saveImgFolderPath, ConfigFolderPath = configFilePath, ModelFolderPath = saveModelForderPath });
                RaisePropertyChanged();
            }
        }
        public string ConfigFilePath
        {
            get => configFilePath;
            set
            {
                configFilePath = value;
                GalaSoft.MvvmLight.Messaging.Messenger.Default.Send(new ConfigMessenger() { ImgFolderPath = saveImgFolderPath, ConfigFolderPath = configFilePath, ModelFolderPath = saveModelForderPath });
                RaisePropertyChanged();
            }
        }
        public string SaveModelForderPath
        {
            get => saveModelForderPath;
            set
            {
                saveModelForderPath = value;
                GalaSoft.MvvmLight.Messaging.Messenger.Default.Send(new ConfigMessenger() { ImgFolderPath = saveImgFolderPath, ConfigFolderPath = configFilePath, ModelFolderPath = saveModelForderPath });
                RaisePropertyChanged();
            }
        }

        public VMConfigView()
        {
            mdConfig = MdConfig.GetMdConfig();
            CmdSaveImgFolderPathSet = new RelayCommand(SaveImgFolderPathSet);
            CmdConfigFilePathSet = new RelayCommand(ConfigFilePathSet);
            CmdSaveModelForderPathSet = new RelayCommand(SaveModelForderPathSet);
            CmdSelectCamNameChange = new RelayCommand(SelectChange);
            CmdConFigSave = new RelayCommand(ConFigSave);
            CamNameList = Md3DFrameGrabber.FrameGrabbers;
            DefaultPathSet();
        }

        private void DefaultPathSet() // Config 디폴트 로드 리팩토링 부분
        {
            if (SaveImgFolderPath == null)
            {
                SaveImgFolderPath = mdConfig.DefaultPathIni()[0];
                ConfigFilePath = mdConfig.DefaultPathIni()[1];
                SaveModelForderPath = mdConfig.DefaultPathIni()[2];
            }
        }
        private void ConFigSave()
        {
            PathData StrPath = new PathData(SaveImgFolderPath, ConfigFilePath, SaveModelForderPath);
            MdJsonSaveLoader.Save(StrPath, ConfigFilePath, "Config.json");
        }

        private void SaveImgFolderPathSet()
        {
            var FolderPath = MdConfig.FolderPathSet();
            if (FolderPath != null)
            {
                SaveImgFolderPath = FolderPath;
            }
        }
        private void ConfigFilePathSet()
        {
            var FolderPath = MdConfig.FolderPathSet();
            if (FolderPath != null)
            {
                ConfigFilePath = FolderPath;
            }
        }
        private void SaveModelForderPathSet()
        {
            var FolderPath = MdConfig.FolderPathSet();
            if (FolderPath != null)
            {
                SaveModelForderPath = FolderPath;
            }
        }
        private void SelectChange()
        {
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Send(new ConfigMessenger() { CamName = SelectedCamName });
        }
    }
}
