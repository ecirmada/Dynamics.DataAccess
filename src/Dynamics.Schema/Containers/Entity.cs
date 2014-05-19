#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Dynamics.Schema.Containers.Attributes;
using Dynamics.Schema.Extensions;
using Dynamics.Schema.Translators;

#endregion

namespace Dynamics.Schema.Containers
{
    [KnownType(typeof(LookupAttribute))]
    [KnownType(typeof(PickListAttribute))]
    [DataContract(Namespace = "")]
    public class Entity
    {
        public Entity()
        {
            Attributes = new Dictionary<string, Attribute>();
        }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string CrmLogicalName { get; set; }
        [DataMember]
        public string CrmSchemaName { get; set; }
        [DataMember]
        public Dictionary<string, Attribute> Attributes { get; set; }
        [DataMember]
        public WhiteListEntity WhiteList { get; set; }

        public override string ToString()
        {
            return Name.SupplimentIfDifferentTo(CrmSchemaName) + (Attributes != null && Attributes.Any() ? String.Format(" +{0} attributes", Attributes.Count()) : "");
        }
    }
}
