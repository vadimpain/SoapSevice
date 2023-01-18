using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomWebAPI.Models
{
    public class JsonMessageForRX
    {
        public class KeyValueofstringstring
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }

        public class Param
        {
            public List<KeyValueofstringstring> KeyValueofstringstring { get; set; }
        }

        public class Root
        {
            public RunScriptAsync RunScriptAsync { get; set; }
        }

        public class RunScriptAsync
        {
            public string Name { get; set; }
            public Param Param { get; set; }
        }
    }
}
