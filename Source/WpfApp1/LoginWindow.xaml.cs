using System;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace WpfApp1
{
    /// <summary>
    /// Login window
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// settingsButton_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void settingsButton_Click(object sender, RoutedEventArgs e)
        {
            var settings = new SettingsWindow();
            settings.ShowDialog();
        }

        /// <summary>
        /// loginButton_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            //var server = ConfigurationManager.AppSettings["server"];
            //var database = ConfigurationManager.AppSettings["database"];
            var username = usernameTextBox.Text;
            var password = passwordBox.Password;

            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            if (rememberCheckBox.IsChecked == true)
            {
                config.AppSettings.Settings["username"].Value = username;
                var passwordInBytes = Encoding.UTF8.GetBytes(password);
                var entropy = new byte[20];
                using (var rng = new RNGCryptoServiceProvider())
                {
                    rng.GetBytes(entropy);
                }
                var cypherText = ProtectedData.Protect(passwordInBytes, entropy, DataProtectionScope.CurrentUser);

                config.AppSettings.Settings["password"].Value = Convert.ToBase64String(cypherText);
                config.AppSettings.Settings["entropy"].Value = Convert.ToBase64String(entropy);

            }
            else
            {
                config.AppSettings.Settings["username"].Value = "";
                config.AppSettings.Settings["password"].Value = "";
                config.AppSettings.Settings["entropy"].Value = "";
            }
            config.Save(ConfigurationSaveMode.Minimal);
            ConfigurationManager.RefreshSection("appSettings");

            var db = new MyStoreEntities4();
            var account = db.Accounts.Find(username);
            var checkpassword = (account != null) ? account.password : string.Empty;

            if (checkpassword.Equals(password))
            {
                var screen = new MainWindow();
                screen.ShowDialog();
            }
            else
                MessageBox.Show("Tên đăng nhập không hợp lệ!!!");
        }

        /// <summary>
        /// Window_Loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var server = ConfigurationManager.AppSettings["server"];
            var database = ConfigurationManager.AppSettings["database"];
            var username = ConfigurationManager.AppSettings["username"];
            var encryptedPassword = Convert.FromBase64String(ConfigurationManager.AppSettings["password"]);

            if (encryptedPassword.Length != 0)
            {
                var entropy = Convert.FromBase64String(ConfigurationManager.AppSettings["entropy"]);

                var passwordInBytes = ProtectedData.Unprotect(encryptedPassword, entropy, DataProtectionScope.CurrentUser);
                var password = Encoding.UTF8.GetString(passwordInBytes);
                usernameTextBox.Text = username;
                passwordBox.Password = password;
            }
        }
    }
}
