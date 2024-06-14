using AutoMapper;
using BLC.Service;
using BLC;
using Entities;
using Entities.IActionResponseDTOs;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DAL.RolesComponent
{
    public class RolesDAL : IRolesDAL
    {
        private readonly ServiceCallApi _callApi;
        private DataSet GlobalOperatorDS;
        private readonly SessionManager _sessionManager;
        private readonly IMapper _mapper;

        public RolesDAL(IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _callApi = new ServiceCallApi();
            GlobalOperatorDS = new DataSet();
            _sessionManager = new SessionManager(httpContextAccessor);
            _mapper = mapper;
        }
        public CheckRolesResponse DQ_CheckRoles(CredentialsDto credentials, string jsonPath)
        {
            this.GlobalOperatorDS = new DataSet();
            var taskName = "CheckRoles";

            var doOpParams = new DoOpMainParams() { Credentials = credentials };

            CommonFunctions.CallDoOperation(_callApi, taskName, doOpParams, jsonPath, ref GlobalOperatorDS);

            return CommonFunctions.HandleNotifications(GlobalOperatorDS, "NOTIFICATION", () =>
            {
                var checkRolesResponse = new CheckRolesResponse();

                if (this.GlobalOperatorDS.Tables["UserIdent"] != null)
                {
                    if (this.GlobalOperatorDS.Tables["UserIdent"].Rows.Count == 1)
                    {
                        var oUserIdent = _mapper.Map<DataSet, cUserIdent>(GlobalOperatorDS);
                        //oUserIdent.RoleID = GlobalOperatorDS.Tables["Codes"].Rows[0]["Code"].ToString().Split("-")[0];

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
                return new CheckRolesResponse() { Error = true };
            });
            
        }

        public void SetRole(string sessionId, string roleId, string jsonPath)
        {
            this.GlobalOperatorDS = new DataSet();
            var taskName = "SetRoles";

            var doOpParams = new DoOpMainParams() { Credentials = new CredentialsDto() { SessionID = sessionId }, RoleID = roleId };
            CommonFunctions.CallDoOperation(_callApi, taskName, doOpParams, jsonPath, ref GlobalOperatorDS);
        }
    }
}
