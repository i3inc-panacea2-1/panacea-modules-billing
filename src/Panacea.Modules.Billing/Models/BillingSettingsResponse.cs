using Panacea.Modularity.Billing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Panacea.Modules.Billing.Models
{
    [DataContract]
    public class GetCurrencySettingsResponse:IBillingSettings
    {
        [DataMember(Name = "symbol")]
        public string Symbol { get; set; }

        [DataMember(Name = "allowRefundRequests")]
        public bool AllowRefunds { get; set; }

        [DataMember(Name = "allowAssistanceRequests")]
        public bool AllowAssistanceRequests { get; set; }
    }
}
