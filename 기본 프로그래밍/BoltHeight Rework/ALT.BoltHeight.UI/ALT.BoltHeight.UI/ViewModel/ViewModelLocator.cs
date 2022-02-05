/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:ALT.BoltHeight"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using CommonServiceLocator;

using GalaSoft.MvvmLight.Ioc;

using System;
using System.Globalization;
using System.Windows.Data;

namespace ALT.BoltHeight.UI.ViewModel
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
            ///

            SimpleIoc.Default.Register<VMMain>();
            SimpleIoc.Default.Register<VMConfigView>();
            SimpleIoc.Default.Register<VMSubModelView>();
            SimpleIoc.Default.Register<VMModelView>();
            SimpleIoc.Default.Register<VMToolSetView>(true);
            SimpleIoc.Default.Register<VMCameraView>(true);
            SimpleIoc.Default.Register<VMRegionNameSet>();
            SimpleIoc.Default.Register<VMCrossSectionToolSetWindow>(true);
        }

        public VMMain Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<VMMain>();
            }
        }
        public VMConfigView Config
        {
            get
            {
                return ServiceLocator.Current.GetInstance<VMConfigView>();
            }
        }
        public VMCameraView Camera => ServiceLocator.Current.GetInstance<VMCameraView>();
        public VMToolSetView ToolSet => ServiceLocator.Current.GetInstance<VMToolSetView>();
        public VMRegionNameSet RegionNameSet => ServiceLocator.Current.GetInstance<VMRegionNameSet>();
        public VMCrossSectionToolSetWindow CrossSectionToolSet => ServiceLocator.Current.GetInstance<VMCrossSectionToolSetWindow>();
        public VMModelView Model
        {
            get
            {
                return ServiceLocator.Current.GetInstance<VMModelView>();
            }
        }
        public VMSubModelView SubModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<VMSubModelView>();
            }
        }
        public static void Cleanup()
        {
            // TODO Clear the ViewModels
        }
    }


    [ValueConversion(typeof(bool), typeof(bool))]
    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(bool))
                throw new InvalidOperationException("Target must be bool");

            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}