using ALT.CVL.GeneralCam.Enum;
using ALT.CVL.GeneralCam.Interface;
using ALT.CVL.GeneralCam.Model.CamBaseModel;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALT.CVL.GeneralCam.Model
{



    public class CameraConnector
    {
        #region Properties
        private static CameraConnector instance;
        private readonly Dictionary<CameraType, IAltFrameGrabbers> dicFgs;
        #endregion

        #region Constructor
        public static CameraConnector Instance
        {
            get
            {

                if (instance is null)
                    instance = new CameraConnector();

                return instance;
            }
        }

        public IReadOnlyList<IAltFrameGrabbers> FrameGrabbersDictionary => dicFgs.Values.ToList();

        private CameraConnector()
        {
            dicFgs = new Dictionary<CameraType, IAltFrameGrabbers>();

        }

        public void InitFrameGrabbers(CameraType camType)
        {
            foreach (CameraType type in System.Enum.GetValues(typeof(CameraType)))
            {
                if (type == CameraType.All)
                    continue;

                if (camType.HasFlag(type))
                {
                    FrameGrabbersBase grabbers = new BaslerFrameGrabbers { CameraType = type };
                    grabbers.Initialize();
                    dicFgs.Add(type, grabbers);
                }
            }
        }
        #endregion


    }
}
