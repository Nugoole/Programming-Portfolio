   M S B A M L   `   `   `  ���� I  ELineScanViewer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null/clr-namespace:LineScanViewerLineScanViewer  $    LineScanViewer.View.MainWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     c _System.Windows.Interactivity, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35Bi:http://schemas.microsoft.com/expression/2010/interactivity  (localclr-namespace:LineScanViewer    � Title$  ALT LineScanViewer��$
��1040��$
��1920�� � WindowState$ 	Maximized=�
������� � WindowStyle$
 None=� � Icon$7 1/LineScanViewer;component/Resources/초록색.png�� � WindowStartupLocation$ CenterScreen=� � DataContext �� *Main+   Locator �� Source# [  .��� 
������� � RowDefinitions �� $	��120 ���  � ColumnDefinitions �� $	��300 ��� .+�� $��0��$��0�����	#FF1DCA1D.+��� $!��../Resources/logo-white.png��	 �� BitmapScalingMode$	 HighQuality=�
 �� EdgeMode$
 Aliased=�� -    �Name$
 grid��$��0��$��1�����	#FF353942.+��� $ ��ALT SYSTEM LineScan Viewer��$
��Left=�$��Center=�$!��Microsoft JhengHei UI Light)����	#FFCECFD1$��20(�$��60,0,0,0q����� *ActualHeight+ �� ElementName$
 grid�� �� Mode$ OneWay=���  �� Orientation$ 
Horizontal=�$��Right=�.+���  � Cursor$
 Hand������ *ActualHeight+$
 grid��$ OneWay=����� *ActualHeight+$ OneWay=� �� RelativeSource �� *Self+��Z� ���� *OnMinimizeBtnClicked+��Z� .���� $6��0/LineScanViewer;component/Resources/minimize.png���� $
 Hand������ *ActualHeight+$
 grid��$ OneWay=����� *ActualHeight+$ OneWay=� �� *Self+��Z� ��Z� ���� *OnCloseBtnClicked+.���� $��70��$��70��$3��-/LineScanViewer;component/Resources/close.png��� $��1��$��0�����	#FF353942 �� $��80 ��� .+�� $��0������  �� Color$ 	#FF1DCA1D�� �� Opacity$	 0.5Q�.+��� $��Center=�$��Center=�$
 Aliased=�
����������� *DateTimeNow+� $��1�� �� �� �� �� �� �� �� �� .+�d� $��0��  ToggleButtonMain#���  $��CONNECT��.  (System.Windows.Interactivity.Interaction   Triggers /  )System.Windows.Interactivity.EventTrigger     	EventName$ Checked��.  (System.Windows.Interactivity.TriggerBase   Actions. 6  0System.Windows.Interactivity.InvokeCommandAction     Command �� *CmdOnConnect+����  �� ImageSource$6 0/LineScanViewer;component/Resources/icon_con.png�� j� Stretch$
 None=� �� RelativeTransform V� .~���  �� CenterY$	 0.5Q� �� CenterX$	 0.5Q���  �� CenterY$	 0.5Q� �� CenterX$	 0.5Q���  �� CenterY$	 0.5Q� �� CenterX$	 0.5Q�T� 	�   RadioButtonMain#���  $��CAMERA��$��1��  	� 	GroupName$  MainMenu������ *CmdLoadCameraView+���� $ ..\Resources\icon_ca.png��$
 None=�	� $	��DCF��$��70(�$��2��#���  $  MainMenu������ *CmdLoadModelView+���� $ ..\Resources\icon_mo.png��$
 None=�	� $��CONFIG��$��50(�$��3��#���  $  MainMenu������ *CmdLoadConfigView+���� $ ..\Resources\icon_set.png��$
 None=�	� $��STORAGE FOLDER��$��50(�$��4��#���  $  MainMenu������ *CmdLoadStorageView+���� $ ..\Resources\icon_fo.png��$
 None=�� $��1��$��1��� $��1��$��1��.+��� $��2��$��1��$��0.2,50,0,0.2q�$��2��$��2������ *
UCMainView+