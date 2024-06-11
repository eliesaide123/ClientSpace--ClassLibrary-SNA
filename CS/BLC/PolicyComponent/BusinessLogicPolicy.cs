using BLC.Service;
using Entities;
using Entities.IActionResponseDTOs;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLC.PolicyComponent
{
    public class BusinessLogicPolicy : IBLCPolicy
    {
        private readonly ServiceCallApi _callApi;
        private DataSet GlobalOperatorDS;
        private readonly SessionManager _sessionManager;
        private readonly string jsonPath;
        public BusinessLogicPolicy(IHttpContextAccessor httpContextAccessor)
        {
            _callApi = new ServiceCallApi();
            GlobalOperatorDS = new DataSet();
            _sessionManager = new SessionManager(httpContextAccessor);
            if (ConfigurationManager.AppSettings != null && ConfigurationManager.AppSettings["jsonFilePath"] != null)
            {
                jsonPath = ConfigurationManager.AppSettings["jsonFilePath"];
            }
        }

        public GetPolicyDetailsResponse DQ_GetPIPolicyDetails(DoOpMainParams parameters)
        {
            this.GlobalOperatorDS = new DataSet();
            var taskName = "GetPIPolicyDetails";
            List<DQParam> Params = new List<DQParam>();

            CommonFunctions.ConstructTask(parameters, jsonPath, taskName, ref Params, ref GlobalOperatorDS);


            _callApi.PostApiData("/api/DQ_DoOperation", ref GlobalOperatorDS, Params);

            if (this.GlobalOperatorDS.Tables["NOTIFICATION"].Rows.Count > 0)
            {
                return new GetPolicyDetailsResponse()
                {
                    Errors = CommonFunctions.GetNotifications("NOTIFICATION", GlobalOperatorDS)
                };
            }

            var RES_Polcom = CommonFunctions.GetListFromData<PolcomPolicyDetailsDto>("Polcom", GlobalOperatorDS);
            var RES_Codes = CommonFunctions.GetListFromData<CodesPolicyDetailsDto>("Codes", GlobalOperatorDS);

            var sendResponse = new GetPolicyDetailsResponse() { Polcom = RES_Polcom, Codes = RES_Codes };

            return sendResponse;
        }

    }
}
