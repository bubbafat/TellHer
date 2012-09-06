using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;
using TellHer.Service;
using System.ServiceModel;
using System.ServiceModel.Description;
using TellHer.Domain;
using StructureMap;
using TellHer.Sms;
using TellHer.Data;
using TellHer.SubscriptionService;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.IO;
using RestSharp.Contrib;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Globalization;

namespace WorkerRole1
{
    public class WorkerRole : RoleEntryPoint
    {
        public WorkerRole()
        {
            ObjectFactory.Initialize(x =>
            {
                x.For<ILogging>().Use<DatabaseLogger>();
                x.For<IOutgoingSmsQueue>().Use<SmsSender>();
                x.For<ISubscriptionService>().Use<SubscriptionService>();
                x.For<ITwilioSmsListener>().Use<TwilioSmsListener>();
                x.For<ITwilioRequestValidator>().Use<TwilioRequestValidator>();
                x.For<IServiceAuthorizationManager>().Use<TwilioServiceAuthorizationManager>();
                x.For<IConfiguration>().Use<TellHerServiceConfiguration>();
#if DEBUG
                x.For<ISmsTransport>().Use<MockSmsTransport>();
                x.For<ITwilioRequestValidator>().Use<MockTwilioRequestValidator>();
#else
                    x.For<ISmsTransport>().Use<TwilioSmsSender>();
                    x.For<ITwilioRequestValidator>().Use<TwilioRequestValidator>();
#endif
                DataStoreInitialize.Initialize(x);
            });
        }

        public override void Run()
        {
            LogManager.Log.Trace("*********************************************************************************************");
            LogManager.Log.Trace("******************************* TellHer Service Role Started ******************************* ");
            LogManager.Log.Trace("*********************************************************************************************");

            while (true)
            {
                try
                {
                    ProcessingLoop loop = new ProcessingLoop();
                    loop.Run();
                }
                catch (Exception ex)
                {
                    LogManager.Log.Error("Unhandled Exception in Processing Loop", ex);
                }
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            try
            {
                CreateServiceHost();
                LogManager.Log.Info("WCF Services started");
            }
            catch (Exception ex)
            {
                LogManager.Log.Error("Exception configuring WCF Services", ex);

                using (IOutgoingSmsQueue outgoing = ObjectFactory.GetInstance<IOutgoingSmsQueue>())
                {
                    IConfiguration config = ObjectFactory.GetInstance<IConfiguration>();
                    outgoing.Send(OutgoingSmsMessage.CreateWithDefaults(config.AdminNumber,
                        "WCF STARTUP ERROR: " + ex.GetType().FullName),
                        null,
                        null);
                }
            }

            return base.OnStart();
        }

        public override void OnStop()
        {
            serviceHost.Close();
            base.OnStop();
        }

        private ServiceHost serviceHost;
        private void CreateServiceHost()
        {
            serviceHost = new ServiceHost(typeof(TwilioSmsListener));

            RoleInstanceEndpoint externalEndPoint =
                RoleEnvironment.CurrentRoleInstance.InstanceEndpoints["TwilioSmsEndpoint"];

            string endpoint = string.Format(CultureInfo.InvariantCulture, "http://{0}/sms", externalEndPoint.IPEndpoint);

            LogManager.Log.Info("WCF Endpoing: {0}", endpoint);

            WebHttpBinding binding = new WebHttpBinding(WebHttpSecurityMode.None);

            WebHttpBehavior behavior = new WebHttpBehavior
            {
                FaultExceptionEnabled = true,
            };

            serviceHost.AddServiceEndpoint(typeof(ITwilioSmsListener), binding, endpoint).Behaviors.Add(behavior);

            serviceHost.Authorization.ServiceAuthorizationManager = new OuterServiceAuthorizationManager();

            serviceHost.Open();
        }
    }
}
