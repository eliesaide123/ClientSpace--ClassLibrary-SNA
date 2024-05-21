using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace BLC.Service
{
    public class ServiceCallApi
    {
        private readonly HttpClient _httpClient;
        private readonly RestClient _client;

        public ServiceCallApi()
        {
            _httpClient = new HttpClient();
            _client = new RestClient("http://localhost:49366");
        }
        public async Task<bool> PostApiDataAsync(string url, DataSet operatorDS, NameValueCollection Params)
        {
            try
            {
                // Serialize NameValueCollection to a dictionary
                var paramDict = Params.Cast<string>().ToDictionary(k => k, k => Params[k]);

                // Serialize the DataSet to JSON
                string operatorDSJson = JsonConvert.SerializeObject(operatorDS, Newtonsoft.Json.Formatting.Indented);

                // Create a combined object to hold both serialized objects
                var bodyContent = new
                {
                    operatorDS = operatorDSJson,
                    paramString = JsonConvert.SerializeObject(paramDict) // Ensure paramDict is serialized to JSON string
                };

                // Serialize the combined object to JSON
                string requestBody = JsonConvert.SerializeObject(bodyContent, Newtonsoft.Json.Formatting.Indented);

                var request = new RestRequest(url, Method.Post);
                request.AddStringBody(requestBody, ContentType.Json); // Use AddStringBody to add the serialized JSON as the request body

                var response = await _client.ExecuteAsync(request);
                var responseBody = response.Content;
                return responseBody != string.Empty ? true : false ;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
                return false;
            }
        }

    }
}
