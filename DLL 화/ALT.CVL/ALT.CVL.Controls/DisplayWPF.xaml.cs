using System;
using System.Collections.Generic;
using System.Linq;
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
    /// DisplayWPF.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DisplayWPF : UserControl
    {
        public DisplayWPF()
        {
            InitializeComponent();
        }
        public ImageSource ImageFilePath
        {
            get { return (ImageSource)GetValue(ImageFilePathProperty); }
            set { SetValue(ImageFilePathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ImageFilePath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageFilePathProperty =
            DependencyProperty.Register("ImageFilePath", typeof(ImageSource), typeof(DisplayWPF), new PropertyMetadata(null));

        public Color ColorFromCursor
        {
            get { return (Color)GetValue(ColorFromCursorProperty); }
            set
            {
                SetValue(ColorFromCursorProperty, value);
                RGBValueChangedEventArgs args = new RGBValueChangedEventArgs(RGBValueChanged, value);
                RaiseEvent(args);
            }
        }

        // Using a DependencyProperty as the backing store for ColorFromCursor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColorFromCursorProperty =
            DependencyProperty.Register("ColorFromCursor", typeof(Color), typeof(DisplayWPF), new PropertyMetadata(null));

        public static readonly RoutedEvent RGBValueChanged =
            EventManager.RegisterRoutedEvent("RGBChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DisplayWPF));

        public event RoutedEventHandler RGBChanged
        {
            add { AddHandler(RGBValueChanged, value); }
            remove { RemoveHandler(RGBValueChanged, value); }
        }

        public static readonly RoutedEvent CursorPointChanged =
           EventManager.RegisterRoutedEvent("CursorChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DisplayWPF));

        public event RoutedEventHandler CursorChanged
        {
            add { AddHandler(CursorPointChanged, value); }
            remove { RemoveHandler(CursorPointChanged, value); }
        }

        public static readonly RoutedEvent ImageSizeChanged =
   EventManager.RegisterRoutedEvent("ImageChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DisplayWPF));

        public event RoutedEventHandler ImageChanged
        {
            add { AddHandler(ImageSizeChanged, value); }
            remove { RemoveHandler(ImageSizeChanged, value); }
        }

        public Point CursorPosition
        {
            get { return (Point)GetValue(CursorPositionProperty); }
            set
            {
                SetValue(CursorPositionProperty, value);
                MousePositionChangedEventArgs args = new MousePositionChangedEventArgs(CursorPointChanged, value);
                RaiseEvent(args);
            }
        }

        // Using a DependencyProperty as the backing store for CursorPosition.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CursorPositionProperty =
            DependencyProperty.Register("CursorPosition", typeof(Point), typeof(DisplayWPF), new PropertyMetadata(null));




        public Size ImageSourceSize
        {
            get { return (Size)GetValue(ImageSizeProperty); }
            set
            {
                SetValue(ImageSizeProperty, value);
                ImageSizeChangedEventArgs args = new ImageSizeChangedEventArgs(ImageSizeChanged, value);
                RaiseEvent(args);
            }
        }

        // Using a DependencyProperty as the backing store for ImageSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageSizeProperty =
            DependencyProperty.Register("ImageSize", typeof(Size), typeof(DisplayWPF), new PropertyMetadata(null));

    }

    public class RGBValueChangedEventArgs : RoutedEventArgs
    {
        private Color rgbColor;

        public Color RGBColor
        {
            get { return rgbColor; }
            set { rgbColor = value; }
        }


        public RGBValueChangedEventArgs(RoutedEvent routedEvent, Color color) : base(routedEvent)
        {
            rgbColor = color;
        }
    }

    public class MousePositionChangedEventArgs : RoutedEventArgs
    {
        private Point cursorPoint;

        public Point CursorPoint
        {
            get { return cursorPoint; }
            set { cursorPoint = value; }
        }

        public MousePositionChangedEventArgs(RoutedEvent routedEvent, Point point) : base(routedEvent)
        {
            cursorPoint = point;
        }
    }

    public class ImageSizeChangedEventArgs : RoutedEventArgs
    {
        private Size imageSourceSize;

        public Size ImageSourceSize { get => imageSourceSize; set => imageSourceSize = value; }
        public ImageSizeChangedEventArgs(RoutedEvent routedEvent, Size size) : base(routedEvent)
        {
            imageSourceSize = size;
        }
    }
}
