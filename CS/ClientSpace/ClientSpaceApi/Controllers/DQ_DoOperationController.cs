using ClientSpaceApi.Classes;
using ClientSpaceApi.Models;
using DQOperator;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace ClientSpaceApi.Controllers
{
    public class DQ_DoOperationController : ApiController
    {
        [HttpPost]
        public async Task<IHttpActionResult> DQ_Operation([FromBody] DQOperationParams data)
        {
            try
            {
                var taskName = data.taskName;
                var jsonPath = data.jsonPath;
                var doOpParams = JsonConvert.DeserializeObject<DoOpMainParams>(data.doOpParams);

                List<DQParam> Params = GetTaskParams(jsonPath, taskName, doOpParams);
                DataSet OperatorDS = new DataSet();
                ConstructDataSet(doOpParams, jsonPath, taskName, ref OperatorDS);

                // Create an instance of the operation class and perform the operation
                DQOperator.DQOperator _operation = new DQOperator.DQOperator();
                _operation.DQ_DoOperation(ref OperatorDS, Params);
                
                return Ok(JsonConvert.SerializeObject(OperatorDS, Newtonsoft.Json.Formatting.None));
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        private List<DQParam> GetTaskParams(string jsonFilePath, string taskName, DoOpMainParams doOpParams)
        {
            using (StreamReader sr = new StreamReader(jsonFilePath))
            using (JsonTextReader reader = new JsonTextReader(sr))
            {
                List<DQParam> paramsList = new List<DQParam>();
                string currentTaskName = null;

                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.PropertyName && string.Equals((string)reader.Value, taskName, StringComparison.OrdinalIgnoreCase))
                    {
                        reader.Read(); // Move to the task object
                        currentTaskName = taskName;
                    }

                    if (currentTaskName == taskName && reader.TokenType == JsonToken.PropertyName && string.Equals((string)reader.Value, "PARAMS", StringComparison.OrdinalIgnoreCase))
                    {
                        reader.Read(); // Move to the start of the array
                        if (reader.TokenType == JsonToken.StartArray)
                        {
                            while (reader.Read() && reader.TokenType != JsonToken.EndArray)
                            {
                                if (reader.TokenType == JsonToken.StartObject)
                                {
                                    DQParam param = new DQParam();

                                    while (reader.Read() && reader.TokenType != JsonToken.EndObject)
                                    {
                                        if (reader.TokenType == JsonToken.PropertyName)
                                        {
                                            string paramName = (string)reader.Value;
                                            reader.Read(); // Move to value

                                            if (string.Equals(paramName, "Name", StringComparison.OrdinalIgnoreCase))
                                            {
                                                param.Name = (string)reader.Value;
                                            }
                                            else if (string.Equals(paramName, "Value", StringComparison.OrdinalIgnoreCase))
                                            {
                                                param.Value = (string)reader.Value;
                                            }
                                            else if (string.Equals(paramName, "Type", StringComparison.OrdinalIgnoreCase))
                                            {
                                                param.Type = (string)reader.Value;
                                            }
                                        }
                                    }
                                    if (param.Value.ToLower().Contains("fill") && (param.Type == "Q" || param.Type == "O"))
                                    {
                                        param.Value = doOpParams.GetType().GetProperty(param.Value.Split('_')[1]).GetValue(doOpParams).ToString();
                                    }
                                    paramsList.Add(param);
                                }
                            }
                        }
                        break; // Exit loop after finding the params for the specified task
                    }
                }
                if (doOpParams != null)
                {
                    var Type = "";
                    if(taskName != "DQNewSession")
                    {
                        Type = "Q";
                    }
                    paramsList.Add(new DQParam() { Name = "SessionID", Value = doOpParams.Credentials.SessionID, Type = Type });
                }
                return paramsList;
            }
        }
        private void GetTableColumns(string jsonFilePath, string taskName, string tableName, ref DataSet GlobalOperatorDS)
        {
            List<Dictionary<string, string>> tableColumns = new List<Dictionary<string, string>>();

            try
            {
                using (StreamReader file = File.OpenText(jsonFilePath))
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    JObject tables = null;
                    string currentTaskName = null;

                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonToken.PropertyName && string.Equals((string)reader.Value, taskName, StringComparison.OrdinalIgnoreCase))
                        {
                            currentTaskName = taskName;
                        }
                        else if (currentTaskName != null && reader.TokenType == JsonToken.StartObject && string.Equals(reader.Path, $"TASKS.{taskName}.Tables", StringComparison.OrdinalIgnoreCase))
                        {
                            tables = JObject.Load(reader);
                            break;
                        }
                    }

                    if (tables == null)
                    {
                        Console.WriteLine($"Tables not found for task '{taskName}'.");
                        return;
                    }

                    // Search for the specified table name within the tables object
                    JProperty tableProperty = tables.Properties().FirstOrDefault(p => string.Equals(p.Name, tableName, StringComparison.OrdinalIgnoreCase));
                    if (tableProperty == null)
                    {
                        Console.WriteLine($"Table '{tableName}' not found in task '{taskName}'.");
                        return;
                    }

                    // Convert the table to a list of dictionaries
                    JToken tableToken = tableProperty.Value;
                    tableColumns = tableToken.ToObject<List<Dictionary<string, string>>>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while processing the JSON file: {ex.Message}");
            }

            DataTable table = new DataTable(tableName);

            foreach (var item in tableColumns)
            {
                table.Columns.Add(item["ColumnName"], Type.GetType(item["ColumnDataType"]));
            }

            GlobalOperatorDS.Tables.Add(table);
        }
        private DataSet GetTables(string jsonFilePath, string taskName)
        {
            DataSet dataSet = new DataSet(taskName);

            try
            {
                using (StreamReader file = File.OpenText(jsonFilePath))
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    JObject jsonObject = JObject.Load(reader);
                    JToken taskToken = jsonObject.SelectToken($"$.TASKS.{taskName}");

                    if (taskToken == null)
                    {
                        Console.WriteLine($"Task '{taskName}' not found in JSON file.");
                        return dataSet;
                    }

                    JToken tablesToken = taskToken["Tables"];
                    if (tablesToken == null || tablesToken.Type != JTokenType.Object)
                    {
                        Console.WriteLine($"No tables found for task '{taskName}'.");
                        return dataSet;
                    }

                    foreach (JProperty tableProperty in tablesToken.Children<JProperty>())
                    {
                        string tableName = tableProperty.Name;
                        JArray columnsArray = tableProperty.Value as JArray;

                        Console.WriteLine($"Processing table: {tableName}");

                        if (string.IsNullOrEmpty(tableName) || columnsArray == null)
                        {
                            Console.WriteLine("Invalid table or columns data.");
                            continue;
                        }

                        GetTableColumns(jsonFilePath, taskName, tableName, ref dataSet);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while processing the JSON file: {ex.Message}");
            }

            return dataSet;
        }
        private void DefaultRow(DataTable dt)
        {
            DataRow defaultRow = dt.NewRow();
            foreach (DataColumn column in dt.Columns)
            {
                switch (column.DataType)
                {
                    case Type t when t == typeof(String):
                        defaultRow[column.ColumnName] = String.Empty;
                        break;
                    case Type t when t == typeof(int) || t == typeof(Int32):
                        defaultRow[column.ColumnName] = 0; // Default value for int
                        break;
                    case Type t when t == typeof(Boolean):
                        defaultRow[column.ColumnName] = false; // Default value for bool
                        break;
                    case Type t when t == typeof(Decimal):
                        defaultRow[column.ColumnName] = new Decimal(0);
                        break;
                    case Type t when t == typeof(DateTime):
                        defaultRow[column.ColumnName] = new DateTime();
                        break;
                }
            }
            dt.Rows.Add(defaultRow);
        }
        private void ConstructDataSet(DoOpMainParams doOpParams, string jsonPath, string taskName, ref DataSet GlobalOperatorDS)
        {

            GlobalOperatorDS = GetTables(jsonPath, taskName);

            foreach (DataTable table in GlobalOperatorDS.Tables)
            {
                var name = table.TableName;

                switch (name)
                {
                    case "Credentials":
                        DataRow row = table.NewRow();
                        row["User_ID"] = doOpParams.Credentials.Username;
                        row["Password"] = doOpParams.Credentials.Password;
                        row["ClientType"] = doOpParams.Credentials.ClientType;
                        row["SessionID"] = doOpParams.Credentials.SessionID;
                        row["IsAuthenticated"] = doOpParams.Credentials.IsAuthenticated;
                        row["IsFirstLogin"] = doOpParams.Credentials.IsFirstLogin;

                        table.Rows.Add(row);
                        break;
                    default:
                        DefaultRow(table);
                        break;
                }
            }

        }
    }
}
