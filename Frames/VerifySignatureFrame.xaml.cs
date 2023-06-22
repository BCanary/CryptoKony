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
    /// Логика взаимодействия для VerifySignatureFrame.xaml
    /// </summary>
    public partial class VerifySignatureFrame : Page
    {
        MainWindow mainWindow;
        public VerifySignatureFrame(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            InitializeComponent();
        }

        private void File_Click(object sender, RoutedEventArgs e)
        {
            PathBox.Text = KonyFileDialog.Open(FileExtension.DOCX);
        }

        private void Verify_Click(object sender, RoutedEventArgs e)
        {
            if(Signature.Verify(PathBox.Text))
            {
                MessageBox.Show("Подпись действительна", "Подпись", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Подпись недействительна", "Подпись", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            certificateBox.Text = $"[Документ был подписан {Signature.date}]\n";
            certificateBox.Text += Signature.certificate.getRawCertificateInfo();
            certificateButton.IsEnabled = true;
        }

        private void Certificate_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.certificateView.setCertificate(Signature.certificate);
            mainWindow.certificateView.showCertificate();
            mainWindow.MainFrame.Navigate(mainWindow.certificateView);
        }
    }
}
