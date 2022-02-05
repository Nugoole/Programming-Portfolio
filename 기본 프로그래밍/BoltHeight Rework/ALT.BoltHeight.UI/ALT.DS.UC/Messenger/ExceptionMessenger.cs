using System;

namespace ALT.DS.UC.Messenger
{
    public class ExceptionMessenger : GalaSoft.MvvmLight.Messaging.MessageBase
    {
        public Exception Exception { get; set; }
    }
}
