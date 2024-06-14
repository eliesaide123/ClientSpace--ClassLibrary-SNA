using Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
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
        public void PostApiData(string url, string taskName, string jsonPath, DoOpMainParams doOpParams, ref DataSet OperatorDS)
        {
            try
            {
                var bodyContent = new
                {
                    jsonPath = jsonPath,
                    taskName = taskName,
                    doOpParams = JsonConvert.SerializeObject(doOpParams)
                };

                // Serialize the combined object to JSON
                string requestBody = JsonConvert.SerializeObject(bodyContent, Newtonsoft.Json.Formatting.Indented);

                var request = new RestRequest(url, Method.Post);
                request.AddStringBody(requestBody, ContentType.Json); // Use AddStringBody to add the serialized JSON as the request body
                                                                      // Execute the async request synchronously
                var responseTask = _client.ExecuteAsync(request);
                responseTask.Wait();
                var response = responseTask.Result;

                if (response.IsSuccessful)
                {
                    RebuildDataSet(response.Content, ref OperatorDS);
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
            }
        }

        public void RebuildDataSet(string responseContent, ref DataSet operatorDS)
        {
            var content = responseContent;

            string cleanedJsonString = content.Trim('"').Replace("\\", "");

            Dictionary<string, object> jsonDictionary = JsonConvert.DeserializeObject<Dictionary<string, object>>(cleanedJsonString);

            operatorDS.Tables.Clear();

            foreach (var kvp in jsonDictionary)
            {
                string tableName = kvp.Key;
                JArray tableData = (JArray)kvp.Value;

                DataTable dataTable = new DataTable(tableName);

                if (tableData.Count > 0)
                {
                    var firstObject = tableData[0];
                    foreach (JProperty column in firstObject)
                    {
                        dataTable.Columns.Add(column.Name, typeof(string));
                    }
                }

                foreach (var row in tableData)
                {
                    DataRow dataRow = dataTable.NewRow();
                    foreach (JProperty column in ((JObject)row).Properties())
                    {
                        dataRow[column.Name] = column.Value.ToString();
                    }
                    dataTable.Rows.Add(dataRow);
                }
                operatorDS.Tables.Add(dataTable);
            }
        }
    }
}
