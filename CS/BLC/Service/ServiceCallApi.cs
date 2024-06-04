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
        public void PostApiData(string url, ref DataSet operatorDS, List<DQParam> Params)
        {
            try
            {

                // Serialize List<DQParam> to a dictionary
                var paramJson = JsonConvert.SerializeObject(Params, Newtonsoft.Json.Formatting.Indented);

                var dataSetJson = new JObject();

                // Iterate over each DataTable in the DataSet
                foreach (DataTable table in operatorDS.Tables)
                {
                    // Create a JObject to store table information (columns and rows)
                    var tableJson = new JObject();

                    // Create a JArray to store column information for the current table
                    var columnInfoArray = new JArray();

                    // Iterate over each DataColumn in the DataTable
                    foreach (DataColumn column in table.Columns)
                    {
                        // Create a JArray containing column name and data type
                        var columnArray = new JArray();
                        columnArray.Add(column.ColumnName);
                        columnArray.Add(column.DataType.FullName); // Getting the full name of the data type

                        // Add the column array to the column information array
                        columnInfoArray.Add(columnArray);
                    }

                    // Add the column information array to the table JObject with key "columns"
                    tableJson["columns"] = columnInfoArray;

                    // Create a JArray to store rows for the current table
                    var rowsArray = new JArray();

                    // Iterate over each DataRow in the DataTable
                    foreach (DataRow row in table.Rows)
                    {
                        // Create a JArray to store the current row's data
                        var rowArray = new JArray();

                        // Iterate over each column in the DataRow
                        foreach (var item in row.ItemArray)
                        {
                            rowArray.Add(item.ToString()); // Add each column value to the row array
                        }

                        // Add the row array to the rows array
                        rowsArray.Add(rowArray);
                    }

                    // Add the rows array to the table JObject with key "rows"
                    tableJson["rows"] = rowsArray;

                    // Add the table JObject to the DataSet JObject with table name as key
                    dataSetJson[table.TableName] = tableJson;
                }

                // Serialize the JObject to JSON
                string operatorDSJson = dataSetJson.ToString();

                // Create a combined object to hold both serialized objects
                var bodyContent = new
                {
                    OperatorDS = operatorDSJson,
                    Params = paramJson
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
                    RebuildDataSet(response.Content, ref operatorDS);
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
