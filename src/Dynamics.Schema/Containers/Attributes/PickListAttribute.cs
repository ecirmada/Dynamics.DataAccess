using Dynamics.Schema.Extensions;

namespace Dynamics.Schema.Containers.Attributes
{
    public class PickListAttribute : Attribute
    {

        public string PickListName { get; set; }
        public string PickListCrmName { get; set; }

        public override string ToString()
        {
            return string.Format("{0}\t{1}-{3}\t{2}", Name.SupplimentIfDifferentTo(CrmSchemaName), SchemaType, Desc, PickListName);
        }
    }
}