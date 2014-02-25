using System.Collections.Generic;
using Dynamics.DataAccess.Repository;
using Microsoft.Xrm.Sdk;

namespace Dynamics.DataAccess.UnitTests
{
    public class TestEntity : Entity
    {
        public TestEntity()
        {
            LogicalName = "new_testentity";
        }

        public EntityReference MainFooEntityReference { get; set; }
        public string Name { get; set; }

        [LookupReferenceName(FieldName = "MainFooEntityReference")]
        public FooEntity MainFooEntity { get; set; }
        [ForeignKeyOnChild(LogicalName = "new_TestEntity")]
        public IList<FooEntity> OtherFooEntities { get; set; }
    }
}
