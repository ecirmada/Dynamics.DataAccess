#region

using System.Collections.Generic;
using Dynamics.Schema.Extensions;
using Dynamics.Schema.Translators;

#endregion

namespace Dynamics.Schema.Containers
{
    public class OptionSet
    {
        public string Name { get; set; }
        public bool IsGlobal { get; set; }
        public string HostEntity { get; set; }
        public Dictionary<string, Option> Options { get; set; }
        public string CrmName { get; set; }
        public WhiteListEntity WhiteList { get; set; }

        public OptionSet()
        {
            Options = new Dictionary<string, Option>();
        }

        public override string ToString()
        {
            return string.Format("{0}{2} (+{1} options)", Name.SupplimentIfDifferentTo(CrmName), Options.Count, 
                IsGlobal?"*":string.Format("\t(entity={0})", HostEntity));
        }
    }
}
