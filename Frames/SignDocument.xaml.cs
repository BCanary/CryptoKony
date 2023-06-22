using CryptoKony_Client.Modules;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
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
using System.Xml;
using static System.Net.Mime.MediaTypeNames;

namespace CryptoKony_Client.Frames
{
    /// <summary>
    /// Логика взаимодействия для SignDocument.xaml
    /// </summary>
    public partial class SignDocument : Page
    {
        MainWindow mainWindow;

        public SignDocument(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            InitializeComponent();
        }

        private void Hash_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = FileExtension.DOCX;
            saveFileDialog.DefaultExt = "docx";
            saveFileDialog.FileName = System.IO.Path.GetFileNameWithoutExtension(PathBox.Text)+".signed.docx";

            if (saveFileDialog.ShowDialog() ?? false)
            {
                string? new_path = saveFileDialog.FileName;
                if (Signature.Sign(PathBox.Text, new_path, File.ReadAllText(certPathBox.Text), File.ReadAllText(KeyPathBox.Text)))
                {
                    MessageBox.Show("Файл успешно подписан", "Подпись", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void File_Click(object sender, RoutedEventArgs e)
        {
            PathBox.Text = KonyFileDialog.Open(FileExtension.DOCX);
        }

        private void Cert_Click(object sender, RoutedEventArgs e)
        {
            certPathBox.Text = KonyFileDialog.Open(FileExtension.CERTIFICATE);
        }

        private void Key_Click(object sender, RoutedEventArgs e)
        {
            KeyPathBox.Text = KonyFileDialog.Open(FileExtension.PRIVATE_KEY);
        }
    }
}
