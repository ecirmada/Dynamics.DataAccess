using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk.Client;
using Dynamics.Schema;
using Microsoft.Xrm.Sdk.Metadata;

namespace Dynamics.Schema.Example
{
    class Program
    {
        static void Main(string[] args)
        {

            var service = GetService();

            var entities = EntityExtractor.GetEntities(service,true);
            SchemaStorage.Instance.ExportSchemas("test");
        }


        public static OrganizationServiceProxy GetService()
        {
            var authenticationType = Properties.Settings.Default.AuthenticationType;
            var userName = Properties.Settings.Default.UserName;
            var password = Properties.Settings.Default.Password;
            var domain = Properties.Settings.Default.Domain;
            var serviceUri = Properties.Settings.Default.CRMServiceURL;

            var credentials = new ClientCredentials();

            authenticationType = authenticationType == null ? null : authenticationType.ToLower();

            switch (authenticationType)
            {
                case "claims":
                    //for claims based authentication
                    credentials.UserName.UserName = !string.IsNullOrEmpty(domain) ? string.Concat(domain, @"\", userName) : userName;
                    credentials.UserName.Password = password;
                    break;
                default:
                    //for AD based authentication
                    credentials.Windows.ClientCredential = new System.Net.NetworkCredential(userName, password, domain);
                    break;
            }

            var organizationUri = new Uri(serviceUri);
            var orgService = new OrganizationServiceProxy(organizationUri, null, credentials, null);
            orgService.ServiceConfiguration.CurrentServiceEndpoint.Behaviors.Add(new ProxyTypesBehavior());
            return orgService;
        }
    }
}
