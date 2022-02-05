using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using LineScanViewer.Messenger;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LineScanViewer.ViewModel
{
    public class VMConfig :ViewModelBase
    {

        public string DCFFolder { get; set; } = "Select DCF File Folder";
        public string ImageSavePath { get; set; } = $@"C:\users\{Environment.UserName}\Desktop";

        public RelayCommand OpenDCFFileFolder { get; set; }
        public RelayCommand OpenImageSaveFolder { get; set; }

        public VMConfig()
        {
            OpenDCFFileFolder = new RelayCommand(OpenDCFFileFolderAction);
            OpenImageSaveFolder = new RelayCommand(OpenImageSaveFolderAction);
        }

        private void OpenImageSaveFolderAction()
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    if (ImageSavePath != dialog.SelectedPath)
                    {
                        ImageSavePath = dialog.SelectedPath;

                        MessengerInstance.Send(new OnImageSavePathChangedMessenger() { ChangedImageSavePath = ImageSavePath });
                    }
                }
            }
        }

        private void OpenDCFFileFolderAction()
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                if(dialog.ShowDialog()==DialogResult.OK)
                {
                    DCFFolder = dialog.SelectedPath;

                    MessengerInstance.Send(new DCFPathSetMessenger() { FilePath = DCFFolder });
                }
            }
        }        
    }
}
