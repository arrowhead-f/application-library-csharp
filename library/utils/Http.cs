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
using System.Text;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;

namespace Arrowhead.Utils
{
    public class Http
    {
        private HttpClient client;

        public Http(Settings settings)
        {
            HttpClientHandler handler = new HttpClientHandler();
            X509Certificate certificate = new X509Certificate2(settings.CertificatePath, settings.CertificatePassword);
            handler.ClientCertificates.Add(certificate);

            if (!settings.VerifyCertificate)
            {
                handler.ServerCertificateCustomValidationCallback =
                    (httpRequestMessage, cert, cetChain, policyErrors) =>
                {
                    return true;
                };
            }
            this.client = new HttpClient(handler);
        }

        public HttpResponseMessage Get(string baseURL, string apiEndpoint)
        {
            HttpResponseMessage response = client.GetAsync(baseURL + apiEndpoint).Result;
            return response;
        }

        public HttpResponseMessage Post(string baseURL, string apiEndpoint, Object payload)
        {
            StringContent content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PostAsync(baseURL + apiEndpoint, content).Result;
            return response;
        }

        public HttpResponseMessage Delete(string baseURL, string apiEndpoint)
        {
            HttpResponseMessage response = client.DeleteAsync(baseURL + apiEndpoint).Result;
            return response;
        }
    }
}
