   M S B A M L   `   `   `  ���� I  ELineScanViewer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:LineScanViewerLineScanViewer  c _System.Windows.Interactivity, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35qPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.InteractivitySystem.Windows.Interactivity &     LineScanViewer.View.UCConfigView   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     .localclr-namespace:LineScanViewer     ^iPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity       � DataContext  �� *Config+   Locator �� Source# [  .���  J� RenderTransformOrigin$ 0.407,0.503&�
������� � RowDefinitions �� $	��100 ��� $��1 ��� .+�� $��1��$��2������  �� EndPoint$ 0.5,1&� �� 
StartPoint$ 0.5,0&�.���  � Color$ White��� $ White�� � Offset$ 0.172Q�.+�F� ����/ĵ��� �@�kZ��� 	$
��Left=�$��10��$��10,0,0,0q� �� Stretch$
 Fill=����	#FFF0F0F0$	��Top=�$
��1512������ $ 0.5,1&�$ 0.5,0&�.��� $ Black��$ 0Q�� $ 	#FF969696��$ 1Q�� $��2��$��	0,0,0,100q� �� �� �� �� 	 � ColumnDefinitions	 �� $��1* ��� $��2* ��� $��1* �.+��� $��DCF File Folder��$��0,0,40,0q�$��25(�$��Center=�$��Center=�$��Right=��� $��SaveImg Path��$��0,0,40,0q�$��25(�$��Center=�$��1��$��Right=�$��Center=��� $��16(�$��Center=�
 �� 
IsReadOnly
 . $��40,49q�$��1��$��1��$��Center=�$��40������ *ImageSavePath+�� $��40��$��70��$	��...��$��20(����	#FFC1C1C1$��0q�$��2��$��1��$
��Left=����� *OpenImageSaveFolder+F� ���ЀB���� рB��;�	$
��Left=�$��527.034��$��0,10,0,0q�$��4��$
 Fill=����	#FF999999$	��Top=�$��1��$��1���� $��16(�$��Center=�
 . $��40,49q�$��1��$��0��$��Center=�$��40������ *	DCFFolder+�� $��40��$��70��$	��...��$��20(����	#FFC1C1C1$
��0,49q�$��2��$��0��$
��Left=����� *OpenDCFFileFolder+� $��0q�.+��� $��Config Settings��$��Center=�$��Center=�$��40(�$
��Bold"�