using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;

namespace Dynamics.DataAccess.Repository
{
    public interface IQueryableRepository : IDisposable
    {
        IEnumerable<T> Execute<T>(IQuery<T> query) where T : Entity;
    }
}