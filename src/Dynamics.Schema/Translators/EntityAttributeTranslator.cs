#region

using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

#endregion

namespace Dynamics.Schema.Translators
{
    public sealed class EntityAttributeTranslator
    {

        public Dictionary<string, WhiteListEntity> EntityWhitelist { get; set; }



        #region Singlton stuff
        private static readonly EntityAttributeTranslator _instance = new EntityAttributeTranslator();

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static EntityAttributeTranslator()
        {
        }

        private EntityAttributeTranslator()
        {

            Console.Out.WriteLine("Loading Entity Whitelist");
            EntityWhitelist = new Dictionary<string, WhiteListEntity>(ConfigurationParser.ExtractWhiteListConfig(Properties.Settings.Default.EntityWhitelist), StringComparer.OrdinalIgnoreCase
             );
        }

        public static EntityAttributeTranslator Instance
        {
            get
            {
                return EntityAttributeTranslator._instance;
            }
        }

        #endregion

        public WhiteListEntity TransformEntity(string entity)
        {
            return IsEntityWhiteListed(entity) ? EntityWhitelist[entity] : new WhiteListEntity { Name = entity, IsPlaceholder = true };
        }
        public WhiteListEntity TransformAttribute(string entity, string attribute)
        {
            var value = Objectify(entity, attribute);
            return IsEntityWhiteListed(value) ? EntityWhitelist[value] : new WhiteListEntity { Name = attribute, IsPlaceholder = true };
        }

        private bool IsEntityWhiteListed(string value)
        {
            return EntityWhitelist.ContainsKey(value);
        }
        private bool IsAttributeWhiteListed(string entity, string attribute)
        {
            return EntityWhitelist.ContainsKey(Objectify(entity, attribute));
        }

        public static string Objectify(string first, string second)
        {
            return string.Format("{0}.{1}", first, second);
        }
    }
}
