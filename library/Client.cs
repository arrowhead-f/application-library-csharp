/********************************************************************************
 * Copyright (c) 2021 LTU
 *
 * This program and the accompanying materials are made available under the
 * terms of the Eclipse Public License 2.0 which is available at
 * http://www.eclipse.org/legal/epl-2.0.
 *
 * SPDX-License-Identifier: EPL-2.0
 *
 * Contributors:
 *   LTU - implementation
 ********************************************************************************/

using System;
using Arrowhead.Core;
using Arrowhead.Models;
using Arrowhead.Utils;
using log4net;
using log4net.Config;

namespace Arrowhead
{
    public class Client
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Client));
        private Service service;
        private Arrowhead.Models.System system;

        private ServiceRegistry ServiceRegistry;
        private Orchestrator Orchestrator;
        private Authorization Authorization;

        public Settings settings;

        /// <summary>
        /// Creates a Client object containing a Arrowhead service.
        /// This service is then registered to the Service Registry
        /// <remarks>
        /// If the service already exists in the registry then the stored entry is unregistered and 
        /// the service is reregistered
        /// </remarks>
        /// </summary>
        /// <param name="settings"></param>
        public Client(Settings settings)
        {
            this.settings = settings;
            this.InitCoreSystems();

            string authInfo = this.settings.getCoreSSL() ? Authorization.GetPublicKey() : "";

            this.system = new Arrowhead.Models.System(this.settings.SystemName, this.settings.Ip, this.settings.Port, "");
            this.service = new Service(this.system, this.settings.ServiceDefinition, this.settings.Interfaces, this.settings.ApiUri);

            try
            {
                ServiceResponse serviceResp = ServiceRegistry.RegisterService(this.service);
                
                this.system.Id = serviceResp.ProviderId;
                log.Info(this.service.ServiceDefinition + " was registered on the system " + this.system.SystemName);
            }
            catch (Exception e)
            {
                log.Error(e.Message);
                throw new Exception("Could not register the service: " + this.settings.ServiceDefinition + " with the system: " + this.settings.SystemName);
            }
        }

        public string GetSystemId()
        {
            return this.system.Id;
        }

        /// <summary>
        /// This methods builds a list of URLs that the producing service can be reached by
        /// These urls are based on the system address and port as wells as the serviceUri 
        /// The method checks what interfaces the service accepts and sets the url to either 
        /// http or https depending on the interfaces specified
        /// </summary>
        /// <returns>A list of URLs that can be used to connect to the service</returns>
        public string[] GetServiceURLs()
        {
            string baseURL = this.system.Address + ":" + this.system.Port + this.service.ServiceUri + "/";

            string[] urls = new string[this.service.Interfaces.Length];

            for (int i = 0; i < this.service.Interfaces.Length; i++)
            {
                if (this.service.Interfaces[i] == "HTTPS-SECURE-JSON")
                {
                    urls[i] = "https://" + baseURL;
                }
                else if (this.service.Interfaces[i] == "HTTP-INSECURE-JSON")
                {
                    urls[i] = "http://" + baseURL;
                }
                else
                {
                    throw new Exception("Invalid interface type " + this.service.Interfaces[i]);
                }
            }

            return urls;
        }

        /// <summary>
        /// Start orchestation 
        /// </summary>
        /// <remarks>
        /// If the response is empty then the Authenticators Intrarules entry and/or the Ochestration Store entry is wrongly configured
        /// </remarks>
        /// <param name="providerServiceDefinition"></param>
        /// <returns>A JSON array containing all available providers the client system has the rights to consume</returns>
        public OrchestratorResponse[] Orchestrate()
        {
            return this.Orchestrator.OrchestrateStatic(this.system);
        }

        /// <summary>
        /// Initializes connection to the mandatory core systems
        /// </summary>
        private void InitCoreSystems()
        {
            Http http = new Http(this.settings);
            this.ServiceRegistry = new ServiceRegistry(http, this.settings);
            this.Authorization = new Authorization(http, this.settings);
            this.Orchestrator = new Orchestrator(http, this.settings);
        }
    }
}
