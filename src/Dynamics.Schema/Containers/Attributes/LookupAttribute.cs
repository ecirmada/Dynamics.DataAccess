﻿

using System.Runtime.Serialization;
using Dynamics.Schema.Extensions;

namespace Dynamics.Schema.Containers.Attributes
{
    [DataContract(Namespace = "")]
    public class LookupAttribute : Attribute
    {
        [DataMember]
        public string LookupRelation { get; set; }
        public override string ToString()
        {
            return string.Format("{0}\t{1}-{3}\t{2}", Name.SupplimentIfDifferentTo(CrmSchemaName), SchemaType, Desc, LookupRelation);
        }
    }
}
