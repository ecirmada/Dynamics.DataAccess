#region

using System.Collections.Generic;
using System.Runtime.Serialization;
using Dynamics.Schema.Extensions;
using Dynamics.Schema.Translators;

#endregion

namespace Dynamics.Schema.Containers
{
    [DataContract(Namespace = "")]
    public class OptionSet
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public bool IsGlobal { get; set; }
        [DataMember]
        public string HostEntity { get; set; }
        [DataMember]
        public Dictionary<string, Option> Options { get; set; }
        [DataMember]
        public string CrmName { get; set; }
        [DataMember]
        public WhiteListEntity WhiteList { get; set; }

        public OptionSet()
        {
            Options = new Dictionary<string, Option>();
        }

        public override string ToString()
        {
            return string.Format("{0}{2} (+{1} options)", Name.SupplimentIfDifferentTo(CrmName), Options.Count,
                IsGlobal ? "*" : string.Format("\t(entity={0})", HostEntity));
        }
    }
}
