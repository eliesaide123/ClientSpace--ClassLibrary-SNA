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
        public DataSet DQ_CheckRoles(CredentialsDto credentials, string jsonPath)
        {
            this.GlobalOperatorDS = new DataSet();
            var taskName = "CheckRoles";

            var doOpParams = new DoOpMainParams() { Credentials = credentials };

            CommonFunctions.CallDoOperation(_callApi, taskName, doOpParams, jsonPath, ref GlobalOperatorDS);

            return GlobalOperatorDS;
            
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
