   M S B A M L   `   `   `  ????B  >WpfApp1, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!clr-namespace:WpfApp1WpfApp1  5   	       WpfApp1.LoginWindow   5   	   _ [MaterialDesignThemes.Wpf, Version=4.5.0.0, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes  5   	   ^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   'localclr-namespace:WpfApp1     5   	   -   5          ? Title$  LoginWindow??5	   	   $	??450??6   $	??800??6*   .??5
      ?? .+?5   
   ?? -   5        $??usernameTextBox??5   ?   )  #MaterialDesignThemes.Wpf.HintAssist   Hint$ UserName??5         
HelperText$ Please input user name??5      
??????  ?? Left$	 180??5   +   
 ?? Top$ 60??6=   $??Center=?6M   $
??Left=?6q   $??Center=?6?   $??35??6?   $	??220??6?   $??5??6?   $	??Top=?6?   G? 5   
   -   5        $??passwordBox??5   ?   $ Password??5      .  (MaterialDesignThemes.Wpf.TextFieldAssist   HasClearButton . $ At least 8 characters??5      
?????? $	 180??6Y   $	 110??6k   $??Center=?6|   $
??Left=?6?   $??Center=?6?   $??36??6?   $	??220??6?   $??5??6	  ?? 5   
   -   5         ??Name$ loginButton??5      $??80??6&   $??35??61   $ 52??6=   $	 164??6N   $??Login??6_   6o   ?? 5   
   -   5         ??Name$ rememberCheckBox??5      $??Remember me??6-   $ 43??6C   $	 214??6T   ?? 5   
   -   5        $ settingsButton??5      $??80??6)   $??35??64   $??Settings??6@   $	 683??6S   $	 375??6e   $y?Hidden=?6?   ?? 5   
   $??
Login User??6   $	 343??6%   $ 10??67   ?? 5   
   $ 43??6   $ 60??6!   $??	Username:??61   ?? 5   
   $ 43??6   $	 115??6!   $??	Password:??62   $??Center=?6F   $	??Top=?6c   5      5      