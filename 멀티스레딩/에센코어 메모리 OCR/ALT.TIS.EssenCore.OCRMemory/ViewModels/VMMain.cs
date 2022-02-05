using ALT.CVL;
using ALT.Log.Interface;
using ALT.Log.Model;
using ALT.TIS.EssenCore.OCRMemory.Converters;
using ALT.TIS.EssenCore.OCRMemory.Views;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Windows;
using System.Windows.Controls;

namespace ALT.TIS.EssenCore.OCRMemory.ViewModels
{
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
    public class VMMain : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>

        #region Constructor

        public VMMain()
        {
            InitView();
            InitCommand();
            ShowMainView();
        }

        private void InitView()
        {
            cameraView = new UCCameraView();
            resultView = new UCResultView();
            recipeView = new UCRecipeView();
            ioView = new UCIOView();
            logView = new UCLogView();
            configView = new UCConfigView();
            isAdmin = Visibility.Hidden;
        }

        private void InitCommand()
        {
            CmdClose = new RelayCommand(Close);
            CmdShowMainView = new RelayCommand(ShowMainView);
            CmdShowLogView = new RelayCommand(ShowLogView);
            CmdShowRecipeView = new RelayCommand(ShowRecipeView);
            CmdShowIOView = new RelayCommand(ShowIOView);
            CmdShowResultView = new RelayCommand(ShowResultView);
            CmdShowConfigView = new RelayCommand(ShowConfigView);
            CmdShowAccessWindow = new RelayCommand(ShowAccessWindow);
            CmdAcceptAccessDialog = new RelayCommand<object>(AcceptAccessDialog);
            CmdCancelAccessDialog = new RelayCommand<object>(CancelAccessDialog);
        }

        #endregion

        #region Variables

        private int blurEffectRadius;
        private UserControl uCMainView;
        private UserControl cameraView;
        private UserControl resultView;
        private UserControl recipeView;
        private UserControl ioView;
        private UserControl logView;
        private UserControl configView;
        private Visibility isAdmin;
        private ILogger SystemLogger => LoggerDictionary.Instance.GetLogger(Log.Enum.LogUsage.System);
        #endregion

        #region Properties

        public int BlurEffectRadius { get => blurEffectRadius; private set => Set(ref blurEffectRadius, value); }
        public UserControl UCMainView { get => uCMainView; set => Set(ref uCMainView, value); }
        public RelayCommand CmdClose { get; private set; }
        public RelayCommand CmdShowMainView { get; private set; }
        public RelayCommand CmdShowLogView { get; private set; }
        public RelayCommand CmdShowRecipeView { get; private set; }
        public RelayCommand CmdShowIOView { get; private set; }
        public RelayCommand CmdShowResultView { get; private set; }
        public RelayCommand CmdShowConfigView { get; private set; }
        public RelayCommand CmdShowAccessWindow { get; private set; }
        public RelayCommand<object> CmdAcceptAccessDialog { get; private set; }
        public RelayCommand<object> CmdCancelAccessDialog { get; private set; }
        public Visibility IsAdmin { get => isAdmin; private set => Set(ref isAdmin, value); }

        #endregion

        #region Functions

        private void ShowMainView()
        {
            UCMainView = cameraView;
        }

        private void ShowResultView()
        {
            UCMainView = resultView;
        }

        private void ShowIOView()
        {
            UCMainView = ioView;
        }

        private void ShowRecipeView()
        {
            UCMainView = recipeView;
        }

        private void ShowLogView()
        {
            UCMainView = logView;
        }

        private void ShowConfigView()
        {
            UCMainView = configView;
        }

        private void ShowAccessWindow()
        {
            BlurEffectRadius = 30;
            var dialogResult = new WdAccess().ShowDialog();

            if (dialogResult.HasValue && dialogResult.Value)
            {

            }
            BlurEffectRadius = 0;
        }
        private void CancelAccessDialog(object accessWindow)
        {
            if (accessWindow is Window wd)
                wd.DialogResult = false;
        }

        private void AcceptAccessDialog(object accessWindow)
        {
            if (accessWindow is LoginParameter loginParameter)
            {
                loginParameter.WindowLogin.DialogResult = true;
                if (loginParameter.PWBox.Password.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                {
                    IsAdmin = Visibility.Visible;
                    SystemLogger.Write(Log.Enum.LogLevel.Info, "Admin Login");
                }

                else if (loginParameter.PWBox.Password.Equals("OP", StringComparison.OrdinalIgnoreCase))
                {
                    IsAdmin = Visibility.Hidden;
                    SystemLogger.Write(Log.Enum.LogLevel.Info, "OP Login");
                }
            }
        }

        private void Close()
        {
            MdFrameGrabbers.Getinstance().Close();
            Application.Current.Shutdown();
        }

        #endregion
    }
}