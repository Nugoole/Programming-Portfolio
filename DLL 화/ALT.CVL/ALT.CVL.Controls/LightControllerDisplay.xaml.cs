using ALT.CVL.Common.Interface;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ALT.CVL.Controls
{
    /// <summary>
    /// LightControllerDisplay.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LightControllerDisplay : UserControl, INotifyPropertyChanged
    {
        public ILightController Controller
        {
            get { return (ILightController)GetValue(ControllerProperty); }
            set
            {
                SetValue(ControllerProperty, value);
                RaisePropertyChanged();
            }
        }

        // Using a DependencyProperty as the backing store for Controller.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ControllerProperty =
            DependencyProperty.Register("Controller", typeof(ILightController), typeof(LightControllerDisplay), new FrameworkPropertyMetadata(OnControllerChanged));

        private static void OnControllerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as LightControllerDisplay;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public LightControllerDisplay()
        {
            InitializeComponent();
            mainGrid.DataContext = this;
            Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
        }



        private void Dispatcher_ShutdownStarted(object sender, EventArgs e)
        {
            Controller?.Disconnect();
        }

        private void RaisePropertyChanged([CallerMemberName] string prop = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
