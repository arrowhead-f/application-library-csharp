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
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Arrowhead.Models;
using Grapevine.Client;
using Grapevine.Shared;

namespace ArrowheadConsumer
{
    class Program
    {
        private Arrowhead.Admin Admin;
        private Arrowhead.Client Client;

        private string ProducerHost = "";
        private string ProducerPort = "";
        private string ServiceUri = "";
        private bool ProducerSSL = false;

        public Program(JObject consumerConfig, JObject adminConfig)
        {
            // load information about the local client
            Arrowhead.Utils.Settings settings = new Arrowhead.Utils.Settings(consumerConfig);
            this.Client = new Arrowhead.Client(settings);

            // Load information about the Provider System that this Client wants to consume
            Arrowhead.Utils.Settings adminSettings = new Arrowhead.Utils.Settings(adminConfig);
            this.Admin = new Arrowhead.Admin(adminSettings);

            // creates orchestration information between this client system and 
            // the system that has been configured in the Admin Config 
            this.Admin.StoreOrchestrate(this.Client.GetSystemId());

            // start orchestration between this client and a producer that this client has the rights to consume
            OrchestratorResponse[] orchestrations = this.Client.Orchestrate();

            foreach (OrchestratorResponse orchestration in orchestrations)
            {
                this.ProducerHost = orchestration.Provider.Address;
                this.ProducerPort = orchestration.Provider.Port;
                this.ServiceUri = orchestration.ServiceUri;

                ProducerSSL = orchestration.Interfaces[0] == "HTTPS-SECURE-JSON";

                Console.WriteLine("Orchestration against http" + (ProducerSSL ? "s://" : "://") + this.ProducerHost + ":" + this.ProducerPort + this.ServiceUri + " was started");
            }
        }

        public void ConsumeService()
        {
            RestClient client = new RestClient();

            client.Host = this.ProducerHost;
            client.Port = Int32.Parse(this.ProducerPort);

            RestRequest request = new RestRequest(this.ServiceUri + "/demo");
            request.HttpMethod = HttpMethod.GET;
            while (true)
            {
                RestResponse response = (RestResponse)client.Execute(request);
                Console.WriteLine(JsonConvert.DeserializeObject<JObject>(response.GetContent()));
                System.Threading.Thread.Sleep(1000);
            }
        }

        static void Main(string[] args)
        {
            JObject consumerConfig = JObject.Parse(File.ReadAllText(@"consumer.json"));
            JObject adminConfig = JObject.Parse(File.ReadAllText(@"admin.json"));
            Program consumer = new Program(consumerConfig, adminConfig);
            consumer.ConsumeService();
        }
    }
}
