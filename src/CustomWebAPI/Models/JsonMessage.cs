using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomWebAPI.Models
{
    public class JsonMessage
    {
        public class Root
        {
            [JsonProperty("?xml")]
            public Xml xml { get; set; }

            [JsonProperty("soapenv:Envelope")]
            public SoapenvEnvelope soapenvEnvelope { get; set; }
        }

        public class SoapenvBody
        {
            [JsonProperty("tem:RunScriptAsync")]
            public TemRunScriptAsync temRunScriptAsync { get; set; }
        }

        public class SoapenvEnvelope
        {
            [JsonProperty("@xmlns:soapenv")]
            public string xmlnssoapenv { get; set; }

            [JsonProperty("@xmlns:tem")]
            public string xmlnstem { get; set; }

            [JsonProperty("soapenv:Body")]
            public SoapenvBody soapenvBody { get; set; }
        }

        public class TemKeyValueofstringstring
        {
            [JsonProperty("#comment")]
            public List<object> comment { get; set; }

            [JsonProperty("tem:Key")]
            public string temKey { get; set; }

            [JsonProperty("tem:Value")]
            public string temValue { get; set; }
        }

        public class TemParams
        {
            [JsonProperty("tem:KeyValueofstringstring")]
            public List<TemKeyValueofstringstring> temKeyValueofstringstring { get; set; }
        }

        public class TemRunScriptAsync
        {
            [JsonProperty("#comment")]
            public List<object> comment { get; set; }

            [JsonProperty("tem:Name")]
            public string temName { get; set; }

            [JsonProperty("tem:Params")]
            public TemParams temParams { get; set; }
        }

        public class Xml
        {
            [JsonProperty("@version")]
            public string version { get; set; }

            [JsonProperty("@encoding")]
            public string encoding { get; set; }
        }
    }
}
