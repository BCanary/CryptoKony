using CryptoKony_Client.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CryptoKony_Client.Frames
{
    /// <summary>
    /// Логика взаимодействия для Encrypt.xaml
    /// </summary>
    public partial class Encrypt : Page
    {
        MainWindow mainWindow;
        public Encrypt(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            InitializeComponent();
        }

        private void Encrypt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string base64 = mainWindow.genkey.PublicKeyBox.Text.Replace("-----BEGIN PUBLIC KEY-----\n", "").Replace("\n-----END PUBLIC KEY-----", "");
                RSAParameters key = KonyCryptography.GetPublicKey(base64);
                ResultBox.Text = KonyCryptography.EncryptText(key, InputBox.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Decrypt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string base64 = mainWindow.genkey.PrivateKeyBox.Text.Replace("-----BEGIN PRIVATE KEY-----\n", "").Replace("\n-----END PRIVATE KEY-----", "");
                RSAParameters key = KonyCryptography.GetPrivateKey(base64);
                ResultBox.Text = KonyCryptography.DecryptText(key, InputBox.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
