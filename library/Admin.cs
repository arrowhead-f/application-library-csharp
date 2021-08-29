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
using Newtonsoft.Json.Linq;
using log4net;
using log4net.Config;

namespace Arrowhead
{
    public class Admin
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Admin));

        private ServiceRegistry ServiceRegistry;
        private Orchestrator Orchestrator;
        private Authorization Authorization;
        public Settings settings;

        /// <summary>
        /// Creates a Admin object with access to the management endpoints 
        /// of the core system APIs
        /// </summary>
        /// <remarks>
        /// <list>
        ///     <item>Consumer System ID of the system that should have access to a specific Provider Service</item>
        /// 
        ///     <item>Provider Service definition of the Service the Consumer wants to consume</item>
        ///     <item>Provider System the wanted Provider Service is a part of</item>
        ///     <item>Provider Service interfaces the Provider allows</item>
        ///     
        ///     <item>Cloud Name</item>
        ///     <item>Cloud Operator</item>
        /// </list>
        /// </remarks>
        ///  
        /// <param name="settings"></param>
        public Admin(Settings settings)
        {
            this.settings = settings;
            this.InitCoreSystems();
        }

        /// <summary>
        /// Given a Consumer System ID it creates the Intracloud ruleset in the Authenticator
        /// and stores the Orchestration Entry in the Orchestrator with the configured system 
        /// in the settings object. This is done so that the Consumer can consume the Provider service
        /// </summary>
        public void StoreOrchestrateEntry(string consumerSystemId)
        {
            JObject providerSystem = new JObject();
            providerSystem.Add("systemName", this.settings.SystemName);
            providerSystem.Add("address", this.settings.Ip);
            providerSystem.Add("port", this.settings.Port);

            JObject cloud = new JObject();
            cloud.Add("operator", this.settings.CloudOperator);
            cloud.Add("name", this.settings.CloudName);

            try
            {
                ServiceResponse resp = ServiceRegistry.GetService(this.settings.ServiceDefinition, providerSystem, this.settings.Interfaces);
                Authorization.Authorize(consumerSystemId, new string[] { resp.ProviderId }, new string[] { resp.InterfaceId }, new string[] { resp.ServiceDefinitionId });
                Orchestrator.StoreOrchestrateEntry(consumerSystemId, this.settings.ServiceDefinition, this.settings.Interfaces[0], providerSystem, cloud);
            }
            catch(IntracloudRulesetExistsException e) {
                log.Info("Could not create Intracloud ruleset, already exists. Continuing...");
            }
            catch(OrchestrationStoreEntryExistsException e) {
                log.Info("Could not store orchestration entry, entry already exists. Continuing...");
            }
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
