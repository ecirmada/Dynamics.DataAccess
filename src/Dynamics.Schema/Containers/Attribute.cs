using System.Runtime.Serialization;
using Dynamics.Schema.Extensions;
using Dynamics.Schema.Translators;

namespace Dynamics.Schema.Containers
{

    [DataContract(Namespace = "")]
    public class Attribute
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string CrmLogicalName { get; set; }
        [DataMember]
        public string CrmSchemaName { get; set; }
        [DataMember]
        public string SchemaType { get; set; }
        [DataMember]
        public string Desc { get; set; }
        [DataMember]
        public WhiteListEntity WhiteList { get; set; }

        public override string ToString()
        {
            return string.Format("{0}\t{1}\t{2}", Name.SupplimentIfDifferentTo(CrmSchemaName), SchemaType, Desc);
        }
    }
}