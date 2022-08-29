using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Windows;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        /// <summary>
        /// SettingsWindow
        /// </summary>
        public SettingsWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// testConnectionButton_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void testConnectionButton_Click(object sender, RoutedEventArgs e)
        {
            var server = serverTextBox.Text;
            var database = databaseTextBox.Text;
            var username = usernameTextBox.Text;
            var password = PasswordTextBox.Password;
            var builder = new SqlConnectionStringBuilder();

            builder.DataSource = server;
            builder.InitialCatalog = database;
            builder.UserID = username;
            builder.Password = password;
        }

        /// <summary>
        /// okButton_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            var server = serverTextBox.Text;
            var database = databaseTextBox.Text;
            var username = usernameTextBox.Text;
            var password = PasswordTextBox.Password;
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            config.AppSettings.Settings["server"].Value = server;
            config.AppSettings.Settings["database"].Value = database;
            config.AppSettings.Settings["username"].Value = username;

            var passwordInBytes = Encoding.UTF8.GetBytes(password);
            var entropy = new byte[20];

            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(entropy);
            }
            var cypherText = ProtectedData.Protect(passwordInBytes, entropy, DataProtectionScope.CurrentUser);
            
            config.AppSettings.Settings["password"].Value = Convert.ToBase64String(cypherText);
            config.AppSettings.Settings["password"].Value = Convert.ToBase64String(entropy);
            config.Save(ConfigurationSaveMode.Minimal);
            ConfigurationManager.RefreshSection("appSettings");
            DialogResult = true;
        }

        /// <summary>
        /// Window_Loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
