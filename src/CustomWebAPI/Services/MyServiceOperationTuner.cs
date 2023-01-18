using Microsoft.AspNetCore.Http;
using NLog;
using SoapCore.Extensibility;
using SoapCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;

namespace CustomWebAPI.Services
{
    public class MyServiceOperationTuner : IServiceOperationTuner
    {
        public void Tune(HttpContext httpContext, object serviceInstance, SoapCore.ServiceModel.OperationDescription operation)
        {
            if (operation.Name.Equals("RunScriptAsync"))
            {
                Service service = serviceInstance as Service;
                string result = string.Empty;

                using (var reader = new StreamReader(httpContext.Request.Body))
                {
                    var body = reader.ReadToEnd();
                }

                //service.SetParameterForSomeOperation(result);
            }
        }
    }

    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            context.Request.EnableBuffering();
            using (var reader = new StreamReader(context.Request.Body, leaveOpen: true))
            {
                logger.Debug("Request body:" + reader.ReadToEnd());
                context.Request.Body.Position = 0;
            }
            await _next(context);
        }
    }
}
