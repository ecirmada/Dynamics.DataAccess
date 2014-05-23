using System;
using Dynamics.EntityGenerator.Model;
using Dynamics.Schema;

namespace Dynamics.EntityGenerator.Design
{
    public class DesignDataService : IDynamicsSchemaService
    {
        public void GetData(Action<DataItem, Exception> callback)
        {
            // Use this to create design time data

            var item = new DataItem("Welcome to MVVM Light [design]");
            callback(item, null);
        }

        public void GetSchema(Action<SchemaStorage, Exception> callback)
        {
            throw new NotImplementedException();
        }
    }
}