using ALT.CVL.Common.Enum;
using ALT.CVL.Common.Interface;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ALT.CVL
{
    internal abstract class LightControllerBase : ILightController, INotifyPropertyChanged
    {
        protected const DllImportSearchPath dllImportSearchPath = DllImportSearchPath.ApplicationDirectory | DllImportSearchPath.UseDllDirectoryForDependencies;
        
        protected object lockObject = new object();
        protected int myControllerIndex = -1;
        private bool updateValueImmediately = true;
        protected ConnectionInfo connectionParam;

        public bool UpdateValueImmediately
        {
            get => updateValueImmediately; set
            {
                foreach (var innerValue in Values)
                {
                    innerValue.UpdateImmediately = value;
                }

                updateValueImmediately = value;
                RaisePropertyChanged();
            }
        }

        
        public abstract int? Segment { get; }
        
        public abstract int Channel { get; }
        public abstract int Page { get; set; }
        public abstract TriggerEdge? TriggerEdge { get; set; }
        public abstract IEnumerable<ILightControllerParameter<int>> Values { get; }
        public IConnectionInfo ConnectionParam => connectionParam;

        public event EventHandler<int> OnPageSetCompleted;
        public event EventHandler<ErrorCode> OnErrorOccurred;
        public event PropertyChangedEventHandler PropertyChanged;
        internal event EventHandler OnDisconnect;


        public LightControllerBase()
        {
            connectionParam = new ConnectionInfo();
        }

        public abstract void ChangeAllValue(int value);


        public abstract void ChangeValue(int channel, int value);


        public abstract bool Connect(ConnectionProtocol protocol, string serialPortName, int baudRate = 9600, string ip = "192.168.10.10", int port = 1000);

        public virtual void Disconnect()
        {
            OnDisconnect?.Invoke(this, EventArgs.Empty);
        }


        protected void CheckError()
        {
            if (Enum.TryParse(Enum.GetName(typeof(ErrorCode), GetLastErrorCode()), out ErrorCode errorCode))
            {
                OnErrorOccurred?.Invoke(this, errorCode);
            };
        }

        protected abstract ulong GetLastErrorCode();


        protected void SetValues()
        {
            foreach (LightControllerParameter<int> value in Values)
            {
                value.Set();
            }
        }

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void RaisePageSetCompleted(int page)
        {
            OnPageSetCompleted?.Invoke(this, page);
        }
    }
}
