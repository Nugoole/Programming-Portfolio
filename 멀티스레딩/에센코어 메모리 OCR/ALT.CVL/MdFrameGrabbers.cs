using Cognex.VisionPro;
using GalaSoft.MvvmLight;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace ALT.CVL
{
    public class MdFrameGrabbers : ObservableObject
    {
        #region Singleton Constructor

        private static readonly Lazy<MdFrameGrabbers> instance = new Lazy<MdFrameGrabbers>(() => new MdFrameGrabbers());

        private MdFrameGrabbers()
        {
            RefreshFrameGrabbers();
        }

        public static MdFrameGrabbers Getinstance() => instance.Value;

        #endregion

        #region Variables

        #endregion

        #region Properties

        public ObservableCollection<ICogAcqFifo> OCAcqFifo { get; private set; } = new ObservableCollection<ICogAcqFifo>();
        
        public ObservableCollection<string> OCIPAddress { get; private set; } = new ObservableCollection<string>();

        #endregion

        #region Functions
        public void RefreshFrameGrabbers()
        {
            CogFrameGrabbers.Refresh();
            var cameras = new CogFrameGrabbers();
            OCAcqFifo.Clear();
            OCIPAddress.Clear();
            foreach (ICogFrameGrabber camera in cameras)
            {
                if (!string.IsNullOrEmpty(camera.SerialNumber) && camera.GetStatus(false) == CogFrameGrabberStatusConstants.Active)
                {
                    var tmpFg = camera.CreateAcqFifo(camera.AvailableVideoFormats[0], CogAcqFifoPixelFormatConstants.Format8Grey, 0, true);
                    tmpFg.OwnedGigEVisionTransportParams.LatencyLevel = 0;
                    tmpFg.OwnedGigEVisionTransportParams.PacketSize = 9000;
                    OCAcqFifo.Add(tmpFg);
                    OCIPAddress.Add(tmpFg.FrameGrabber.OwnedGigEAccess.CurrentIPAddress);
                }
            }
            //마지막 ip 리스트에 empty값 입력
            OCIPAddress.Add(string.Empty);
        }

        //selectedItem과 같은 값의 acqFifo를 리턴
        public ICogAcqFifo GetAcqFifo(string selectedItem)
        {
            return OCAcqFifo?.Where(x => x.FrameGrabber.OwnedGigEAccess.CurrentIPAddress == selectedItem).FirstOrDefault();
        }
        public void Dispose()
        {
            foreach (ICogAcqFifo fg in OCAcqFifo)
            {
                if (fg.FrameGrabber.GetStatus(false) == CogFrameGrabberStatusConstants.Active)
                    fg.FrameGrabber.Disconnect(false);
            }
        }
        public void Close()
        {
            CogFrameGrabbers cogFrameGrabbers = new CogFrameGrabbers();
            foreach (ICogFrameGrabber item in cogFrameGrabbers)
            {
                item.Disconnect(false);
            }
            cogFrameGrabbers.Dispose();
        }
        #endregion
    }
}
