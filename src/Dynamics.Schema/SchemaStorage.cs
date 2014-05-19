#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using Dynamics.Schema.Containers;
using Dynamics.Schema.Containers.Attributes;
using Dynamics.Schema.Extensions;
using Dynamics.Schema.Translators;
using Microsoft.Xrm.Sdk.Metadata;
using Attribute = Dynamics.Schema.Containers.Attribute;

#endregion

namespace Dynamics.Schema
{
    [DataContract]
    public sealed class SchemaStorage
    {

        #region Singlton stuff
        private static readonly SchemaStorage instance = new SchemaStorage();

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static SchemaStorage()
        {
        }

        private SchemaStorage()
        {
            Entities = new Dictionary<string, Entity>();
            OptionSets = new Dictionary<string, OptionSet>();
        }

        public static SchemaStorage Instance
        {
            get
            {
                return instance;
            }
        }
        #endregion

        [DataMember]
        public Dictionary<string, Entity> Entities { get; set; }
        [DataMember]
        public Dictionary<string, OptionSet> OptionSets { get; set; }


        public string AddEntity(EntityMetadata entityMetadata)
        {
            var whitelist = EntityAttributeTranslator.Instance.TransformEntity(entityMetadata.SchemaName);
            var name = whitelist.Name;
            if (!Entities.ContainsKey(name))
            {
                Entities.Add(name, new Entity
                {
                    Name = name,
                    CrmSchemaName = entityMetadata.SchemaName,
                    CrmLogicalName = entityMetadata.LogicalName,
                    WhiteList = (whitelist.IsPlaceholder ? null : whitelist)
                }
                );
            }
            return name;
        }

        public string AddAttribute(EntityMetadata entityMetadata, AttributeMetadata attributeMetadata)
        {
            var entity = AddEntity(entityMetadata);
            var whitelist = EntityAttributeTranslator.Instance.TransformAttribute(entityMetadata.SchemaName, attributeMetadata.SchemaName);
            var name = whitelist.Name;
            Attribute attribute = null;
            string description = null;
            if (attributeMetadata.Description != null && attributeMetadata.Description.UserLocalizedLabel != null)
            {
                description = attributeMetadata.Description.UserLocalizedLabel.Label;
            }
            switch (attributeMetadata.AttributeType)
            {
                //Unhandled attributes
                case null:
                    Console.Error.WriteLine("Unhandled Attribute of null");
                    break;
                case AttributeTypeCode.Customer:
                case AttributeTypeCode.PartyList:

                case AttributeTypeCode.CalendarRules:
                case AttributeTypeCode.Virtual:
                case AttributeTypeCode.ManagedProperty:
                case AttributeTypeCode.EntityName:
                    Console.Error.WriteLine("Unhandled Attribute: '{0}'", Enum.GetName(typeof(AttributeTypeCode), attributeMetadata.AttributeType));

                    break;

                //Handled Attributes
                case AttributeTypeCode.Memo:
                case AttributeTypeCode.Money:
                case AttributeTypeCode.Owner:
                case AttributeTypeCode.Uniqueidentifier:

                    attribute = new Attribute
                    {
                        Name = name,
                        CrmLogicalName = attributeMetadata.LogicalName,
                        CrmSchemaName = attributeMetadata.SchemaName,
                        SchemaType = Enum.GetName(typeof(AttributeTypeCode), attributeMetadata.AttributeType),
                        Desc = (description ?? ""),
                        WhiteList = (whitelist.IsPlaceholder ? null : whitelist)
                    };
                    break;
                case AttributeTypeCode.Lookup:
                    var targets = ((LookupAttributeMetadata)(attributeMetadata)).Targets;
                    attribute = new LookupAttribute
                    {
                        Name = name,
                        CrmLogicalName = attributeMetadata.LogicalName,
                        CrmSchemaName = attributeMetadata.SchemaName,
                        SchemaType = Enum.GetName(typeof(AttributeTypeCode), attributeMetadata.AttributeType),
                        Desc = (description ?? ""),
                        WhiteList = (whitelist.IsPlaceholder ? null : whitelist),
                        LookupRelation = (targets != null && targets.Any() ? targets.Aggregate((a, b) => a + ", " + b) : "")
                    };
                    break;
                case AttributeTypeCode.State:
                case AttributeTypeCode.Status:
                case AttributeTypeCode.Picklist:
                    var picklistName = ((EnumAttributeMetadata) (attributeMetadata)).OptionSet.Name;
                    attribute = new PickListAttribute
                    {
                        Name = name,
                        CrmLogicalName = attributeMetadata.LogicalName,
                        CrmSchemaName = attributeMetadata.SchemaName,
                        SchemaType = Enum.GetName(typeof(AttributeTypeCode), attributeMetadata.AttributeType),
                        Desc = (description ?? ""),
                        WhiteList = (whitelist.IsPlaceholder ? null : whitelist),
                        PickListName = OptionSetOptionTranslator.Instance.TransformOptionSet(picklistName).Name,
                        PickListCrmName = picklistName
                    };
                    break;
                default:
                    attribute = new Attribute
                    {
                        Name = name,
                        CrmLogicalName = attributeMetadata.LogicalName,
                        CrmSchemaName = attributeMetadata.SchemaName,
                        SchemaType = Enum.GetName(typeof(AttributeTypeCode), attributeMetadata.AttributeType),
                        Desc = (description ?? ""),
                        WhiteList = (whitelist.IsPlaceholder ? null : whitelist)
                    };
                    break;
            }

            if (attribute != null && !Entities[entity].Attributes.ContainsKey(name))
            {
                Entities[entity].Attributes.Add(name, attribute);
            }

            return name;


        }

        public string AddOptionSet(OptionSetMetadataBase optionSet, EntityMetadata entityMetadata = null)
        {
            var whitelist = OptionSetOptionTranslator.Instance.TransformOptionSet(optionSet.Name);
            var name = whitelist.Name;
            var isglobal = optionSet.IsGlobal.HasValue && optionSet.IsGlobal.Value;
            var hostentity = "";
            if (entityMetadata != null)
            {
                if (!isglobal)
                {
                    // Find the attribute which uses the specified OptionSet.
                    var attribute =
                        (from a in entityMetadata.Attributes
                         where a.AttributeType == AttributeTypeCode.Picklist
                               && ((EnumAttributeMetadata)a).OptionSet.MetadataId
                               == optionSet.MetadataId
                         select a).FirstOrDefault();

                    // Check for null, since statuscode attributes on custom entities are not
                    // global, but their optionsets are not included in the attribute
                    // metadata of the entity, either.
                    if (attribute != null)
                    {
                        // Concatenate the name of the entity and the name of the attribute
                        // together to form the OptionSet name.
                        hostentity = String.Format("{0}.{1}", entityMetadata.SchemaName, attribute.SchemaName);
                    }

                }
            }

            if (!OptionSets.ContainsKey(name))
            {
                OptionSets.Add(name, new OptionSet
                {
                    Name = name,
                    CrmName = optionSet.Name,
                    IsGlobal = isglobal,
                    HostEntity = hostentity,
                    WhiteList = (whitelist.IsPlaceholder ? null : whitelist)

                });
            }
            return name;
        }

        public string AddOption(OptionSetMetadataBase optionSetMetadata, OptionMetadata optionMetadata)
        {
            var optionSetName = AddOptionSet(optionSetMetadata);

            var whitelist = OptionSetOptionTranslator.Instance.TransformOption(optionSetMetadata.Name, optionMetadata.Label.UserLocalizedLabel.Label);
            var optionName = whitelist.Name;

            optionName = EnsureValidIdentifier(optionName);
            optionName = EnsureUniqueOption(optionSetName, optionName);
            int? value = optionMetadata.Value;

            if (!OptionSets[optionSetName].Options.ContainsKey(optionName))
            {
                OptionSets[optionSetName].Options.Add(optionName, new Option
                {
                    Name = optionName,
                    Number = value,
                    CrmName = optionMetadata.Label.UserLocalizedLabel.Label,
                    WhiteList = (whitelist.IsPlaceholder ? null : whitelist)
                });
            }
            return optionName;
        }

        /// <summary>
        /// Checks to make sure that the name begins with a valid character. If the name
        /// does not begin with a valid character, then add an underscore to the
        /// beginning of the name.
        /// </summary>
        private static String EnsureValidIdentifier(String name)
        {
            // Check to make sure that the option set begins with a word character
            // or underscore.
            name = Regex.Replace(name, @"[^A-Za-z0-9_]", "");
            const string pattern = @"^[A-Za-z_][A-Za-z0-9_]*$";
            if (!Regex.IsMatch(name, pattern))
            {

                // Prepend an underscore to the name if it is not valid.
                name = String.Format("_{0}", name);
                // Trace.TraceInformation(String.Format("Name of the option changed to {0}", name));
            }
            return name;
        }

        private string EnsureUniqueOption(string optionSetName, string optionName)
        {
            if (OptionSets.ContainsKey(optionSetName))
            {
                if (OptionSets[optionSetName].Options.ContainsKey(optionName))
                {
                    // Increment the number of times that an option with this name has been found.
                    return String.Format("{0}_{1}", optionName, ++OptionSets[optionSetName].Options[optionName].Count);
                }
            }
            return optionName;
        }

        public void ExportSchemas(string filename, bool humanReadable=false)
        {
            if (humanReadable)
            {
                //Output the Entities schema
                using (TextWriter output = new StreamWriter(string.Format("{0}.Entities.Schema", filename), false))
                {
                    foreach (Entity entity in Entities.Values)
                    {
                        output.WriteLine("----------------------");
                        output.WriteLine(entity);
                        output.WriteLine("----------------------");
                        foreach (Attribute attribute in entity.Attributes.Values)
                        {
                            output.WriteLine(attribute);
                        }
                    }
                }

                //Output the Option Set schema
                using (TextWriter output = new StreamWriter(string.Format("{0}.Enums.Schema", filename), false))
                {
                    foreach (var optionSet in OptionSets.Values)
                    {
                        output.WriteLine("----------------------");
                        output.WriteLine(optionSet);
                        output.WriteLine("----------------------");
                        foreach (var option in optionSet.Options.Values)
                        {
                            output.WriteLine(option);
                        }
                    }
                }
            }
            else
            {
                using (var output = new StreamWriter(string.Format("{0}.Schema.xml", filename), false))
                {
                    var serialiser = new DataContractSerializer(this.GetType());

                    serialiser.WriteObject(output.BaseStream, this);
                }
            }
        }
    }
}
