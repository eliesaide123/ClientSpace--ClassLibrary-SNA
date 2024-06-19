using BLC.Service;
using DAL.PolicyComponent;
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
        private readonly IPolicyDAL _DAL;
        public BusinessLogicPolicy(IHttpContextAccessor httpContextAccessor, IPolicyDAL DAL)
        {
            _callApi = new ServiceCallApi();
            GlobalOperatorDS = new DataSet();
            _sessionManager = new SessionManager(httpContextAccessor);
            _DAL = DAL;
            jsonPath = CommonFunctions.GetJSONFileLocation();
        }

        public GetPolicyDetailsResponse DQ_GetPIPolicyDetails(DoOpMainParams parameters)
        {
            DataSet GlobalOperatorDS = _DAL.DQ_GetPIPolicyDetails(parameters, jsonPath);

            return CommonFunctions.HandleNotifications<GetPolicyDetailsResponse>(GlobalOperatorDS, "NOTIFICATION", () =>
            {
                var RES_Polcom = CommonFunctions.GetListFromData<PolcomPolicyDetailsDto>("Polcom", GlobalOperatorDS);
                var RES_Codes = CommonFunctions.GetListFromData<CodesPolicyDetailsDto>("Codes", GlobalOperatorDS);

                var sendResponse = new GetPolicyDetailsResponse() { Polcom = RES_Polcom, Codes = RES_Codes };

                return sendResponse;
            });
        }

    }
}
