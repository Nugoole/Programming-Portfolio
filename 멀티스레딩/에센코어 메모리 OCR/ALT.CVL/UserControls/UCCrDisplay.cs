using Cognex.VisionPro;

using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ALT.CVL
{
    public partial class UCCrDisplay : UserControl
    {
        private string[] allowedImageExtensions = new string[2] { ".bmp", ".jpg" };

        #region Constructor

        public UCCrDisplay()
        {
            InitializeComponent();
            (Display as Control).DragEnter += CogDisplay_DragEnter;
            (Display as Control).DragDrop += CogDisplay_DoDrop;
            
        }

        #endregion

        #region Variables

        #endregion

        #region Properties

        public CogRecordDisplay Display { get => cogRecordDisplay1; set => cogRecordDisplay1 = value; }

        #endregion

        #region Functions
        private void CogDisplay_DragEnter(object sender, DragEventArgs e)
        {
            var lst = (IEnumerable<string>)e.Data.GetData(DataFormats.FileDrop);
            bool isArchive = CheckImageFile(lst.First(), allowedImageExtensions);

            e.Effect = isArchive ? DragDropEffects.Copy : DragDropEffects.None;
        }
        private bool CheckImageFile(string fileName, params string[] exts)
        {
            if (!File.Exists(fileName))
                return false;
            
            foreach (var ext in exts)
            {
                if (Path.GetExtension(fileName).Equals(ext))
                    return true;
            }

            return false;
        }
        private void CogDisplay_DoDrop(object sender, DragEventArgs e)
        {
            CogRecordDisplay Display = sender as CogRecordDisplay;

            var lst = (IEnumerable<string>)e.Data.GetData(DataFormats.FileDrop);

            if (CheckImageFile(lst.First(), allowedImageExtensions))
            {
                using (Bitmap image = new Bitmap(lst.First()))
                {
                    Display.DrawingEnabled = false;
                    if (image.PixelFormat == System.Drawing.Imaging.PixelFormat.Format24bppRgb)
                        Display.Image = new CogImage24PlanarColor(image);
                    else
                        Display.Image = new CogImage8Grey(image);
                    Display.Fit();
                    Display.DrawingEnabled = true;
                }
            }
        }
        #endregion

    }
}
