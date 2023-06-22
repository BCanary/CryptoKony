using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Security.Cryptography;
using System.Windows;
using System.Printing;
using System.Diagnostics.Eventing.Reader;
using System.Transactions;
using System.Globalization;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Shapes;

namespace CryptoKony_Client.Modules
{
    public class CertificateException : Exception
    {
        public Certificate certificate;
        public CertificateException()
        {
            certificate = new Certificate();
        }

        public CertificateException(string message) : base(message)
        {
            certificate = new Certificate();
        }

        public CertificateException(Certificate certificate, string message) : base(message)
        {
            this.certificate = certificate;
        }
    }

    enum CertBlock
    {
        None = -1,
        Certificate = 0,
        OpenKey = 1,
        //Hash = 2,
        Sign = 3
    }

    public class Certificate
    {
        public string public_key = "";
        public Dictionary<string, string> certificate_info = new Dictionary<string, string> {};
        //public Dictionary<string, string> hash_info = new Dictionary<string, string> { };
        public string sign = "";
        public string computed_hash = "";
        public Certificate parent_certificate;

        public static List<Certificate> trusted_certificates = new List<Certificate> { };
        private static bool trusted_inited = false;

        public static void InitTrustedCertificates()
        {
            if (trusted_inited)
            {
                return;
            }
            trusted_inited = true;
            //string path = @"C:\Users\mikec\source\repos\CryptoKony Client\CryptoKony Client\bin\Debug\net6.0-windows\Trusted";
            string path = PATH.TRUSTED; 
            string[] files = Directory.GetFiles(path, "*.ckcert");

            foreach (string file in files)
            {
                Certificate certificate = new Certificate(file);
                certificate.Validate();
                trusted_certificates.Add(certificate);
            }
        }

        public static void GenerateCertificate(string subject, string issuer, string expires, RSAParameters key, string public_key, string out_file)
        {
            using (StreamWriter sw = new StreamWriter(out_file))
            {
                var rsa = new RSACryptoServiceProvider();
                rsa.ImportParameters(key);
                
                string cert = "[СЕРТИФИКАТ]\n"
                + $"КОМУ ВЫДАН={subject}\n"
                + $"КЕМ ВЫДАН={issuer}\n"
                + $"СОЗДАН={DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}\n"
                + $"ИСТЕКАЕТ={expires}\n"
                + $"ID={Guid.NewGuid().ToString()}\n"
                + "[ОТКРЫТЫЙ КЛЮЧ]\n"
                + $"{public_key}";

                string data = KonyCryptography.SignData(rsa.ExportParameters(true), Encoding.UTF8.GetBytes(cert.Replace("\n", "")));

                sw.WriteLine("-----НАЧАЛО СЕРТИФИКАТА-----");
                sw.WriteLine(cert);
                sw.WriteLine("[ПОДПИСЬ]");
                sw.WriteLine($"{data}");
                sw.WriteLine("-----КОНЕЦ СЕРТИФИКАТА-----");
            }
        }

        public Certificate()
        {
            parent_certificate = this;
        }

        public Certificate(string filename, bool istext=false)
        {
            InitTrustedCertificates();
            if (istext)
            {
                getFromText(filename);
            }
            else
            {
                getFromFile(filename);
            }
            computed_hash = ComputeSHA256();
            if (certificate_info["КЕМ ВЫДАН"] == certificate_info["КОМУ ВЫДАН"])
            {
                string path = PATH.TRUSTED;
                string[] files = Directory.GetFiles(path, "*.ckcert");
                string text;
                if (istext)
                {
                    text = filename;
                }
                else
                {
                    text = File.ReadAllText(filename);
                }
                foreach (var i in files)
                {
                    if (File.ReadAllText(i).Replace("\r", "").Replace("\n", "") == text.Replace("\r", "").Replace("\n", ""))
                    {
                        parent_certificate = this;
                        return;
                    }
                }
                throw new CertificateException(this, $"Сертификат является самоподписанным - но не доверенным");
            }
            else
            {
                foreach (var certificate in trusted_certificates)
                {
                    if (certificate_info["КЕМ ВЫДАН"] == certificate.certificate_info["КОМУ ВЫДАН"])
                    {
                        parent_certificate = certificate;
                    }
                }
            }
            if (parent_certificate == null)
            {
                parent_certificate = this;
                //throw new CertificateException(this, $"Данный сертификат не является доверенным.");
            }
        }

        public void Validate()
        {
            // Если это не корневой сертификат и при этом его родитель это он
            if (certificate_info["КЕМ ВЫДАН"] != certificate_info["КОМУ ВЫДАН"] && parent_certificate==this)
            {
                throw new CertificateException(this, $"Не найден доверенный сертификат {certificate_info["КЕМ ВЫДАН"]}");
            }
            // Проверяем хэш и подпись
            if (!VerifySign())
            {
                throw new CertificateException(this, $"Хэш сертификата и хэш полученный из подписи не совпадают: {computed_hash} != {VerifySign()}");
            }

            // Проверяем время
            DateTime now = DateTime.Now;
            DateTime created = DateTime.ParseExact(certificate_info["СОЗДАН"], "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            DateTime expires = DateTime.ParseExact(certificate_info["ИСТЕКАЕТ"], "dd.MM.yyyy HH:mm:ss", CultureInfo.InvariantCulture);
            if (now < created)
            {
                throw new CertificateException(this, $"Сертификат ещё не вступил в силу: {created}");
            }
            if (now > expires)
            {
                throw new CertificateException(this, $"Сертификат истёк: {expires}");
            }
        }

        public string getRawCertificateInfo()
        {
            string raw_certificate_info = "";

            raw_certificate_info += "[СЕРТИФИКАТ]\n";
            foreach (var item in certificate_info)
            {
                raw_certificate_info += item.Key + "=" + item.Value + "\n";
            }

            raw_certificate_info += "[ОТКРЫТЫЙ КЛЮЧ]\n";
            raw_certificate_info += public_key;

            return raw_certificate_info;
        }

        public bool VerifySign()
        {
            // Проверяем подпись ключом родительского сертификата
            RSAParameters key = KonyCryptography.GetPublicKey(parent_certificate.public_key.Replace("-----BEGIN PUBLIC KEY-----\n", "").Replace("\n-----END PUBLIC KEY-----", ""));
            return KonyCryptography.VerifyData(key, Encoding.UTF8.GetBytes(getRawCertificateInfo().Replace("\n", "")), Convert.FromBase64String(sign));
        }

        public string ComputeSHA256()
        {
            
            string raw_certificate_info = getRawCertificateInfo();
            byte[] inputBytes = Encoding.UTF8.GetBytes(raw_certificate_info);

            SHA256 sha256 = SHA256.Create();
            byte[] hashBytes = sha256.ComputeHash(inputBytes);

            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }

        public void getFromFile(string filename)
        {
            FileStream fs = new FileStream(filename, FileMode.Open);
            StreamReader sr = new StreamReader(fs);

            CertBlock previous_mode = CertBlock.None;
            CertBlock mode = CertBlock.None;

            string[] parts;
            while (!sr.EndOfStream)
            {
                // Переключение режима считывания
                string line = sr.ReadLine() ?? "";
                switch (line)
                {
                    case "[СЕРТИФИКАТ]":
                        mode = CertBlock.Certificate;
                        break;
                    case "[ОТКРЫТЫЙ КЛЮЧ]":
                        mode = CertBlock.OpenKey;
                        break;
                    //case "[ХЭШ]":
                    //    mode = CertBlock.Hash;
                    //    break;
                    case "[ПОДПИСЬ]":
                        mode = CertBlock.Sign;
                        break;
                    case "-----КОНЕЦ СЕРТИФИКАТА-----":
                        mode = CertBlock.None;
                        break;
                }

                // Пропускаем считывание если произошла смена режима чтения
                if (previous_mode != mode)
                {
                    previous_mode = mode;
                    continue;
                }

                switch (mode)
                {
                    case CertBlock.Certificate:
                        parts = line.Split("=", 2);
                        certificate_info[parts[0]] = parts[1].Replace("\n", "");
                        break;
                    case CertBlock.OpenKey:
                        public_key += line+"\n";
                        break;
                    case CertBlock.Sign:
                        sign += line;
                        break;
                }
            }

            fs.Close();
            sr.Close();
        }

        public void getFromText(string text)
        {
            //FileStream fs = new FileStream(filename, FileMode.Open);
            //StreamReader sr = new StreamReader(;

            CertBlock previous_mode = CertBlock.None;
            CertBlock mode = CertBlock.None;

            string[] parts;
            foreach (string line in text.Split("\n")) {
                // Переключение режима считывания
                switch (line)
                {
                    case "[СЕРТИФИКАТ]":
                        mode = CertBlock.Certificate;
                        break;
                    case "[ОТКРЫТЫЙ КЛЮЧ]":
                        mode = CertBlock.OpenKey;
                        break;
                    //case "[ХЭШ]":
                    //    mode = CertBlock.Hash;
                    //    break;
                    case "[ПОДПИСЬ]":
                        mode = CertBlock.Sign;
                        break;
                    case "-----КОНЕЦ СЕРТИФИКАТА-----":
                        mode = CertBlock.None;
                        break;
                }

                // Пропускаем считывание если произошла смена режима чтения
                if (previous_mode != mode)
                {
                    previous_mode = mode;
                    continue;
                }

                switch (mode)
                {
                    case CertBlock.Certificate:
                        parts = line.Split("=", 2);
                        certificate_info[parts[0]] = parts[1].Replace("\n", "");
                        break;
                    case CertBlock.OpenKey:
                        public_key += line + "\n";
                        break;
                    case CertBlock.Sign:
                        sign += line;
                        break;
                }
            }
        }
    }
}
