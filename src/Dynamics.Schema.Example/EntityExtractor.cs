using System.Collections.Generic;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using Entity = Dynamics.Schema.Containers.Entity;

namespace Dynamics.Schema.Example
{
    public static class EntityExtractor
    {
        public static EntityMetadata GetSchema( OrganizationServiceProxy serviceProxy, string entity)
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
                foreach (var attribute in metadata.Attributes)
                {
                    SchemaStorage.Instance.AddAttribute(metadata, attribute);
                }
            }

            return SchemaStorage.Instance.Entities;
        }


    }
}
