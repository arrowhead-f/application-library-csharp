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
using Newtonsoft.Json.Linq;

namespace Arrowhead.Models
{
    public class Service
    {
        public string ServiceDefinition { get; set; }
        public string[] Interfaces { get; set; }
        public System ProviderSystem;
        public int Id;
        public string ServiceUri;

        public Service(System system, string serviceDefinition, string[] interfaces, string serviceUri)
        {
            this.ProviderSystem = system;
            this.ServiceDefinition = serviceDefinition;
            this.Interfaces = interfaces;
            this.ServiceUri = serviceUri;
        }

        public override string ToString()
        {
            return this.ServiceDefinition;
        }


        /// <summary>
        /// Build a Service object based on the json fetched from the service registry
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static Service Build(JToken json)
        {
            JToken systemJson = json.SelectToken("provider");
            System sys = System.Build(systemJson);

            JToken serviceJson = json.SelectToken("serviceDefinition");
            string serviceDef = serviceJson.SelectToken("serviceDefinition").ToString();

            JArray interfacesJson = JArray.Parse(json.SelectToken("interfaces").ToString());

            string[] interfaces = new string[interfacesJson.Count];
            for (int i = 0; i < interfaces.Length; i++)
            {
                interfaces[i] = interfacesJson.Value<JToken>(i).SelectToken("interfaceName").ToString();
            }
            Service service = new Service(sys, serviceDef, interfaces, "");
            service.Id = json.SelectToken("id").ToObject<Int32>();
            return service;
        }
    }
}
