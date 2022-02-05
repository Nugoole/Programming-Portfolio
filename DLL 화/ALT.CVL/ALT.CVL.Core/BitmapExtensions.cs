using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ALT.CVL.Core
{
    public static class BitmapExtensions
    {
        public static ImageSource ConvertToImageSource(this Bitmap bitmap)
        {
            BitmapImage image = new BitmapImage();
            Stream bitmapStream = new MemoryStream();
            bitmap.Save(bitmapStream, System.Drawing.Imaging.ImageFormat.Bmp);
            bitmapStream.Seek(0, SeekOrigin.Begin);
            image.BeginInit();
            image.StreamSource = bitmapStream;
            image.EndInit();

            return image;
        }
    }
}
