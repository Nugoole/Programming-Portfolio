

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
using System.Windows.Shapes;

using Cognex.VisionPro;
using Cognex.VisionPro.Display;
using Cognex.VisionPro.Implementation;
using Cognex.VisionPro3D;


namespace Viewer
{
    /// <summary>
    /// CrossSectionView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CrossSectionView : Window
    {
        private ICogVisionData currentVisionData;
        private Cog3DRangeImageCrossSectionProfileParams profileCreator = new Cog3DRangeImageCrossSectionProfileParams();
        private CogRecordDisplay cogDisplay = new CogRecordDisplay();
        private Cog3DBox crossSectionBox;
        private double visionDataWidth;
        private double visionDataHeight;
        private double visionDataZ;


        [DllImport("kernel32.dll", SetLastError = false)]
        static extern void CopyMemory(IntPtr Destination, IntPtr Source, uint Length);

        public int SectionSize
        {
            get { return (int)GetValue(SectionSizeProperty); }
            set { SetValue(SectionSizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SectionSize.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SectionSizeProperty =
            DependencyProperty.Register("SectionSize", typeof(int), typeof(CrossSectionView), new PropertyMetadata(1, OnSectionSizeChanged));

        private static void OnSectionSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as CrossSectionView;
            if ((int)e.NewValue <= 0)
                return;

            control.crossSectionBox.Size = new Cog3DVect3(control.crossSectionBox.Size.X, (int)e.NewValue, control.crossSectionBox.Size.Z);

            control.YaxisSlider.Maximum = control.visionDataHeight - (int)e.NewValue;
            if (control.YaxisSlider.Value > control.YaxisSlider.Maximum)
                control.YaxisSlider.Value = control.YaxisSlider.Maximum;
        }

        public CrossSectionView(ICogVisionData visionData)
        {
            InitializeComponent();

            wfHost.Child = cogDisplay;
            currentVisionData = visionData;
            Cog3DViewer.SetRangeImage(viewer3D, visionData);
            
            if(visionData is CogImage16Range rangeImage)
            {
                
                using (var memory = rangeImage.GetPixelData().Get16GreyPixelMemory(CogImageDataModeConstants.Read, 0, 0, rangeImage.Width, rangeImage.Height))
                {
                    var values = GetValues(memory.Scan0, rangeImage.Width * rangeImage.Height);
                    var maxZValue = values.Max();
                    var box3D = new Cog3DBox(new Cog3DVect3(rangeImage.Width, 10, maxZValue), new Cog3DTransformRigid(new Cog3DTransformRotation(), new Cog3DVect3(0, 0, 0)), Cog3DShapeStateConstants.Surface);
                    crossSectionBox = box3D;
                    var box3DGraphic = new Cog3DBoxGraphic(box3D);
                    viewer3D.Viewer.Add(box3DGraphic, viewer3D.Viewer.GetVisionDataGraphic(0));
                    //crossSectionBox.Size = new Cog3DVect3(crossSectionBox.Size.X, 10, crossSectionBox.Size.Z);
                    visionDataWidth = rangeImage.Width;
                    visionDataHeight = rangeImage.Height;
                    visionDataZ = maxZValue;
                }

                YaxisSlider.Maximum = visionDataHeight - crossSectionBox.Size.Y;
            }
        }

        private ushort[] GetValues(IntPtr scan0, int length)
        {
            ushort[] destination = new ushort[length];
            var gch = GCHandle.Alloc(destination, GCHandleType.Pinned);
            try
            {
                var targetPtr = Marshal.UnsafeAddrOfPinnedArrayElement(destination, 0);
                var bytesToCopy = Marshal.SizeOf(typeof(ushort)) * length;

                CopyMemory(targetPtr, scan0, (uint)bytesToCopy);
            }
            finally
            {
                gch.Free();
            }

            return destination;
        }

        private void YaxisSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            crossSectionBox.GetOriginVertexXVectorYVectorZ(out _, out Cog3DVect3 xVector, out Cog3DVect3 yVector, out double z);
            crossSectionBox.SetOriginVertexXVectorYVectorZ(new Cog3DVect3(0, e.NewValue, 0), xVector, yVector, z);
            var surfaceRect3D = crossSectionBox.GetSurfaces()[1];
            var profileRegion = new CogRectangleAffine();
            profileRegion.SetOriginLengthsRotationSkew(0, e.NewValue, visionDataWidth, crossSectionBox.Size.Y, 0, 0);
            profileCreator.Region = profileRegion;
            var outputProfile = profileCreator.Execute((ICogImage)currentVisionData);

            var record = new CogRecord("Profile", typeof(ICogImage), CogRecordUsageConstants.Diagnostic, true, outputProfile.ImageUsedForProfileDisplay(), "");

            record.SubRecords.Add(new CogRecord("Graphic", typeof(CogGraphicCollection), CogRecordUsageConstants.Diagnostic, true, outputProfile.BuildProfileGraphics(false), ""));
            cogDisplay.Record = record;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(!cogDisplay.IsDisposed)
                cogDisplay.Dispose();

            crossSectionBox.Dispose();
            profileCreator.Dispose();

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;

            if(int.TryParse(textBox.Text, out int result))
            {
                SectionSize = result;
            }
        }
    }
}
