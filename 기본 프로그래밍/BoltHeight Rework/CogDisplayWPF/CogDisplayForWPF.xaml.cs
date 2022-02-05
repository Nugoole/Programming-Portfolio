using Cognex.VisionPro;
using Cognex.VisionPro.Display;
using Cognex.VisionPro.DSCameraSetup.Implementation.Internal;
using Cognex.VisionPro.Exceptions;
using Cognex.VisionPro.ImageProcessing;
using Cognex.VisionPro3D;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

namespace CogDisplayWPF
{
    /// <summary>
    /// CogDisplayForWPF.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CogDisplayForWPF : UserControl
    {
        public bool onDragging = false;

        #region Property-Image
        public static readonly DependencyProperty ImageProperty = DependencyProperty.RegisterAttached("Image", typeof(ICogImage), typeof(CogDisplayForWPF), new PropertyMetadata(default(ICogImage), OnImageChanged));

        private static void OnImageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as CogDisplayForWPF;
            if (!control.IsVisible)
                return;
            if (control.cogDisplayUC.InvokeRequired)
            {
                control.cogDisplayUC.Invoke((Action<DependencyObject, DependencyPropertyChangedEventArgs>)OnImageChanged, d, e);
                return;
            }

            //do Something
            if (!control.cogDisplayUC.IsDisposed)
            {
                control.cogDisplayUC.Image = e.NewValue as ICogImage;

                if (e.NewValue.GetType().Equals(typeof(CogImage16Range)))
                {
                    if (GetColorMapEnable(control))
                    {
                        control.cogDisplayUC.ColorMapPredefined = Cognex.VisionPro.Display.CogDisplayColorMapPredefinedConstants.Height;

                        CogHistogramTool tool = new CogHistogramTool();

                        var image = control.cogDisplayUC.Image as CogImage16Range;
                        tool.InputImage = image.GetPixelData();
                        tool.Run();

                        var average = tool.Result.Mean;
                        var stdDev = tool.Result.StandardDeviation;
                        var MaxVal = tool.Result.Maximum;
                        var MinVal = tool.Result.Minimum;

                        var lowerpixelPoint = -2.32 * stdDev + average;

                        var upperpixelPoint = 1.65 * stdDev + average;

                        var lowerPoint = -1*(lowerpixelPoint - MinVal) / (MaxVal - MinVal);

                        var upperPoint = (upperpixelPoint - MinVal) / (MaxVal - MinVal);
                         
                        if(!(lowerPoint.Equals(double.NaN) || upperPoint.Equals(double.NaN)))
                            control.cogDisplayUC.SetColorMapRoiLimits(lowerPoint, upperPoint);
                    }
                    else
                    {
                        control.cogDisplayUC.ColorMapPredefined = Cognex.VisionPro.Display.CogDisplayColorMapPredefinedConstants.None;
                    }
                }

                control.cogDisplayUC.Fit();

                if (GetSaveImageEnable(control))
                {
                    control.OnSaveImage?.Invoke();
                }
            }


        }

        public static void SetImage(UIElement element, ICogImage property)
        {
            element.SetValue(ImageProperty, property);
        }

        public static ICogImage GetImage(UIElement element)
        {
            return (ICogImage)element.GetValue(ImageProperty);
        }
        #endregion

        #region Property-Record
        public static readonly DependencyProperty RecordProperty = DependencyProperty.RegisterAttached("Record", typeof(ICogRecord), typeof(CogDisplayForWPF), new PropertyMetadata(default(ICogRecord), OnRecordChanged));

        private static void OnRecordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as CogDisplayForWPF;
            if (!control.IsVisible)
                return;
            //do Something
            control.cogDisplayUC.DrawingEnabled = false;
            control.cogDisplayUC.Record = e.NewValue as ICogRecord;

            control.cogDisplayUC.Fit(true);

            control.cogDisplayUC.DrawingEnabled = true;
            if (control.cogDisplayUC.Record.Content != null)
            {
                if (control.cogDisplayUC.Record.Content.GetType().Equals(typeof(CogImage16Range)))
                {
                    if (GetColorMapEnable(control))
                    {
                        control.cogDisplayUC.ColorMapPredefined = Cognex.VisionPro.Display.CogDisplayColorMapPredefinedConstants.Height;
                        CogHistogramTool tool = new CogHistogramTool();

                        var image = control.cogDisplayUC.Image as CogImage16Range;
                        tool.InputImage = image.GetPixelData();
                        tool.Run();

                        var average = tool.Result.Mean;
                        var stdDev = tool.Result.StandardDeviation;
                        var MaxVal = tool.Result.Maximum;
                        var MinVal = tool.Result.Minimum;

                        var lowerpixelPoint = -2.32 * stdDev + average;

                        var upperpixelPoint = 1.65 * stdDev + average;

                        var lowerPoint = -1 * (lowerpixelPoint - MinVal) / (MaxVal - MinVal);

                        var upperPoint = (upperpixelPoint - MinVal) / (MaxVal - MinVal);


                        control.cogDisplayUC.SetColorMapRoiLimits(lowerPoint, upperPoint);
                    }
                    else
                    {
                        control.cogDisplayUC.ColorMapPredefined = Cognex.VisionPro.Display.CogDisplayColorMapPredefinedConstants.None;
                    }
                }
            }

            if (GetSaveImageEnable(control))
            {
                control.OnSaveImage?.Invoke();
            }
        }

        public static void SetRecord(UIElement element, ICogRecord property)
        {
            element.SetValue(RecordProperty, property);
        }

        public static ICogRecord GetRecord(UIElement element)
        {
            return (ICogRecord)element.GetValue(RecordProperty);
        }
        #endregion

        #region Property-AcqFifo
        public static readonly DependencyProperty AcqFifoProperty = DependencyProperty.RegisterAttached("AcqFifo", typeof(ICogAcqFifo), typeof(CogDisplayForWPF), new PropertyMetadata(default(ICogAcqFifo), OnAcqFifoChanged));

        private static void OnAcqFifoChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as CogDisplayForWPF;
            if (!control.IsVisible)
                return;
            //do Something


        }

        public static void SetAcqFifo(UIElement element, ICogAcqFifo property)
        {
            element.SetValue(AcqFifoProperty, property);
        }

        public static ICogAcqFifo GetAcqFifo(UIElement element)
        {
            return (ICogAcqFifo)element.GetValue(AcqFifoProperty);
        }
        #endregion

        #region Property-StartLive
        public static readonly DependencyProperty StartLiveProperty = DependencyProperty.RegisterAttached("StartLive", typeof(bool), typeof(CogDisplayForWPF), new PropertyMetadata(default(bool), OnStartLiveChanged));

        private static void OnStartLiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as CogDisplayForWPF;
            if (!control.IsVisible)
                return;
            //do Something
            bool start = (bool)e.NewValue;
            ICogAcqFifo fifo = d.GetValue(AcqFifoProperty) as ICogAcqFifo;

            if (fifo is null)
                return;


            if (start)
            {

                control.cogDisplayUC.StartLiveDisplay(d.GetValue(AcqFifoProperty) as ICogAcqFifo, (bool)d.GetValue(LiveAcquisitionControlProperty));
            }

            else
                control.cogDisplayUC.StopLiveDisplay();
        }

        public static void SetStartLive(UIElement element, bool property)
        {
            element.SetValue(StartLiveProperty, property);
        }

        public static bool GetStartLive(UIElement element)
        {
            return (bool)element.GetValue(StartLiveProperty);
        }
        #endregion

        #region Property-LiveAcquisitionControl
        public static readonly DependencyProperty LiveAcquisitionControlProperty = DependencyProperty.RegisterAttached("LiveAcquisitionControl", typeof(bool), typeof(CogDisplayForWPF), new PropertyMetadata(default(bool), OnLiveAcquisitionControlChanged));

        private static void OnLiveAcquisitionControlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as CogDisplayForWPF;

            //do Something

        }

        public static void SetLiveAcquisitionControl(UIElement element, bool property)
        {
            element.SetValue(LiveAcquisitionControlProperty, property);
        }

        public static bool GetLiveAcquisitionControl(UIElement element)
        {
            return (bool)element.GetValue(LiveAcquisitionControlProperty);
        }
        #endregion

        #region Property-SelectedGraphic

        public static readonly DependencyProperty SelectedGraphicProperty = DependencyProperty.RegisterAttached("SelectedGraphic", typeof(ICogGraphicInteractive), typeof(CogDisplayForWPF), new PropertyMetadata(default(ICogGraphicInteractive), OnSelectedGraphicChanged));

        private static void OnSelectedGraphicChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as CogDisplayForWPF;

            //do Something

        }

        public static void SetSelectedGraphic(UIElement element, ICogGraphicInteractive property)
        {
            element.SetValue(SelectedGraphicProperty, property);
        }

        public static ICogGraphicInteractive GetSelectedGraphic(UIElement element)
        {
            return (ICogGraphicInteractive)element.GetValue(SelectedGraphicProperty);
        }

        #endregion

        #region Property-InteractiveGraphicCollection
        public static readonly DependencyProperty InteractiveGraphicSourceProperty = DependencyProperty.Register("InteractiveGraphicSource", typeof(IEnumerable<ICogGraphicInteractive>), typeof(CogDisplayForWPF), new PropertyMetadata(default(IEnumerable<ICogGraphicInteractive>), OnInteractiveGraphicCollectionChanged));



        private static void OnInteractiveGraphicCollectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as CogDisplayForWPF;

            control.OnCollectionChanged(e);
        }

        private void OnCollectionChanged(DependencyPropertyChangedEventArgs e)
        {
            var oldCollection = e.OldValue as INotifyCollectionChanged;
            var newCollection = e.NewValue as INotifyCollectionChanged;

            if (oldCollection != null)
                oldCollection.CollectionChanged -= OldCollection_CollectionChanged;

            if (newCollection != null)
                newCollection.CollectionChanged += OldCollection_CollectionChanged;
        }

        private void OldCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var control = this;

            if(control.cogDisplayUC.InvokeRequired)
            {
                control.cogDisplayUC.Invoke((Action<object, NotifyCollectionChangedEventArgs>)OldCollection_CollectionChanged, sender, e);
                return;
            }

            var OnDragging = new CogDraggingEventHandler((object send, CogDraggingEventArgs args) =>
            {
                var graphics = control.cogDisplayUC.InteractiveGraphics;
                for (int i = 0; i < graphics.Count; i++)
                {
                    if (!graphics[i].Selected)
                        if (graphics[i].Visible)
                        {
                            onDragging = true;
                            graphics[i].Visible = false;
                        }
                }
            });

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (ICogGraphicInteractive item in e.NewItems as IList)
                {
                    item.Changed += Item_Changed;

                    if (GetGraphicClearOnDrag(control))
                        item.Dragging += OnDragging;

                    control.cogDisplayUC.InteractiveGraphics.Add(item, item.TipText, false);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (ICogGraphicInteractive item in e.OldItems as IList)
                {
                    control.cogDisplayUC.InteractiveGraphics.RemoveItem(item);

                    if (GetGraphicClearOnDrag(control))
                        item.Changed -= Item_Changed;

                    item.Dragging -= OnDragging;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                var graphics = control.cogDisplayUC.InteractiveGraphics;

                for (int i = 0; i < graphics.Count; i++)
                {
                    graphics[i].Changed -= Item_Changed;

                    if (GetGraphicClearOnDrag(control))
                        graphics[i].Dragging -= OnDragging;
                }

                graphics.Clear();
            }
        }



        private void Item_Changed(object sender, CogChangedEventArgs e)
        {
            if (!onDragging)
            {
                var graphic = sender as ICogGraphicInteractive;

                if (graphic.Selected)
                    SetValue(SelectedGraphicProperty, graphic);
            }
            else
                onDragging = false;
        }
        #endregion

        #region Property-BackColor
        public static readonly DependencyProperty BackColorProperty = DependencyProperty.RegisterAttached("BackColor", typeof(System.Drawing.Color), typeof(CogDisplayForWPF), new PropertyMetadata(default(System.Drawing.Color), OnBackColorChanged));


        private static void OnBackColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as CogDisplayForWPF;


            //do Something
            control.cogDisplayUC.BackColor = (System.Drawing.Color)e.NewValue;
        }

        public static void SetBackColor(UIElement element, System.Drawing.Color property)
        {
            element.SetValue(BackColorProperty, property);
        }

        public static System.Drawing.Color GetBackColor(UIElement element)
        {
            return (System.Drawing.Color)element.GetValue(BackColorProperty);
        }
        #endregion

        #region SaveImageEnable
        public static readonly DependencyProperty SaveImageEnableProperty = DependencyProperty.RegisterAttached("SaveImageEnable", typeof(bool), typeof(CogDisplayForWPF), new PropertyMetadata(default(bool), OnSaveImageEnableChanged));


        private static void OnSaveImageEnableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as CogDisplayForWPF;


            //do Something

        }

        public static void SetSaveImageEnable(UIElement element, bool property)
        {
            element.SetValue(SaveImageEnableProperty, property);
        }

        public static bool GetSaveImageEnable(UIElement element)
        {
            return (bool)element.GetValue(SaveImageEnableProperty);
        }
        #endregion

        #region Property-ColorMapEnable

        public static readonly DependencyProperty ColorMapEnableProperty = DependencyProperty.RegisterAttached("ColorMapEnable", typeof(bool), typeof(CogDisplayForWPF), new PropertyMetadata(default(bool), OnColorMapEnableChanged));

        private static void OnColorMapEnableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as CogDisplayForWPF;

            //do Something

        }

        public static void SetColorMapEnable(UIElement element, bool property)
        {
            element.SetValue(ColorMapEnableProperty, property);
        }

        public static bool GetColorMapEnable(UIElement element)
        {
            return (bool)element.GetValue(ColorMapEnableProperty);
        }

        #endregion


        public static readonly DependencyProperty StatusBarVisibleProperty = DependencyProperty.Register("StatusBarVisible", typeof(bool), typeof(CogDisplayForWPF), new FrameworkPropertyMetadata(false));


        public static void SetStatusBarVisible(UIElement element, bool property)
        {
            element.SetValue(StatusBarVisibleProperty, property);
        }

        public static bool GetStatusBarVisible(UIElement element)
        {
            return (bool)element.GetValue(StatusBarVisibleProperty);
        }


        #region Prooperty-GraphicClearOnDrag
        public static readonly DependencyProperty GraphicClearOnDragProperty = DependencyProperty.RegisterAttached("GraphicClearOnDrag", typeof(bool), typeof(CogDisplayForWPF), new PropertyMetadata(default(bool), OnGraphicClearOnDragChanged));

        private static void OnGraphicClearOnDragChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as CogDisplayForWPF;


        }

        public static void SetGraphicClearOnDrag(UIElement element, bool property)
        {
            element.SetValue(GraphicClearOnDragProperty, property);
        }

        public static bool GetGraphicClearOnDrag(UIElement element)
        {
            return (bool)element.GetValue(GraphicClearOnDragProperty);
        }
        #endregion





        public void SaveImage(bool overlayGraphic, string path)
        {
            if (overlayGraphic)
                cogDisplayUC.CreateContentBitmap(Cognex.VisionPro.Display.CogDisplayContentBitmapConstants.Display).Save(path, ImageFormat.Bmp);
            else
                cogDisplayUC.CreateContentBitmap(Cognex.VisionPro.Display.CogDisplayContentBitmapConstants.Image).Save(path, ImageFormat.Bmp);
        }


        public IEnumerable<ICogGraphicInteractive> InteractiveGraphicSource
        {
            get { return GetValue(InteractiveGraphicSourceProperty) as IEnumerable<ICogGraphicInteractive>; }
            set { SetValue(InteractiveGraphicSourceProperty, value); }
        }
        public ICogImage Image { get; set; }
        public ICogRecord Record { get; set; }
        public ICogAcqFifo AcqFifo { get; set; }
        public bool GraphicClearOnDrag { get; set; }
        public bool StartLive { get; set; }
        public bool LiveAcquisitionControl { get; set; }
        public bool SaveImageEnable { get; set; }
        public bool ColorMapEnable { get; set; }
        public bool StatusBarVisible { get; set; }
        public System.Drawing.Color BackColor { get; set; }


        public event Action OnSaveImage;
        public ICogGraphicInteractive SelectedGraphic
        {
            get
            {
                for (int i = 0; i < cogDisplayUC.InteractiveGraphics.Count; i++)
                {
                    if (cogDisplayUC.InteractiveGraphics[i].Selected)
                        return cogDisplayUC.InteractiveGraphics[i];
                }
                return null;
            }
            set
            {
                for (int i = 0; i < cogDisplayUC.InteractiveGraphics.Count; i++)
                {
                    var item = cogDisplayUC.InteractiveGraphics[i];

                    if (item.Equals(value))
                        if (!item.Selected)
                            item.Selected = true;
                }
            }
        }
        public CogDisplayForWPF()
        {
            InitializeComponent();

            cogStatusBar.Display = cogDisplayUC;
            cogDisplayUC.HorizontalScrollBar = false;
            cogDisplayUC.VerticalScrollBar = false;

            (cogDisplayUC as System.Windows.Forms.Control).DragEnter += DoDragCheck;
            (cogDisplayUC as System.Windows.Forms.Control).DragDrop += DoDragDrop;
            cogDisplayUC.AllowDrop = true;

            Loaded += CogDisplayForWPF_Loaded;
        }

        private void CogDisplayForWPF_Loaded(object sender, RoutedEventArgs e)
        {
            wfHostStatusBar.Visibility = GetStatusBarVisible(this) ? Visibility.Visible : Visibility.Hidden;
            if (wfHostStatusBar.Visibility == Visibility.Hidden)
                Grid.SetRowSpan(wfHost, 2);
        }

        internal void DoDragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            var lst = (IEnumerable<string>)e.Data.GetData(DataFormats.FileDrop);

            if (CheckImageFile(lst.First(), ".bmp", ".jpg"))
            {
                using (Bitmap image = new Bitmap(lst.First()))
                {
                    SetImage(this, new CogImage8Grey(image));
                    cogDisplayUC.Fit();
                }
            }
        }

        internal void DoDragCheck(object sender, System.Windows.Forms.DragEventArgs e)
        {
            var lst = (IEnumerable<string>)e.Data.GetData(DataFormats.FileDrop);
            bool isArchive = CheckImageFile(lst.First(), ".bmp", "jpg");

            e.Effect = isArchive ? System.Windows.Forms.DragDropEffects.Copy : System.Windows.Forms.DragDropEffects.None;
        }

        private static bool CheckImageFile(string fileName, params string[] exts)
        {
            if (!File.Exists(fileName))
                return false;

            foreach (var ext in exts)
            {
                if (System.IO.Path.GetExtension(fileName).Equals(ext))
                    return true;
            }

            return false;
        }
    }
}
