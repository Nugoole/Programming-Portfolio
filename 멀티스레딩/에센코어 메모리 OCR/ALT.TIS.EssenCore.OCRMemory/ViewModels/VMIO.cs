using ALT.Log.Model;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;

namespace ALT.TIS.EssenCore.OCRMemory.ViewModels
{
    public class VMIO
    {
        
        #region Constructor

        public VMIO()
        {
            IpcChannel serverChannel = new IpcChannel(serverName);
            ChannelServices.RegisterChannel(serverChannel, false);
            //RemotingConfiguration.RegisterWellKnownServiceType(typeof(RemoteObject));
        }

        #endregion

        #region Variables

        private readonly string serverName = "Server";

        #endregion

        #region Properties
        public string SendMessage { get; set; }
        #endregion

        #region Functions

        #endregion

    }
}
