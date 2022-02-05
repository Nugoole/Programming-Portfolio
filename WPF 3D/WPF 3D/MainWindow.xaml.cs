using Sick.GenIStream;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPF_3D
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string imageFilePath = @"C:\Users\MSI\Desktop\Images\calibration zig arrow calibrated.dat";
        private Sick.GenIStream.Image image;
        private bool isLeftPressed = false;
        private Point3D positionBuffer;
        private Point pointBuffer;
        private ushort width;
        private ushort height;
        public MainWindow()
        {
            InitializeComponent();

            
        }

        
    }
}
