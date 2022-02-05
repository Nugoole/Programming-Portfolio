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
    public partial class PulseDisplay : UserControl, INotifyPropertyChanged
    {
        private double lowerCutScale = 0.2;
        private double upperCutScale = 0.9;
        private bool isClicked;
        private Point clickedPosition;
        private double currentValueBuffer;
        private PointCollection linePoints;

        public event PropertyChangedEventHandler PropertyChanged;




        //public Point BottomFirst => new Point(0, grid.ActualHeight);
        //public Point BottomSecond => new Point(grid.ActualWidth * lowerCutScale, grid.ActualHeight);
        //public Point BottomThird => new Point(grid.ActualWidth * (currentValueBuffer / MaxValue * (upperCutScale - lowerCutScale) + lowerCutScale), grid.ActualHeight);
        //public Point BottomFourth => new Point(grid.ActualWidth, grid.ActualHeight);
        //public Point TopFirst => new Point(grid.ActualWidth * lowerCutScale, 0);
        //public Point TopSecond => new Point(grid.ActualWidth * (currentValueBuffer / MaxValue * (upperCutScale - lowerCutScale) + lowerCutScale), 0);



        //DP - 선 색상
        public Brush LineColor
        {
            get { return (Brush)GetValue(LineColorProperty); }
            set { SetValue(LineColorProperty, value); }
        }


        public double MaxValue
        {
            get { return (double)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }


        public double CurrentValue
        {
            get { return (double)GetValue(CurrentValueProperty); }
            set
            {
                if (value > MaxValue)
                    value = MaxValue;

                if (value < 0)
                    value = 0;

                SetValue(CurrentValueProperty, value);
                UpdatePoints();
            }
        }

        // Using a DependencyProperty as the backing store for CurrentValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentValueProperty =
            DependencyProperty.Register("CurrentValue", typeof(double), typeof(PulseDisplay), new PropertyMetadata(0.0, OnCurrentValueChanged));

        private static void OnCurrentValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as PulseDisplay;
            control.CurrentValue = (double)e.NewValue;
            control.currentValueBuffer = (double)e.NewValue;
        }








        //PolyLine의 점들
        public PointCollection LinePoints { get => (PointCollection)(linePoints.GetCurrentValueAsFrozen()); set => linePoints = value; }

        public PulseDisplay()
        {
            InitializeComponent();
            polyLine.DataContext = this;

            LinePoints = new PointCollection();
            linePoints.Add(new Point(0, 0));
            linePoints.Add(new Point(0, 0));
            linePoints.Add(new Point(0, 0));
            linePoints.Add(new Point(0, 0));
            linePoints.Add(new Point(0, 0));
            linePoints.Add(new Point(0, 0));


            this.Loaded += LightControllerDisplay_Loaded;
            this.MouseLeftButtonDown += LightControllerDisplay_MouseLeftButtonDown;
            this.MouseLeftButtonUp += LightControllerDisplay_MouseLeftButtonUp;
            this.MouseMove += LightControllerDisplay_MouseMove;
            this.SizeChanged += LightControllerDisplay_SizeChanged;
            this.MouseLeave += LightControllerDisplay_MouseLeave;

            this.PreviewMouseLeftButtonDown += LightControllerDisplay_PreviewMouseLeftButtonDown;
        }

        private void LightControllerDisplay_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void LightControllerDisplay_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
                UpdatePoints();
        }

        private void LightControllerDisplay_MouseLeave(object sender, MouseEventArgs e)
        {

        }





        //드래그 구현
        private void LightControllerDisplay_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsMouseOver)
                Cursor = Cursors.SizeWE;



            if (isClicked && e.LeftButton == MouseButtonState.Pressed)
            {
                //클릭 점과의 X 변화량
                var deltaX = (clickedPosition - e.GetPosition(this)).X * MaxValue / (grid.ActualWidth * (upperCutScale - lowerCutScale));

                currentValueBuffer = CurrentValue - deltaX;
                var temp = currentValueBuffer / MaxValue;

                if ((deltaX > 0 && temp < 0) || (deltaX < 0 && temp > 1))
                {
                    if (temp < 0)
                        currentValueBuffer = 0;
                    else if (temp > 1)
                        currentValueBuffer = MaxValue;

                    return;
                }



                UpdatePoints();
            }
        }


        //마우스 클릭 해제 시
        private void LightControllerDisplay_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

            clickedPosition = new Point(0, 0);
            CurrentValue = currentValueBuffer;
            isClicked = false;
        }




        //마우스 클릭 시 
        private void LightControllerDisplay_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            clickedPosition = e.GetPosition(this);

            currentValueBuffer = CurrentValue;
            isClicked = true;
        }


        //컨트롤 로드 후
        private void LightControllerDisplay_Loaded(object sender, RoutedEventArgs e)
        {
            UpdatePoints();
        }

        private void UpdatePoints()
        {

            var bottomFirst = linePoints[0];
            bottomFirst.Y = grid.ActualHeight;
            linePoints[0] = bottomFirst;

            var bottomSecond = linePoints[1];
            bottomSecond.X = grid.ActualWidth * lowerCutScale;
            bottomSecond.Y = grid.ActualHeight;
            linePoints[1] = bottomSecond;

            var topFirst = linePoints[2];
            topFirst.X = grid.ActualWidth * lowerCutScale;
            linePoints[2] = topFirst;

            var topSecond = linePoints[3];
            topSecond.X = grid.ActualWidth * (currentValueBuffer / MaxValue * (upperCutScale - lowerCutScale) + lowerCutScale);
            linePoints[3] = topSecond;

            var bottomThird = linePoints[4];
            bottomThird.X = grid.ActualWidth * (currentValueBuffer / MaxValue * (upperCutScale - lowerCutScale) + lowerCutScale);
            bottomThird.Y = grid.ActualHeight;
            linePoints[4] = bottomThird;

            var bottomFourth = linePoints[5];
            bottomFourth.X = grid.ActualWidth;
            bottomFourth.Y = grid.ActualHeight;
            linePoints[5] = bottomFourth;





            RaisePropertyChanged(nameof(LinePoints));
        }

        private void RaisePropertyChanged([CallerMemberName] string prop = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        // Using a DependencyProperty as the backing store for LineColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LineColorProperty =
            DependencyProperty.Register("LineColor", typeof(Brush), typeof(PulseDisplay), new PropertyMetadata(OnLineColorChanged));

        private static void OnLineColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as PulseDisplay;
            control.polyLine.Stroke = e.NewValue as Brush;
        }

        // Using a DependencyProperty as the backing store for MaxValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(double), typeof(PulseDisplay), new PropertyMetadata(0.0));
    }
}
