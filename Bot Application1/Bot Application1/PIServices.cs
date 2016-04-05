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
        public static async Task<string> GetEnergyUsage(piluis message)
        {
            var client = new HttpClient(new HttpClientHandler() { UseDefaultCredentials = true });
            string authInfo = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(String.Format("{0}:{1}", "hackuser046", "ti6djbly2A$")));
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authInfo);

            try
            {
                //this will cause the server call to ingore invailid SSL Cert - should remove for production
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                string baseUri = @"https://proghackuc2016.osisoft.com/piwebapi/streams/A0EPUDmN4uvgkyiAt_SPv5vtgFz7Nu6ry5RGAvwANOjKA4AzXZ8VG9YKFQFQXLyhra6pASlVQSVRFUjAwMVxTQU4gRElFR08gQUlSUE9SVFxVVElMSVRJRVNcVEVSTUlOQUxTXEFJUlBPUlQgVEVSTUlOQUwgMVwzMzEyfDE1LU1JTiBFTEVDVFJJQ0lUWSBVU0FHRQ/";
                string time = "*";
                string element = "";
                foreach (var e in message.entities)
                {
                    switch (e.type)
                    {
                        case "DataSource":
                            element = e.entity;
                            break;
                        case "builtin.datetime.time":
                            time = e.resolution.time;
                            break;
                        case "builtin.datetime.date":
                            time = e.resolution.date;
                            break;
                        default:
                            break;
                    }
                }

                string uri = baseUri + string.Format("value?time={0}", time);


                HttpResponseMessage response = await client.GetAsync(uri);

                string content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    var responseMessage = "Response status code does not indicate success: " + (int)response.StatusCode + " (" + response.StatusCode + " ). ";
                    throw new HttpRequestException(responseMessage + Environment.NewLine + content);
                }

                var data = JObject.Parse(content);
                string results = element;
                results += " Energy usage: ";
                results += data["Value"].Value<string>() + " "+ data["UnitsAbbreviation"].Value<string>();
                results += " "+ data["Timestamp"].Value<string>();


                // "{\"Timestamp\":\"2016-04-05T04:08:05.5826686Z\",\"Value\":17.057534145410219,\"UnitsAbbreviation\":\"kWh\",\"Good\":true,\"Questionable\":false,\"Substituted\":false}"

                return results;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}