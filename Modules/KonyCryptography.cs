using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CryptoKony_Client.Modules
{
    internal class KonyCryptography
    {
        public static string publicKey;
        public static string privateKey;

        public static string format_publicKey;
        public static string format_privateKey;

        public static void generateKeyPair()
        {
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                RSAParameters key = rsa.ExportParameters(true);
                key.Exponent = new byte[] { 1, 0, 1 }; // 65537

                rsa.ImportParameters(key);

                publicKey = rsa.ToXmlString(false); // Экспорт публичного ключа
                privateKey = rsa.ToXmlString(true); // Экспорт приватного ключа

                format_publicKey = Convert.ToBase64String(Encoding.UTF8.GetBytes(publicKey));
                format_publicKey = "-----BEGIN PUBLIC KEY-----\n" + format_publicKey + "\n-----END PUBLIC KEY-----";

                format_privateKey = Convert.ToBase64String(Encoding.UTF8.GetBytes(privateKey));
                format_privateKey = "-----BEGIN PRIVATE KEY-----\n" + format_privateKey + "\n-----END PRIVATE KEY-----";
            }
        }

        public static string FormatPublicKey(RSAParameters key)
        {
            var rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(key);
            string publicKey = Convert.ToBase64String(Encoding.UTF8.GetBytes(rsa.ToXmlString(false)));
            publicKey = "-----BEGIN PUBLIC KEY-----\n" + publicKey + "\n-----END PUBLIC KEY-----";
            return publicKey;
        }

        public static string FormatPrivateKey(RSAParameters key)
        {
            var rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(key);
            string privateKey = Convert.ToBase64String(Encoding.UTF8.GetBytes(rsa.ToXmlString(true)));
            privateKey = "-----BEGIN PUBLIC KEY-----\n" + privateKey + "\n-----END PUBLIC KEY-----";
            return privateKey;
        }

        public static RSAParameters GetPublicKey(string base64)
        {
            string xml = Encoding.UTF8.GetString(Convert.FromBase64String(base64));
            var rsa = new RSACryptoServiceProvider(2048);
            rsa.FromXmlString(xml);
            return rsa.ExportParameters(false);
        }

        public static RSAParameters GetPrivateKey(string base64)
        {
            string xml = Encoding.UTF8.GetString(Convert.FromBase64String(base64));
            var rsa = new RSACryptoServiceProvider(2048);
            rsa.FromXmlString(xml);
            return rsa.ExportParameters(true);
        }

        public static string EncryptText(RSAParameters key, string text)
        {
            byte[] b_text = Encoding.UTF8.GetBytes(text);

            RSA rsa = new RSACryptoServiceProvider(2048);

            rsa.ImportParameters(key);
            byte[] data = rsa.Encrypt(b_text, RSAEncryptionPadding.Pkcs1);

            return Convert.ToBase64String(data);
        }

        public static string DecryptText(RSAParameters key, string text)
        {
            byte[] b_text = Convert.FromBase64String(text);

            RSA rsa = new RSACryptoServiceProvider(2048);

            rsa.ImportParameters(key);
            byte[] data = rsa.Decrypt(b_text, RSAEncryptionPadding.Pkcs1);

            return Encoding.UTF8.GetString(data);
        }

        public static string SignData(RSAParameters key, byte[] text)
        {
            //byte[] b_text = Convert.FromBase64String(text);

            RSA rsa = new RSACryptoServiceProvider(2048);

            rsa.ImportParameters(key);
            byte[] data = rsa.SignData(text, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

            return Convert.ToBase64String(data);
        }

        public static bool VerifyData(RSAParameters key, byte[] text, byte[] signature)
        {
            //byte[] b_text = Convert.FromBase64String(text);

            RSA rsa = new RSACryptoServiceProvider(2048);

            rsa.ImportParameters(key);
            bool result = rsa.VerifyData(text, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

            return result;
        }
    }
}
