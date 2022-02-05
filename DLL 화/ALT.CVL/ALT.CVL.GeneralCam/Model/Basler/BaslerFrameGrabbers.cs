

using ALT.CVL.GeneralCam.Interface;
using System.Linq;

using Basler.Pylon;

using System.Collections.Generic;
using ALT.CVL.GeneralCam.Enum;
using ALT.CVL.GeneralCam.CamBaseModel;

namespace ALT.CVL.GeneralCam.Model.CamBaseModel
{
    internal class BaslerFrameGrabbers : FrameGrabbersBase
    {
        internal BaslerFrameGrabbers() : base()
        {

        }


        public override void Initialize()
        {
            if (IsInitialized)
                return;


            //새 카메라리스트 가져옴
            var newCameraInfos = CameraFinder.Enumerate();
            
            //기존의 카메라 리스트에서 새 카메라 리스트에 없는 카메라 => 지워야 하는 카메라 목록 저장
            List<FrameGrabberBase> fgtoRemove = this.Where(x => !newCameraInfos.Exists(y => y[CameraInfoKey.SerialNumber].Equals(x.CamInfo.SerialNumber))).Cast<FrameGrabberBase>().ToList();


            //기존의 카메라리스트에서 없어진 카메라를 지워버림
            RemoveAll(x => fgtoRemove.Contains(x));


            //새 카메라 리스트들 추가
            foreach (var newCameraInfo in newCameraInfos)
            {
                if (this.Exists(y => y.CamInfo.SerialNumber.Equals(newCameraInfo[CameraInfoKey.SerialNumber])))
                    continue;

                var types = System.Enum.GetValues(typeof(InterfaceType));

                InterfaceType interfaceType = default;

                foreach (var type in types)
                {
                    if (newCameraInfo[CameraInfoKey.DeviceType].ToLower().Contains(type.ToString().ToLower()))
                    {
                        interfaceType = (InterfaceType)System.Enum.Parse(typeof(InterfaceType), type.ToString());
                        break;
                    }
                }




                this.Add(new BaslerGrabber(new CamInfo
                {
                    CamName = newCameraInfo[CameraInfoKey.FriendlyName],
                    SerialNumber = newCameraInfo[CameraInfoKey.SerialNumber],
                    InterfaceType = interfaceType
                }));

            }


            IsInitialized = true;
        }

        public override void Dispose()
        {
            base.Dispose();

            //추가 작업
        }
    }
}