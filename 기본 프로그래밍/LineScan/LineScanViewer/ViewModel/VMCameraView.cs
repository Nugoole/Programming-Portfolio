using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using LineScanViewer.Messenger;
using LineScanViewer.Model;

using PropertyChanged;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

using System.Windows.Media.Imaging;

using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace LineScanViewer.ViewModel
{
    internal class DependsOnPropertyAttribute : Attribute
    {
        public readonly List<string> Dependences;

        public DependsOnPropertyAttribute(params string[] otherProperties)
        {
            Dependences = otherProperties.ToList();
        }
    }

    public class ScanLengthValidator : ValidationRule
    {
        public int MinScanSize { get; set; }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null)
                return new ValidationResult(false, "11");

            if (int.TryParse(value.ToString(), out int scanLength))
            {
                if ((scanLength % MinScanSize) == 0)
                    return new ValidationResult(true, null);
                else
                {
                    value = 0;
                    return new ValidationResult(false, $"{MinScanSize}의 배수로 값을 맞춰주세요");
                }
            }
            else
                return new ValidationResult(false, "알맞지 않은 값입니다.");
        }
    }

    public class VMCameraView : ViewModelBase
    {
        #region Field
        private const string FileDialogFormat_Bitmap = "Bitmap files(*.bmp)|*.bmp";
        private const string FileDialogFormat_Jpg = "Jpg files(*.jpg)|*.jpg";
        private const string FileDialogFormat_DCF = "DCF Files(*.dcf)|*.dcf";
        private const string FileDialogFormat_AllFiles = "All File(*.*)|*.*";
        private string redValue;
        private string currentDCFFileDirectory;
        private bool _isCameraGrabbing;
        private MCam LineScanCamera;
        private BitmapSource image;
        private IEnumerable<string> dCFFileNames;
        private Dictionary<string, List<string>> DependencyMap;
        private string dCFFileName;
        #endregion

        #region Property
        public string RedValue { get => redValue; set => Set(ref redValue, value); }
        public string ScanLineLength { get; set; }
        public string LiveScanLength
        {
            get
            {
                if (LineScanCamera != null)
                    return LineScanCamera.LiveYLength.ToString();
                else
                    return string.Empty;
            }
            set
            {
                if (int.TryParse(value, out int result))
                    LineScanCamera.LiveYLength = result;

                RaisePropertyChanged();
            }
        }
        [DoNotNotify]
        public Enums.ImageFormat ImageFormat
        {
            get
            {
                if (LineScanCamera != null)
                    return LineScanCamera.ImageFormat;
                else
                    return Enums.ImageFormat.Mono;
            }
            set
            {
                RaisePropertyChanged();
            }
        }

        public BitmapSource Image
        {
            get => image; set
            {
                image = value;

                RaisePropertyChanged();
            }
        }
        [DependsOnProperty(nameof(IsCameraGrabbing), nameof(IsDCFSet))]
        public bool IsCameraGrabbable
        {
            get
            {
                return IsDCFSet && !IsCameraGrabbing;
            }
        }



        public bool IsCameraGrabbing
        {
            get
            {
                return _isCameraGrabbing;
            }

            set
            {
                Set(ref _isCameraGrabbing, value);
            }
        }

        [DoNotNotify]
        public bool IsDCFSet
        {
            get
            {
                if (LineScanCamera != null)
                    return LineScanCamera.IsDigitizerSet;
                else
                    return true;
            }
            private set
            {
                RaisePropertyChanged();
            }
        }
        public IEnumerable<string> DCFFileNames
        {
            get => dCFFileNames; set
            {
                dCFFileNames = value;
                RaisePropertyChanged();
            }
        }

        public string DCFFileName { get => dCFFileName; set => Set(ref dCFFileName, value); }
        #endregion

        #region Commands
        public ICommand DCFFileOpenClicked { get; set; }
        public ICommand SetImageFilePath { get; set; }
        public ICommand OnGrabBtnClicked { get; set; }
        public ICommand OnDCFListCBBOpened { get; set; }
        public ICommand OnDCFFileListSelected { get; set; }
        public ICommand OnToggleLiveBtn { get; set; }
        public ICommand OnImageSaveBtnClicked { get; set; }
        public ICommand TextBoxEnterPressed { get; set; }
        public string DefaultImageSavePath { get; set; }
        #endregion

        public VMCameraView()
        {
            if (!IsInDesignMode)
            {
                InitDependenceAttribute();
                InitLineScanCamera();
                InitCommand();


                App.Current.Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;

                MessengerInstance.Register<OnImageSavePathChangedMessenger>(this, OnImageSavePathChanged);
            }
        }

        private void OnImageSavePathChanged(OnImageSavePathChangedMessenger obj)
        {
            DefaultImageSavePath = obj.ChangedImageSavePath;
        }

        private void InitDependenceAttribute()
        {
            DependencyMap = new Dictionary<string, List<string>>();

            foreach (var property in GetType().GetProperties())
            {
                var attributes = property.GetCustomAttributes<DependsOnPropertyAttribute>();

                foreach (var dependsAttr in attributes)
                {
                    if (dependsAttr == null)
                        continue;

                    foreach (var dependence in dependsAttr.Dependences)
                    {
                        if (!DependencyMap.ContainsKey(dependence))
                            DependencyMap.Add(dependence, new List<string>());

                        DependencyMap[dependence].Add(property.Name);
                    }
                }
            }
        }

        private void InitLineScanCamera()
        {
            if (!IsInDesignMode)
            {
                LineScanCamera = new MCam();

                LineScanCamera.OnGrab += LineScanCamera_OnLive;
                MessengerInstance.Send(new OnCameraInitializedMessenger { InitializedCamera = LineScanCamera });
            }
        }

        private void InitCommand()
        {
            OnGrabBtnClicked = new RelayCommand(OnGrabBtnClickedAction);
            OnToggleLiveBtn = new RelayCommand<bool>(OnToggleLiveBtnAction);
            OnImageSaveBtnClicked = new RelayCommand(OnImageSaveBtnClickedAction);
            TextBoxEnterPressed = new RelayCommand<TextBox>(OnTextBoxEnterPressed);
            MessengerInstance.Register<LoadDCFMessenger>(this, OnLoadDCF);
            MessengerInstance.Register<OpenCameraMessenger>(this, OnOpenCamera);
        }

        private void OnOpenCamera(OpenCameraMessenger obj)
        {
            LineScanCamera.CameraOpen(obj.SelectedCamNode);
            IsDCFSet = true;
            ImageFormat = LineScanCamera.ImageFormat;

            RaisePropertyChanged(nameof(IsCameraGrabbable));
        }

        private void OnTextBoxEnterPressed(TextBox element)
        {
            element.GetBindingExpression(TextBox.TextProperty).UpdateSource();
        }

        private void OnLoadDCF(LoadDCFMessenger obj)
        {
            LineScanCamera.DCFFileSet(obj.DCFFilePath);
            IsDCFSet = true;
            ImageFormat = LineScanCamera.ImageFormat;
            DCFFileName = obj.DCFFilePath;

            RaisePropertyChanged(nameof(IsCameraGrabbable));
        }

        private void OnImageSaveBtnClickedAction()
        {
            if (Image == null)
                return;


            if (string.IsNullOrEmpty(DefaultImageSavePath))
            {
                SaveFileDialog dialog = new SaveFileDialog()
                {
                    AddExtension = true,
                    InitialDirectory = DefaultImageSavePath,
                    Filter = DialogFilterGenerator(false, FileDialogFormat_Bitmap)
                };

                var result = dialog.ShowDialog();

                if (result.HasValue && result.Value)
                {
                    SaveImageToPath(dialog.FileName);
                }
            }
            else
            {
                SaveImageToPath($@"{DefaultImageSavePath}\{DateTime.Now:yy-mm-ss}.bmp");
                MessageBox.Show("기본 저장 폴더에 저장되었습니다.");
            }
        }

        private void SaveImageToPath(string path)
        {
            BmpBitmapEncoder encoder = new BmpBitmapEncoder();

            using (FileStream fs = new FileStream(path, FileMode.CreateNew))
            {
                encoder.Frames.Add(BitmapFrame.Create(Image));
                encoder.Save(fs);
            }
        }

        private void Dispatcher_ShutdownStarted(object sender, EventArgs e)
        {
            if (LineScanCamera != null)
                LineScanCamera.CloseCam();
        }

        private void LineScanCamera_OnLive(object sender, BitmapSource e)
        {
            e.Freeze();
            App.Current.Dispatcher.Invoke(() =>
            {
                Image = e;
            });
        }

        private void OnToggleLiveBtnAction(bool obj)
        {
            IsCameraGrabbing = !obj;
            LineScanCamera.ToggleLive(!obj);
        }


        private void OnGrabBtnClickedAction()
        {
            if (LineScanCamera.IsDigitizerSet)
            {
                if (int.TryParse(ScanLineLength, out int result))
                {
                    IsCameraGrabbing = true;
                    LineScanCamera.Grab(result);
                    IsCameraGrabbing = false;
                }
            }
        }




        #region AddOn
        private string DialogFilterGenerator(bool createAllFileFilter, params string[] filters)
        {
            StringBuilder dialogFilter = new StringBuilder();

            foreach (var filter in filters)
            {
                if (dialogFilter.ToString().Contains(filter))
                    continue;

                dialogFilter.Append(filter);
                if (!filters.ElementAt(filters.Count() - 1).Equals(filter))
                    dialogFilter.Append('|');
            }

            if (createAllFileFilter)
                dialogFilter.Append('|' + FileDialogFormat_AllFiles);

            return dialogFilter.ToString();
        }

        public override void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.RaisePropertyChanged(propertyName);

            if (!DependencyMap.ContainsKey(propertyName))
                return;


            foreach (var dependentPropertyName in DependencyMap[propertyName])
            {
                RaisePropertyChanged(dependentPropertyName);
            }
        }
        #endregion
    }

    [ValueConversion(typeof(bool), typeof(string))]
    public class BoolToSetStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!bool.TryParse(value.ToString(), out bool result))
                return null;
            else
            {
                if (result)
                    return "Set";
                else
                    return "Not Set";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(bool), typeof(bool))]
    public class BoolInverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!bool.TryParse(value.ToString(), out bool result))
                return null;
            else
            {
                return !result;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(object), typeof(string))]
    public class ObjectToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType.IsEnum)
                return Enum.Parse(targetType, value.ToString());

            return System.Convert.ChangeType(value, targetType);
        }
    }

    public class MultiBooleanAndConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values.All(x => (bool)x);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}


