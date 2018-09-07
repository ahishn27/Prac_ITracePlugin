using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;

namespace ITraceServicePlugin
{
    public class TracePlugin : IPlugin
        
    {
     

        public void Execute(IServiceProvider serviceProvider)
        {
                 //Tracing Service
            ITracingService tracingservice = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            tracingservice.Trace("Successfully Invoked ITraceingService");

            int step = 0;
            //Obtain execution context from service provider
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            IOrganizationServiceFactory organizationServiceFactory= (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = organizationServiceFactory.CreateOrganizationService(context.UserId);

            try
            {
                step = 1;

                if(context.InputParameters.Contains("Target") & context.InputParameters["Target"] is Entity)
                {

                    Guid contactID;
                    String phone;

                    Entity account = (Entity)context.InputParameters["Target"];
                    tracingservice.Trace("Succesfully obtained Account record" + account.Id.ToString());

                    if (account.LogicalName != "account")
                        return;
                 
                    try

                    {
                        tracingservice.Trace("Attempting to obtain Phone value...");
                        phone = account["telephone1"].ToString();

                    }

                    catch (Exception error)

                    {
                        tracingservice.Trace("Failed to obtain Phone field. Error Details: " + error.ToString());
                        throw new InvalidPluginExecutionException("A problem has occurred. Please press OK to continue using the application.");

                    }
                    if (phone != "")

                    {

                        //Build our contact record to create.

                        Entity contact = new Entity("contact");

                        contact["firstname"] = "Ned";
                        contact["lastname"] = "Flanders";

                        contact["parentcustomerid"] = new EntityReference("account", account.Id);

                        contact["description"] = "Ned's work number is " + phone + ".";

                        contactID = service.Create(contact);

                        tracingservice.Trace("Succesfully created Contact record " + contactID.ToString());

                        tracingservice.Trace("Done!");

                    }

                    else

                    {

                        tracingservice.Trace("Phone number was empty, Contact record was not created.");

                    }








                }
            }
            catch (Exception ex)
            {
                tracingservice.Trace("{0}", ex.ToString());
                throw;
            }




        }
    }
}
