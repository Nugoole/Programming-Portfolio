using Basler.Pylon;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestWPFDotNetFramework
{
    public class BaslerCam
    {
        private Camera camera;
        private PixelDataConverter converter;
        public event EventHandler<IGrabResult> OnGrabbed;
        public int Width { get; set; }
        public int Height { get; set; }

        public BaslerCam()
        {
            var serial = CameraFinder.Enumerate().First()[CameraInfoKey.SerialNumber];
            camera = new Camera(serial);
            converter = new PixelDataConverter();
            camera.CameraOpened += Configuration.AcquireContinuous;
            camera.StreamGrabber.ImageGrabbed += StreamGrabber_ImageGrabbed;


            camera.Open();


            camera.Parameters[PLCamera.PixelFormat].SetValue("Mono8");
            Width = (int)camera.Parameters[PLCamera.Width].GetValue();
            Height = (int)camera.Parameters[PLCamera.Height].GetValue();
        }

        private void StreamGrabber_ImageGrabbed(object sender, ImageGrabbedEventArgs e)
        {

            IGrabResult grabResult = e.GrabResult;


            var width = grabResult.Width;
            var height = grabResult.Height;



            OnGrabbed?.Invoke(this, grabResult);

        }

        public void StartLive()
        {
            camera.StreamGrabber.Start(GrabStrategy.OneByOne, GrabLoop.ProvidedByStreamGrabber);
        }
        public void StopLive()
        {
            if (camera.StreamGrabber.IsGrabbing)
            {
                camera.StreamGrabber.Stop();
            }
        }

        public void Close()
        {
            if (camera.StreamGrabber.IsGrabbing)
                camera.StreamGrabber.Stop();

            camera.Close();
            camera.Dispose();
            camera = null;
        }
    }
}
