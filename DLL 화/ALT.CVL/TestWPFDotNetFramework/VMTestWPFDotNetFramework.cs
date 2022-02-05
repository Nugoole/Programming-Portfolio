using Basler.Pylon;

using Cognex.VisionPro;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms.Integration;
using System.Windows.Threading;

namespace TestWPFDotNetFramework
{
    public class VMTestWPFDotNetFramework : ViewModelBase
    {
        private CogRecordDisplay display;
        private CogRecordDisplay display2;
        private CogRecordDisplay display3;
        private WindowsFormsHost wfHost;
        private WindowsFormsHost wfHost2;

        private WindowsFormsHost wfHost3;




        private object content;
        private object content2;

        public VMTestWPFDotNetFramework()
        {
            if (!IsInDesignMode)
            {
                display = new CogRecordDisplay();
                display2 = new CogRecordDisplay();
                display3 = new CogRecordDisplay();

                wfHost = new WindowsFormsHost();
                
                wfHost2 = new WindowsFormsHost();
                wfHost3 = new WindowsFormsHost();
                wfHost.Child = display;
                wfHost2.Child = display2;
                wfHost3.Child = display3;
                Content = wfHost;
                Content2 = wfHost2;
                converter = new PixelDataConverter();
                OnStart = new RelayCommand(OnStartClicked);
                OnStop = new RelayCommand(OnStopClicked);
                OnOpenWindow = new RelayCommand<Window>(OnOpenWindowClicked);
                cam = new BaslerCam();
                cam.OnGrabbed += Cam_OnGrabbed;
                Image = new CogImage8Grey(cam.Width, cam.Height);
                
                pxMem = Image.Get8GreyPixelMemory(CogImageDataModeConstants.Write, 0, 0, cam.Width, cam.Height);

                Dispatcher.CurrentDispatcher.ShutdownStarted += CurrentDispatcher_ShutdownStarted;
            }
        }

        private void OnOpenWindowClicked(Window mainWindow)
        {
            wfHost.Child.BackColor = System.Drawing.Color.Transparent;
            wfHost.Opacity = 0.5;
            wfHost2.Opacity = 0.5;
            Window window = new Window();
            window.Content = wfHost3;
            //mainWindow.AllowsTransparency = true;
            Opacity = 0.5;
            window.Show();
        }

        private void OnStopClicked()
        {
            cam.StopLive();
        }

        private void CurrentDispatcher_ShutdownStarted(object sender, EventArgs e)
        {
            cam?.Close();
        }

        private void Cam_OnGrabbed(object sender, Basler.Pylon.IGrabResult e)
        {
            //pxMem = Image.Get8GreyPixelMemory(CogImageDataModeConstants.Write, 0, 0, cam.Width, cam.Height);

            if (display.Image == null)
                display.Image = Image;
            if (display2.Image == null)
                display2.Image = Image;

            converter.Convert(pxMem.Scan0, converter.GetBufferSizeForConversion(e), e);
            //display.Update();
            //pxMem.Dispose();


            //if (count++ % 4 == 0)
            //    GC.Collect();
        }

        private void OnStartClicked()
        {
            cam.StartLive();
        }

        public object Content { get => content; set => Set(ref content, value); }
        public object Content2 { get => content2; set => Set(ref content2, value); }
        public bool AllowsTransparencyBool { get => allowsTransparencyBool; set => Set(ref allowsTransparencyBool, value); }
        public double Opacity { get => opacity; set => Set(ref opacity, value); }
        public RelayCommand OnStart { get; set; }
        public RelayCommand OnStop { get; set; }
        public RelayCommand<Window> OnOpenWindow { get; set; }

        private BaslerCam cam;
        private CogImage8Grey Image;
        private ICogImage8PixelMemory pxMem;
        private int count;
        private PixelDataConverter converter;
        private bool allowsTransparencyBool = false;
        private double opacity = 1.0f;
    }
}
