/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:LineScanViewer"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using CommonServiceLocator;

using GalaSoft.MvvmLight.Ioc;

using LineScanViewer.Interface;

namespace LineScanViewer.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
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
            //SimpleIoc.Default.Register<VMCameraView>();
            SimpleIoc.Default.Register<VMConfig>();
            SimpleIoc.Default.Register<VMDCF>();
            //SimpleIoc.Default.Register<VMImageStorage>(true);
        }

        public IVMMain Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<VMMain>();
            }
        }

        //public VMCameraView CameraView
        //{
        //    get
        //    {
        //        return ServiceLocator.Current.GetInstance<VMCameraView>();
        //    }
        //}

        //public VMImageStorage ImageStorage => ServiceLocator.Current.GetInstance<VMImageStorage>();
        public VMConfig Config => ServiceLocator.Current.GetInstance<VMConfig>();
        public VMDCF DCF => ServiceLocator.Current.GetInstance<VMDCF>();
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }
}