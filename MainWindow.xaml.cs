using CryptoKony_Client.Frames;
using CryptoKony_Client.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public CertificateView certificateView = new CertificateView();
        public CertificateFrame certificateFrame;
        public Genkey genkey;
        //public Encrypt encrypt;
        //public SignFile signFile;
        public SignDocument signDocument;
        public VerifySignatureFrame verifySignatureFrame;

        public MainWindow()
        {
            StorageManager.Init();
            InitializeComponent();
            genkey = new Genkey(this);
            //encrypt = new Encrypt(this);
            //signFile = new SignFile(this);
            signDocument = new SignDocument(this);
            certificateFrame = new CertificateFrame(this);
            verifySignatureFrame = new VerifySignatureFrame(this);

            MainFrame.Navigate(certificateView);
        }

        private void ChangeFrameButtonCheck_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(certificateView);
        }

        private void ChangeFrameButtonCertificate_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(certificateFrame);
        }

        private void ChangeFrameButtonGenkey_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(genkey);
        }

        /*
        private void ChangeFrameButtonEncrypt_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(encrypt);
        }
        */

        /*
        private void ChangeFrameButtonHash_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(signFile);
        }
        */

        private void ChangeFrameButtonDocument_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(signDocument);
        }

        private void ChangeFrameButtonVerify_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(verifySignatureFrame);
        }
    }
}
