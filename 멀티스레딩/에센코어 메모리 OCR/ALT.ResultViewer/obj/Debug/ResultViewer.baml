   M S B A M L   `   `   `  ����K  GALT.ResultViewer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null3clr-namespace:ALT.ResultViewerALT.ResultViewer  5      #    ALT.ResultViewer.ResultViewer   5      W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      0localclr-namespace:ALT.ResultViewer     5      l hSystem.Windows.Controls.DataVisualization.Toolkit, Version=4.0.0.0, Culture=neutral, PublicKeyToken=nullJtkAhttp://schemas.microsoft.com/winfx/2006/xaml/presentation/toolkit  5        � 	Resources  5
      (   "ALT.ResultViewer.FontSizeConverter   fontSizeConverter��%   &	          5   
   6"   5      .��5      �� -   5         ��Name$ header��5      $��Stretch=�6   ��5   
   �� 5      -   5         ��Name$ camNameTextBlock��5      
���������5      `� 5       `� 	Converter# [  6#   .1�5      ��  �� Path$ ActualWidth�6"    �� ElementName$ header��65   �� 5      $ ActualHeight�6"   $ header��66   5      5      .��5   
   �  � ColumnDefinitions 5      �� 5      $��* �6#   �� 5      $	��200 �6#   .+�5      >  8System.Windows.Controls.DataVisualization.Charting.Chart     LegendStyle 5      �� 5        �� 
TargetType6  0System.Windows.Controls.DataVisualization.Legend"  6   .�5!      �� 	 �� Property	 � ��
 �� Value$
 0��62   �� 5"      	 � ��$
 0��63   5#         Axes 5&      C  =System.Windows.Controls.DataVisualization.Charting.LinearAxis  D  >System.Windows.Controls.DataVisualization.Charting.DisplayAxis   ShowGridLines . =  7System.Windows.Controls.DataVisualization.Charting.Axis   Orientation$ Y=�69   D  >System.Windows.Controls.DataVisualization.Charting.NumericAxis   Minimum�  �System.Windows.Controls.DataVisualization.Charting.NullableConverter`1[[System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]$ 0 6I      Maximum$ 10 6U   
�������   Interval$ 1 6y   E	  ?System.Windows.Controls.DataVisualization.Charting.DateTimeAxis	  5'      -   5         	 Name$ dateTimeAxis��5'   &    .  	  Interval$ 1 6R    	  IntervalType$ Months=�6_   $ X=�6v   
�������55         Series. 57      C
  =System.Windows.Controls.DataVisualization.Charting.LineSeries
  -   5         
 Name$ 
lineSeries��57       H  BSystem.Windows.Controls.DataVisualization.Charting.DataPointSeries   DataPointStyle 58      �� 59      F  @System.Windows.Controls.DataVisualization.Charting.LineDataPoint"  6    .�5:      �� 	 �@J�Opacity$
 0Q�68   5;      5=      5>      � 5?      $��1��6    � RowDefinitions 5@      �� 5A      �� 5B      �� 5C      .+�5E      �� -   5        $ totalTextBlock��5E   �   $��0��6   $��10,0,0,0q�6)    �� TextAlignment$
 Left=�6;   $
��Left=�6P   $��Center=�6k   
���������5F      `� 5G      # [  6'   .1�5H      �� $ ActualWidth�6&   $ header��69   �� 5I      $ ActualHeight�6&   $ header��6:   5J      5L      �� 5N      -   5        $ okTextBlock��5N   �   $��1��6   $��10,0,0,0q�6)   $
 Left=�6;   $
��Left=�6P   $��Center=�6k   
���2�2���5O      `� 5P      # [  6'   .1�5Q      �� $ ActualWidth�6&   $ header��69   �� 5R      $ ActualHeight�6&   $ header��6:   5S      5U      �� 5W      -   5        $ ngTextBlock��5W   �   $��2��6   $��10,0,0,0q�6)   $
 Left=�6;   $
��Left=�6P   $��Center=�6k   
��� E����5X      `� 5Y      # [  6'   .1�5Z      �� $ ActualWidth�6&   $ header��69   �� 5[      $ ActualHeight�6&   $ header��6:   5\      5^      5a      5b      5d      5e      