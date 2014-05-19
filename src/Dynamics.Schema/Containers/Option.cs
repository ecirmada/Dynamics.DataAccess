using System.Runtime.Serialization;
using Dynamics.Schema.Extensions;
using Dynamics.Schema.Translators;

namespace Dynamics.Schema.Containers
{
    [DataContract(Namespace = "")]
    public class Option
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string CrmName { get; set; }
        [DataMember]
        public int? Number { get; set; }
        [DataMember]
        public int Count { get; set; }
        [DataMember]
        public WhiteListEntity WhiteList { get; set; }

        public Option()
        {
            Count = 1;
        }

        public override string ToString()
        {
            return string.Format("{0}-{1}", Name.SupplimentIfDifferentTo(CrmName), Number);
        }
    }
}