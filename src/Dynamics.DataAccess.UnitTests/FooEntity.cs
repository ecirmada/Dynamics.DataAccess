using System.Collections.Generic;
using Dynamics.DataAccess.Repository;
using Microsoft.Xrm.Sdk;

namespace Dynamics.DataAccess.UnitTests
{
    public class FooEntity : Entity
    {
        public EntityReference TestEntityReference { get; set; }
        public EntityReference BarEntityReference { get; set; }

        [LookupReferenceName(FieldName = "BarEntityReference")]
        public BarEntity BarEntity { get; set; }
        [ForeignKeyOnChild(LogicalName = "new_FooEntity")]
        public List<BarEntity> OtherBarEntities { get; set; }
    }
}