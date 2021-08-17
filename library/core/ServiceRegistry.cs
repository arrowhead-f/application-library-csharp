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
using Arrowhead.Models;
using Arrowhead.Utils;

namespace Arrowhead.Core
{
    public class ServiceRegistry
    {

        private Http Http;
        private string BaseUrl;

        public ServiceRegistry(Http http, Settings settings)
        {
            this.BaseUrl = settings.getServiceRegistryUrl() + "/serviceregistry";
            this.Http = http;
        }

        public object RegisterService(Service payload)
        {
            JObject deserializedJson = JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(payload));
            JObject providerSystem = (JObject)deserializedJson.GetValue("providerSystem");
            deserializedJson.Remove("id");
            deserializedJson.Remove("providerSystem");
            providerSystem.Remove("id");
            deserializedJson.Add("providerSystem", providerSystem);
            
            HttpResponseMessage resp = this.Http.Post(this.BaseUrl, "/register", deserializedJson);
            string respMessage = resp.Content.ReadAsStringAsync().Result;
            if (resp.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                if (UnregisterService(payload))
                {
                    return (ServiceResponse)RegisterService(payload);
                }
                else
                {
                    throw new Exception("Could not unregister existing service");
                }
            }
            else
            {
                resp.EnsureSuccessStatusCode();
                return new ServiceResponse(JsonConvert.DeserializeObject<JObject>(respMessage));
            }
        }

        public bool UnregisterService(Service payload)
        {
            // setup query parameters
            string serviceDefinition = "service_definition=" + payload.ServiceDefinition;
            string address = "address=" + payload.ProviderSystem.Address;
            string port = "port=" + payload.ProviderSystem.Port;
            string systemName = "system_name=" + payload.ProviderSystem.SystemName;
            try
            {
                HttpResponseMessage resp = this.Http.Delete(this.BaseUrl, "/unregister?" + serviceDefinition + "&" + address + "&" + port + "&" + systemName);
                string respMessage = resp.Content.ReadAsStringAsync().Result;
                resp.EnsureSuccessStatusCode();
                return true;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        /// <summary>
        /// Returns a ServiceResponse with IDs of provider, interfaces and service definition of a wanted
        /// service, system and interface combination
        /// </summary>
        /// <remarks>
        /// NOTE This is a call to the Management endpoint of the Orchestrator, thus it requires a Sysop Certificate
        /// </remarks>
        /// <param name = "serviceDefinition" ></ param >
        /// <param name="providerSystem"></param>
        /// <param name="providerInterfaces"></param>
        /// <returns></returns>
        public ServiceResponse GetService(string serviceDefinition, JObject providerSystem, string[] providerInterfaces)
        {
            HttpResponseMessage resp = this.Http.Get(this.BaseUrl, "/mgmt/servicedef/" + serviceDefinition);
            resp.EnsureSuccessStatusCode();

            string respMessage = resp.Content.ReadAsStringAsync().Result;
            JObject respObject = JsonConvert.DeserializeObject<JObject>(respMessage);
            JArray services = (JArray)respObject.SelectToken("data");

            int length = (int)respObject.SelectToken("count");
            if (length == 0) {
                throw new Exception("No existing services");
            }

            for (int i = 0; i < length; i++)
            {
                JObject service = (JObject)services[i];
                JObject system = (JObject)service.SelectToken("provider");
                JArray interfaces = (JArray)service.SelectToken("interfaces");

                if (EqualSystems(system, providerSystem) && EqualInterfaces(interfaces, providerInterfaces))
                {
                    return new ServiceResponse(service);
                }
            }

            throw new Exception("No service found");
        }


        public ServiceResponse[] GetServices()
        {
            HttpResponseMessage resp = this.Http.Get(this.BaseUrl, "/mgmt?direction=ASC&sort_field=id");
            resp.EnsureSuccessStatusCode();
            
            string respMessage = resp.Content.ReadAsStringAsync().Result;
            JObject respObject = JsonConvert.DeserializeObject<JObject>(respMessage);

            JArray servicesJson = (JArray)respObject.SelectToken("data");
            int length = (int)respObject.SelectToken("count");

            ServiceResponse[] services = new ServiceResponse[length];

            for (int i = 0; i < length; i++)
            {
                JObject service = (JObject)servicesJson[i];
                JObject system = (JObject)service.SelectToken("provider");
                JArray interfaces = (JArray)service.SelectToken("interfaces");

                services[i] = new ServiceResponse(service);
            }
            return services;
        }
        
        public object GetSystems()
        {
            HttpResponseMessage resp = this.Http.Get(this.BaseUrl, "/mgmt/systems?direction=ASC&sort_field=id");
            resp.EnsureSuccessStatusCode();
            
            try
            {
                string respMessage = resp.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject(respMessage);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine(e.Message);
                return e;
            }
        }

        /// <summary>
        /// Check if two systems have the same name, address and port
        /// </summary>
        /// <param name="sys1"></param>
        /// <param name="sys2"></param>
        /// <returns></returns>
        private static bool EqualSystems(JObject sys1, JObject sys2)
        {
            return sys1.GetValue("systemName").ToString() == sys2.GetValue("systemName").ToString() &&
                   sys1.GetValue("address").ToString() == sys2.GetValue("address").ToString() &&
                   sys1.GetValue("port").ToString() == sys2.GetValue("port").ToString();
        }

        /// <summary>
        /// Check if 2 lists of interfaces share a interface name
        /// </summary>
        /// <param name="interfaces1"></param>
        /// <param name="interfaces2"></param>
        /// <returns></returns>
        private static bool EqualInterfaces(JArray interfaces1, string[] interfaces2)
        {
            for (int i = 0; i < interfaces1.Count; i++)
            {
                for (int j = 0; j < interfaces2.Length; j++)
                {
                    JObject i1 = (JObject)interfaces1[i];
                    if (i1.GetValue("interfaceName").ToString() == interfaces2[j])
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
