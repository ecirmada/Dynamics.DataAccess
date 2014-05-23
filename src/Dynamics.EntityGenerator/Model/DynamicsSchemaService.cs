using System;
using Dynamics.Schema;

namespace Dynamics.EntityGenerator.Model
{
    public class DynamicsSchemaService : IDynamicsSchemaService
    {
        public void GetData(Action<DataItem, Exception> callback)
        {
            // Use this to connect to the actual data service

            var item = new DataItem("Welcome to MVVM Light");
            callback(item, null);
        }

        public void GetSchema(Action<SchemaStorage, Exception> callback)
        {
            throw new NotImplementedException();
        }
    }
}