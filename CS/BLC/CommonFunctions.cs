using Entities;
using Newtonsoft.Json;
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
                result.Add(new ErrorDescriptor() { Code = code, Description = desc});
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
                                if(value.ToString() == "")
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

    }
}
