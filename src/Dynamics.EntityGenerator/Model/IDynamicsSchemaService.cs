using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dynamics.Schema;

namespace Dynamics.EntityGenerator.Model
{
    public interface IDynamicsSchemaService
    {
        void GetData(Action<DataItem, Exception> callback);
        void GetSchema(Action<SchemaStorage, Exception> callback);
    }
}
