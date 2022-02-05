using CommonServiceLocator;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using LineScanViewer.Interface;
using LineScanViewer.Messenger;
using LineScanViewer.Model;
using LineScanViewer.View;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LineScanViewer.ViewModel
{
    public class TreeNode
    {
        public string Value { get; set; }
    }

    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class VMMain : ViewModelBase, IVMMain
    {

        private UserControl uCMainView;

        private readonly UCCameraView cameraView;
        private readonly UCConfigView configView;
        private readonly UCModelView modelView;
        private readonly UCImageStorage storageView;
        private string menuCategory;
        private Tree<CamNode> camTree;

        public UserControl UCMainView { get => uCMainView; set => Set(ref uCMainView, value); }

        public string MenuCategory { get => menuCategory; set => Set(ref menuCategory, value); }

        public ICommand CmdLoadCameraView { get; set; }

        public ICommand CmdLoadModelView { get; set; }

        public ICommand CmdLoadConfigView { get; set; }

        public ICommand CmdLoadStorageView { get; set; }

        public ICommand CmdOnClosing { get; set; }
        public ICommand CmdOnOpenCamera { get; set; }

        public ICommand CmdOnConnect { get; set; }

        public ICommand CmdOnDisConnect { get; set; }
        public ICommand OnCloseBtnClicked { get; set; }
        public ICommand OnMinimizeBtnClicked { get; set; }
        public Tree<CamNode> CamTree { get => camTree; set => Set(ref camTree, value); }
        public VMMain()
        {
            MessengerInstance.Register<OnCameraInitializedMessenger>(this, OnCameraInitialized);

            CmdLoadCameraView = new RelayCommand(LoadCameraView);
            CmdLoadConfigView = new RelayCommand(LoadConfigView);
            CmdLoadModelView = new RelayCommand(OpenDCF);
            CmdLoadStorageView = new RelayCommand(LoadStorageView);
            CmdOnClosing = new RelayCommand<CancelEventArgs>(OnClosing);
            CmdOnConnect = new RelayCommand(LoadDCF);
            CmdOnOpenCamera = new RelayCommand<Tree<CamNode>>(OpenCamera);
            CmdOnDisConnect = new RelayCommand(UnLoadDCF);
            OnCloseBtnClicked = new RelayCommand(OnCloseBtnClickedAction);
            OnMinimizeBtnClicked = new RelayCommand(OnMinimizeBtnClickedAction);

            cameraView = new UCCameraView();
            configView = new UCConfigView();
            modelView = new UCModelView();
            storageView = new UCImageStorage();

            MenuCategory = "Category";

            
        }

        private void OnCameraInitialized(OnCameraInitializedMessenger obj)
        {
            CamTree = obj.InitializedCamera.CamTree;
            RaisePropertyChanged(nameof(CamTree));
        }

        private void OpenCamera(Tree<CamNode> obj)
        {
            if (string.IsNullOrEmpty(obj[0].Key))
                return;

            MessengerInstance.Send(new OpenCameraMessenger { SelectedCamNode = obj[0] });
        }

        private void OpenDCF()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "DCF File(*.dcf) | *.dcf";

            bool? result = dialog.ShowDialog();

            if (result.HasValue && result.Value)
            {
                MessengerInstance.Send(new LoadDCFMessenger { DCFFilePath = dialog.FileName });
            }
        }

        private void UnLoadDCF() => MessengerInstance.Send(new MainConnectMessenger() { LoadCommand = LoadCommand.UnLoad });


        private void LoadDCF() => MessengerInstance.Send(new MainConnectMessenger() { LoadCommand = LoadCommand.Load });


        private void LoadCameraView() => ChangeMenuItem(cameraView, "Camera");

        private void LoadModelView() => ChangeMenuItem(modelView, "DCF");

        private void LoadConfigView() => ChangeMenuItem(configView, "Config");

        private void LoadStorageView() => Process.Start(ServiceLocator.Current.GetInstance<VMConfig>().ImageSavePath ?? $@"c:\users\{Environment.UserName}\desktop");

        private void ChangeMenuItem(UserControl control, string menuName)
        {
            UCMainView = control;
            MenuCategory = $"Menu > {menuName}";
        }

        private void OnClosing(CancelEventArgs e)
        {
            var messenger = new MainFormClosingMessenger();

            GalaSoft.MvvmLight.Messaging.Messenger.Default.Send(messenger);
        }

        private void OnMinimizeBtnClickedAction()
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

        private void OnCloseBtnClickedAction()
        {
            if (MessageBox.Show("정말로 종료하시겠습니까?", "프로그램 종료", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
                Application.Current.Shutdown();
        }
    }
}