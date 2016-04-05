using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace Bot_Application1
{
    public static class PIServices
    {
        public static async Task<string> GetEnergyUsage(string message)
        {
            var client = new HttpClient(new HttpClientHandler() { UseDefaultCredentials = true });
            string authInfo = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(String.Format("{0}:{1}", "hackuser046", "ti6djbly2A$")));
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authInfo);

            try
            {
                //this will cause the server call to ingore invailid SSL Cert - should remove for production
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                string uri = @"https://proghackuc2016.osisoft.com/piwebapi/streams/A0EPUDmN4uvgkyiAt_SPv5vtgFz7Nu6ry5RGAvwANOjKA4AzXZ8VG9YKFQFQXLyhra6pASlVQSVRFUjAwMVxTQU4gRElFR08gQUlSUE9SVFxVVElMSVRJRVNcVEVSTUlOQUxTXEFJUlBPUlQgVEVSTUlOQUwgMVwzMzEyfDE1LU1JTiBFTEVDVFJJQ0lUWSBVU0FHRQ/interpolated";
                HttpResponseMessage response = await client.GetAsync(uri);

                string content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    var responseMessage = "Response status code does not indicate success: " + (int)response.StatusCode + " (" + response.StatusCode + " ). ";
                    throw new HttpRequestException(responseMessage + Environment.NewLine + content);
                }

                var data = (JArray)JObject.Parse(content)["Items"];
                var result = new JArray();
                foreach (var item in data)
                {
                    if (item["Good"].Value<bool>())
                    {
                        var dataPair = new JObject();
                        dataPair.Add("Timestamp", item["Timestamp"].Value<string>());
                        dataPair.Add("Value", item["Value"].Value<double>());
                        result.Add(dataPair);
                    }
                }

                return "test";
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}