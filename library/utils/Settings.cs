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
using log4net;
using log4net.Config;

namespace Arrowhead.Utils
{
    public class Settings
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Settings));

        public Boolean CoreSSL, VerifyCertificate;
        private string ServiceRegistryAddress, ServiceRegistryPort;
        private string OrchestratorAddress, OrchestratorPort;
        private string AuthorizationAddress, AuthorizationPort;
        public string CertificatePath, CertificatePassword;
        public string SystemName, Ip, Port, ConsumerSystemId;
        public string ServiceDefinition, ApiUri;
        public string[] Interfaces;
        public string CloudName, CloudOperator;

        /// <summary>
        /// Initializes a settings object with default settings
        /// </summary>
        public Settings()
        {
            this.Interfaces = new string[] { "HTTPS-SECURE-JSON" };

            this.SystemName = "";
            this.Ip = "127.0.0.1";
            this.Port = "8080";

            this.CoreSSL = true;
            this.VerifyCertificate = false;
            this.ServiceRegistryAddress = "127.0.0.1";
            this.ServiceRegistryPort = "8443";
            this.OrchestratorAddress = "127.0.0.1";
            this.OrchestratorPort = "8441";
            this.AuthorizationAddress = "127.0.0.1";
            this.AuthorizationPort = "8445";

            this.CertificatePath = "/home/user/Projects/arrowhead/core-java-spring/certificates/testcloud2/sysop.p12";
        }

        public Settings(JObject config)
        {
            this.SystemName = config.SelectToken("system.name").ToString();
            this.Ip = config.SelectToken("system.ip").ToString();
            this.Port = config.SelectToken("system.port").ToString();

            this.ServiceDefinition = config.SelectToken("service.serviceDefinition").ToString();
            this.ApiUri = config.SelectToken("service.apiUri").ToString();
            this.Interfaces = ((JArray)config.SelectToken("service.interfaces")).ToObject<string[]>();

            this.ServiceRegistryAddress = config.SelectToken("core.serviceregistry.address").ToString();
            this.ServiceRegistryPort = config.SelectToken("core.serviceregistry.port").ToString();
            this.OrchestratorAddress = config.SelectToken("core.orchestrator.address").ToString();
            this.OrchestratorPort = config.SelectToken("core.orchestrator.port").ToString();
            this.AuthorizationAddress = config.SelectToken("core.authorization.address").ToString();
            this.AuthorizationPort = config.SelectToken("core.authorization.port").ToString();

            this.CoreSSL = config.SelectToken("core.ssl").ToObject<bool>();
            this.VerifyCertificate = config.SelectToken("core.verifyCertificate").ToObject<bool>();
            this.CertificatePath = config.SelectToken("core.certificatePath").ToString();
            this.CertificatePassword = config.SelectToken("core.certificatePassword").ToString();

            try
            {
                this.CloudOperator = config.SelectToken("cloud.operator").ToString();
                this.CloudName = config.SelectToken("cloud.name").ToString();
            }
            catch (Exception e)
            {
                log.Info("Could not parse cloud information in settings");
            }
        }

        public void SetServiceSettings(string serviceDefinition, string[] interfaces, string apiUri)
        {
            this.ServiceDefinition = serviceDefinition;
            this.Interfaces = interfaces;
            this.ApiUri = apiUri;
        }

        public void SetSystemSettings(string systemName, string ip, string port)
        {
            this.SystemName = systemName;
            this.Ip = ip;
            this.Port = port;
        }

        public void SetSystemSettings(string systemName, string ip, string port, string id)
        {
            this.SystemName = systemName;
            this.Ip = ip;
            this.Port = port;
            this.ConsumerSystemId = id;
        }

        public void SetCloudSettings(string cloudName, string cloudOperator)
        {
            this.CloudOperator = cloudOperator;
            this.CloudName = cloudName;
        }

        public void SetCertPath(string certPath)
        {
            this.CertificatePath = certPath;
        }

        public string getServiceRegistryUrl()
        {
            string scheme = this.CoreSSL ? "https://" : "http://";
            return scheme + this.ServiceRegistryAddress + ":" + this.ServiceRegistryPort;
        }
        public string getOrchestratorUrl()
        {
            string scheme = this.CoreSSL ? "https://" : "http://";
            return scheme + this.OrchestratorAddress + ":" + this.OrchestratorPort;
        }

        public string getAuthorizationUrl()
        {
            string scheme = this.CoreSSL ? "https://" : "http://";
            return scheme + this.AuthorizationAddress + ":" + this.AuthorizationPort;
        }

        public bool getCoreSSL()
        {
            return CoreSSL;
        }
    }
}
