using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using CustomWebAPI.Models;
using CustomWebAPI.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SoapCore;
using SoapCore.Extensibility;

namespace CustomWebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            // Загрузить класс интерфейса и службы как шаблон Singleton.
            services.TryAddSingleton<IService, Service>();
            services.TryAddSingleton<IFaultExceptionTransformer, DefaultFaultExceptionTransformer>();
            services.AddMvc();

            services.AddSoapMessageProcessor(async (message, httpcontext, next) =>
            {
                var bufferedMessage = message.CreateBufferedCopy(int.MaxValue);

                Services.CustomMessage.Setup();
                var convertMessagee = Services.CustomMessage.MessageToString(bufferedMessage.CreateMessage());
                var guid = Services.CustomMessage.GenerateUniqueGuid();

                try
                {
                    Services.CustomMessage.ValidateMessage(convertMessagee);
                    Services.CustomMessage.SaveMessageToFile(Services.CustomMessage.PathIn, guid, convertMessagee, Services.CustomMessage.TypeMessage.Input);

                    Services.CustomMessage.SendCustomMessage(convertMessagee);
                    Services.CustomMessage.SaveMessageToFile(Services.CustomMessage.PathOut, guid, convertMessagee, Services.CustomMessage.TypeMessage.Output);
                }
                catch (Exception ex)
                {
                    var errorMessage = Message.CreateMessage(message.Version, null, Services.CustomMessage.CreateErrorBody(guid.ToString(), ex.Message));
                    Services.CustomMessage.SaveMessageToFile(Services.CustomMessage.PathError, guid, convertMessagee, Services.CustomMessage.TypeMessage.Error);

                    return errorMessage;
                }

                return Message.CreateMessage(message.Version, null);
            });
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            // Запустить сервис из шаблона по указанному адресу.

            app.UseSoapEndpoint<IService>("/api/Soap/Service.asmx", new SoapEncoderOptions(), SoapSerializer.XmlSerializer);
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
