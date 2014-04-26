using Dynamics.Schema.Extensions;
using Dynamics.Schema.Translators;

namespace Dynamics.Schema.Containers
{
    public class Attribute
    {
        public string Name { get; set; }
        public string CrmLogicalName { get; set; }
        public string CrmSchemaName { get; set; }       
        public string SchemaType { get; set; }
        public string Desc { get; set; }
        public WhiteListEntity WhiteList { get; set; }

        public override string ToString()
        {
            return string.Format("{0}\t{1}\t{2}", Name.SupplimentIfDifferentTo(CrmSchemaName), SchemaType, Desc);
        }
    }
}