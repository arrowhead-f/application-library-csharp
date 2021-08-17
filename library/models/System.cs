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

using Newtonsoft.Json.Linq;
namespace Arrowhead.Models
{
    public class System
    {
        public string SystemName, Address, Port, Id;
        public string AuthenticationInfo;

        public System(string systemName, string address, string port, string authenticationInfo)
        {
            this.SystemName = systemName;
            this.Address = address;
            this.Port = port;
            this.AuthenticationInfo = authenticationInfo;
        }

        /// <summary>
        /// Returns the system name in the Arrowhead common format of "systemname.cloudName.operator.Arrowhead.eu"  
        /// </summary>
        /// <param name="op">operator name</param>
        /// <param name="cloudName"></param>
        /// <returns></returns>
        public string ArrowheadCommonName(string op, string cloudName)
        {
            return this.SystemName + "." + cloudName + "." + op + ".Arrowhead.eu";
        }

        /// <summary>
        /// Build a System object based on a json fetched from the service registry
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static System Build(JToken json)
        {
            string systemName = json.SelectToken("systemName").ToString();
            string address = json.SelectToken("address").ToString();
            string port = json.SelectToken("port").ToString();
            string authInfo = json.SelectToken("authenticationInfo").ToString();

            System system = new System(systemName, address, port, authInfo);
            system.Id = json.SelectToken("id").ToString();

            return system;
        }
    }
}
