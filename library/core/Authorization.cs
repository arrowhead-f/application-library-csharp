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
using Newtonsoft.Json.Linq;
using Arrowhead.Utils;
using Arrowhead.Models;
using log4net;
using log4net.Config;

namespace Arrowhead.Core
{
    public class Authorization
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Authorization));

        private Http Http;
        private string BaseUrl;

        public Authorization(Http http, Settings settings)
        {
            this.BaseUrl = settings.getAuthorizationUrl() + "/authorization";
            this.Http = http;
        }

        /// <summary>
        /// Add a intracloud Authorization ruleset for a consumer system to a list of provider systems with a list of service definitions and interaces
        /// </summary>
        /// <remarks>
        /// NOTE This is a call to the Management endpoint of the Authorization, thus it requires a Sysop Certificate
        /// This can also be done via the Management Tool on the Arrowhead Tools Github page
        /// https://github.com/arrowhead-tools/mgmt-tool-js
        /// </remarks>
        /// <param name="consumerSystemId"></param>
        /// <param name="providerIds"></param>
        /// <param name="interfaceIds"></param>
        /// <param name="serviceDefinitionIds"></param>
        /// <returns></returns>
        public void Authorize(string consumerSystemId, string[] providerIds, string[] interfaceIds, string[] serviceDefinitionIds)
        {
            JObject payload = new JObject();
            payload.Add("consumerId", consumerSystemId);
            payload.Add("providerIds", new JArray(providerIds));
            payload.Add("interfaceIds", new JArray(interfaceIds));
            payload.Add("serviceDefinitionIds", new JArray(serviceDefinitionIds));

            HttpResponseMessage resp = this.Http.Post(this.BaseUrl, "/mgmt/intracloud", payload);
            resp.EnsureSuccessStatusCode();

            JObject respMessage = JObject.Parse(resp.Content.ReadAsStringAsync().Result);
            if (respMessage.SelectToken("count").ToObject<int>() > 0)
            {
                log.Info("Intracloud ruleset created");
            }
            else
            {
                throw new IntracloudRulesetExistsException();
            }
        }

        public string GetPublicKey()
        {
            HttpResponseMessage resp = this.Http.Get(this.BaseUrl, "/publickey");
            return resp.Content.ReadAsStringAsync().Result;
        }
    }
}
