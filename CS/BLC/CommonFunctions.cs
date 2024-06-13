using BLC.Service;
using Entities;
using Entities.IActionResponseDTOs;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BLC
{
    public static class CommonFunctions
    { 
        public static Dictionary<string, object> TransformDataToDictionary(string tblName, DataSet GlobalOperatorDS)
        {
            DataTable dataTable = GlobalOperatorDS.Tables[tblName];
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();

            foreach (DataRow row in dataTable.Rows)
            {
                Dictionary<string, object> rowDict = new Dictionary<string, object>();
                foreach (DataColumn column in dataTable.Columns)
                {
                    rowDict[column.ColumnName] = row[column];
                }
                rows.Add(rowDict);
            }

            var result = new Dictionary<string, object>
            {
                { dataTable.TableName, rows }
            };

            return result;
        }
        public static string TransformDataToJson(string tblName, DataSet GlobalOperatorDS)
        {
            var result = TransformDataToDictionary(tblName, GlobalOperatorDS);
            return JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.Indented);
        }
        public static List<ErrorDescriptor> GetNotifications(string tblName, DataSet GlobalOperatorDS)
        {
            var result = new List<ErrorDescriptor>();

            foreach (DataRow row in GlobalOperatorDS.Tables[tblName].Rows)
            {
                string code = row["NOTIFICATION_CODE"]?.ToString();
                string desc = row["NOTIFICATION_DESC"]?.ToString();
                result.Add(new ErrorDescriptor() { Code = code, Description = desc });
            }

            return result;
        }
        public static List<T> GetListFromData<T>(string tblName, DataSet GlobalOperatorDS)
        {
            var result = new List<T>();

            foreach (DataRow row in GlobalOperatorDS.Tables[tblName].Rows)
            {
                var obj = Activator.CreateInstance<T>();

                foreach (DataColumn column in GlobalOperatorDS.Tables[tblName].Columns)
                {
                    var columnName = column.ColumnName;
                    var propertyName = columnName.Contains("-") ? columnName.Replace("-", "_") : columnName;
                    var value = row[columnName];

                    var propertyType = typeof(T).GetProperty(propertyName)?.PropertyType;
                    if (propertyType != null)
                    {
                        switch (propertyType)
                        {
                            case Type t when t == typeof(string):
                                value = value?.ToString() ?? string.Empty; // Handle null values
                                break;
                            case Type t when t == typeof(int) || t == typeof(Int32):
                                if (value is string)
                                {
                                    Int32.TryParse((string)value, out Int32 intValue);
                                    value = intValue;
                                }
                                else
                                {
                                    value = Convert.ChangeType(value, propertyType);
                                }
                                break;
                            case Type t when t == typeof(bool):
                                if (value.ToString() == "")
                                {
                                    value = false;
                                }
                                else
                                {
                                    value = Convert.ChangeType(value, propertyType);
                                }

                                break;
                            case Type t when t == typeof(double):
                                if (value is string)
                                {
                                    double doubleValue;
                                    if (double.TryParse((string)value, out doubleValue))
                                    {
                                        value = doubleValue;
                                    }
                                }
                                else
                                {
                                    value = Convert.ChangeType(value, propertyType);
                                }
                                break;
                            default:
                                value = Convert.ChangeType(value, propertyType);
                                break;
                        }

                        typeof(T).GetProperty(propertyName).SetValue(obj, value);
                    }
                }

                result.Add(obj);
            }

            return result;
        }
        public static List<DQParam> GetTaskParams(string jsonFilePath, string taskName,DoOpMainParams? doOpParams)
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
                                    if(param.Value.ToLower().Contains("fill") && (param.Type == "Q" || param.Type == "O"))
                                    {
                                        param.Value = doOpParams.GetType().GetProperty(param.Value.Split("_")[1]).GetValue(doOpParams).ToString();
                                    }
                                    paramsList.Add(param);
                                }
                            }
                        }
                        break; // Exit loop after finding the params for the specified task
                    }
                }
                if (doOpParams != null) {
                    paramsList.Add(new DQParam() { Name = "SessionID", Value = doOpParams.Credentials.SessionID, Type = "Q" });
                }
                return paramsList;
            }
        }
        public static void GetTableColumns(string jsonFilePath, string taskName, string tableName, ref DataSet GlobalOperatorDS)
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

            foreach(var item in tableColumns)
            {
                table.Columns.Add(item["ColumnName"], Type.GetType(item["ColumnDataType"]));
            }

            GlobalOperatorDS.Tables.Add(table);
        }
        public static DataSet GetTables(string jsonFilePath, string taskName)
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
        public static void DefaultRow(DataTable dt)
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
        public static void ConstructTask(DoOpMainParams doOpParams, string jsonPath, string taskName, ref List<DQParam> Params, ref DataSet GlobalOperatorDS)
        {
            Params = GetTaskParams(jsonPath, taskName, doOpParams);

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
        public static Dictionary<string, string> GetOutputParams(ref DataSet GlobalOperatorDS)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();

            foreach (DataRow row in GlobalOperatorDS.Tables["PARAMETERS"].Rows)
            {
                if (row["PARAM_TYPE"].ToString() == "O")
                {
                    parameters.Add(row["PARAM_NAME"].ToString(), row["PARAM_VALUE"].ToString());
                }
            }

            return parameters;
        }

        public static void CallDoOperation(  ServiceCallApi _callApi, string taskName, DoOpMainParams doOpParams, string JSONpath, ref  List<DQParam> Params , ref DataSet GlobalOperatorDS) {
          
            ConstructTask(doOpParams, JSONpath, taskName, ref Params, ref GlobalOperatorDS);

            _callApi.PostApiData("/api/DQ_DoOperation", ref GlobalOperatorDS, Params);
        }

        public static bool HasNotifications(DataSet dataSet, string tableName)
        {
           
            if (dataSet.Tables.Contains(tableName) && dataSet.Tables[tableName].Rows.Count > 0)
            {
               
                return true;
            }

            return false;
        }

        public static string GetJSONFileLocation()
        {
            string assemblyLocation = Assembly.GetExecutingAssembly().Location;
            string assemblyDirectory = Path.GetDirectoryName(assemblyLocation);
            string relativePath = @"DoOperationTasks.json";

            return Path.Combine(assemblyDirectory, relativePath);
        }

    }
}
