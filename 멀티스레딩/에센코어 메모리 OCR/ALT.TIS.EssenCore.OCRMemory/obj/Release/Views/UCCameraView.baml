   M S B A M L   `   `   `  ���� a  ]Cognex.VisionPro.Controls, Version=71.2.0.0, Culture=neutral, PublicKeyToken=ef0f902af9dee505�gclr-namespace:Cognex.VisionPro.DSCameraSetup.Implementation.Internal;assembly=Cognex.VisionPro.Controls6Cognex.VisionPro.DSCameraSetup.Implementation.Internal  V RALT.TIS.EssenCore.OCRMemory, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null_4clr-namespace:ALT.TIS.EssenCore.OCRMemory.ViewModels&ALT.TIS.EssenCore.OCRMemory.ViewModels U/clr-namespace:ALT.TIS.EssenCore.OCRMemory.Views!ALT.TIS.EssenCore.OCRMemory.Views 4   .ALT.TIS.EssenCore.OCRMemory.Views.UCCameraView   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     |Internalgclr-namespace:Cognex.VisionPro.DSCameraSetup.Implementation.Internal;assembly=Cognex.VisionPro.Controls     X TXceed.Wpf.Toolkit, Version=4.0.0.0, Culture=neutral, PublicKeyToken=3e4669d2f30244f42tk)http://schemas.xceed.com/wpf/xaml/toolkit  =vm4clr-namespace:ALT.TIS.EssenCore.OCRMemory.ViewModels  ;local/clr-namespace:ALT.TIS.EssenCore.OCRMemory.Views    � DataContext  ��    Locator �� Source# [   � 	Resources ?  9ALT.TIS.EssenCore.OCRMemory.ViewModels.NullCheckConverter  NullCheckConverterC  =ALT.TIS.EssenCore.OCRMemory.ViewModels.PMAlignResultConverter  pmResultConverter  NumericUpDownStyle1 �� 
TargetTypeJ   DCognex.VisionPro.DSCameraSetup.Implementation.Internal.NumericUpDown �� Property �� Value �� 
TargetType �� Padding J� SnapsToDevicePixels��%�   &	       &	      &	 
         �� "  .���  � ��$ Center=���  � ��$ 10q���  � �� �� "  .R��� #��z��#��z��#��z��# z�� . ._��� #��z��#��z��.��� 	 � ColumnDefinitions	 �� �� 
 � RowDefinitions
 �� $��3* ��� .+��� $��10q����� *Display.OCWfhDisplay[0]+� $��1��	 �� $	��350 ��� .+��� $��	10,0,0,10q�$��0��
�������$	��0.5q����� 
�������$��Pattern��$��20(�.��� 
 �� �� �� 	 �� �� �� .+��� $��5q�$��0��$��0��$��Search Region��  ButtonStylemainBtn#���  $��13(����� *Tool.CmdPatternRegion+�� $��5q�$��1��$��0��$��Train Region��#���  $��15(����� *Tool.CmdPatternTrainRegion+�� $��5q�$��2��$��0��$��Train��#���  $��15(����� *Tool.CmdTrainPattern+�� $��3q�$��0��$��1��#���  $��Pattern Region������ *Tool.CmdKLEVVSearchRegion+�� $��3q�$��1��$��1��#���  $��Train Region������ *Tool.CmdKLEVVTrainRegion+�� $��3q�$��2��$��1��#���  $��Train Pattern������ *Tool.CmdTrainKLEVVPattern+�� $��3q�$��0��$��2��#���  $��Direction Region������ *$"Tool.CmdStickDirectionSearchRegion+�� $��3q�$��1��$��2��#���  $��Direction Train������ *#!Tool.CmdStickDirectionTrainRegion+�� $��3q�$��2��$��2��#���  $��Train Direction������ *Tool.CmdTrainStickDirection+� $��
10,0,10,10q�$��1��
 �� �� $��60 �.+�� $��1��	 �� $��2* ��� $��1* �.+��� $��30��$��20(�$��10,0,0,0q����� *Tool.CapacityCharacters+ �� Mode$ TwoWay=� �� BindsDirectlyToSource .  �� UpdateSourceTrigger$ PropertyChanged=�{��� *Tool.CurrentOCRType+ �� 	Converter# [ �� $��7q�$��3��$��1��$��Train��#���  ���� *Tool.CmdTrainSegmentedOCR+{��� *Tool.CurrentOCRType+# [ �� $��0��
�������$	��0.5q����� 
�������$��OCR & Buttons��$��20(�.��� 
 �� �� �� 	 �� �� �� �� �� .+��� $��3q�$��0��$��0��#���  $	��P/N������ *Tool.CmdPartNoRegion+�� $��3q�$��0��$��1��#���  $��Capacity������ *Tool.CmdStickCapacityRegion+�� $��3q�$��0��$��2��#���  $��Speed������ *Tool.CmdSpeedRegion+�� $��3q�$��0��$��3��$��
OCR Region��#���  $��15(����� *Tool.CmdCapacityRegion+�� $��3q�$��0��$��4��#���  $��Barcode������ *Tool.CmdBarcodeRegion+�� $��3q�$��1��$��3��$��2��$��2��#���  $
��Run!��
��� ������� *Tool.CmdRun+�� $��3q�$��2��$��1��#���  $��Segment������ *Tool.CmdSegmentOCRChars+{��� *Tool.CurrentOCRType+# [ ��  �� Orientation$ 
Horizontal=�$y�Hidden=�.+��� $��SearchRegion��
 � Tag$ 0��  EditButtonControlTemplate#���  ���� *Tool.CmdKLEVVSearchRegion+�� $��
P/N Region��$ 1��#���  ���� *Tool.CmdPartNoRegion+�� $��Capacity Region��$ 2��#���  ���� *Tool.CmdStickCapacityRegion+�� $��Speed Region��$ 3��#���  ���� *Tool.CmdSpeedRegion+�� $��BarcodeRegion��$ 4��#���  ���� *Tool.CmdBarcodeRegion+�� $��Segment��$ 5��#���  ���� *Tool.CmdSegmentMemOCRChars+�� $��StickDirectionRegion��$ 6��#���  ���� *$"Tool.CmdStickDirectionSearchRegion+�� $��StickDirectionTRegion��$ 7��#���  ���� *#!Tool.CmdStickDirectionTrainRegion+�� $��Region1��$ 8��#���  �� $��Region1��$ 9��#���  �� $��1��$ 
Horizontal=�$y�Hidden=�.+��� $��TrainRegion��$ 0��  TrainButtonControlTemplate#���  ���� *Tool.CmdKLEVVTrainRegion+�� $��	Train OCR��$ 1��#���  ���� *Tool.CmdTrainSegmentedOCR+�� $��Train Pattern��$ 2��#���  ���� *Tool.CmdTrainKLEVVPattern+�� $	��Run��$ 3��#���  ���� *Tool.CmdRun+�� $��Train StickDirection��$ 4��#���  ���� *Tool.CmdTrainStickDirection+�� $��Train��$ 5��#���  �� $��Train��$ 6��#���  �� $��Train��$ 7��#���  �� $��Train��$ 8��#���  �� $��Train��$ 9��#���  � $��1��	 �� �� �� 
 �� $	��100 ��� .+���   ButtonStyleMainMenu#���  $��Trigger��$��10q����� *Camera.CmdOneShot+d� -    d�Name$ toggleButtonContinue��$��Continue��$��10q�$��1�� d� 	IsChecked �� *Camera.IsContinueMode+���� *Camera.CmdContinueShot+���� *	IsChecked+ �� ElementName$ toggleButtonContinue��d� -   $ toggleButtonOnline��$��Online��$��10q�$��2������ *Camera.CmdOnline+���� *	IsChecked+$ toggleButtonOnline��� $��1��$��3��$��0,0,0,0q�	 �� $
��Auto ��� $	��100 ��� $	��100 ��� $	��100 ��� $	��100 ��� 
 �� $��50 ��� $��50 ��� $��50 ��� $��50 ��� $��50 ��� $��50 ��� $��50 ��� .+��� $��Exposure��  TblStyleStandard#���    $��1��#���     Step$
 0.01~�   DecimalPlaces$ 2��   Value �� *
Exposure+$ TwoWay=��� $��2�� 	 TextBlockStyleMainMenu#��� 	 $��10q�$	��Set������ *CmdSetCameraParams+�� $��6��$��1��$��
Initialize��$��20(����� *Tool.CmdInitialize+�� $��1��$��1��
������� �� TextAlignment$ Center=�$��Center=����� *Tool.CapacityResult+�� $��2��$��1��
�������$ Center=�$��Center=����� *Tool.PartNo[0]+�� $��2��$��2��
�������$ Center=�$��Center=����� *Tool.Capacity[0]+�� $��2��$��3��
�������$ Center=�$��Center=����� *Tool.Speed[0]+�� $��2��$��4��
�������$ Center=�$��Center=����� *Tool.StickDirection[0]+�� $��2��$��5��
�������$ Center=�$��Center=����� *Tool.Barcode[0]+�� $��3��$��1��
�������$ Center=�$��Center=����� *Tool.PartNo[1]+�� $��3��$��2��
�������$ Center=�$��Center=����� *Tool.Capacity[1]+�� $��3��$��3��
�������$ Center=�$��Center=����� *Tool.Speed[1]+�� $��3��$��4��
�������$ Center=�$��Center=����� *Tool.StickDirection[1]+�� $��4��$��2��
�������$ Center=�$��Center=����� *Tool.TacTime+�� $��5��$��1��
�������$��First Fix Enable�� �� *Tool.FirstFixtureEnable+$ TwoWay=��� $��5��$��2��
�������$��Second Fix Enable�� �� *Tool.SecondFixtureEnable+$ TwoWay=��� $��7��$��0��$��5��$��0,0,0,50q����� $��Main & SubFixture��
�������$��20(�.��� 	 �� �� .+�� $��0��
 �� �� �� .+��� #���  $��Main TrainRegion������ *Tool.CmdMainTrainRegion+�� $��1��#���  $��Train Main Pattern������ *Tool.CmdTrainMainPattern+� $��1��
 �� �� �� .+��� #���  $��Sub TrainRegion������ *Tool.CmdSubTrainRegion+�� $��1��#���  $��Sub SearchRegion������ *Tool.CmdSubSearchRegion+�� $��2��#���  $��Train SubPattern������ *Tool.CmdTrainSubPattern+� $��1��$��1��	 �� $��2* ��� $	��120 ��� $��* �.+��� $��0�� �� SelectedValuePath$ PMTool��
������ ���� *Tool.PMTools+ �� SelectedValue �� *Tool.SelectedTool+���� .R�� $��30��	 �� $	��100 ��� $	��100 ��� $	��150 ��� $	��150 �.+��� $��Stretch=�$��20(�
������ 
������ 
����������� *PMTool.Name+�� $��1��$��Center=�$��Center=�
����������� *PMTool.Results+# [ �� $��2��$	��150������ *Tool.Spaces+# [   �� *'%PMTool.SearchRegion.SelectedSpaceName+$ TwoWay=��� $��3��$��50�� �� *Enable+�� $��1��
������ ���� .R�A�  A� Columns$ 2��
������ .@��� $��30��$��35��$��+��$��24(�$
��Bold"�$��Center=�$��Center=�
������ 
���2�2�
���2�2����� *Tool.CmdAddPMTool+�� $��30��$��35��$��-��$��28(�$
��Bold"�$��Center=�$��Center=�
������ 
���  ��
���  ������ *Tool.CmdRemovePMTool+�� $��3q�$��40��$��35��$��SR������ *Tool.CmdPMToolSearchRegion+�� $��3q�$��40��$��35��$��TR������ *Tool.CmdPMToolTrainRegion+�� $��3q�$��40��$��35��$��Train������ *Tool.CmdTrainPMTool+�� $��3q�$��40��$��35��$	��Run������ *Tool.CmdRunPMTools+� $��2��	 �� �� .+��� $��0��$
��Save������ *Tool.CmdSave+�� $��1��$
��Load������ *Tool.CmdLoad+