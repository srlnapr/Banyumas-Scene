   M S B A M L   `   `   `  ����H  DBanyumasScene, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null?$clr-namespace:BanyumasScene.ControlsBanyumasScene.Controls  5   	   -clr-namespace:BanyumasSceneBanyumasScene  5   	       BanyumasScene.Navbar   5   	   -ctls$clr-namespace:BanyumasScene.Controls  5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   -localclr-namespace:BanyumasScene     5   	   -   5        $
��home��5        � Title$  
MainWindow��5	   	   $	��650��6   $
��1000��6)    � WindowStartupLocation$ CenterScreen=�5
   	    � WindowStyle$
 None=�6.   
������  � AllowsTransparency .  � 
ResizeMode$ NoResize=�5   	   $��Poppins)�6   .��5      ��    PrimaryBackgroundColor#���   6    �� CornerRadius$ 8��5      ._�5   
   �  � ColumnDefinitions 5      �� 5      $
��Auto �6#   �� 5      $��* �6#   .+�5      �� $��0��6     SecundaryBackgroundColor#���  5      $ 8 0 0 8��5      ._�5      � -   5         �Name$ GridNav��5      $��60��6(    � RowDefinitions 5      �� 5      $��60 �6(   �� 5      $��* �6(   .+�5       � $��0��6   .+�5!      �� $��14 0 0 0q�5"      $��32��5#      $��24��6*   $
��Left=�5$      ��5%      �� 5&   "   	 �� ImageSource$	 Resources/logo.png��6-   
 j� Stretch$
 Uniform=�6N   5(      �� 5*      $��BMSCENE��6      PrimaryTextColor#���  5+       $
��Bold"�5,       $��16(�5-       $��Center=�5.       $
��Left=�5/       $��60 0 0 0q�50       51      �� 52      $��1��6!   $��
-1 40 0 20q�6.   .+�53      �� -   5         ��Name$ btnHome��53   !     PopupButtonStyle#���  62   .��54      %   BanyumasScene.Controls.MenuItem     	GroupName$ MenuItem��6,     PrimaryBlueColor   IndicatorBrush# �  6A     home   Icon# �  6u      	IconWidth$ 16��6�      Text$
 Home��6�   $��Center=�6�   55      �� 56      -   5        $ btnDashboard��56   !   #���  67   .��57        $ MenuItem��6,   # �  6A     	dashboard# �  6u   $ 16��6�   $ Favorite��6�   $��Center=�6�   58      �� 59      -   5        $ 
btnKuliner��59   !   #���  65   .��5:        $ MenuItem��6,   # �  6A     Kuliner# �  6u   $ 16��6�   $ Kuliner��6�   $��Center=�6�   5;      �� 5<      -   5        $ 	btnWisata��5<   !   #���  64   .��5=        $ MenuItem��6,   # �  6A    
 Wisata# �  6u   $ 16��6�   $ Wisata��6�   $��Center=�6�   5>      �� 5?      -   5        $ btnHotel��5?   !   #���  63   .��5@        $ MenuItem��6,   # �  6A    		 Hotel# � 	 6u   $ 16��6�   $ Hotel��6�   $��Center=�6�   5A      �� 5B      -   5        $ btnEvent��5B   !   #���  63   .��5C        $ MenuItem��6,   # �  6A    	
 Event# � 
 6u   $ 19��6�   $
 Foto��6�   $��Center=�6�   5D      �� 5E      -	   5        $ btnFoto��5E   !   #���  62   .��5F        $ MenuItem��6,   # �  6A     Foto# �  6u   $ 17��6�   $ Rating��6�   $��Center=�6�   5G      �� 5H      -
   5        $ 	btnReview��5H   !   #���  64   .��5I        $ MenuItem��6,   # �  6A    
 Review# �  6u   $ 19��6�   $ Akun Manager��6�   $��Center=�6�   5J      �� 5K      -   5        $ 	btnUpload��5K   !   #���  64   .��5L        $ MenuItem��6,   # �  6A     settings# �  6u   $ 18��6�   $ Logout��6�   $��Center=�6�   5M      5N      � 5T      -   5         �Name$ Popup��5T       � AllowsTransparency .  � HorizontalOffset$ 0��6G    � VerticalOffset$ 10��6\   .��5U      &    BanyumasScene.Controls.UserPopup  -   5          Name$ Header��5U   )   #���  69   5V      5Y      5Z      � 5\      $��1��6    5]      �� 5^      $��60 �6$   �� 5_      $��* �6$   .+�5b      d� -   5         d�Name$ Tg_Btn��5b      $��1��6/   $��Center=�6?     iToggleButtonStyle#���  6Z   $
��Left=�6�   $��0q�6�   
������ $��
10,15,0,15q�6�   $��30��6�   $��30��6�    � Triggers 5c      :� 5d       :� RoutedEvent$ ToggleButton.Unchecked��6'   .[�5e      �� .��5f   "   �� -   5         ��Name$ HideStackPanel��5f   -   .��5g   &   W�  �� 
TargetName$ GridNav��66    �� TargetProperty$ Width�6V    g� 	BeginTime$ 0:0:0X�6x    W� From$	 210X�6�   	 W� To$ 60X�6�    g� Duration$ 0:0:0.5D�6�   5h   '   5i   #   5j      5k      :� 5l      $ ToggleButton.Checked��6'   .[�5m      �� .��5n   "   �� -   5        $ ShowStackPanel��5n   -   .��5o   &   W� $ GridNav��66   $ Width�6V   $ 0:0:0X�6x   $ 60X�6�   $	 210X�6�   $ 0:0:0.5D�6�   5p   '   5q   #   5r      5s      5u      �� 5x      $��0��6   $��Right=�6*   $	��Top=�6F   $��0,5,10,0q�6^   $��30��6p     �� Orientation$  
Horizontal=�6|   $��RightToLeft=�6�   $��2��6�   .+�5y      �� -   5        $ btnClose��5y        IconButtonsStyle#��[ 6/    	 close#���  6Y   
! � Tag$! IsCloseButton��6�   �� 5z      -   5        $ 
btnRestore��5z      $	��5,0q�61   ��5{      �� 5|      " �� 
TargetType"" ��6$   # �� BasedOn## [ 6A   .�5}   "   �� $ �� Property$ � ��  maximize% �� Value#% �  6<   & �� Triggers& 5~   "   �� 5   &   ' �� Binding' 62   �� ( �� ElementName$
( home��) �� Path$) WindowState�* �� Value* 	Maximized6i   .a�5�   *   �� $ � ��  restore#% �  6D   5�   '   5�      5�      �� 5�      -   5        $ btnMinimize��5�      #���  62     minimize#���  6]   6�   5�       � 5�      -   5        $��
fContainer��5�      $��1��5�      $��20q�5�      +  � NavigationUIVisibility$+ Hidden=�5�      5�      �� 5�      -   5        , ��Name$, txtNamaPengguna��5�      $
��Left=�63   $��32��6N   - �� TextWrapping$
- Wrap=�6Z   $�� ��6n   $	��Top=�6v   $	��252��6�   
�������$��18(�6�   $��
306,44,0,0q�6�   {�.  $��2��6�   �� 5�      $	��HAI��6   $
��Left=�6&   $��
269,49,0,0q�6A   $	��Top=�6U   $��48��6m   $��18(�6x   $��2��6�   5�      5�      5�      5�      