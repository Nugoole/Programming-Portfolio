using Cognex.VisionPro;

using GalaSoft.MvvmLight;

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms.Integration;

namespace TestWPF
{
    public class VMCogDisplayTest : ViewModelBase
    {
        private object content;
        private CogRecordDisplay display;
        private WindowsFormsHost wfHost;

        public object Content { get => content; set => Set(ref content, value); }

        public VMCogDisplayTest()
        {
            if (!IsInDesignMode)
            {
                display = new CogRecordDisplay();
                display.BeginInit();
                display.EndInit();
                wfHost = new WindowsFormsHost();
                wfHost.Child = display;
                Content = wfHost;
            }
        }


    }
}
