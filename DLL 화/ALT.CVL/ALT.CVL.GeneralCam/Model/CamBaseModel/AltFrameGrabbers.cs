using ALT.CVL.GeneralCam.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALT.CVL.GeneralCam.Model.CamBaseModel
{
    public class AltFrameGrabbers
    {

        #region Constructor

        public AltFrameGrabbers()
        {

        }

        #endregion

        #region Variables

        protected List<IAltFrameGrabber> listFrameGrabber = new List<IAltFrameGrabber>();

        #endregion

        #region Properties

        #endregion

        #region Functions

        public void Disconnect()
        {
            for (int i = 0; i < Count; i++)
            {
                listFrameGrabber[i].Disconnect();
            }
        }

        #endregion

        public IAltFrameGrabber this[int index] { get => listFrameGrabber[index]; }
        public int Count { get => listFrameGrabber.Count; }

        public List<IAltFrameGrabber> AvailableCameras { get => listFrameGrabber; set => listFrameGrabber = value;}
    }
}
