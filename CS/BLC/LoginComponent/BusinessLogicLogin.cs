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

namespace BLC.LoginComponent
{
    public class BusinessLogicLogin : IBLCLogin
    {
        private readonly ServiceCallApi _callApi;
        private DataSet GlobalOperatorDS;
        private readonly SessionManager _sessionManager;
        private readonly string jsonPath;
        private readonly IMapper _mapper;
        public BusinessLogicLogin(IHttpContextAccessor httpContextAccessor, IMapper mapper)
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
        public async void GetSession(string sessionId)
        {
            this.GlobalOperatorDS = new DataSet();
            List<DQParam> Params = new List<DQParam>();
            Params.Add(new DQParam() { Name = "TASK_NAME", Value = "DQNewSession", Type = "" });
            Params.Add(new DQParam() { Name = "SessionID", Value = sessionId, Type = "" });
            _callApi.PostApiData("/api/DQ_DoOperation", ref GlobalOperatorDS, Params);

        }
        public LoginUserResponse Authenticate(CredentialsDto credentials)
        {
            this.GlobalOperatorDS = new DataSet();
            var taskName = "DQWebAuthentication";
            
            List<DQParam> Params = new List<DQParam>();
           
            var doOpParams = new DoOpMainParams() { Credentials = credentials };
            CommonFunctions.ConstructTask(doOpParams, jsonPath, taskName, ref Params, ref GlobalOperatorDS);

            _callApi.PostApiData("/api/DQ_DoOperation", ref GlobalOperatorDS, Params);

            if (GlobalOperatorDS.Tables["NOTIFICATION"].Rows.Count > 0)
            {
                return new LoginUserResponse()
                {
                    Errors = CommonFunctions.GetNotifications("NOTIFICATION", GlobalOperatorDS),
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