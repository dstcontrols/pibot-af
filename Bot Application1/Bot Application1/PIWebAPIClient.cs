using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net;

namespace Bot_Application1
{
    public class PIWebAPIClient
    {
        private HttpClient client;

        /* Initiating HttpClient using the default credentials.
         * This can be used with Kerberos authentication for PI Web API. */
        public PIWebAPIClient()
        {
            client = new HttpClient(new HttpClientHandler() { UseDefaultCredentials = true });
        }

        /* Initializing HttpClient by providing a username and password. The basic authentication header is added to the HttpClient.
         * This can be used with Basic authentication for PI Web API. */
        public PIWebAPIClient(string userName, string password)
        {
            client = new HttpClient();
            string authInfo = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(String.Format("{0}:{1}", userName, password)));
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authInfo);
        }

        /* Async GET request. This method makes a HTTP GET request to the uri provided
         * and throws an exception if the response does not indicate a success. */
        public async Task<JObject> GetAsync(string uri)
        {
            try
            {
                //this will cause the server call to ingore invailid SSL Cert - should remove for production
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                HttpResponseMessage response = await client.GetAsync(uri);

                string content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    //var responseMessage = "Response status code does not indicate success: " + (int)response.StatusCode + " (" + response.StatusCode + " ). ";
                    return null;
                    //throw new HttpRequestException(responseMessage + Environment.NewLine + content);
                }
                return JObject.Parse(content);
            }
            catch (Exception e)
            {
                //throw e;
                return null;
            }
        }

        /* Async POST request. This method makes a HTTP POST request to the uri provided
         * and throws an exception if the response does not indicate a success. */
        public async Task PostAsync(string uri, string data)
        {
            HttpResponseMessage response = await client.PostAsync(uri, new StringContent(data, Encoding.UTF8, "application/json"));

            string content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                var responseMessage = "Response status code does not indicate success: " + (int)response.StatusCode + " (" + response.StatusCode + " ). ";
                throw new HttpRequestException(responseMessage + Environment.NewLine + content);
            }
        }

        /* Calling the GetAsync method and waiting for the results. */
        public JObject GetRequest(string url)
        {
            Task<JObject> t = this.GetAsync(url);
            t.Wait();
            return t.Result;
        }

        /* Calling the PostAsync method and waiting for the results. */
        public void PostRequest(string url, string data)
        {
            Task t = this.PostAsync(url, data);
            t.Wait();
        }

        /* Disposing the HttpClient. */
        public void Dispose()
        {
            client.Dispose();
        }
    }
}