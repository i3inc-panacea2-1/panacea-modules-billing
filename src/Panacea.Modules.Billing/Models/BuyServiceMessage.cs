using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Panacea.Modules.Billing.Models
{
    [DataContract]
    public class BuyServiceMessage
    {
        [DataMember(Name = "data")]
        public BuyServiceMessageData Data { get; set; }
    }

    [DataContract]
    public class BuyServiceMessageData
    {
        [DataMember(Name = "serviceIDs")]
        public List<string> ServiceIds { get; set; }
    }
}
