#region

using System;
using System.Collections.Generic;
using System.Linq;
using Dynamics.Schema.Extensions;
using Dynamics.Schema.Translators;

#endregion

namespace Dynamics.Schema.Containers
{
    public class Entity
    {
        public Entity()
        {
            Attributes = new Dictionary<string, Attribute>();
        }
        public string Name { get; set; }
        public string CrmLogicalName { get; set; }
        public string CrmSchemaName { get; set; }
        public Dictionary<string, Attribute> Attributes { get; set; }
        public WhiteListEntity WhiteList { get; set; }

        public override string ToString()
        {
            return Name.SupplimentIfDifferentTo(CrmSchemaName) + (Attributes != null && Attributes.Any() ? String.Format(" +{0} attributes", Attributes.Count()) : "");
        }
    }
}
