using AutoMapper;
using BLC;
using BLC.Service;
using Entities;
using Entities.IActionResponseDTOs;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.LoginComponent
{
    public class LoginDAL : ILoginDAL
    {
        private readonly ServiceCallApi _callApi;
        private DataSet GlobalOperatorDS;
        private readonly SessionManager _sessionManager;
        private readonly IMapper _mapper;

        public LoginDAL(IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _callApi = new ServiceCallApi();
            GlobalOperatorDS = new DataSet();
            _sessionManager = new SessionManager(httpContextAccessor);
            _mapper = mapper;
        }

        public DataSet Authenticate(CredentialsDto credentials, string jsonPath)
        {
            this.GlobalOperatorDS = new DataSet();            

            var taskName = "DQWebAuthentication";
            var doOpParams = new DoOpMainParams() { Credentials = credentials };

            CommonFunctions.CallDoOperation(_callApi, taskName, doOpParams, jsonPath, ref GlobalOperatorDS);

           return GlobalOperatorDS;
        }        
    }
}
