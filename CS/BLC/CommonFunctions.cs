using Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
        public static List<DQParam> GetTaskParams(string jsonFilePath, string taskName)
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

                                    paramsList.Add(param);
                                }
                            }
                        }
                        break; // Exit loop after finding the params for the specified task
                    }
                }

                return paramsList;
            }
        }
        public static DataTable GetTableColumns(string jsonFilePath, string taskName, string tableName)
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
                        return new DataTable(tableName);
                    }

                    // Search for the specified table name within the tables object
                    JProperty tableProperty = tables.Properties().FirstOrDefault(p => string.Equals(p.Name, tableName, StringComparison.OrdinalIgnoreCase));
                    if (tableProperty == null)
                    {
                        Console.WriteLine($"Table '{tableName}' not found in task '{taskName}'.");
                        return new DataTable(tableName);
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

            return table;
        }
        public static void DefaultRow(ref DataTable dt, ref DataSet GlobalOperatorDS)
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
            GlobalOperatorDS.Tables.Add(dt);
        }
    }
}
