   M S B A M L   `   `   `  ���� U  QMessanger.KeysGeneratorWPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullG(clr-namespace:Messanger.KeysGeneratorWPFMessanger.KeysGeneratorWPF  e7clr-namespace:Messanger.KeysGeneratorWPF.MVVM.ViewModel)Messanger.KeysGeneratorWPF.MVVM.ViewModel  +    %Messanger.KeysGeneratorWPF.MainWindow   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     :local(clr-namespace:Messanger.KeysGeneratorWPF     M	viewModel7clr-namespace:Messanger.KeysGeneratorWPF.MVVM.ViewModel       � Title$  
MainWindow��$	��300��$	��300�����#2F3136 � WindowStyle$
 None=� � AllowsTransparency$
 True�� � 
ResizeMode$ NoResize=�$��1q�
������� � DataContext C  =Messanger.KeysGeneratorWPF.MVVM.ViewModel.MainWindowViewModel  .���  � RowDefinitions �� $��30 ��� �� .+��� -   $��0��$��0�����#252525._�� $��Stretch=�.+��� $��Keys Generator��
�������$
��Bold"�$��15(�$��8 0 0 0q��� $��Right=� �� Orientation$ 
Horizontal=�.+��� -   $��35��$��35��$	��___��
�������$��Right=�
������ $��0q��� -   $��35��$��35��$	��⨉��
�������$��Right=�
������ $��0q��� $��1��$��0 10 0 0q�.+��� $	��Key��$��Center=�$��Medium"�
�������$��16(�$��8 0 0 0q��� 
�������$��13(�$��40��$��SemiBold"�
������ $��2q�
�������$��5 5 5 5q�$��	10 0 10 0q� �� 
CaretBrush
 �����$��Center=����� *Key+ �� UpdateSourceTrigger$ PropertyChanged=�.��	 � 	Resources	 
 �� 
TargetType �� Property �� Value��%:   '��       �� "
 ��.���  �@��CornerRadius$ 10��.���� $��2��$��40��$	��Top=�$��Generate key��
�������$��14(�$��SemiBold"����#229ED9$��5 5 5 5q����� *GenerateButtonCommand+$ PropertyChanged=�	 ��%:   '��       �� "
 ��.���  �@��CornerRadius$ 14��