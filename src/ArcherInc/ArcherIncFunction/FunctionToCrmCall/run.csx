using System;
using System.Net;

using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Discovery;
using Microsoft.Xrm.Sdk.Query;


public static void Run(TimerInfo myTimer, TraceWriter log)
{
    log.Info($"C# Timer trigger function executed at: {DateTime.Now}");

    var xrmServiceUrl = "/XRMServices/2011/Organization.svc";
    var discoveryServiceUrl = "/XRMServices/2011/Discovery.svc";

    var org = "ArcherInc";
    var host = "http://vm-crm2016.vm-crm2016.dev";

    var xrmEndpoint = host + "/" + org + "/" + xrmServiceUrl;
    var discoveryEndpoint = host + "/" + org + "/" + discoveryServiceUrl;

    log.Info($"xrmEndpoint: {xrmEndpoint}");

    var serviceManagement = Microsoft.Xrm.Sdk.Client.ServiceConfigurationFactory.CreateManagement<IOrganizationService>(new Uri(xrmEndpoint));
    var discoveryService = ServiceConfigurationFactory.CreateManagement<IDiscoveryService>(new Uri(discoveryEndpoint));
    var endpointType = serviceManagement.AuthenticationType;
    var ac = new AuthenticationCredentials();
    ac.ClientCredentials.Windows.ClientCredential = new NetworkCredential("{user name}", "{password}", "vmcrm2016");


    var proxy = new OrganizationServiceProxy(serviceManagement, ac.ClientCredentials);
    proxy.Timeout = TimeSpan.FromHours(1);

    var account = proxy.Retrieve("account", new Guid("61A7F30C-29F3-E611-80C5-00155D07290A"), new ColumnSet(true));
    var name = account["name"];
    log.Info($"Account Name: {name}");
}