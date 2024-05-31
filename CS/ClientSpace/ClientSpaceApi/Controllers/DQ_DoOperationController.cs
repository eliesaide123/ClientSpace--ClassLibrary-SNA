using ClientSpaceApi.Classes;
using DQOperator;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using System.Web.Http;

namespace ClientSpaceApi.Controllers
{
    public class DQ_DoOperationController : ApiController
    {
        [HttpPost]
        public async Task<IHttpActionResult> DQ_Operation([FromBody] dynamic data)
        {
            try
            {
                // Extract the properties from the dynamic object
                var operatorDSJson = data.operatorDS.ToString();
                var paramString = data.paramString.ToString();

                var dataSetJson = JObject.Parse(operatorDSJson);

                var OperatorDS = new DataSet();

                foreach (var property in dataSetJson.Properties())
                {
                    var tableName = property.Name;
                    var tableJson = (JObject)property.Value;

                    var dataTable = new DataTable(tableName);

                    var columnsArray = (JArray)tableJson["columns"];

                    foreach (JArray columnInfoArray in columnsArray)
                    {
                        var columnName = (string)columnInfoArray[0];
                        var columnTypeFullName = (string)columnInfoArray[1];

                        var columnType = Type.GetType(columnTypeFullName);
                        dataTable.Columns.Add(new DataColumn(columnName, columnType));
                    }

                    var rowsArray = (JArray)tableJson["rows"];

                    foreach (JArray rowDataArray in rowsArray)
                    {
                        var row = dataTable.NewRow();

                        for (int i = 0; i < rowDataArray.Count; i++)
                        {
                            var column = dataTable.Columns[i];
                            var value = rowDataArray[i];

                            // Convert the value to the appropriate data type based on the column's DataType
                            if (value.Type == JTokenType.Null)
                            {
                                // Handle null values
                                row[i] = DBNull.Value;
                            }
                            else if (column.DataType == typeof(int) || column.DataType == typeof(Int32))
                            {
                                row[i] = value.Value<int>();
                            }
                            else if (column.DataType == typeof(long) || column.DataType == typeof(Int64))
                            {
                                row[i] = value.Value<long>();
                            }
                            else if (column.DataType == typeof(double))
                            {
                                row[i] = value.Value<double>();
                            }
                            else if (column.DataType == typeof(float))
                            {
                                row[i] = value.Value<float>();
                            }
                            else if (column.DataType == typeof(DateTime))
                            {
                                row[i] = value.Value<DateTime>();
                            }
                            else
                            {
                                row[i] = value.ToString();
                            }
                        }

                        // Add the row to the DataTable
                        dataTable.Rows.Add(row);
                    }

                    // Add the DataTable to the DataSet
                    OperatorDS.Tables.Add(dataTable);
                }

                List<DQParam> Params = JsonConvert.DeserializeObject<List<DQParam>>(paramString);

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
    }
}
