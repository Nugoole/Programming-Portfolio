   M S B A M L   `   `   `  ����V  RALT.TIS.EssenCore.OCRMemory, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullU/clr-namespace:ALT.TIS.EssenCore.OCRMemory.Views!ALT.TIS.EssenCore.OCRMemory.Views  5      4    .ALT.TIS.EssenCore.OCRMemory.Views.UCRecipeView   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Alocal/clr-namespace:ALT.TIS.EssenCore.OCRMemory.Views     5        � DataContext  5   7   ��  �� Mode$ OneWay=�   Locator �� Source# [  .��5	      �  � RowDefinitions 5
   
   �� 5      $��2* �6   �� 5      $��2* �6   �� 5      $��1* �6    � ColumnDefinitions 5   
   �� 5      $��1* �6   �� 5      $��1* �6   �� 5      $��1* �6   �� 5      $��1* �6   .+�5   
   �� -   5         ��Name$ listView��5      $��2��6%   $��50q�69   $��2��6E   $��1��6V     listViewContainerStyle#���  6f   ��6�   �� *FileIO.RecipeData.OCRecipe+ �� SelectedIndex 6�   �� * FileIO.RecipeListSelectedIndex+�� 5   
   $��+��6   $��
10,0,0,100q�6   $��3��61   $��Bottom=�6A   $
��Left=�6\   $��40��6w   $��50��6�     ButtonStyleMainMenu#���  6�   ��6�   �� *FileIO.CmdAddRecipeDlgOpen+6�   �� 5   
   $��	Up Button��6   #���  6%   $��	10,0,0,20q�6S   $��3��6f   $��Bottom=�6v   $
��Left=�6�   $��40��6�   $	��150��6�   ��6�   �� *FileIO.CmdRecipeSelectingUP+�� 5   
   $��Down Button��6   #���  6'   $��
10,20,-2,0q�6U   $��3��6i   $	��Top=�6y   $
��Left=�6�   $��40��6�   $	��150��6�   $��1��6�   ��6�   �� *FileIO.CmdRecipeSelectingDown+�� 5   
   $
��SAVE��6   #���  6    $��2��6N   $��70��6^   $��2��6j   $	��150��6w   ��6�   �� *FileIO.CmdRecipeSave+��6�   �� *SelectedItem+ �� ElementName$ listView���� 5   
   $
��LOAD��6   #���  6    $��70��6N   $	��150��6Z   $��2��6f   $��1��6s   ��6�   �� *FileIO.CmdRecipeLoad+��6�   �� *SelectedItem+$ listView���� 5   
   $��-��6   #���  6   $��
70,0,0,100q�6K   $��3��6_   $��Bottom=�6o   $
��Left=�6�   $��40��6�   $��50��6�   ��6�   �� *FileIO.CmdDeleteRecipe+5      5      