using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using CustomWebAPI.IntegrationServicesClient;
using CustomWebAPI.IntegrationServicesClient.Models;
using CustomWebAPI.Models;
using Microsoft.AspNetCore.Http;
using NLog;
using Simple.OData.Client;

namespace CustomWebAPI.Services
{
    public class Service : IService
    {
        public static Logger logger = LogManager.GetCurrentClassLogger();
        private const string DefaultConfigSettingsName = @"_ConfigSettings.xml";

        /// <summary>
        /// Метод для получения измененных реквизитов.
        /// </summary>
        /// <param name="Name">Название метода.</param>
        /// <param name="Params">Параметры.</param>
        /// <returns>Пустое сообщение.</returns>
        public string RunScriptAsync(string Name, KeyValueofstringstring[] Params)
        {
            return string.Empty;
        }

        private void InitOdata()
        {
            #region Аутентификация.
            ConfigSettingsService.SetSourcePath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DefaultConfigSettingsName));
            Client.Setup(logger);
            #endregion
        }
    }
}
