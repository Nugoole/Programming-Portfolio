   M S B A M L   `   `   `  ����V  RALT.TIS.EssenCore.OCRMemory, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullU/clr-namespace:ALT.TIS.EssenCore.OCRMemory.Views!ALT.TIS.EssenCore.OCRMemory.Views  5      1    +ALT.TIS.EssenCore.OCRMemory.Views.UCLogView   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Alocal/clr-namespace:ALT.TIS.EssenCore.OCRMemory.Views     5      .��5	      �   � RowDefinitions  5
   
   �� 5      $��50 �6   �� 5      �� 5      $��40 �6    � ColumnDefinitions 5   
   �� 5      �� 5      �� 5      �� 5      .+�5   
   �� $��0��6   $��File I/O Log��6)      TextBlockStyleMainMenu#���   6@   $��5q�6q   �� 5   
   $��1��6   $��Tool Log��6)   #���   6<   $��5q�6m   �� 5   
   $��2��6   $��PLC Communication Log��6)   #���   6I   $��5q�6z   �� 5   
   $��3��6   $��
System Log��6)   #���   6>   $��5q�6o   �� 5   
   $��0��6     LogViewerTemplate#���  6#   $��5q�6R   $��1��6]   ��6j   �� *Log.FileIOLogs+�� 5   
   $��1��6   #���  6#   $��5q�6R   $��1��6]   ��6j   �� *Log.ToolLogs+�� 5   
   $��2��6   #���  6#   $��5q�6R   $��1��6]   ��6j   �� *Log.ComIOLogs+�� 5   
   $��3��6   #���  6#   $��5q�6R   $��1��6]   ��6j   �� *Log.SystemLogs+�� 5   
     ButtonStyleMainMenu#���  6   $��2��6?   $��Clear��6L   $��5q�6\   ��6g   �� *Log.CmdClearFileIOLogs+�� 5   
   #���  6   $��2��6?   $��Clear��6L   $��5q�6\   $��1��6g   ��6w   �� *Log.CmdClearToolLogs+�� 5   
   #���  6   $��2��6?   $��Clear��6L   $��5q�6\   $��2��6g   ��6w   �� *Log.CmdClearComIOLogs+�� 5    
   #���  6   $��2��6?   $��Clear��6L   $��5q�6\   $��3��6g   ��6w   �� *Log.CmdClearSystemLogs+5!      5"      