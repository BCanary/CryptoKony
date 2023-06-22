using CryptoKony_Client.Modules;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using static System.Net.WebRequestMethods;

namespace CryptoKony_Client.Frames
{
    /// <summary>
    /// Логика взаимодействия для CertificateFrame.xaml
    /// </summary>
    public partial class CertificateFrame : Page
    {
        MainWindow mainWindow;
        RSAParameters private_key;
        string public_key;
        public CertificateFrame(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            InitializeComponent();
        }

        /*
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Private Key Files (.key)|*.key";

            openFileDialog.ShowDialog();
            pathToPrivateKeyBox.Text = openFileDialog.FileName;
        }
        */

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = "certificate";
            saveFileDialog.DefaultExt = "ckcert";
            saveFileDialog.Filter = "Crypto Kony Certificate(*.ckcert)|*.ckcert";

            if (saveFileDialog.ShowDialog() ?? false)
            {
                string path = saveFileDialog.FileName ?? "";
                Certificate.GenerateCertificate(subjectBox.Text, issuerBox.Text, dateBox.SelectedDate.Value.ToString("dd.MM.yyyy HH:mm:ss"), private_key, public_key, path);
            }
        }

        private void ImportKey_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Private Key (*.key)|*.key";

            openFileDialog.ShowDialog();
            

            string path = openFileDialog.FileName ?? "";

            if (path != "")
            {
                keyBox.Text = path;
                using (StreamReader sr = new StreamReader(openFileDialog.FileName ?? ""))
                {
                    string key = "";
                    while (!sr.EndOfStream)
                    {
                        key += (sr.ReadLine() ?? "") + "\n";
                    }

                    private_key = KonyCryptography.GetPrivateKey(key.Replace("-----BEGIN PRIVATE KEY-----\n", "").Replace("\n-----END PRIVATE KEY-----", ""));
                }
            }
        }

        private void ImportPublicKey_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Public Key (*.pub)|*.pub";

            openFileDialog.ShowDialog();


            string path = openFileDialog.FileName ?? "";

            if (path != "")
            {
                publicKeyBox.Text = path;
                using (StreamReader sr = new StreamReader(openFileDialog.FileName ?? ""))
                {
                    string key = "";
                    while (!sr.EndOfStream)
                    {
                        key += (sr.ReadLine() ?? "") + "\n";
                    }

                    public_key = key;
                }
            }
        }
    }
}
