using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using Dynamics.Schema.Extensions;

namespace Dynamics.Schema.Translators
{
    public class WhiteListEntity
    {
        public string OriginalName { get; set; }
        public string Name { get; set; }
        public List<CodeAttributeDeclaration> Properties { get; set; }
        public bool IsPlaceholder { get; set; }
        public WhiteListEntity()
        {
            Properties = new List<CodeAttributeDeclaration>();
            IsPlaceholder = false;
        }

        public override string ToString()
        {
            if (IsPlaceholder) return "<!--{0} is a Placeholder-->".Fill(Name);
            var newName = string.IsNullOrWhiteSpace(Name) ? "" : "=" + Name;
            string attributes = "";
            foreach (var property in Properties)
            {
                if (string.IsNullOrWhiteSpace(attributes)) {attributes = "|";}
                else{attributes += ",";}

                attributes += property.Name;
                var args = property.Arguments.Cast<CodeAttributeArgument>().Aggregate("", (current, argument) => current + ("-" + argument.ToString()));
                attributes += args;

            }
           
            return "{0}{1}{2}".Fill(OriginalName, newName , attributes);

        }
    }
}