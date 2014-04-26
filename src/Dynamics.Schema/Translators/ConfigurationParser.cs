using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Dynamics.Schema.Translators
{
    public sealed class ConfigurationParser
    {
        internal static Dictionary<string, WhiteListEntity> ExtractWhiteListConfig(StringCollection collection)
        {
            var dic = new Dictionary<string, WhiteListEntity>();
            foreach (var entity in collection)
            {
                var key = "";
                var value = new WhiteListEntity();

                //Split the entity and attributes
                string entityPart;
                string attributePart = "";
                if (entity.Contains('|'))
                {
                    var split = entity.Split('|');
                    entityPart = split[0].Trim();
                    attributePart = split[1];
                    //todo ensure our parts aren't blank
                }
                else
                {
                    entityPart = entity.Trim();
                }

                //Don't do the rest if we don't have an entity
                if (string.IsNullOrWhiteSpace(entityPart)) continue;

                //set the key, and entity name
                if (entityPart.Contains('='))
                {
                    var split = entityPart.Split('=');
                    key = split[0].Trim();
                    value.Name = split[1].Trim();
                    if (dic.ContainsKey(key))
                    {
                        Console.Error.WriteLine("Whitelist: Duplicate Key '{0}' found.", key);
                        continue;
                    }
                }
                else
                {
                    key = entityPart;

                    var valueInEntityPart = entityPart;
                    if (entityPart.Contains('.'))
                    {
                        valueInEntityPart = valueInEntityPart.Split('.')[1];
                    }
                    value.Name = valueInEntityPart;
                }

                //don't go on unless we've got out key
                if (string.IsNullOrWhiteSpace(key)) continue;


                //Do the attributes
                if (!string.IsNullOrWhiteSpace(attributePart))
                {
                    var attributes = attributePart.Split(',');

                    foreach (var attribute in attributes)
                    {
                        CodeAttributeDeclaration attributeDeclaration = new CodeAttributeDeclaration();
                        if (attribute.Contains("-"))
                        {
                            var arguments = attribute.Split('-');

                            attributeDeclaration.Name = arguments[0].Trim();
                            for (int index = 1; index < arguments.Length; index++)
                            {
                                attributeDeclaration.Arguments.Add(
                                    new CodeAttributeArgument(
                                        new CodeSnippetExpression(arguments[index])
                                        )
                                    );
                            }
                        }
                        else
                        {
                            attributeDeclaration.Name = attribute.Trim();
                        }
                        value.Properties.Add(attributeDeclaration);
                    }
                }

                value.OriginalName = key; //set out key in the value
                dic.Add(key, value);

            }
            return dic;
        }
    }
}