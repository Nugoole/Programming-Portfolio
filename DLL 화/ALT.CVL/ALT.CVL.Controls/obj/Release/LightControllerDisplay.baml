   M S B A M L   `   `   `  ���� K  GALT.CVL.Controls, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:ALT.CVL.ControlsALT.CVL.Controls  I)clr-namespace:ALT.CVL.Controls.DictionaryALT.CVL.Controls.Dictionary  O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem =#clr-namespace:ALT.CVL.Controls.UtilALT.CVL.Controls.Util  -    'ALT.CVL.Controls.LightControllerDisplay   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     0localclr-namespace:ALT.CVL.Controls     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     8rd)clr-namespace:ALT.CVL.Controls.Dictionary     6sys&clr-namespace:System;assembly=mscorlib     4util#clr-namespace:ALT.CVL.Controls.Util     -     >�Name$  ucThis�� � 	Resources 0   *ALT.CVL.Controls.Util.DoubleToIntConverter   DoubleToIntConverter��%   &	          .���  � ColumnDefinitions �� $	��150 ��� $��* �.+�� $��0�� � RowDefinitions �� $��84 ��� $��* �.+��� -    ��Name$
 Logo��$��0��$3��-/ALT.CVL.Controls;component/Resource/logo.png��� $��1��  �� Source�� $A ;/ALT.CVL.Controls;component/Dictionary/StyleDictionary.xaml?�%     �� �� �� �� �� ��  �� $��* ��� $��3* �.+��� $��0��$��2��$��Center=�$��Center=����� *
Foreground+ � DataContext �� *
Controller+���� *	Channel+ �� StringFormat$ Controller Name : ER{0}S���� $��1��$��0��$��Center=�$��Center=�$��IP : ������ *
Foreground+�� $��1��$��1��$��Stretch=�$��Center=�
������   textBoxStyle1#���   �� *
Controller+���� *ConnectionParam.IP+�� $��2��$��0��$��Center=�$��Center=�$��Port : ������ *
Foreground+�� $��2��$��1��$��Stretch=�$��Center=�
������ #���   �� *
Controller+���� *$"Controller.ConnectionParam.TCPPort+�� $��1��$��Stretch=�
������ $��Disabled=�$��Disabled=� �� *
Controller+���� *Values+ �� TargetNullValue Z� 	 �� FallbackValue	 Z� ���� .R�A� $	��Top=�
 A� Rows
 �� *Items.Count+ �� RelativeSource ��  �� AncestorType$ ListBoxL�����  �� 
TargetType" ��.���  �� Property � �� �� Value$
 auto������ .R�� $��Stretch=� �� $��* ��� $��3* ��� $��* �.+��� $��0��$��Stretch=�$��Center=����� *
Foreground+ �� ElementName$ ucThis������ *Name+#   ALT.CVL.Controls.PulseDisplay    Name$ pulseDisplay��$��1��$��Center=�$��Stretch=�   	LineColor �	#FF72CC72   MaxValue$
 2048Q���Z�    CurrentValue �� *Value+ �� 	Converter# [   �� Mode$ TwoWay=��� $��2��$��Center=�$��Center=�0   *ALT.CVL.Controls.Util.InputBindingsManager+   $UpdatePropertySourceWhenEnterPressed � ������ *
Foreground+$ ucThis������ *CurrentValue+$ pulseDisplay��