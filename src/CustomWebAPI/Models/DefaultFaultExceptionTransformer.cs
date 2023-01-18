using SoapCore.Extensibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using System.Xml;

namespace CustomWebAPI.Models
{
    public class DefaultFaultExceptionTransformer : IFaultExceptionTransformer
    {
        public Message ProvideFault(Exception exception, MessageVersion messageVersion, Message requestMessage, XmlNamespaceManager xmlNamespaceManager)
        {
            return Message.CreateMessage(messageVersion, "Exception", exception.ToString());
        }
    }
}
