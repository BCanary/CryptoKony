using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;

namespace CryptoKony_Client.Modules
{
    public class SignatureInfoData
    {
        public string Signature { get; set; }
        public string Date { get; set; }
        public string Certificate { get; set; }
    }

    public class SignatureData
    {
        public string Signature { get; set; }
    }

    internal class Signature
    {
        public static Certificate certificate = new Certificate();
        public static string date = "";

        public Signature()
        {

        }
        
        public static bool Sign(string file_path, string new_path, string certificate, string private_key)
        {
            try
            {
                using (ZipArchive doc = ZipFile.Open(file_path, ZipArchiveMode.Update))
                {
                    // Проверяем. Если подпись уже есть, удаляем и перетераем её
                    ZipArchiveEntry? document_sig = doc.GetEntry("signature.xml");

                    if (document_sig != null)
                    {
                        document_sig.Delete();
                    }

                    ZipArchiveEntry? document_info_sig = doc.GetEntry("signature_info.xml");

                    if (document_info_sig != null)
                    {
                        document_info_sig.Delete();
                    }

                    // Добавляем информацию о сертификате и подписи
                    ZipArchiveEntry signature = doc.CreateEntry("signature_info.xml");
                    XmlSerializer serializer = new XmlSerializer(typeof(SignatureInfoData));
                    SignatureInfoData sign_file = new SignatureInfoData
                    {
                        Certificate = certificate,
                        Date = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")
                    };

                    using (Stream stream = signature.Open())
                    {
                        using (XmlWriter xw = XmlWriter.Create(stream))
                        {
                            serializer.Serialize(xw, sign_file);
                        }
                    }
                }

                File.Copy(file_path, new_path, true);

                byte[] documentBytes = File.ReadAllBytes(new_path);

                string base64 = private_key.Replace("-----BEGIN PRIVATE KEY-----\n", "").Replace("\n-----END PRIVATE KEY-----", "");
                RSAParameters key = KonyCryptography.GetPrivateKey(base64);

                string sign_data = KonyCryptography.SignData(key, documentBytes);

                // Копируем файл
                using (ZipArchive doc = ZipFile.Open(new_path, ZipArchiveMode.Update))
                {
                    ZipArchiveEntry signature = doc.CreateEntry("signature.xml");
                    XmlSerializer serializer = new XmlSerializer(typeof(SignatureData));
                    SignatureData sign_data_file = new SignatureData
                    {
                        Signature = sign_data,
                    };

                    using (Stream stream = signature.Open())
                    {
                        using (XmlWriter xw = XmlWriter.Create(stream))
                        {
                            serializer.Serialize(xw, sign_data_file);
                        }
                    }
                    // Добавляем подпись
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public static bool Verify(string file_path)
        {
            //bool is_signature_deleted = false;
            //XmlDocument xml = new XmlDocument();
            MemoryStream copy = new MemoryStream();
            string? text_signature = "";

            SignatureInfoData sign_info_data;
            SignatureData sign_data;

            try
            {
                using (ZipArchive doc = ZipFile.Open(file_path, ZipArchiveMode.Read))
                {
                    ZipArchiveEntry? document_sig = doc.GetEntry("signature.xml");
                    if (document_sig == null)
                    {
                        throw new Exception("В данном файле нет подписи");
                    }

                    using (XmlReader reader = XmlReader.Create(document_sig.Open()))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(SignatureData));
                        sign_data = (SignatureData)serializer.Deserialize(reader);
                    }

                    document_sig = doc.GetEntry("signature_info.xml");
                    if (document_sig == null)
                    {
                        throw new Exception("В данном файле нет информации о подписи");
                    }
                    using (XmlReader reader = XmlReader.Create(document_sig.Open()))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(SignatureInfoData));
                        sign_info_data = (SignatureInfoData)serializer.Deserialize(reader);
                    }
                }

                // Создание дубликата данного архива
                using (FileStream fs = new FileStream(file_path, FileMode.Open))
                {
                    fs.CopyTo(copy);
                }
                
                // Удаляем оттуда подпись
                using (ZipArchive doc = new ZipArchive(copy, ZipArchiveMode.Update))
                {
                    ZipArchiveEntry? document_sig = doc.GetEntry("signature.xml");
                    document_sig.Delete();
                }

                certificate = new Certificate(sign_info_data.Certificate, true);
                certificate.Validate();

                date = sign_info_data.Date;

                text_signature = sign_data.Signature; //xml.SelectSingleNode($"/Signature/Data").InnerText;

                byte[] documentBytes = copy.ToArray() ; //File.ReadAllBytes(file_path);
                string base64 = certificate.public_key.Replace("-----BEGIN PUBLIC KEY-----\n", "").Replace("\n-----END PUBLIC KEY-----", "");
                RSAParameters key = KonyCryptography.GetPublicKey(base64);


                byte[] signature = Convert.FromBase64String(text_signature);

                bool result = KonyCryptography.VerifyData(key, documentBytes, signature);
                if (result)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            /* Восстанавливаем сигнатуру если удалили её из файла /*
            if (is_signature_deleted)
            {
                using (ZipArchive doc = ZipFile.Open(file_path, ZipArchiveMode.Update))
                {
                    ZipArchiveEntry document_sig = doc.CreateEntry("signature.xml");

                    //temp.Seek(0, SeekOrigin.Begin);
                    using (Stream fs = document_sig.Open())
                    {
                        //temp.CopyTo(fs);
                        xml.Save(fs);
                    }
                }
            }*/
        }
    }
}
