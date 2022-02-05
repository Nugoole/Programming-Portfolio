/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:ALT.TIS.EssenCore.OCRMemory"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using ALT.CVL;
using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;

namespace ALT.TIS.EssenCore.OCRMemory.ViewModels
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        private readonly string keyVMOCR = "OCR";
        private readonly string keyVMTool0 = "Tool0";
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            //프레임 그래버 초기화 완료
            MdFrameGrabbers mdFrameGrabbers = MdFrameGrabbers.Getinstance();
            //Config 파일 초기화
            foreach (var item in MdConfigData.Getinstance().ConfigParam.OCCameraData)
            {
                item.IPList = mdFrameGrabbers.OCIPAddress;
            }

            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            ////if (ViewModelBase.IsInDesignModeStatic)
            ////{
            ////    // Create design time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
            ////}
            ////else
            ////{
            ////    // Create run time view services and models
            ////    SimpleIoc.Default.Register<IDataService, DataService>();
            ////}

            SimpleIoc.Default.Register<VMMain>();
            SimpleIoc.Default.Register<VMDisplay>();
            SimpleIoc.Default.Register<VMFileIO>();
            SimpleIoc.Default.Register<VMIO>();
            SimpleIoc.Default.Register<VMLog>();
            SimpleIoc.Default.Register<VMEnvironment>();
            SimpleIoc.Default.Register(() => new VMCamera(0), keyVMOCR);
            SimpleIoc.Default.Register(() => new VMTool(0), keyVMTool0);
        }

        public VMMain Main => ServiceLocator.Current.GetInstance<VMMain>();
        public VMDisplay Display => ServiceLocator.Current.GetInstance<VMDisplay>();
        public VMFileIO FileIO => ServiceLocator.Current.GetInstance<VMFileIO>();
        public VMIO IO => ServiceLocator.Current.GetInstance<VMIO>();
        public VMLog Log => ServiceLocator.Current.GetInstance<VMLog>();
        public VMEnvironment Environment => ServiceLocator.Current.GetInstance<VMEnvironment>();
        public VMCamera Camera => ServiceLocator.Current.GetInstance<VMCamera>(keyVMOCR);
        public VMTool Tool => ServiceLocator.Current.GetInstance<VMTool>(keyVMTool0);
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}