using AutoMapper;
using BLC.Service;
using Entities;
using Entities.IActionResponseDTOs;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BLC.RolesComponent
{
    public class BusinessLogicRoles : IBLCRoles
    {
        private readonly ServiceCallApi _callApi;
        public DataSet GlobalOperatorDS;
        private readonly SessionManager _sessionManager;
        private readonly string jsonPath;
        private readonly IMapper _mapper;
        public BusinessLogicRoles(IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _callApi = new ServiceCallApi();
            GlobalOperatorDS = new DataSet();
            _sessionManager = new SessionManager(httpContextAccessor);
            _mapper = mapper;
            if (ConfigurationManager.AppSettings != null && ConfigurationManager.AppSettings["jsonFilePath"] != null)
            {
                jsonPath = ConfigurationManager.AppSettings["jsonFilePath"];
            }
        }

        public CheckRolesResponse DQ_CheckRoles(CredentialsDto credentials)
        {
            GlobalOperatorDS.Tables.Clear();
            var taskName = "CheckRoles";
            List<DQParam> Params = new List<DQParam>();
           
            var doOpParams = new DoOpMainParams() { Credentials = credentials };
            CommonFunctions.ConstructTask(doOpParams, jsonPath, taskName, ref Params, ref GlobalOperatorDS);

            _callApi.PostApiData("/api/DQ_DoOperation", ref GlobalOperatorDS, Params);

            if (this.GlobalOperatorDS != null)
            {
                var checkRolesResponse = new CheckRolesResponse();

                if (this.GlobalOperatorDS.Tables["UserIdent"] != null)
                {
                    if (this.GlobalOperatorDS.Tables["UserIdent"].Rows.Count == 1)
                    {
                        var oUserIdent = _mapper.Map<DataTable, cUserIdent>(GlobalOperatorDS.Tables["UserIdent"]);
                        oUserIdent.RoleID = GlobalOperatorDS.Tables["Codes"].Rows[0]["Code"].ToString().Split("-")[0];

                        _sessionManager.SetSessionValue("DQUserIdent", JsonConvert.SerializeObject(oUserIdent));
                        checkRolesResponse.Error = false;
                        checkRolesResponse.SUCCESS = oUserIdent;
                        return checkRolesResponse;
                    }
                }
                else
                {
                    checkRolesResponse.Error = true;
                    checkRolesResponse.Errors = CommonFunctions.GetNotifications("NOTIFICATION", GlobalOperatorDS);
                    return checkRolesResponse;
                }
            }
            return new CheckRolesResponse() { Error = true };
        }

        public void SetRole(string sessionId, string roleId)
        {
            GlobalOperatorDS.Tables.Clear();
            var taskName = "SetRoles";
            //List<DQParam> Params = CommonFunctions.GetTaskParams(jsonPath, taskName);
            List<DQParam> Params = new List<DQParam>();
            //Params.Add(new DQParam() { Name = "SessionID", Value = sessionId, Type = "Q" });
            //Params.Add(new DQParam() { Name = "ROLEID", Value = roleId, Type="Q" });

            //DataTable tbl_VARIABLES = CommonFunctions.GetTableColumns(jsonPath, taskName, "VARIABLES");
            //CommonFunctions.DefaultRow(ref tbl_VARIABLES, ref GlobalOperatorDS);

            var doOpParams = new DoOpMainParams() { Credentials = new CredentialsDto() { SessionID = sessionId}, RoleID = roleId };

            CommonFunctions.ConstructTask(doOpParams, jsonPath, taskName, ref Params, ref GlobalOperatorDS);

            _callApi.PostApiData("/api/DQ_DoOperation", ref GlobalOperatorDS, Params);
        }

    }
}