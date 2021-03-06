using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using LineScanViewer.Messenger;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LineScanViewer.ViewModel
{
    public class ImagePreview
    {
        public ImageSource PreviewImage { get; set; }

        public string Name { get; set; }

        public ImagePreview(string path)
        {
            if (File.Exists(path) && (path.EndsWith(".bmp") || path.EndsWith(".BMP")))
            {
                PreviewImage = new BitmapImage(new Uri(path));
                Name = path.Split('\\').Last();
            }
        }
    }
    public class VMImageStorage : ViewModelBase
    {
        public string CurrentImageFolder { get; set; }
        public ObservableCollection<string> FilesInImageFolder { get; set; }


        public ICommand OnUpdateFileList { get; set; }
        public VMImageStorage()
        {
            FilesInImageFolder = new ObservableCollection<string>();

            OnUpdateFileList = new RelayCommand(UpdateFileList);

            MessengerInstance.Register<OnImageSavePathChangedMessenger>(this, (msg) =>
            {
                CurrentImageFolder = msg.ChangedImageSavePath;
                Process.Start(CurrentImageFolder);
                UpdateFileList();
            });

            
        }

        private void UpdateFileList()
        {
            if (!Directory.Exists(CurrentImageFolder))
                return;

            FilesInImageFolder.Clear();

            foreach (var imageFile in Directory.GetFiles(CurrentImageFolder, "*.bmp"))
            {
                FilesInImageFolder.Add(imageFile);
            }
        }
    }
    [ValueConversion(typeof(string), typeof(string))]
    public class FilePathToFileNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (File.Exists(value.ToString()))
            {
                return value.ToString().Split('\\').Last();
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    [ValueConversion(typeof(string), typeof(ImageSource))]
    class FilePathToImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (File.Exists(value.ToString()))
            {
                return new BitmapImage(new Uri(value.ToString()));
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
