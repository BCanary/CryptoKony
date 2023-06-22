using CryptoKony_Client.Modules;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
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

namespace CryptoKony_Client
{
    /// <summary>
    /// Логика взаимодействия для SignFile.xaml
    /// </summary>
    public partial class SignFile : Page
    {
        MainWindow mainWindow;
        public SignFile(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            InitializeComponent();
        }

        private void File_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();

            bool? result = fileDialog.ShowDialog();
            if (result == null || !File.Exists(fileDialog.FileName))
            {
                return;
            }

            PathBox.Text = fileDialog.FileName;
        }

        private void Hash_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //SHA256 sha256 = SHA256.Create();

                byte[] documentBytes = File.ReadAllBytes(PathBox.Text);
                //byte[] data = sha256.ComputeHash(documentBytes);

                //HashBox.Text = BitConverter.ToString(data).Replace("-", "").ToLower();

                string base64 = mainWindow.genkey.PrivateKeyBox.Text.Replace("-----BEGIN PRIVATE KEY-----\n", "").Replace("\n-----END PRIVATE KEY-----", "");
                RSAParameters key = KonyCryptography.GetPrivateKey(base64);

                SignBox.Text = KonyCryptography.SignData(key, documentBytes);

                //base64 = mainWindow.genkey.PublicKeyBox.Text.Replace("-----BEGIN PUBLIC KEY-----\n", "").Replace("\n-----END PUBLIC KEY-----", "");
                //key = KonyCryptography.GetPublicKey(base64);

                //byte[] signature = Convert.FromBase64String(SignBox.Text);

                //VerifyBox.Text = KonyCryptography.VerifyData(key, documentBytes, signature).ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Verify_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                byte[] documentBytes = File.ReadAllBytes(PathBox.Text);
                string base64 = mainWindow.genkey.PublicKeyBox.Text.Replace("-----BEGIN PUBLIC KEY-----\n", "").Replace("\n-----END PUBLIC KEY-----", "");
                RSAParameters key = KonyCryptography.GetPublicKey(base64);

                byte[] signature = Convert.FromBase64String(SignBox.Text);

                VerifyBox.Text = KonyCryptography.VerifyData(key, documentBytes, signature).ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
