using GalaSoft.MvvmLight;

using LineScanViewer.Messenger;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace LineScanViewer.ViewModel
{
    public class DCF
    {
        public string Name { get; set; }
        public string FilePath { get; set; }
        public bool IsSelected { get; set; }
    }
    public class VMDCF : ViewModelBase
    {
        public ObservableCollection<DCF> DCFs { get; set; }
        public VMDCF()
        {
            DCFs = new ObservableCollection<DCF>();
            MessengerInstance.Register<DCFPathSetMessenger>(this, OnDCFPathSet);
            MessengerInstance.Register<MainConnectMessenger>(this, OnConnect);
        }

        private void OnConnect(MainConnectMessenger obj)
        {
            if (obj.LoadCommand == LoadCommand.Load)
            {                
                var selecteDCFFilePath = DCFs.Where(x => x.IsSelected).Select(y => y.FilePath)?.FirstOrDefault();
                if(!string.IsNullOrEmpty(selecteDCFFilePath))
                    MessengerInstance.Send(new LoadDCFMessenger() { DCFFilePath = selecteDCFFilePath });
            }
        }

        private void OnDCFPathSet(DCFPathSetMessenger obj)
        {
            DirectoryInfo info = new DirectoryInfo(obj.FilePath);
            DCFs.Clear();

            foreach (var dcf in info.GetFiles("*.dcf"))
            {
                DCFs.Add(new DCF() { FilePath = dcf.FullName, Name = dcf.Name });
            }
        }
    }

    
}
