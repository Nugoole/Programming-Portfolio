   M S B A M L   `   `   `  ���� K  GALT.CVL.Controls, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullI)clr-namespace:ALT.CVL.Controls.DictionaryALT.CVL.Controls.Dictionary  3clr-namespace:ALT.CVL.ControlsALT.CVL.Controls  #    ALT.CVL.Controls.CamViewerWPF   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     b ^GalaSoft.MvvmLight.Platform, Version=5.4.1.0, Culture=neutral, PublicKeyToken=5f873c45e98af8a1-Custom http://www.galasoft.ch/mvvmlight  :d2http://schemas.microsoft.com/expression/blend/2008  4dict)clr-namespace:ALT.CVL.Controls.Dictionary  c _System.Windows.Interactivity, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Bi:http://schemas.microsoft.com/expression/2010/interactivity  *localclr-namespace:ALT.CVL.Controls  Dmc;http://schemas.openxmlformats.org/markup-compatibility/2006    � 	Resources   �� MergedDictionaries �� Source;   5ALT.CVL.Controls.Dictionary.ParameterTemplateSelector   paramTemplateSelector9   3ALT.CVL.Controls.Dictionary.ReverseBooleanConverter  ReverseBooleanConverter/  )ALT.CVL.Controls.MultiBooleanAndConverter  BooleanAndConverter  GridSplitterPreviewStyle �� Property �� Value  gridSplitterStyle �� 
TargetType �� 
TargetType	 �� X1	 �� X2		 �� Y1	
 �� Y2��  �� $ DictionaryUC.xaml?��� $ DictionaryButton.xaml?�%�  &	        &	      &	 
     &	      &	 F     7        �� .���  � �� �� .R��� ���	#80000000�� " ��.���  � ��# � ����  �@��PreviewStyle8   ��  � �� �� " ��.R��� 
������ #��z��#��z��._�� .+��� $��Center=�$��Center=�
���2�2�$	��1.5��$ 1��$ 1��$	 0��$
 30���� $
��Left=�$��Center=�
���2�2�$	��1.5��$ 1��$ 1��$	 0��$
 25���� $��Right=�$��Center=�
���2�2�$	��1.5��$ 1��$ 1��$	 0��$
 25�� � DataContext    ALT.CVL.Controls.VMCamera  .��� -    �Name$
 grid�� � RowDefinitions �� $��50 ���  � ColumnDefinitions �� $	��250 ��� $
��Auto ��� .+��  �� $	��100 ��� .+��� $N��Hpack://application:,,,/ALT.CVL.Controls;Component/Resource/초록색.png���� $��1��$	��3,0q�$��Stretch=�$��Center=�
������� �� DisplayMemberPath$ CamInfo.CamName��$��16(����� *AvailableCameras+ �� SelectedValue �� *CamNameToConnect+�� $��2��$��	0,0,150,0q� �� Orientation$ 
Horizontal=�.+��� $��50��
���2�2�$
��Grab��  buttonStyle#��[ ���� *
GrabOnce+{��� *IsLive+ �� 	Converter# [ �� $��10��d� $��50��$��0,1,0,-1q�$
��Live�� d� 	IsChecked$ falseY�  toggleButtonLive#���  �� $��10���� $	��100��
�������$��WhiteBalance��#��[ ���� *WhiteBalance+{�`�  `� 	Converter# [ .1��� # [  �� Path$ IsMono��� # [ $ IsLive��� $��0��$��2��$��1��$��10��$��Center=�$��Stretch=�
�������
���   � �� ShowsPreview . #���  �� $��2��$��0,0,20,0q�$��Right=�$��Center=�
������� �� TextAlignment$
 Left=����� *FPS+ �� NotifyOnSourceUpdated .  �� StringFormat$ FPS : {0:F3}��!   ALT.CVL.Controls.DisplayWPF  -     Name$ uCMain��$��1��$��2��   ImageFilePath �� *Image+� $��1��$��0�� �� �� $	��150 �.+��� -    ��Name$ listBox��$	��2,5q�$��Stretch=�#��[  ���� *
Parameters+����  �� Color$ 	AliceBlue��� $��1�� �� $��1* ��� $��3* �.+��  �� �� �� �� .+��� $��Center=�$��Center=�$��20(�
�������$��X���� $��1��$��Center=�$��Center=�
����������� *CursorPosition.X+ �� ElementName$ uCMain��$ {0:F0}���� $��2��$��Center=�$��Center=�$��20(�
�������$��Y���� $��3��$��Center=�$��Center=�
����������� *CursorPosition.Y+$ uCMain��$ {0:F0}��� $��1��$��3�� �� �� ��  �� �� .+��� $��0��$��0��$��Center=�$��Center=�$��20(�
���  ��$	��Red���� $��1��$��0��$��Center=�$��Center=�$��20(�
��� � �$��Green���� $��2��$��0��$��Center=�$��Center=�$��20(����	#FF2D4BD3�� ���	#FF2C52FF$
�Blue���� $��0��$��1��$��Center=�$��Center=�
����������� *ColorFromCursor.R+$ uCMain���� $��1��$��1��$��Center=�$��Center=�
����������� *ColorFromCursor.G+$ uCMain���� $��2��$��1��$��Center=�$��Center=�
����������� *ColorFromCursor.B+$ uCMain��