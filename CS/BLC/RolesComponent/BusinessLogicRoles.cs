using AutoMapper;
using BLC.Service;
using DAL.RolesComponent;
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
        private DataSet GlobalOperatorDS;
        private readonly SessionManager _sessionManager;
        private readonly string jsonPath;
        private readonly IMapper _mapper;
        private readonly IRolesDAL _DAL;
        public BusinessLogicRoles(IHttpContextAccessor httpContextAccessor, IMapper mapper, IRolesDAL DAL)
        {
            _callApi = new ServiceCallApi();
            GlobalOperatorDS = new DataSet();
            _sessionManager = new SessionManager(httpContextAccessor);
            _mapper = mapper;
            _DAL = DAL;
            jsonPath = CommonFunctions.GetJSONFileLocation();
        }

        public CheckRolesResponse DQ_CheckRoles(CredentialsDto credentials)
        {
            return _DAL.DQ_CheckRoles(credentials, jsonPath);
        }

        public void SetRole(string sessionId, string roleId)
        {
            _DAL.SetRole(sessionId, roleId, jsonPath);
        }

    }
}