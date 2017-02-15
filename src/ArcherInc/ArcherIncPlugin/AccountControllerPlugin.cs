using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;

namespace ArcherIncPlugin
{
    public class AccountControllerPlugin : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            #region extract services
            var tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            var executionContext = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            var serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            
            var endpointNotificationService = (IServiceEndpointNotificationService)serviceProvider.GetService(typeof(IServiceEndpointNotificationService));
            #endregion

            try
            {
                tracingService.Trace("AccountControllerPlugin plugin started.");

                SendContextToQueue(executionContext, endpointNotificationService, tracingService);

                SendContextToAzureFunction(executionContext, tracingService);

                tracingService.Trace("AccountControllerPlugin plugin completed.");
            }
            catch (Exception e)
            {
                tracingService.Trace("Exception: {0}", e.ToString());
                throw;
            }

        }

        private void SendContextToAzureFunction(IPluginExecutionContext executionContext,
            ITracingService tracingService)
        {
            //send message to Function
            tracingService.Trace("*** FUNCTION START ****");
            var response = Demo.CallAzureFunction(executionContext, tracingService);
            tracingService.Trace("response -> " + response);
            tracingService.Trace("*** FUNCTION END ****");

        }

        private void SendContextToQueue(IPluginExecutionContext executionContext, 
            IServiceEndpointNotificationService endpointNotificationService,
            ITracingService tracingService)
        {
            //send message to the queue
            var serviceEndpointId = new Guid("3dce31eb-7cf1-e611-80c5-00155d07290a");

            var azureQueue = new EntityReference("serviceendpoint", serviceEndpointId);

            endpointNotificationService.Execute(azureQueue, executionContext);

        }
    }
}
