﻿using BLC.Service;
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
        public DataSet GlobalOperatorDS;
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
            GlobalOperatorDS.Tables.Clear();
            var taskName = "GetPIPolicyDetails";
            List<DQParam> Params = CommonFunctions.GetTaskParams(jsonPath, taskName);
            Params.Add(new DQParam() { Name = "SessionID", Value = parameters.Credentials.SessionID, Type = "Q" });
            Params.Add(new DQParam() { Name = "ROLEID", Value = parameters.RoleID, Type = "Q" });
            Params.Add(new DQParam() { Name = "PolSerNo", Value = parameters.PolSerNo.ToString(), Type = "O" });

            DataTable tbl_Polcom = CommonFunctions.GetTableColumns(jsonPath, taskName, "Polcom");
            CommonFunctions.DefaultRow(ref tbl_Polcom, ref GlobalOperatorDS);
            DataTable tbl_Codes = CommonFunctions.GetTableColumns(jsonPath, taskName, "Codes");
            CommonFunctions.DefaultRow(ref tbl_Codes, ref GlobalOperatorDS);

            _callApi.PostApiData("/api/DQ_DoOperation", ref GlobalOperatorDS, Params);

            var RES_Polcom = CommonFunctions.GetListFromData<PolcomPolicyDetailsDto>("Polcom", GlobalOperatorDS);
            var RES_Codes = CommonFunctions.GetListFromData<CodesPolicyDetailsDto>("Codes", GlobalOperatorDS);

            var sendResponse = new GetPolicyDetailsResponse() { Polcom = RES_Polcom, Codes = RES_Codes };

            return sendResponse;
        }

    }
}
