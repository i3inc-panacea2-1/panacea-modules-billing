using Panacea.Modularity.Billing;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Panacea.Modules.Billing.Models
{

    [DataContract]
    public class GetServicesAndPackagesResponse
    {
        [DataMember(Name = "packages")]
        public BillingP Packages { get; set; }

    }

    [DataContract]
    public class BillingP
    {
        [DataMember(Name = "Billing")]
        public BillingPackage Billing { get; set; }
    }

    public class BillingPackage
    {
        [DataMember(Name = "billingPackages")]
        public List<Package> BillingPackages { get; set; }
    }
}
