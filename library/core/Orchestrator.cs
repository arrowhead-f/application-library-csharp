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
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Arrowhead.Utils;
using Arrowhead.Models;
using log4net;
using log4net.Config;

namespace Arrowhead.Core
{
    public class Orchestrator
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Orchestrator));

        private Http Http;
        private string BaseUrl;
        public Orchestrator(Http http, Settings settings)
        {
            this.BaseUrl = settings.getOrchestratorUrl() + "/orchestrator";
            this.Http = http;
        }

        /// <summary>
        /// Start static orchestration by fetching provider data from the Orchestrator.
        /// This will provide a list of 
        /// </summary>
        /// <remarks>
        /// This is a call to the Client endpoint of the Orchestrator API thus it requires:
        /// <list>
        /// <item> A certificate for the consumer system</item>
        /// <item> An intracloud ruleset stored in the Authenticator by an Admin via the management endpoint or the Management Tools</item>
        /// <item> An store entry in the Orchestrator by an Admin via the management endpoint or the Management Tools</item>
        /// </list>
        /// </remarks>
        /// <param name="consumer"></param>
        /// <returns></returns>
        public OrchestratorResponse[] OrchestrateStatic(Arrowhead.Models.System consumer)
        {
            JObject payload = new JObject();
            JObject requesterSystem = JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(consumer));
            requesterSystem.Remove("id");

            JObject orchestrationFlags = new JObject();
            orchestrationFlags.Add("overrideStore", false);

            payload.Add("requesterSystem", requesterSystem);
            payload.Add("orchestrationFlags", orchestrationFlags);

            HttpResponseMessage resp = this.Http.Post(this.BaseUrl, "/orchestration", payload);
            string responseMessage = resp.Content.ReadAsStringAsync().Result;
            JObject jsonMessage = JsonConvert.DeserializeObject<JObject>(responseMessage);
            JArray response = (JArray)jsonMessage.SelectToken("response");
            OrchestratorResponse[] orchestration = new OrchestratorResponse[response.Count];

            for(int i = 0; i < response.Count; i++) {
                orchestration[i] = new OrchestratorResponse((JObject) response[i]);
            }

            return orchestration;
        }

        /// <summary>
        /// Add a store entry to the orchestration store, this is needed for the Consumer to 
        /// be able to fetch the orchestration information about the Provider from the Orchestrator
        /// </summary>
        /// <remarks>
        /// NOTE This is a call to the Management endpoint of the Orchestrator, thus it requires a Sysop Certificate
        /// This can also be done via the Management Tool on the Arrowhead Tools Github page
        /// https://github.com/arrowhead-tools/mgmt-tool-js
        /// </remarks>
        /// <param name="consumerSystemId"></param>
        /// <param name="requestedServiceDefinition"></param>
        /// <param name="serviceInterfaceName"></param>
        /// <param name="providerSystem"></param>
        /// <param name="cloud"></param>
        /// <returns></returns>
        public void StoreOrchestrateEntry(string consumerSystemId, string requestedServiceDefinition, string serviceInterfaceName, JObject providerSystem, JObject cloud)
        {
            JObject entry = new JObject();
            entry.Add("serviceDefinitionName", requestedServiceDefinition);
            entry.Add("consumerSystemId", consumerSystemId);
            entry.Add("cloud", cloud);
            entry.Add("providerSystem", providerSystem);
            entry.Add("serviceInterfaceName", serviceInterfaceName);
            entry.Add("priority", 1);

            // the api takes the input as a list of new entries
            JArray payload = new JArray();
            payload.Add(entry);

            HttpResponseMessage resp = this.Http.Post(this.BaseUrl, "/mgmt/store", payload);
            resp.EnsureSuccessStatusCode();

            JObject respMessage = JObject.Parse(resp.Content.ReadAsStringAsync().Result);
            if (respMessage.SelectToken("count").ToObject<int>() > 0)
            {
                log.Info("Orchestration store entry added");
            }
            else
            {
                throw new OrchestrationStoreEntryExistsException();
            }
        }

        public string GetOrchestrationById(string id)
        {
            HttpResponseMessage resp = this.Http.Get(this.BaseUrl, "/orchestration/" + id);
            return resp.Content.ReadAsStringAsync().Result;
        }
    }
}
