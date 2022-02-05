using ALT.CVL.GeneralCam.Enum;
using ALT.CVL.GeneralCam.Interface;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALT.CVL.GeneralCam.Model
{
    internal abstract class FrameGrabbersBase : List<IAltFrameGrabber>, IAltFrameGrabbers, IDisposable
    {
        //public new IAltFrameGrabber this[int index]
        //{
        //    get
        //    {
        //        if (index > Count)
        //            return null;

                
        //        return this[index];
        //    }
        //}

        


        protected FrameGrabbersBase()
        {
            IsInitialized = false;
        }

        

        public bool IsInitialized { get; protected set; }
        public CameraType CameraType { get; internal set; }

        public abstract void Initialize();

        public virtual void Dispose()
        {
            foreach (FrameGrabberBase fg in this)
            {
                fg.Dispose();
            }
        }

        public void Refresh()
        {
            IsInitialized = false;

            Initialize();
        }
    }
}
