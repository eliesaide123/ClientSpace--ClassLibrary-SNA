using ClientSpaceApi.Classes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
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
                Console.WriteLine("Elie");
                // Extract the properties from the dynamic object
                string operatorDSJson = data.operatorDS.ToString();
                string paramString = data.paramString.ToString();

                // Deserialize the JSON strings to their respective types
                DataSet OperatorDS = JsonConvert.DeserializeObject<DataSet>(operatorDSJson);
                NameValueCollection Params = JsonConvert.DeserializeObject<Dictionary<string, string>>(paramString)
                                              .ToNameValueCollection();

                // Create an instance of the operation class and perform the operation
                DQOperator.DQOperator _operation = new DQOperator.DQOperator();
                _operation.DQ_DoOperation(ref OperatorDS, Params);

                return Ok("Data Operation performed Successfully.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
