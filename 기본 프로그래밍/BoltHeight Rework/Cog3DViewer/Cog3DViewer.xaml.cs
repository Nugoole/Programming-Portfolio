using Cognex.VisionPro;
using Cognex.VisionPro.Exceptions;
using Cognex.VisionPro.ImageProcessing;
using Cognex.VisionPro3D;

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Viewer
{
    /// <summary>
    /// Cog3DViewer.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Cog3DViewer : UserControl
    {
        private static CogCopyRegionTool splitter = new CogCopyRegionTool();
        private static CogRectangle splitRegion = new CogRectangle();
        private static CogVisionDataContainer container = new CogVisionDataContainer();





        public static bool GetEnableCrossSectionWindowButton(DependencyObject obj)
        {
            return (bool)obj.GetValue(EnableCrossSectionWindowButtonProperty);
        }

        public static void SetEnableCrossSectionWindowButton(DependencyObject obj, bool value)
        {
            obj.SetValue(EnableCrossSectionWindowButtonProperty, value);
        }

        // Using a DependencyProperty as the backing store for EnableCrossSectionWindowButton.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableCrossSectionWindowButtonProperty =
            DependencyProperty.RegisterAttached("EnableCrossSectionWindowButton", typeof(bool), typeof(Cog3DViewer), new PropertyMetadata(false, OnEnableCrossSectionWindowButtonChanged));

        private static void OnEnableCrossSectionWindowButtonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as Cog3DViewer;

            if((bool)e.NewValue == true)
            {
                control.crossSectionWindowButton.Visibility = Visibility.Visible;
            }
            else
            {
                control.crossSectionWindowButton.Visibility = Visibility.Hidden;
            }
        }

        public static readonly DependencyProperty RangeWithGreyProperty = DependencyProperty.RegisterAttached("RangeWithGrey", typeof(bool), typeof(Cog3DViewer), new PropertyMetadata(default(bool), OnRangeWithGreyChanged));

        private static void OnRangeWithGreyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as Cog3DViewer;

            //do Something

        }

        public static void SetRangeWithGrey(UIElement element, bool property)
        {
            element.SetValue(RangeWithGreyProperty, property);
        }

        public static bool GetRangeWithGrey(UIElement element)
        {
            return (bool)element.GetValue(RangeWithGreyProperty);
        }



        public static readonly DependencyProperty RangeImageProperty = DependencyProperty.RegisterAttached("RangeImage", typeof(ICogVisionData), typeof(Cog3DViewer), new PropertyMetadata(default(ICogVisionData), OnRangeImageChanged));

        private static void OnRangeImageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as Cog3DViewer;

            //do Something
            control.Viewer.Clear();





            var originalImage = (ICogImage)e.NewValue;
            var originalGreyImage = (originalImage as CogImage16Range).GetPixelData();

            if(GetRangeWithGrey(control))
            {
                if(originalImage.Width % 2 == 0)
                {
                    splitRegion.SelectedSpaceName = "#";
                    splitRegion.SetXYWidthHeight(0, 0, originalImage.Width / 2, originalImage.Height);
                    var RangeImage = splitter.RunParams.Execute(originalImage, splitRegion, null, out _, out _,out _);
                    splitRegion.SetXYWidthHeight(originalGreyImage.Width / 2, 0, originalGreyImage.Width / 2, originalGreyImage.Height);
                    var GreyImage = splitter.RunParams.Execute(originalGreyImage, splitRegion, null, out _, out _, out _);

                    container.Clear();
                    container.Add("range", RangeImage as CogImage16Range);
                    container.Add("grey", GreyImage as CogImage16Grey);


                    var graphic = Cog3DGraphicFactory.Create(container, false);
                    
                    control.Viewer.Add(graphic);
                    
                }
            }
            else
            {
                var graphic = new Cog3DRangeImageGraphic(originalImage as CogImage16Range);
                
                
                control.Viewer.Add(graphic);
            }
                

            control.Viewer.ResetView();
        }

        public static void SetRangeImage(UIElement element, ICogVisionData property)
        {
            element.SetValue(RangeImageProperty, property);
            OnRangeImageChanged(element, new DependencyPropertyChangedEventArgs(RangeImageProperty, null, property));
        }

        public static ICogVisionData GetRangeImage(UIElement element)
        {
            return (ICogVisionData)element.GetValue(RangeImageProperty);
        }



        public ICogVisionData RangeImage { get; set; }
        public bool RangeWithGrey { get; set; }
        public bool EnableCrossSectionWindowButton { get; set; }
        public Cog3DViewer()
        {
            InitializeComponent();

            statusBar.Display = Viewer;

            try
            {
                CogLicense.CheckLicense(CogLicenseConstants.Cog3DRangeTools);
            }
            catch (CogSecurityViolationException)
            {
                crossSectionWindowButton.IsEnabled = false;
            }
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            Window crossSectionWindow = new CrossSectionView(GetRangeImage(this));
            crossSectionWindow.Owner = Window.GetWindow(this);
            crossSectionWindow.Show();
            
        }
    }
}
