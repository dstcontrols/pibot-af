using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace Bot_Application1
{
    public class PIServices
    {
        private PIWebAPIClient piWebAPIClient;
        private string serverUrl;
        private string baseElement;

        public PIServices(string serverUrl, string baseElement, string userName, string password)
        {
            piWebAPIClient = new PIWebAPIClient(userName, password);
            this.serverUrl = serverUrl;
            this.baseElement = baseElement;

        }
public async Task<string> GetKPI(piluis message)
{
    try
    {
        //this will cause the server call to ingore invailid SSL Cert - should remove for production
        string time = "*";
        string element = "";
        string kpi = "";
        string kpiDescription = "";
        string asset = "";
        foreach (var e in message.entities)
        {
            switch (e.type)
            {
                case "Asset":
                    asset = e.entity;
                    break;
                case "KPI":
                    if (e.entity.Contains("energy"))
                    {
                        kpi = "Real Power";
                        kpiDescription = "Energy Usage";
                    }
                    break;
                case "builtin.datetime.date":
                case "builtin.datetime.time":
                    time = LUISParse.ParseDateTime(e);
                    break;
                default:
                    break;
            }
        }
        if (time != "")
        {
            var data = await GetKPIData(asset, kpi, time);
            string results = data["AssetName"].Value<string>();
            results += " " + kpiDescription + ": ";
            double x;
            var t = double.TryParse(data["Value"].Value<string>(), out x);
            results += x.ToString("F2", CultureInfo.InvariantCulture);
            results += " " + data["UnitsAbbreviation"].Value<string>();
            results += " " + data["Timestamp"].Value<string>();
            return results;

        }
        else
        {
            return "I did not understand your request...";
        }
    }
    catch (Exception e)
    {
        throw e;
    }
}

        private async Task<JObject> GetData(string time)
        {
            var client = new HttpClient(new HttpClientHandler() { UseDefaultCredentials = true });
            string authInfo = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(String.Format("{0}:{1}", "hackuser046", "ti6djbly2A$")));
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authInfo);

            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            string baseUri = @"https://proghackuc2016.osisoft.com/piwebapi/streams/A0EPUDmN4uvgkyiAt_SPv5vtgFz7Nu6ry5RGAvwANOjKA4AzXZ8VG9YKFQFQXLyhra6pASlVQSVRFUjAwMVxTQU4gRElFR08gQUlSUE9SVFxVVElMSVRJRVNcVEVSTUlOQUxTXEFJUlBPUlQgVEVSTUlOQUwgMVwzMzEyfDE1LU1JTiBFTEVDVFJJQ0lUWSBVU0FHRQ/";

            string uri = baseUri + string.Format("value?time={0}", time);
            HttpResponseMessage response = await client.GetAsync(uri);
            string content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                var responseMessage = "Response status code does not indicate success: " + (int)response.StatusCode + " (" + response.StatusCode + " ). ";
                return null;
                //                throw new HttpRequestException(responseMessage + Environment.NewLine + content);
            }
            var data = JObject.Parse(content);
            return data;
        }

        private async Task<JObject> GetElement(string asset, string time)
        {
            var client = new HttpClient(new HttpClientHandler() { UseDefaultCredentials = true });
            string authInfo = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(String.Format("{0}:{1}", "hackuser046", "ti6djbly2A$")));
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authInfo);

            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            string baseUri = @"https://proghackuc2016.osisoft.com/piwebapi/elements/E0PUDmN4uvgkyiAt_SPv5vtgET7Nu6ry5RGAvwANOjKA4ASlVQSVRFUjAwMVxTQU4gRElFR08gQUlSUE9SVFxVVElMSVRJRVNcVEVSTUlOQUxT";
            string uri = baseUri + string.Format("value?time={0}", time);
            HttpResponseMessage response = await client.GetAsync(uri);
            string content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                var responseMessage = "Response status code does not indicate success: " + (int)response.StatusCode + " (" + response.StatusCode + " ). ";
                return null;
                //                throw new HttpRequestException(responseMessage + Environment.NewLine + content);
            }
            var data = JObject.Parse(content);
            return data;
        }

        private async Task<JObject> GetKPIData(string asset, string kpi, string time)
        {
            //get the element
            string path = @"path=" + baseElement;// 
            string url = serverUrl + "/elements?" + path;
            JObject element = await piWebAPIClient.GetAsync(url);

            //get the meter elements links
            var results = element["Links"]["Elements"];
            var filter = results.ToString() + "?nameFilter=*" + asset + "*";
            var jobj = await piWebAPIClient.GetAsync(filter);
            var item = jobj.SelectToken("Items").First();
            var assetName = item["Name"].ToString();

            var temp = item.SelectToken("Links").SelectToken("Attributes").ToString();
            jobj = await piWebAPIClient.GetAsync(temp);
            var aItems = jobj.SelectToken("Items");
            var aLinks = aItems.Children().Where(m => m["Name"].ToString() == kpi).First();
            var data = await GetAttributeValue(aLinks["Links"]["Value"].ToString(), time);
            data.Add("AssetName", assetName);


            return data;
        }

        private async Task<JObject> GetAttributeValue(string attributeLink, string start)
        {

            //get interpolated data using attribute link
            attributeLink += string.Format("?time={0}", start);
            var jobj = await piWebAPIClient.GetAsync(attributeLink);

            return jobj;

        }

        private async Task<JObject> GetElementData(string id)
        {
            //get the element
            string path = @"path=" + baseElement + @"\" + id;
            string url = serverUrl + "/elements?" + path;
            JObject jobj = await piWebAPIClient.GetAsync(url);
            return jobj;
        }
    }
}