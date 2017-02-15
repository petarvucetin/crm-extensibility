using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ArcherIncPlugin
{
    public static class Demo
    {
        public static string ToJSon(this IExecutionContext context)
        {
            var serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(typeof(IExecutionContext));
            using (MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms, context);
                return Encoding.Default.GetString(ms.ToArray());
            }
        }

        public static string CallAzureFunction(IExecutionContext context, ITracingService tracingService)
        {
            try
            {
                //var ctx = context.ToJSon();

                //design note: move to ConfigurationEntity or pass as configuration to plugin
                //var webAddress = new Uri("https://crm-archerinc.azurewebsites.net/api/Crm-ArcherIncHttp?code=Ggahdjw10AwqqgxmQmgQd/As1mx3mPtHTMVTgoc1ptOrjCMnu1MSOg==");
                var webAddress = new Uri("http://vm-crm2016.vm-crm2016.dev:7071/api/Crm-ArcherIncHttp");
            

                var postData = "{ 'name' : 'CRM Plugin' }";

                using (WebClient client = new WebClient())
                {
                    client.Headers["Content-Type"] = "application/json; charset=utf-8";
                    client.Headers["x-functions-key"] = "Ggahdjw10AwqqgxmQmgQd/As1mx3mPtHTMVTgoc1ptOrjCMnu1MSOg==";
                    var response = client.UploadString(webAddress, postData);
                    return response;
                }
            }

            catch (WebException exception)
            {
                string str = string.Empty;
                if (exception.Response != null)
                {
                    using (StreamReader reader =
                        new StreamReader(exception.Response.GetResponseStream()))
                    {
                        str = reader.ReadToEnd();
                    }
                    exception.Response.Close();
                }
                if (exception.Status == WebExceptionStatus.Timeout)
                {
                    throw new InvalidPluginExecutionException(
                        "The timeout elapsed while attempting to issue the request.", exception);
                }
                throw new InvalidPluginExecutionException(String.Format(CultureInfo.InvariantCulture,
                    "A Web exception occurred while attempting to issue the request. {0}: {1}",
                    exception.Message, str), exception);
            }
        }
    }
}
