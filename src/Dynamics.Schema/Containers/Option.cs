using Dynamics.Schema.Extensions;
using Dynamics.Schema.Translators;

namespace Dynamics.Schema.Containers
{
    public class Option
    {
        public string Name { get; set; }
        public string CrmName { get; set; }
        public int? Number { get; set; }
        public int Count { get; set; }
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