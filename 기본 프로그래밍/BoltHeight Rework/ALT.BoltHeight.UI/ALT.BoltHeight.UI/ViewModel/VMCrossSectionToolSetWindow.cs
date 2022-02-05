using ALT.BoltHeight.Messenger;
using ALT.DSCamera.Tool;
using Cognex.VisionPro;
using Cognex.VisionPro3D;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace ALT.BoltHeight.UI.ViewModel
{
    public class VMCrossSectionToolSetWindow : ViewModelBase
    {
        private readonly ResourceDictionary DictionaryCrossSection;

        public static List<string> PointTypes
        {
            get
            {
                return null;
            }
        }

        private Dictionary<OperatorType, object> OperatorParameterUI;
        public ObservableCollection<string> Enums
        {
            get
            {
                var enums = new ObservableCollection<string>();

                foreach (var item in System.Enum.GetNames(typeof(Cog3DRangeImageCrossSectionPointTypeConstants)))
                {
                    enums.Add(item);
                }

                return enums;
            }
        }
        public ICogRecord DisplayRecord
        {
            get
            {
                return MdCrossSectionTool.Instance.ProfileRecord;
            }
        }
        public string RegionName { get; set; }
        public IEnumerable Operators
        {
            get
            {
                return MdCrossSectionTool.Instance.GetOperators(RegionName);
            }
        }

        public object CurrentOP
        {
            get
            {
                return MdCrossSectionTool.Instance.GetOperator(RegionName, MdCrossSectionTool.Instance.GetCurrentOPName(RegionName));
            }
        }

        public ObservableCollection<ICogGraphicInteractive> Regions { get; set; }
        public object ParameterSetContent { get; set; }
        public RelayCommand OnLoaded { get; set; }
        public RelayCommand<string> OnOperatorTypeClicked { get; set; }
        public RelayCommand<string> OnSelectedOperatorRemoved { get; set; }
        public RelayCommand<string> OnOperatorSelected { get; set; }

        public VMCrossSectionToolSetWindow()
        {
            OnLoaded = new RelayCommand(OnLoadedAction);
            OnOperatorTypeClicked = new RelayCommand<string>(OnOperatorTypeClickedAction);
            OnSelectedOperatorRemoved = new RelayCommand<string>(OnSelectedOperatorRemovedAction);
            OnOperatorSelected = new RelayCommand<string>(OnOperatorSelectedAction);



            Regions = new ObservableCollection<ICogGraphicInteractive>();
            DictionaryCrossSection = new ResourceDictionary();
            DictionaryCrossSection.Source = new Uri(@"Dictionaries/DictionaryCrossSection.xaml", UriKind.Relative);
            OperatorParameterUI = new Dictionary<OperatorType, object>();

            foreach (var key in DictionaryCrossSection.Keys)
            {
                if (Enum.TryParse(key.ToString(), out OperatorType type))
                {
                    OperatorParameterUI.Add(type, (DictionaryCrossSection[key] as DataTemplate).LoadContent());
                }
            }

            MdCrossSectionTool.Instance.ReDrawProfile += Instance_ReDrawProfile;

            GalaSoft.MvvmLight.Messaging.Messenger.Default.Register<CrossSectionRegionNameSetMessenger>(this, RegionNameSet);
            GalaSoft.MvvmLight.Messaging.Messenger.Default.Register<ParamSaveLoadMessenger<MdCrossSectionTool>>(this, OnCrossSectionToolLoaded);
        }

        private void OnCrossSectionToolLoaded(ParamSaveLoadMessenger<MdCrossSectionTool> obj)
        {
            //TODO: 로드 이후 Region들 활성화가 안됨 이를 해결해야함
            Regions.Clear();

            foreach (ICogGraphicInteractive region in obj.ParamContainer.CrossSectionRegions)
            {
                Regions.Add(region);
            }
        }



        #region EventConnection
        public void OnLineSelectedInComputeDistancePointLineAction(object sender, SelectionChangedEventArgs e)
        {
            object addedItem = null;

            if (e.AddedItems.Count > 0)
                addedItem = e.AddedItems[0];

            if (addedItem != null)
                MdCrossSectionTool.Instance.SetLineInDistancePointLine(RegionName, addedItem.GetType().GetProperty("Name").GetValue(addedItem).ToString());
        }

        public void OnPointSelectedInComputeDistancePointLineAction(object sender, SelectionChangedEventArgs e)
        {
            object addedItem = null;

            if (e.AddedItems.Count > 0)
                addedItem = e.AddedItems[0];

            if (addedItem != null)
                MdCrossSectionTool.Instance.SetPointInDistancePointLine(RegionName, addedItem.GetType().GetProperty("Name").GetValue(addedItem).ToString());
        }

        public void OnPointTypeSelectionChanged()
        {
            MdCrossSectionTool.Instance.RunOperators(RegionName);
            RaisePropertyChanged(nameof(DisplayRecord));
        }
        #endregion

        private void OnOperatorSelectedAction(string opName)
        {
            if (string.IsNullOrEmpty(opName))
                return;

            MdCrossSectionTool.Instance.SetCurrentOPName(RegionName, opName);
            foreach (var item in Enum.GetNames(typeof(OperatorType)))
            {
                if (opName.Contains(item) || opName.Equals(item))
                    ChangeParameterUI((OperatorType)Enum.Parse(typeof(OperatorType), item), opName);
            }

            RaisePropertyChanged(nameof(Enums));
        }

        private void Instance_ReDrawProfile(object sender, EventArgs e)
        {
            RaisePropertyChanged(nameof(DisplayRecord));
        }



        private void OnSelectedOperatorRemovedAction(string toolName)
        {
            MdCrossSectionTool.Instance.RemoveSelectedGraphic(RegionName, toolName);
            RaisePropertyChanged(nameof(Operators));
            RaisePropertyChanged(nameof(DisplayRecord));
        }

        private void RegionNameSet(CrossSectionRegionNameSetMessenger obj)
        {
            RegionName = obj.RegionName;
            MdCrossSectionTool.Instance.ResetParams();
        }

        private void OnOperatorTypeClickedAction(string operatorType)
        {
            if (Enum.TryParse(operatorType, out OperatorType result))
            {

                MdCrossSectionTool.Instance.AddNewOperator(RegionName, result);
                (OperatorParameterUI[result] as FrameworkElement).DataContext = this;

                ParameterSetContent = OperatorParameterUI[result];


                RaisePropertyChanged(nameof(DisplayRecord));
                RaisePropertyChanged(nameof(Enums));
                RaisePropertyChanged(nameof(Operators));
                RaisePropertyChanged(nameof(ParameterSetContent));
            }
        }

        private void ChangeParameterUI(OperatorType type, string opName)
        {
            (OperatorParameterUI[type] as FrameworkElement).DataContext = this;
            ParameterSetContent = OperatorParameterUI[type];

            RaisePropertyChanged(nameof(ParameterSetContent));
        }
        private void OnLoadedAction()
        {
            MdCrossSectionTool.Instance.RunOperators(RegionName);
            RaisePropertyChanged(nameof(DisplayRecord));
        }
    }
}
