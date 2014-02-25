using Dynamics.DataAccess.Repository;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Dynamics.DataAccess.UnitTests
{
    [TestClass]
    public class RepositoryTests
    {
        [TestMethod]
        public void HydrateSimpleProperty_DoesNotThrow()
        {
            var repository = new Repository<TestEntity>(null);
            var testEntity = new TestEntity();
            repository.HydrateProperties(testEntity, e => e.Id);
        }

        [TestMethod]
        public void HydrateSimpleList_DoesNotThrow()
        {
            var repository = new Repository<TestEntity>(null);
            var testEntity = new TestEntity();
            repository.HydrateProperties(testEntity, e => e.OtherFooEntities);
        }

        [TestMethod]
        public void HydrateMultipleProperties_DoesNotThrow()
        {
            var repository = new Repository<TestEntity>(null);
            var testEntity = new TestEntity();
            repository.HydrateProperties(testEntity,
                e => e.MainFooEntity,
                e => e.OtherFooEntities);
        }

        [TestMethod]
        public void HydrateListWithIncludes_DoesNotThrow()
        {
            var repository = new Repository<TestEntity>(null);
            var testEntity = new TestEntity();
            repository.HydrateProperties(testEntity, 
                e => e.OtherFooEntities.Include(
                    f => f.BarEntity,
                    f => f.OtherBarEntities));
        }

        [TestMethod]
        public void HydrateNestedIncludes_DoesNotThrow()
        {
            var repository = new Repository<TestEntity>(null);
            var testEntity = new TestEntity();
            repository.HydrateProperties(testEntity, 
                e => e.OtherFooEntities.Include(
                    f => f.BarEntity.Include(
                        b => b.SomeRandomEntity)));
        }

        [TestMethod]
        public void HydrateNestedListIncludes_DoesNotThrow()
        {
            var repository = new Repository<TestEntity>(null);
            var testEntity = new TestEntity();
            repository.HydrateProperties(testEntity, 
                e => e.OtherFooEntities.Include(
                    f => f.OtherBarEntities.Include(
                        b => b.SomeRandomEntity)));
        }
    }
}
