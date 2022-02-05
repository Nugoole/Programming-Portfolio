/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:ALT.DS.UC"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using CommonServiceLocator;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Threading;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ALT.DS.UC.ViewModel
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class DSCameraViewmodelLocator
    {
        private static DSCameraViewmodelLocator instance;
        private static SimpleIoc mySimpleIoC;
        private static List<Type> keys;
        private static bool initialized = false;
        public static void Initialize()
        {
            if (!initialized)
            {
                initialized = true;
                instance = new DSCameraViewmodelLocator();
                mySimpleIoC = new SimpleIoc();
                keys = new List<Type>();

                mySimpleIoC.Register(() => new VMCameraView(), true);
                keys.Add(typeof(VMCameraView));
                mySimpleIoC.Register(() => new VMToolSetView(), true);
                keys.Add(typeof(VMToolSetView));
                mySimpleIoC.Register(() => new VMRegionNameSet(), true);
                keys.Add(typeof(VMRegionNameSet));
                mySimpleIoC.Register(() => new VMCrossSectionToolSetWindow(), true);
                keys.Add(typeof(VMCrossSectionToolSetWindow));


                DispatcherHelper.Initialize();
                DispatcherHelper.UIDispatcher.ShutdownStarted += (o, e) => Cleanup();
            }
        }



        private IServiceLocator MyServiceLocator
        {
            get
            {
                return mySimpleIoC;
            }
        }
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public DSCameraViewmodelLocator()
        {
            Initialize();

        }


        public VMCameraView Camera => MyServiceLocator.GetInstance<VMCameraView>();
        public VMToolSetView ToolSet => MyServiceLocator.GetInstance<VMToolSetView>();
        public VMRegionNameSet RegionNameSet => MyServiceLocator.GetInstance<VMRegionNameSet>();
        public VMCrossSectionToolSetWindow CrossSectionToolSet => MyServiceLocator.GetInstance<VMCrossSectionToolSetWindow>();

        public static void Cleanup()
        {
            // TODO Clear the ViewModels

            foreach (Type key in keys)
            {
                (mySimpleIoC.GetInstance(key) as ViewModelBase)?.Cleanup();
            }
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