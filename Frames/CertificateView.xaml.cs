using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace CryptoKony_Client.Frames
{
    /// <summary>
    /// Логика взаимодействия для CertificateView.xaml
    /// </summary>
    public partial class CertificateView : Page
    {
        List<Modules.Certificate> certificateChain = new List<Modules.Certificate>();
        private Modules.Certificate cr;
        public CertificateView()
        {
            cr = new Modules.Certificate();
            InitializeComponent();
        }

        public void enableElements()
        {
            CheckButton.IsEnabled = true;
            CertificateTree.IsEnabled = true;
        }
        
        public void setCertificate(Modules.Certificate cr)
        {
            this.cr = cr;
            enableElements();
            showCertificate();
            CertificateTree.Items.Clear();
            certificateChain.Clear();

            CertificateTree.Items.Add(cr.certificate_info["КОМУ ВЫДАН"]);
            certificateChain.Add(cr);

            CertificateTree.SelectedIndex = 0;
            Modules.Certificate next_cr = cr.parent_certificate;
            while (next_cr != cr)
            {
                CertificateTree.Items.Add(next_cr.certificate_info["КОМУ ВЫДАН"]);
                certificateChain.Add(next_cr);
                cr = next_cr;
                next_cr = next_cr.parent_certificate;
            }
        }

        public void showCertificate()
        {
            CertBox.Text = "";

            CertBox.Text = cr.getRawCertificateInfo();
            CertBox.Text += "\n [SHA256 HASH]: " + cr.ComputeSHA256();
            CertBox.Text += "\n[SIGNATURE]\n" + cr.sign;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Crypto Kony Certificate(*.ckcert)|*.ckcert";

            Nullable<bool> result = fileDialog.ShowDialog();
            if (result == null || !File.Exists(fileDialog.FileName))
            {
                return;
            }

            string cr_path = fileDialog.FileName;
            try
            {
                //cr = new Modules.Certificate(cr_path);
                setCertificate(new Modules.Certificate(cr_path));
            }
            catch (Modules.CertificateException err)
            {
                string error_text = $"В сертификате {err.certificate.certificate_info["КОМУ ВЫДАН"]} произошла следующая ошибка:\n{err.Message}";
                MessageBox.Show(error_text, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Check_Click(object sender, RoutedEventArgs e)
        {
            if (cr.sign == "")
            {
                MessageBox.Show("Нечего проверять", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            try
            {
                cr.Validate();
                MessageBox.Show("Сертификат является действительным!", "Проверка", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Modules.CertificateException err)
            {
                string error_text = $"В сертификате {err.certificate.certificate_info["КОМУ ВЫДАН"]} произошла следующая ошибка:\n{err.Message}";
                MessageBox.Show(error_text, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ToParent_Click(object sender, RoutedEventArgs e)
        {
            CertBox.Text = "";

            try
            {
                cr = cr.parent_certificate;
            }
            catch (Modules.CertificateException err)
            {
                string error_text = $"В сертификате {err.certificate.certificate_info["КОМУ ВЫДАН"]} произошла следующая ошибка:\n{err.Message}";
                MessageBox.Show(error_text, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                CertBox.Text = cr.getRawCertificateInfo();
                CertBox.Text += "\n[SHA256 HASH]: " + cr.ComputeSHA256();
                CertBox.Text += "\n[SIGNATURE]\n" + cr.sign;
            }
        }

        private void Verify_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool sign_hash = cr.VerifySign();
                if (sign_hash)
                {
                    MessageBox.Show($"Успешно", "Подпись", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show($"Сертификат не подтверждён!", "Подпись", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Modules.CertificateException err)
            {
                string error_text = $"В сертификате {err.certificate.certificate_info["КОМУ ВЫДАН"]} произошла следующая ошибка:\n{err.Message}";
                MessageBox.Show(error_text, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CertificateTree_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CertificateTree.SelectedIndex < certificateChain.Count && CertificateTree.SelectedIndex != -1)
            {
                cr = certificateChain[CertificateTree.SelectedIndex];
                showCertificate();
            }
        }
    }
}
