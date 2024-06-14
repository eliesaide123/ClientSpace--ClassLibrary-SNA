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
        private readonly string jsonPath;

        public LoginDAL(IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _callApi = new ServiceCallApi();
            GlobalOperatorDS = new DataSet();
            _sessionManager = new SessionManager(httpContextAccessor);
            _mapper = mapper;
        }

        public LoginUserResponse Authenticate(CredentialsDto credentials, string jsonPath)
        {
            this.GlobalOperatorDS = new DataSet();            

            var taskName = "DQWebAuthentication";
            var doOpParams = new DoOpMainParams() { Credentials = credentials };

            CommonFunctions.CallDoOperation(_callApi, taskName, doOpParams, jsonPath, ref GlobalOperatorDS);

            if (CommonFunctions.HasNotifications(GlobalOperatorDS, "NOTIFICATION"))
            {
                return new LoginUserResponse()
                {
                    Errors = CommonFunctions.GetNotifications("NOTIFICATION", GlobalOperatorDS)
                };
            }

            if (credentials.IsAuthenticated == true)
            {
                _sessionManager.SetSessionValue("DQ_SessionID", credentials.SessionID);
                _sessionManager.SetSessionValue("DQ_IsLoggedIn", "true");

                if (credentials.IsFirstLogin == false)
                {
                    _sessionManager.SetSessionValue("DQCULTURE", "EN");
                    _sessionManager.SetSessionValue("DQWEBDIR", "ltr");
                }
            }

            return new LoginUserResponse()
            {
                Credentials = _mapper.Map<DataSet, CredentialsDto>(GlobalOperatorDS)
            };
        }
    }
}
