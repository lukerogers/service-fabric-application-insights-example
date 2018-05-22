﻿using System;
using System.Collections.Generic;
using System.Fabric;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.ServiceFabric;
using Microsoft.ApplicationInsights.ServiceFabric.Module;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using StatelessBackend.Interfaces;

namespace StatelessBackend
{
    /// <summary>
    /// The FabricRuntime creates an instance of this class for each service type instance. 
    /// </summary>
    internal sealed class StatelessBackend : StatelessService, IStatelessBackendService
    {
        public StatelessBackend(StatelessServiceContext context)
            : base(context)
        {
            var config = TelemetryConfiguration.Active;
            config.TelemetryInitializers.Add(FabricTelemetryInitializerExtension.CreateFabricTelemetryInitializer(this.Context));

            var requestTrackingModule = new ServiceRemotingRequestTrackingTelemetryModule();
            var dependencyTrackingModule = new ServiceRemotingDependencyTrackingTelemetryModule();
            requestTrackingModule.Initialize(config);
            dependencyTrackingModule.Initialize(config);
        }

        public Task<long> GetCountAsync()
        {
            var rand = new Random();
            return Task.FromResult((long)rand.Next());
        }

        /// <summary>
        /// Optional override to create listeners (like tcp, http) for this service instance.
        /// </summary>
        /// <returns>The collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return this.CreateServiceRemotingInstanceListeners();
            //return new ServiceInstanceListener[]
            //{
            //    new ServiceInstanceListener(serviceContext =>
            //        new KestrelCommunicationListener(serviceContext, "ServiceEndpoint", (url, listener) =>
            //        {
            //            ServiceEventSource.Current.ServiceMessage(serviceContext, $"Starting Kestrel on {url}");

            //            return new WebHostBuilder()
            //                        .UseKestrel()
            //                        .ConfigureServices(
            //                            services => services
            //                                .AddSingleton<StatelessServiceContext>(serviceContext))
            //                        .UseContentRoot(Directory.GetCurrentDirectory())
            //                        .UseStartup<Startup>()
            //                        .UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.None)
            //                        .UseUrls(url)
            //                        .Build();
            //        }))
            //};
        }
    }
}