using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;

namespace Dynamics.DataAccess.Repository
{
    public interface IQuery<out T>
        where T : Entity
    {
        IEnumerable<T> Execute(OrganizationServiceContext context);
    }
}