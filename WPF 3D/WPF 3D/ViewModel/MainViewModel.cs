using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using Sick.GenIStream;

using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;


using Expression = System.Linq.Expressions.Expression;

namespace WPF_3D.ViewModel
{
    public static class New<T> where T : new()
    {
        public static Func<T> Instance = Expression.Lambda<Func<T>>(Expression.New(typeof(T))).Compile();
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
    public class MainViewModel : ViewModelBase
    {
        private const string imageFilePath = @"C:\Users\MSI\Desktop\Images\calibration zig arrow calibrated.dat";
        private Sick.GenIStream.Image image;
        private ushort width;
        private ushort height;
        private Point3DCollection positions;
        private Int32Collection indices;
        private Point3D cameraPosition;
        private Vector3D cameraLookDirection = new Vector3D(0, 0, -1);
        private bool isLeftPressed;
        private Point3D positionBuffer;
        private Point pointBuffer;
        private double angleBufferX;
        private double angleBufferY;
        private double fov = 60;
        private Vector positionDelta;
        private double angleX;
        private double angleY;
        private double offsetX;
        private double offsetY;
        private bool isRightPressed;
        private double offsetXBuffer;
        private double offsetYBuffer;
        private double maxZ;
        private double offSetZ;

        public double OffsetX { get => offsetX; set => Set(ref offsetX, value); }
        public double OffsetY { get => offsetY; set => Set(ref offsetY, value); }
        public double OffSetZ { get => offSetZ; set => Set(ref offSetZ, value); }
        public double AngleX { get => angleX; set => Set(ref angleX, value); }
        public double AngleY { get => angleY; set => Set(ref angleY, value); }
        public Vector PositionDelta { get => positionDelta; set => Set(ref positionDelta, value); }
        public double FOV { get => fov; set => Set(ref fov, value); }
        public Vector3D CameraLookDirection { get => cameraLookDirection; set => Set(ref cameraLookDirection, value); }
        public Point3DCollection Positions { get => positions; set => Set(ref positions, value); }
        public Int32Collection Indices { get => indices; set => Set(ref indices, value); }
        public Point3D CameraPosition { get => cameraPosition; set => Set(ref cameraPosition, value); }
        public RelayCommand<MouseButtonEventArgs> MouseDown { get; set; }
        public RelayCommand<MouseButtonEventArgs> MouseUp { get; set; }
        public RelayCommand<MouseEventArgs> MouseMove { get; set; }
        public RelayCommand<MouseWheelEventArgs> MouseWheel { get; set; }
        public RelayCommand ResetView { get; set; }
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            InitializeCommands();
            LoadImage();
            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}

            RenderPoint();
        }

        private void InitializeCommands()
        {
            MouseDown = new RelayCommand<MouseButtonEventArgs>(OnMouseDown);
            MouseUp = new RelayCommand<MouseButtonEventArgs>(OnMouseUp);
            MouseMove = new RelayCommand<MouseEventArgs>(OnMouseMove);
            MouseWheel = new RelayCommand<MouseWheelEventArgs>(OnMouseWheel);
            ResetView = new RelayCommand(OnResetView);
        }

        private void OnResetView()
        {
            CameraPosition = new Point3D(width / 2, height / 2, maxZ * 2);
            FOV = 60;
            AngleX = 0;
            AngleY = 0;
        }

        private void OnMouseWheel(MouseWheelEventArgs obj)
        {
            FOV -= obj.Delta / 100;
        }


        private void OnMouseMove(MouseEventArgs obj)
        {
            PositionDelta = obj.GetPosition(null) - pointBuffer;

            if (isLeftPressed)
            {

                AngleX = angleBufferX + PositionDelta.X / 500 * 45;
                AngleY = angleBufferY + PositionDelta.Y / 250 * 45;
            }

            if (isRightPressed)
            {
                var newPosX = positionBuffer.X - PositionDelta.X;
                var newPosY = positionBuffer.Y + PositionDelta.Y;

                CameraPosition = new Point3D(newPosX, newPosY, positionBuffer.Z);

            }
        }

        private void OnMouseUp(MouseButtonEventArgs obj)
        {
            if (obj.LeftButton == MouseButtonState.Released)
                isLeftPressed = false;

            if (obj.RightButton == MouseButtonState.Released)
                isRightPressed = false;
        }

        private void OnMouseDown(MouseButtonEventArgs obj)
        {
            if (obj.LeftButton == MouseButtonState.Pressed)
            {
                isLeftPressed = true;
                positionBuffer = CameraPosition;
                pointBuffer = obj.GetPosition(null);
                angleBufferX = AngleX;
                angleBufferY = AngleY;
            }

            if (obj.RightButton == MouseButtonState.Pressed)
            {
                isRightPressed = true;
                positionBuffer = CameraPosition;
                pointBuffer = obj.GetPosition(null);
                offsetXBuffer = offsetX;
                offsetYBuffer = offsetY;
            }
        }

        private void LoadImage()
        {
            Sick.GenIStream.IFrame frame = IFrame.Load(imageFilePath);
            image = frame.GetRange().CreateCopiedImage();
        }

        private void RenderPoint()
        {
            var pixelDataAddr = image.GetData();
            var loofCount = image.GetDataSize() / Marshal.SizeOf<UInt16>();
            Point3DCollection positions = new Point3DCollection();

            Int32Collection indices = new Int32Collection();
            width = image.GetWidth();
            height = image.GetHeight();
            for (int i = 0; i < loofCount; i++)
            {
                var Z = Marshal.ReadInt16(pixelDataAddr, i * Marshal.SizeOf<ushort>());

                positions.Add(new Point3D(i % width, i / width, Z));
            }

            int kernelXSize = 4;
            int kernelYSize = 4;


            /////////////////////////
            ///     0 ----------------------- 1
            ///     | *                       |
            ///     |   *                     |
            ///     |     *                   |
            ///     |       *                 |
            ///     |         *               |
            ///     |           *             |
            ///     |             *           |
            ///     |               *         |
            ///     |                 *       |
            ///     |                   *     |
            ///     |                     *   |
            ///     |                       * |
            ///     2 ------------------------3
            /// 
            /// -> 0 2 3 , 0 3 1
            ///
            /// 
            /// 
            /// Mesh Simplification
            /////////////////////////
            ///     0 ----------1------------ 2     0 ----------------------- 2
            ///     | *         |             |     | *                       |
            ///     |   *       |             |     |   *                     |
            ///     |     *     |             |     |     *                   |
            ///     |       *   |             |     |       *                 |
            ///     |         * |             |     |         *               |
            ///     3-----------4-------------5  ==>|           *             |
            ///     |           | *           |     |             *           |
            ///     |           |   *         |     |               *         |
            ///     |           |     *       |     |                 *       |
            ///     |           |       *     |     |                   *     |
            ///     |           |         *   |     |                     *   |
            ///     |           |           * |     |                       * |
            ///     6 ----------7-------------8     6 ------------------------8
            /// 
            /// -> 0 6 8 , 0 8 2
            ///



            for (int h = 0; h < height - kernelYSize; h+=kernelYSize)
            {
                for (int w = 0; w < width - kernelXSize; w+=kernelXSize)
                {
                    indices.Add(w + (h + kernelYSize / 2) * width + (kernelXSize / 2)); //4
                    indices.Add(w + h * width); //0
                    indices.Add(w + h * width + kernelXSize); // 2

                    indices.Add(w + (h + kernelYSize / 2) * width + (kernelXSize / 2)); //4
                    indices.Add(w + h * width + kernelXSize); // 2
                    indices.Add(w + (h + kernelYSize) * width + kernelXSize); // 8

                    indices.Add(w + (h + kernelYSize / 2) * width + (kernelXSize / 2)); //4
                    indices.Add(w + (h + kernelYSize) * width + kernelXSize); // 8
                    indices.Add(w + (h + kernelYSize) * width); // 6

                    indices.Add(w + (h + kernelYSize / 2) * width + (kernelXSize / 2)); //4
                    indices.Add(w + (h + kernelYSize) * width); // 6
                    indices.Add(w + h * width); //0
                }
            }

            Positions = positions;
            Indices = indices;

            var maxpointZ = positions.Select(x => x.Z).Max();

            maxZ = maxpointZ;

            CameraPosition = new Point3D(width / 2, height / 2, maxpointZ * 2);
            //Marshal.ReadInt32()
        }
    }
}