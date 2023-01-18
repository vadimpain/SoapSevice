using CustomWebAPI.IntegrationServicesClient;
using CustomWebAPI.Models;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using static CustomWebAPI.Constants;

namespace CustomWebAPI.Services
{
    public class CustomMessage
    {
        public static Logger logger = LogManager.GetCurrentClassLogger();
        private const string DefaultConfigSettingsName = @"_ConfigSettings.xml";

        public static string PathIn = string.Empty;
        public static string PathOut = string.Empty;
        public static string PathError = string.Empty;
        public static string PathGuidBase = string.Empty;

        /// <summary>
        /// Типы сообщения.
        /// </summary>
        public enum TypeMessage
        {
            Input,
            Output,
            Error
        }

        /// <summary>
        /// Инициализация необходимых файлов, папок и путей.
        /// </summary>
        public static void Setup()
        {
            ConfigSettingsService.SetSourcePath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DefaultConfigSettingsName));

            PathIn = Path.Combine(ConfigSettingsService.GetConfigSettingsValueByName(ConfigServices.LogPath), "In");
            PathOut = Path.Combine(ConfigSettingsService.GetConfigSettingsValueByName(ConfigServices.LogPath), "Out");
            PathError = Path.Combine(ConfigSettingsService.GetConfigSettingsValueByName(ConfigServices.LogPath), "Error");
            PathGuidBase = Path.Combine(ConfigSettingsService.GetConfigSettingsValueByName(ConfigServices.LogPath), "GuidBase");

            if (!Directory.Exists(PathIn))
                Directory.CreateDirectory(PathIn);

            if (!Directory.Exists(PathOut))
                Directory.CreateDirectory(PathOut);

            if (!Directory.Exists(PathError))
                Directory.CreateDirectory(PathError);

            if (!Directory.Exists(PathGuidBase))
                Directory.CreateDirectory(PathGuidBase);

            if (!File.Exists(Path.Combine(PathGuidBase, "GuidBase.txt")))
                File.Create(Path.Combine(PathGuidBase, "GuidBase.txt"));
        }

        /// <summary>
        /// Сохраняет сообщение в файле.
        /// </summary>
        /// <param name="path">Путь.</param>
        /// <param name="guid">Уникальное узначение.</param>
        /// <param name="message">Сообщение.</param>
        /// <param name="typeMessage">Тип сообщения.</param>
        public static void SaveMessageToFile(string path, Guid guid, string message, TypeMessage typeMessage)
        {
            try
            {
                var test = ConfigSettingsService.GetConfigSettingsValueByName(ConfigServices.IsInputMessage);

                if ((TypeMessage.Input == typeMessage && bool.Parse(ConfigSettingsService.GetConfigSettingsValueByName(ConfigServices.IsInputMessage))) ||
                    (TypeMessage.Output == typeMessage && bool.Parse(ConfigSettingsService.GetConfigSettingsValueByName(ConfigServices.IsOutputMessage))) ||
                    (TypeMessage.Error == typeMessage && bool.Parse(ConfigSettingsService.GetConfigSettingsValueByName(ConfigServices.IsErrorMessage))))
                {

                    using (FileStream file = File.Create(path + "/" + guid.ToString() + ".xml"))
                    {
                        var formatMessage = DateTime.Now.ToString() + " " + message;
                        byte[] info = new UTF8Encoding(true).GetBytes(formatMessage);
                        // Add some information to the file.
                        file.Write(info, 0, info.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Failed to create file: " + ex);
            }
        }

        /// <summary>
        /// Конвертирует сообщение в строку
        /// </summary>
        /// <param name="message">Сообщение.</param>
        /// <returns>Сообщение в виде строки.</returns>
        /// <remarks>Полноценной конвертации сообщения я не нашел, пришлось реализовывать руками. Источник - https://stackoverflow.com/questions/1319043/how-to-get-message-content-from-system-servicemodel-channels-message </remarks>
        public static string MessageToString(Message message)
        {
            // copy the message into a working buffer.
            MessageBuffer mb = message.CreateBufferedCopy(int.MaxValue);

            // re-create the original message, because "copy" changes its state.
            message = mb.CreateMessage();

            Stream stream = new MemoryStream();
            XmlWriter xmlWriter = XmlWriter.Create(stream);
            mb.CreateMessage().WriteMessage(xmlWriter);
            xmlWriter.Flush();
            stream.Position = 0;

            byte[] bXML = new byte[stream.Length];
            stream.Read(bXML, 0, (int)stream.Length);

            // sometimes bXML[] starts with a BOM
            if (bXML[0] != (byte)'<')
            {
                return Encoding.UTF8.GetString(bXML, 3, bXML.Length - 3);
            }
            else
            {
                return Encoding.UTF8.GetString(bXML, 0, bXML.Length);
            }
        }

        /// <summary>
        /// Валидирует сообщение.
        /// </summary>
        /// <param name="message">Сообщение.</param>
        public static void ValidateMessage(string message)
        {
            var path = ConfigSettingsService.GetConfigSettingsValueByName(ConfigServices.PathXSDSchemas);
            XmlTextReader reader = new XmlTextReader(path);
            XmlSchema schema = XmlSchema.Read(reader, ValidationEventHandler);

            var xml = new XmlDocument();
            xml.LoadXml(message);

            xml.Schemas.Add(schema);
            xml.Validate(ValidationEventHandler);
        }

        /// <summary>
        /// Генерация уникального Guid.
        /// </summary>
        /// <returns>Возвращает уникальный guid.</returns>
        public static Guid GenerateUniqueGuid()
        {
            var path = Path.Combine(PathGuidBase, "GuidBase.txt");
            var isGenerateUniqueGuid = true;
            var guid = Guid.Empty;

            while (isGenerateUniqueGuid)
            {
                guid = Guid.NewGuid();

                if (CheckUniqueGuid(guid))
                { 
                    isGenerateUniqueGuid = false;
                    File.AppendAllText(path, guid.ToString() + Environment.NewLine);
                }    
                   
            }
            return guid;
        }

        /// <summary>
        /// Проверка guid на уникальность.
        /// </summary>
        /// <param name="guid"></param>
        /// <returns>True, если guid уникальный.</returns>
        public static bool CheckUniqueGuid(Guid guid)
        {
            var path = Path.Combine(PathGuidBase, "GuidBase.txt");
            var listGuid = File.ReadAllLines(path);

            foreach (var item in listGuid)
            {
                if (guid.CompareTo(Guid.Parse(item)) == 0)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Обработчик валидации.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            XmlSeverityType type = XmlSeverityType.Warning;
            if (Enum.TryParse<XmlSeverityType>("Error", out type))
            {
                if (type == XmlSeverityType.Error) throw new Exception(e.Message);
            }
        }

        public static Fault CreateErrorBody(string guid, string exeption)
        {
            var fault = new Fault
            {
                faultcode = "SOAP-ENV:Server",
                faultguid = guid,
                faultstring = string.Concat(exeption, " is not a valid integer value")
            };

            return fault;
        }

        /// <summary>
        /// Отправляет сообщение в RX
        /// </summary>
        /// <param name="message">Сообщение.</param>
        public static void SendCustomMessage(string message)
        {
            InitOdata();
            Client.SendRequest(ConvertXmlToJson(message));
        }

        public static string ConvertXmlToJson(string messgae)
        {
            var xmldoc = new XmlDocument();
            xmldoc.LoadXml(messgae);
            var fromXml = JsonConvert.SerializeXmlNode(xmldoc);
            var fromJson = JsonConvert.DeserializeObject<JsonMessage.Root>(fromXml);

            var editJson = new JsonMessageForRX.Root
            {
                RunScriptAsync = new JsonMessageForRX.RunScriptAsync
                {
                    Name = fromJson.soapenvEnvelope.soapenvBody.temRunScriptAsync.temName,
                    Param = new JsonMessageForRX.Param()
                    {
                        KeyValueofstringstring = new List<JsonMessageForRX.KeyValueofstringstring>()
                    }
                }
            };

            foreach (var item in fromJson.soapenvEnvelope.soapenvBody.temRunScriptAsync.temParams.temKeyValueofstringstring)
            {
                var keyValue = new JsonMessageForRX.KeyValueofstringstring();
                keyValue.Key = item.temKey;
                keyValue.Value = item.temValue;
                editJson.RunScriptAsync.Param.KeyValueofstringstring.Add(keyValue);
            }

            var json = JsonConvert.SerializeObject(editJson);

            return json;

        }

        /// <summary>
        /// Инициализация Odata клиента.
        /// </summary>
        private static void InitOdata()
        {
            #region Аутентификация.
            ConfigSettingsService.SetSourcePath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DefaultConfigSettingsName));
            Client.Setup(logger);
            #endregion
        }
    }
}
