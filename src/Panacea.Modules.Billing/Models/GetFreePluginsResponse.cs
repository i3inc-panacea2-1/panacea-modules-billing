using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Panacea.Modules.Billing.Models
{
    [DataContract]
    class GetFreePluginsResponse
    {
        [DataMember(Name = "freePlugins")]
        public List<string> FreePlugins { get; set; }

        [DataMember(Name = "nonFreePlugins")]
        public List<string> PaidPlugins { get; set; }
    }
}
