using System.Collections.Generic;
using System.Linq;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Entity = Dynamics.Schema.Containers.Entity;

namespace Dynamics.Schema.Example
{
    public static class EntityExtractor
    {
        public static EntityMetadata GetEntitySchema( OrganizationServiceProxy serviceProxy, string entity)
        {
            //var req = new RetrieveEntityRequest()
            var request = new RetrieveEntityRequest
            {
                EntityFilters = EntityFilters.Entity | EntityFilters.Attributes,
                LogicalName = entity,
                
            };
            var response = (RetrieveEntityResponse)serviceProxy.Execute(request);



            return response.EntityMetadata;

        }

        public static Dictionary<string, Entity> GetEntities( OrganizationServiceProxy serviceProxy, bool IncludeAttributes = false)
        {
            //var req = new RetrieveEntityRequest()
            var request = new RetrieveAllEntitiesRequest()
            {
                EntityFilters = IncludeAttributes ? EntityFilters.Entity | EntityFilters.Attributes : EntityFilters.Entity
            };
            var response = (RetrieveAllEntitiesResponse)serviceProxy.Execute(request);

            foreach (var metadata in response.EntityMetadata)
            {
                SchemaStorage.Instance.AddEntity(metadata);
                if (metadata.Attributes == null) continue;
                foreach (var attribute in metadata.Attributes)
                {
                    SchemaStorage.Instance.AddAttribute(metadata, attribute);
                }
            }

            return SchemaStorage.Instance.Entities;
        }

        public static Dictionary<string, Entity> GetOptionSets(OrganizationServiceProxy serviceProxy)
        {
            //var req = new RetrieveEntityRequest()
            var request = new RetrieveAllOptionSetsRequest();
            var response = (RetrieveAllOptionSetsResponse)serviceProxy.Execute(request);

            foreach (OptionSetMetadata metadata in response.OptionSetMetadata.Where(o => o.OptionSetType != OptionSetType.Boolean))
            {
                SchemaStorage.Instance.AddOptionSet(metadata);
                if (!metadata.Options.Any()) continue;
                foreach (var option in metadata.Options)
                {
                    SchemaStorage.Instance.AddOption(metadata, option);
                }
            }

            return SchemaStorage.Instance.Entities;
        }

    }
}
