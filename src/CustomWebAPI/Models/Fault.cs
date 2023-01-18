using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace CustomWebAPI.Models
{
    /// <summary>
    /// Модель для отправки сообщения об ошибке.
    /// </summary>
    public class Fault
    {
        public string faultcode { get; set; }

        public string faultstring { get; set; }

        public string faultguid { get; set; }
    }
}
