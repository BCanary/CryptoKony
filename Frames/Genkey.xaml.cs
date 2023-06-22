using CryptoKony_Client.Modules;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для Genkey.xaml
    /// </summary>
    public partial class Genkey : Page
    {
        MainWindow mainWindow;
        public Genkey(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            

            InitializeComponent();

            //PublicKeyBox.Text = StorageManager.Load("public_key");
            //PrivateKeyBox.Text = StorageManager.Load("private_key");
            //Button_Click(new object(), new RoutedEventArgs());
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            KonyCryptography.generateKeyPair();
            PublicKeyBox.Text = KonyCryptography.format_publicKey;
            PrivateKeyBox.Text = KonyCryptography.format_privateKey;

            //string base64 = PublicKeyBox.Text.Replace("-----BEGIN PUBLIC KEY-----\n", "").Replace("\n-----END PUBLIC KEY-----", "");
            /*
            MessageBoxResult result = MessageBox.Show("Импортировать эти ключи во вкладку шифрование?", "Импорт", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);
            if (result == MessageBoxResult.Yes)
            {
                mainWindow.encrypt.PublicKeyBox.Text = PublicKeyBox.Text;
                mainWindow.encrypt.PrivateKeyBox.Text = PrivateKeyBox.Text;
            }
            */
            //StorageManager.Save("public_key", PublicKeyBox.Text);
            //StorageManager.Save("private_key", PrivateKeyBox.Text);
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog savefile = new SaveFileDialog();
            savefile.FileName = "key";
            savefile.ShowDialog();

            if (savefile.FileName != null)
            {
                using (StreamWriter writer = new StreamWriter(savefile.FileName + ".key"))
                {
                    // Записываем текст в файл
                    writer.WriteLine(PrivateKeyBox.Text);
                }

                using (StreamWriter writer = new StreamWriter(savefile.FileName + ".pub"))
                {
                    // Записываем текст в файл
                    writer.WriteLine(PublicKeyBox.Text);
                }
            }

        }

        private void Import_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog();
            string path = openFileDialog.FileName ?? "";

            if (path != "")
            {
                PublicKeyBox.Text = "";
                PrivateKeyBox.Text = "";
                using (StreamReader sr = new StreamReader(openFileDialog.FileName ?? ""))
                {
                    string key = "";
                    while (!sr.EndOfStream)
                    {   
                        key += (sr.ReadLine() ?? "") + "\n";
                    }

                    if (path.EndsWith(".key"))
                    {
                        PrivateKeyBox.Text = key;
                        RSAParameters public_key = KonyCryptography.GetPublicKey(key.Replace("-----BEGIN PRIVATE KEY-----\n", "").Replace("\n-----END PRIVATE KEY-----", ""));
                        PublicKeyBox.Text = KonyCryptography.FormatPublicKey(public_key);
                    }
                    else if (path.EndsWith(".pub"))
                    {
                        PublicKeyBox.Text = key;
                    }
                }
            }
            //StorageManager.Save("public_key", PublicKeyBox.Text);
            //StorageManager.Save("private_key", PrivateKeyBox.Text);
        }
    }
}
