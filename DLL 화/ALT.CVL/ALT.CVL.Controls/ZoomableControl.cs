using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ALT.CVL.Controls
{
    public class ZoomableControl : System.Windows.Controls.UserControl, INotifyPropertyChanged
    {
        private Point origin;
        private Point start;
        private Rect viewport;
        private Rect viewBox;



        public Rect ViewPort
        {
            get => viewport; set
            {
                viewport = value;
                RaisePropertyChanged();
            }
        }

        public Rect ViewBox
        {
            get => viewBox; set
            {
                viewBox = value;
                RaisePropertyChanged();
            }
        }
        public ZoomableControl()
        {
            TransformGroup tfGroup = new TransformGroup();

            ScaleTransform scaletf = new ScaleTransform();

            TranslateTransform translatetf = new TranslateTransform();

            tfGroup.Children.Add(translatetf);
            tfGroup.Children.Add(scaletf);

            this.RenderTransform = tfGroup;

            MouseWheel += ZoomableControl_MouseWheel;

            ViewBox = new Rect(0, 0, 50, 50);
            
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void ZoomableControl_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {

            var transform = (ScaleTransform)((TransformGroup)RenderTransform).Children.First(x => x is ScaleTransform);

            double zoom = e.Delta > 0 ? 0.2 : -0.2;
            transform.ScaleX += zoom;
            transform.ScaleY += zoom;
        }


        protected void RaisePropertyChanged([CallerMemberName] string prop = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
