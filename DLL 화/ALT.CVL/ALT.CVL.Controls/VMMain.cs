using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ALT.CVL.Controls
{
    public class VMMain : ViewModelBase
    {
        private ScrollViewer scrollViewer;
        private Image imgControl;
        //private Image imgControl;
        private double scaleX;
        private double scaleY;
        Point? lastCenterPositionOnTarget;
        Point? lastMousePositionOnTarget;
        Point? lastDragPoint;
        public RelayCommand<RoutedEventArgs> CmdImageChanged { get; set; }
        public RelayCommand<ScrollViewer> CmdInitial { get; set; }

        public ICommand CmdMouseLeftButtonUp { get; set; }
        public ICommand CmdMouseWheel { get; set; }
        public ICommand CmdMouseLeftButtonDown { get; set; }
        public ICommand CmdMouseMove { get; set; }
        public ICommand CmdScrollChanged { get; set; }

        public double ScaleX
        {
            get { return scaleX; }
            set
            {
                if (value >= 1)
                {
                    Set(ref scaleX, value);
                }
            }
        }
        public double ScaleY
        {
            get { return scaleY; }
            set
            {
                if (value >= 1)
                {
                    Set(ref scaleY, value);
                }
            }
        }

        public VMMain()
        {
            ScaleX = 1.0;
            ScaleY = 1.0;

            
            //CmdScrollChanged = new RelayCommand<ScrollChangedEventArgs>(OnScrollViewerScrollChanged);
            CmdMouseLeftButtonUp = new RelayCommand<MouseButtonEventArgs>(OnMouseLeftButtonUp);
            CmdMouseLeftButtonDown = new RelayCommand<MouseButtonEventArgs>(OnMouseLeftButtonDown);
            CmdMouseWheel = new RelayCommand<MouseWheelEventArgs>(OnPreviewMouseWheel);
            CmdMouseMove = new RelayCommand<MouseEventArgs>(OnMouseMove);
            CmdImageChanged = new RelayCommand<RoutedEventArgs>(ImageChanged);
            CmdInitial = new RelayCommand<ScrollViewer>(i => Initial(i));

        }
        private void Initial(ScrollViewer scrollViewer)
        {
            this.scrollViewer = scrollViewer;
            var grid = scrollViewer.Content as Grid;
            imgControl = grid.Children[0] as Image;
            //imgControl = grid.Children[0] as Image;
        }

        private void ImageChanged(RoutedEventArgs e)
        {
            /*var imageSource = e.Source as Image;
            if (imageSource.Source != null)
            {
                var originSource = imageSource.Source;
                grid.Width = originSource.Width;
                grid.Height = originSource.Height;
            }*/
        }

        void OnMouseMove(MouseEventArgs e)
        {
            var mainView = scrollViewer.Parent as DisplayWPF;
            Grid grid = scrollViewer.Content as Grid;
            Point posNow = e.GetPosition(scrollViewer);
            try
            {
                if (lastDragPoint.HasValue)
                {
                    double dX = posNow.X - lastDragPoint.Value.X;
                    double dY = posNow.Y - lastDragPoint.Value.Y;

                    lastDragPoint = posNow;

                    scrollViewer.ScrollToHorizontalOffset(scrollViewer.HorizontalOffset - dX);
                    scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - dY);
                }
                else
                {
                    if (imgControl != null && imgControl.Source != null)
                    {
                        Point posImgNow = e.GetPosition(imgControl);
                        BitmapSource bmpImage = (BitmapSource)imgControl.Source;
                        Point pixelPosition = new Point(posImgNow.X * bmpImage.PixelWidth / imgControl.ActualWidth, posImgNow.Y*bmpImage.PixelHeight/imgControl.ActualHeight);
                        //RenderTargetBitmap targetBitmap = new RenderTargetBitmap((int)scrollViewer.DesiredSize.Width, (int)scrollViewer.DesiredSize.Height, 96, 96, PixelFormats.Default);
                        //CroppedBitmap cb = new CroppedBitmap(targetBitmap, new Int32Rect((int)posNow.X, (int)posNow.Y, 1, 1));
                        CroppedBitmap cb = new CroppedBitmap(imgControl.Source as BitmapSource, new Int32Rect((int)pixelPosition.X, (int)pixelPosition.Y, 1, 1));
                        var targetPixel = new byte[4];
                        try
                        {
                            cb.CopyPixels(targetPixel, 4, 0);
                            mainView.ColorFromCursor = Color.FromRgb(targetPixel[2], targetPixel[1], targetPixel[0]);
                            mainView.CursorPosition = pixelPosition;
                            mainView.ImageSourceSize = new Size(imgControl.Source.Width, imgControl.Source.Height);
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
            }
            catch(Exception ex)
            { }
        }

        void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            var mousePos = e.GetPosition(scrollViewer);
            if (mousePos.X <= scrollViewer.ViewportWidth && mousePos.Y < scrollViewer.ViewportHeight) //make sure we still can use the scrollbars
            {
                scrollViewer.Cursor = Cursors.SizeAll;
                lastDragPoint = mousePos;
                Mouse.Capture(scrollViewer);
            }
        }

        void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            /*lastMousePositionOnTarget = e.GetPosition(imgControl);

            if (e.Delta > 0)
            {
                ScaleX += 1;
                ScaleY += 1;
            }
            if (e.Delta < 0)
            {
                ScaleX -= 1;
                ScaleY -= 1;
            }

            var centerOfViewport = new Point(scrollViewer.ViewportWidth / 2, scrollViewer.ViewportHeight / 2);
            lastCenterPositionOnTarget = scrollViewer.TranslatePoint(centerOfViewport, imgControl);

            e.Handled = true;*/
            var v = ElementFindName(e.Source as FrameworkElement, "scrollViewer");
            Grid imageGrid = v.FindName("ImageGrid") as Grid;
            var scrollViewer = imageGrid.Parent as ScrollViewer;
            double width = Mouse.GetPosition(imageGrid).X;
            double height = Mouse.GetPosition(imageGrid).Y;
            if (e.Delta == 120)
            {
                ScaleX += 1;
                ScaleY += 1;
                width += scrollViewer.HorizontalOffset;
                height += scrollViewer.VerticalOffset;
                scrollViewer.ScrollToHorizontalOffset(width);
                scrollViewer.ScrollToVerticalOffset(height);
            }
            else
            {
                ScaleX -= 1;
                ScaleY -= 1;
                width = scrollViewer.HorizontalOffset - width;
                height = scrollViewer.VerticalOffset - height;
                scrollViewer.ScrollToHorizontalOffset(width);
                scrollViewer.ScrollToVerticalOffset(height);
            }
        }

        private FrameworkElement ElementFindName(FrameworkElement originalSource, string v)
        {
            FrameworkElement element = new FrameworkElement();
            try
            {
                if (originalSource.Name == v)
                {
                    element = originalSource;
                }
                else
                {
                    element = ElementFindName(originalSource.Parent as FrameworkElement, v);
                }
            }
            catch (Exception)
            {
            }
            return element;
        }

        void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            scrollViewer.Cursor = Cursors.Arrow;
            scrollViewer.ReleaseMouseCapture();
            lastDragPoint = null;
        }

        /*void OnScrollViewerScrollChanged(ScrollChangedEventArgs e)
        {
            if (e.ExtentHeightChange != 0 || e.ExtentWidthChange != 0)
            {
                Point? targetBefore = null;
                Point? targetNow = null;

                if (!lastMousePositionOnTarget.HasValue)
                {
                    if (lastCenterPositionOnTarget.HasValue)
                    {
                        var centerOfViewport = new Point(scrollViewer.ViewportWidth / 2, scrollViewer.ViewportHeight / 2);
                        Point centerOfTargetNow = scrollViewer.TranslatePoint(centerOfViewport, imgControl);

                        targetBefore = lastCenterPositionOnTarget;
                        targetNow = centerOfTargetNow;
                    }
                }
                else
                {
                    targetBefore = lastMousePositionOnTarget;
                    targetNow = Mouse.GetPosition(imgControl);

                    lastMousePositionOnTarget = null;
                }

                if (targetBefore.HasValue)
                {
                    double dXInTargetPixels = targetNow.Value.X - targetBefore.Value.X;
                    double dYInTargetPixels = targetNow.Value.Y - targetBefore.Value.Y;

                    double multiplicatorX = e.ExtentWidth / imgControl.Width;
                    double multiplicatorY = e.ExtentHeight / imgControl.Height;

                    double newOffsetX = scrollViewer.HorizontalOffset - dXInTargetPixels * multiplicatorX;
                    double newOffsetY = scrollViewer.VerticalOffset - dYInTargetPixels * multiplicatorY;

                    if (double.IsNaN(newOffsetX) || double.IsNaN(newOffsetY))
                    {
                        return;
                    }

                    scrollViewer.ScrollToHorizontalOffset(newOffsetX);
                    scrollViewer.ScrollToVerticalOffset(newOffsetY);
                }
            }
        }*/

    }
}
