#region

using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

#endregion

namespace Dynamics.Schema.Translators
{
    public sealed class OptionSetOptionTranslator
    {

        public Dictionary<string, WhiteListEntity> OptionSetWhitelist { get; set; }

        #region Singlton stuff
        private static readonly OptionSetOptionTranslator _instance = new OptionSetOptionTranslator();

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static OptionSetOptionTranslator()
        {
        }

        private OptionSetOptionTranslator()
        {
            OptionSetWhitelist = new Dictionary<string, WhiteListEntity>();

            Console.Out.WriteLine("Loading OptionSet Whitelist");
            OptionSetWhitelist = new Dictionary<string, WhiteListEntity>(
                 ConfigurationParser.ExtractWhiteListConfig(Properties.Settings.Default.OptionSetWhitelist), StringComparer.OrdinalIgnoreCase
             );


        }

        public static OptionSetOptionTranslator Instance
        {
            get
            {
                return _instance;
            }
        }

        #endregion

        public WhiteListEntity TransformOptionSet(string optionSet)
        {
            return OptionSetWhitelist.ContainsKey(optionSet) ? OptionSetWhitelist[optionSet] : new WhiteListEntity { Name = optionSet, IsPlaceholder = true };
        }
        public WhiteListEntity TransformOption(string optionSetName, string optionName)
        {
            var value = Objectify(optionSetName, optionName);
            return OptionSetWhitelist.ContainsKey(value) ? OptionSetWhitelist[value] : new WhiteListEntity { Name = optionName, IsPlaceholder = true };
        }

        private bool IsOptionSetWhiteListed(string value)
        {
            return OptionSetWhitelist.ContainsKey(value);
        }
        private bool IsOptionWhiteListed(string optionSet, string option)
        {
            return OptionSetWhitelist.ContainsKey(Objectify(optionSet, option));
        }

        public static string Objectify(string first, string second)
        {
            return string.Format("{0}.{1}", first, second);
        }
    }


}
