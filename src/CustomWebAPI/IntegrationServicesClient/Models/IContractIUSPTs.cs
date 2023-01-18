using System;

namespace CustomWebAPI.IntegrationServicesClient.Models
{
    [EntityName("Реквизиты договора для интеграции с ИУС ПТ")]
    public class IContractIUSPTs 
    {
        public string Name { get; set; }
        public string Status { get; set; }
        public int Id { get; set; }
        public bool IsCardExported { get; set; }
        public bool IsBodyExported { get; set; }
        public string IDSAP { get; set; }
        public string GTCString25 { get; set; }
        public DateTimeOffset? Date { get; set; }
        public int IdContract { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
