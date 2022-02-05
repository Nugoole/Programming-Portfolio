using ALT.CVL.Common.Enum;
using ALT.CVL.Common.Interface;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ALT.CVL.Common.CamBaseModel
{
    public class CamInfo : ICamInfo, INotifyPropertyChanged
    {
        private string camName;
        private string serialNumber;
        private InterfaceType interfaceType;

        public CamInfo()
        {
            
        }

        public string CamName
        {
            get => camName; set
            {
                camName = value;
                RaisePropertyChanged();
            }
        }
        public string SerialNumber
        {
            get => serialNumber; set
            {
                serialNumber = value;
                RaisePropertyChanged();
            }
        }

        public InterfaceType InterfaceType
        {
            get => interfaceType;set
            {
                interfaceType = value;
                RaisePropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
