using ALT.CVL.GeneralCam.Enum;
using ALT.CVL.GeneralCam.Interface;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ALT.CVL.GeneralCam.CamBaseModel
{
    internal class CamInfo : ICamInfo, INotifyPropertyChanged
    {
        private string camName;
        private string serialNumber;
        private InterfaceType interfaceType;
        public string CamName
        {
            get => camName; internal set
            {
                camName = value;
                RaisePropertyChanged();
            }
        }
        public string SerialNumber
        {
            get => serialNumber; internal set
            {
                serialNumber = value;
                RaisePropertyChanged();
            }
        }

        public InterfaceType InterfaceType
        {
            get => interfaceType;internal set
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
