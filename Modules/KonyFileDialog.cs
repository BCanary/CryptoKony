using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoKony_Client.Modules
{
    class FileExtension
    {
        public static readonly string DOCX = "Document (*.docx)|*.docx";
        public static readonly string CERTIFICATE = "Crypto Kony Certificate (*.ckcert)|*.ckcert";
        public static readonly string PUBLIC_KEY = "Public Key (*.pub)|*.pub";
        public static readonly string PRIVATE_KEY = "Private Key (*.key)|*.key";
    }

    internal class KonyFileDialog
    {
        public static string Open(string filter)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = filter;

            bool? result = fileDialog.ShowDialog();
            if (result == null || !File.Exists(fileDialog.FileName))
            {
                return "";
            }

            return fileDialog.FileName;
        }

        public static bool Save(string filename, string extenstion, string data, string filter)
        {
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.Filter = filter;
            fileDialog.FileName = filename;
            fileDialog.DefaultExt = extenstion;

            bool? result = fileDialog.ShowDialog();
            if (result == null)
            {
                return false;
            }

            using (StreamWriter sw = new StreamWriter(fileDialog.FileName))
            {
                sw.Write(Encoding.UTF8.GetBytes(data));
            }

            return true;
        }
    }
}
