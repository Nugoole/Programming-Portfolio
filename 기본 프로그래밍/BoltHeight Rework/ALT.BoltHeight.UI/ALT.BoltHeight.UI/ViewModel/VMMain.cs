using ALT.BoltHeight.Messenger;
using ALT.DSCamera;
using ALT.BoltHeight.Views;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ALT.BoltHeight.UI.ViewModel
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
        private UserControl uCMainView;

        private Window loadingWindow;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public VMMain()
        {
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}
            ///
            loadingWindow = new LoadingWindow();

            ResourceDictionary dictionary = new ResourceDictionary();
            dictionary.Source = new Uri(@"Dictionaries\DictionaryUC.xaml", UriKind.Relative);

            CameraView = (dictionary["dtCameraView"] as DataTemplate).LoadContent();
            ConfigView = (dictionary["dtConfigView"] as DataTemplate).LoadContent();
            ModelView = (dictionary["dtModelView"] as DataTemplate).LoadContent();
            ToolSetView = (dictionary["dtToolSetView"] as DataTemplate).LoadContent();





            Config = MdConfig.GetMdConfig();

            CmdLoadCameraView = new RelayCommand(LoadToolSetView);
            CmdLoadConfigView = new RelayCommand(LoadConfigView);
            CmdLoadModelView = new RelayCommand(LoadModelView);
            OnCloseBtnClicked = new RelayCommand(OnCloseBtnClickedAction);
            OnMinimizeBtnClicked = new RelayCommand(OnMinimizeBtnClickedAction);

            //20.06.17 Code_by YS
            timerDateTime = new DispatcherTimer();
            timerDateTime.Interval = TimeSpan.FromSeconds(1);
            timerDateTime.Tick += TimerDateTime_Tick;
            timerDateTime.Start();
            //Code_by JD
            OnClosing = new RelayCommand<CancelEventArgs>(OnClosingAction);
            //(ToolSetView as UCToolSetView).cogDisplayForWPF.OnSaveImage += SimpleIoc.Default.GetInstance<VMToolSetView>().OnImageSavingAction;
            //SimpleIoc.Default.GetInstance<VMToolSetView>().SaveImageEvent += (ToolSetView as UCToolSetView).cogDisplayForWPF.SaveImage;

            GalaSoft.MvvmLight.Messaging.Messenger.Default.Register<OnLoadingMessenger>(this, OnLoadingAction);
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

        private void OnLoadingAction(OnLoadingMessenger obj)
        {
            if (obj.isLoading)
            {
                if (!loadingWindow.IsVisible)
                {
                    loadingWindow.Close();


                    loadingWindow = new LoadingWindow();
                    loadingWindow.Show();

                }
            }
            else
            {
                if (loadingWindow.IsVisible)
                {
                    loadingWindow.Dispatcher.Invoke(() => loadingWindow.Close());
                }
            }
        }


        //Code_by JD
        private void OnClosingAction(CancelEventArgs obj)
        {
            var messenger = new MainFormClosingMessenger();

            GalaSoft.MvvmLight.Messaging.Messenger.Default.Send(messenger);

            loadingWindow.Close();
        }





        #region Function

        private void LoadModelView() => ChangeMenuItem(ModelView as UserControl, "Model");

        private void LoadConfigView() => ChangeMenuItem(ConfigView as UserControl, "Config");

        private void LoadToolSetView() => ChangeMenuItem(ToolSetView as UserControl, "ToolSet");

        private void ChangeMenuItem(UserControl control, string menuName)
        {
            UCMainView = control;
            MenuCategory = $"Menu > {menuName}";
            RaisePropertyChanged("MenuCategory");
        }

        private void TimerDateTime_Tick(object sender, EventArgs e)
        {
            DateTimeNow = $"{DateTime.Now.ToLongDateString()} {DateTime.Now.ToShortTimeString()}";
        }

        #endregion

        #region Property

        public string MenuCategory { get; set; } = "Category";
        public MdConfig Config { get; set; }
        //public UCConfigView ConfigView { get; set; }
        //public UCModelView ModelView { get; set; }
        //public UCToolSetView ToolSetView { get; set; }
        public RelayCommand CmdLoadCameraView { get; set; }
        public RelayCommand CmdLoadConfigView { get; set; }
        public RelayCommand CmdLoadModelView { get; set; }
        public RelayCommand OnCloseBtnClicked { get; set; }
        public RelayCommand OnMinimizeBtnClicked { get; set; }
        #region JD_UC_ReuseTest
        public object CameraView { get; set; }
        public object ConfigView { get; set; }
        public object ModelView { get; set; }
        public object ToolSetView { get; set; }

        #endregion

        private readonly DispatcherTimer timerDateTime;
        private string dateTimeNow;

        public string DateTimeNow { get => dateTimeNow; set => Set(ref dateTimeNow, value); }


        //Code_by JD
        public RelayCommand<CancelEventArgs> OnClosing { get; set; }


        public UserControl UCMainView
        {
            get => uCMainView; set
            {
                uCMainView = value;
                RaisePropertyChanged();
            }
        }

        #endregion

    }
}