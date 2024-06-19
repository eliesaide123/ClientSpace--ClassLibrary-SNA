using BLC.Service;
using Entities;
using Entities.IActionResponseDTOs;
using Microsoft.AspNetCore.Http;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using RestSharp;
using System.Text;
using AutoMapper;
using System.Reflection;
using System.Diagnostics;
using DAL.LoginComponent;

namespace BLC.LoginComponent
{
    public class BusinessLogicLogin : IBLCLogin
    {
        private readonly ServiceCallApi _callApi;
        private DataSet GlobalOperatorDS;
        private readonly SessionManager _sessionManager;
        private readonly string jsonPath;
        private readonly IMapper _mapper;
        private readonly ILoginDAL _DAL;
        
        public BusinessLogicLogin(IHttpContextAccessor httpContextAccessor, IMapper mapper, ILoginDAL DAL)
        {
            _callApi = new ServiceCallApi();
            GlobalOperatorDS = new DataSet();
            _sessionManager = new SessionManager(httpContextAccessor);
            _mapper = mapper;
            _DAL = DAL;
            jsonPath = CommonFunctions.GetJSONFileLocation();
        }
        public async void GetSession(string sessionId)
        {
            this.GlobalOperatorDS = new DataSet();

            var taskName = "DQNewSession";
            var doOpParams = new DoOpMainParams() { Credentials = new CredentialsDto() { SessionID = sessionId } };

            CommonFunctions.CallDoOperation(_callApi, taskName, doOpParams, jsonPath, ref GlobalOperatorDS);

        }

        //test1
        public LoginUserResponse Authenticate(CredentialsDto credentials)
        {            
            DataSet GlobalOperatorDS = _DAL.Authenticate(credentials, jsonPath);
            return CommonFunctions.HandleNotifications(GlobalOperatorDS, "NOTIFICATION", () =>
            {
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
            });
        }
    }
}