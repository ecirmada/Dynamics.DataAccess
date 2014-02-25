using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;

namespace Dynamics.DataAccess.Repository
{
    public class QueryableRepository : IQueryableRepository
    {
        private OrganizationServiceContext context;

        public QueryableRepository(OrganizationServiceContext context)
        {
            this.context = context;
        }

        public IEnumerable<T> Execute<T>(IQuery<T> query) where T : Entity
        {
            return query.Execute(context);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (context != null)
                {
                    context.Dispose();
                    context = null;
                }
            }
        }

        ~QueryableRepository()
        {
            Dispose(false);
        }
    }
}