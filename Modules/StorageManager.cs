using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.Win32;
using System.Xml;

namespace CryptoKony_Client.Modules
{
    public static class PATH
    {
        public readonly static string DOCUMENTS = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        public readonly static string MAINFOLDER = $@"{DOCUMENTS}\KonyCrypto";
        public readonly static string SAVEFILE = $@"{MAINFOLDER}\save.xml";
        public readonly static string TRUSTED = $@"{MAINFOLDER}\Trusted";
    }
    internal class StorageManager
    {
        //public static string currentPath = PATH.SAVEFILE;
        //private static XmlDocument doc = new XmlDocument();

        public static void Init()
        {
            if(!Directory.Exists(PATH.MAINFOLDER))
            {
                Directory.CreateDirectory(PATH.MAINFOLDER);
            }

            if (!Directory.Exists(PATH.TRUSTED))
            {
                Directory.CreateDirectory(PATH.TRUSTED);
                File.Copy("cryptokonyca.ckcert", PATH.TRUSTED+"\\cryptokonyca.ckcert");
            }

            /*
            if (!File.Exists(PATH.SAVEFILE))
            {
                XmlElement newElem = doc.CreateElement("Save");
                doc.AppendChild(newElem);
                doc.Save(PATH.SAVEFILE);
            }
            */
            //doc.Load(PATH.SAVEFILE);
        }

        /*
        // метод сохранения
        public static void Save(string param, string value)
        {
            // ищем в XML-файле узел с указанным именем
            XmlNode node = doc.SelectSingleNode($"/Save/{param}");

            // если не нашли, то создаем новый узел
            if (node == null)
            {
                // получаем корневой узел XML-файла
                XmlNode root = doc.DocumentElement;

                // создаем новый элемент с указанным именем и значением
                XmlElement elem = doc.CreateElement(param);
                elem.InnerText = value;

                // добавляем новый элемент в корневой узел
                root.AppendChild(elem);
            }
            else
            {
                // если нашли, то записываем новое значение в найденный узел
                node.InnerText = value;
            }

            // сохраняем изменения в XML-файле
            doc.Save(PATH.SAVEFILE);
        }

        // метод чтения
        public static string Load(string param)
        {
            // загружаем XML-файл
            doc.Load(PATH.SAVEFILE);

            // ищем в XML-файле узел с указанным именем
            XmlNode node = doc.SelectSingleNode($"/Save/{param}");

            // если узел найден, то возвращаем его значение
            if (node != null)
            {
                return node.InnerText;
            }
            else
            {
                // стоит учитывать возможность ошибок в случае, если узел с таким именем не найден
                return "";
            }
        }
        */
    }
}
