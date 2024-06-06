﻿using BLC.Service;
using Entities;
using Entities.IActionResponseDTOs;
using Microsoft.AspNetCore.Http;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using RestSharp;
using System.Text;

namespace BLC.LoginComponent
{
    public class BusinessLogicLogin : IBLCLogin
    {
        private readonly ServiceCallApi _callApi;
        public DataSet GlobalOperatorDS;
        private readonly SessionManager _sessionManager;
        private readonly string jsonPath;
        public BusinessLogicLogin(IHttpContextAccessor httpContextAccessor)
        {
            _callApi = new ServiceCallApi();
            GlobalOperatorDS = new DataSet();
            _sessionManager = new SessionManager(httpContextAccessor);
            if (ConfigurationManager.AppSettings != null && ConfigurationManager.AppSettings["jsonFilePath"] != null)
            {
                jsonPath = ConfigurationManager.AppSettings["jsonFilePath"];
            }
        }
        public async void GetSession(string sessionId)
        {
            GlobalOperatorDS.Tables.Clear();
            List<DQParam> Params = new List<DQParam>();
            Params.Add(new DQParam() { Name = "TASK_NAME", Value = "DQNewSession", Type = "" });
            Params.Add(new DQParam() { Name = "SessionID", Value = sessionId, Type = "" });
            _callApi.PostApiData("/api/DQ_DoOperation", ref GlobalOperatorDS, Params);

        }
        public CredentialsDto Authenticate(CredentialsDto credentials)
        {
            GlobalOperatorDS.Tables.Clear();
            var taskName = "DQWebAuthentication";
            List<DQParam> Params = CommonFunctions.GetTaskParams(jsonPath, taskName);
            Params.Add(new DQParam() { Name = "SessionID", Value = credentials.SessionID, Type = "Q" });

            // Add your logic to create and populate the DataSet
            DataTable tbl_Credentials = CommonFunctions.GetTableColumns(jsonPath, taskName, "Credentials");

            DataRow row = tbl_Credentials.NewRow();
            row["User_ID"] = credentials.Username;
            row["Password"] = credentials.Password;
            row["ClientType"] = credentials.ClientType;
            row["SessionID"] = credentials.SessionID;
            row["IsAuthenticated"] = credentials.IsAuthenticated;
            row["IsFirstLogin"] = credentials.IsFirstLogin;

            tbl_Credentials.Rows.Add(row);
            GlobalOperatorDS.Tables.Add(tbl_Credentials);

            _callApi.PostApiData("/api/DQ_DoOperation", ref GlobalOperatorDS, Params);

            //if (DQOperator.DQOperator.DQ_GlobalDS.Tables["Credentials"] != null)
            //{
            //    if (this.DQ_GlobalDS.Tables["Credentials"].Rows.Count == 1)
            //    {
            //        credentials.IsAuthenticated = Convert.ToBoolean(this.DQ_GlobalDS.Tables["Credentials"].Rows[0]["IsAuthenticated"].ToString());
            //        credentials.IsFirstLogin = Convert.ToBoolean(this.DQ_GlobalDS.Tables["Credentials"].Rows[0]["IsFirstLogin"].ToString());
            //    }
            //}


            return new CredentialsDto()
            {
                Username = GlobalOperatorDS.Tables["Credentials"].Rows[0]["User_ID"].ToString(),
                Password = GlobalOperatorDS.Tables["Credentials"].Rows[0]["Password"].ToString(),
                ClientType = GlobalOperatorDS.Tables["Credentials"].Rows[0]["ClientType"].ToString(),
                SessionID = GlobalOperatorDS.Tables["Credentials"].Rows[0]["SessionID"].ToString(),
                IsAuthenticated = Convert.ToBoolean(GlobalOperatorDS.Tables["Credentials"].Rows[0]["IsAuthenticated"].ToString()),
                IsFirstLogin = Convert.ToBoolean(GlobalOperatorDS.Tables["Credentials"].Rows[0]["IsFirstLogin"].ToString())
            };
        }

        public LoginUserResponse IsFirstLogin(CredentialsDto credentials)
        {
            NameValueCollection oServerResponse = new NameValueCollection();

            if (credentials.IsAuthenticated == true)
            {
                _sessionManager.SetSessionValue("DQ_SessionID", credentials.SessionID);
                _sessionManager.SetSessionValue("DQ_IsLoggedIn", "true");

                if (credentials.IsFirstLogin == true)
                {
                    //if statement to check company name
                    oServerResponse.Add("ENTRY|STATUS", "2");
                }
                else
                {
                    _sessionManager.SetSessionValue("DQCULTURE", "EN");
                    _sessionManager.SetSessionValue("DQWEBDIR", "ltr");

                    oServerResponse.Add("ENTRY|STATUS", "1");
                }

            }
            oServerResponse.Add("TECHERROR|TECHERROR", DQ_GetTechnicalErrorMessage());
            oServerResponse.Add("NOTIFICATION|NOTIFICATION", DQ_GetBusinessNotificationMessage());
            oServerResponse.Add("ERROR|ERROR", DQ_GetBusinessErrorMessage());
            oServerResponse.Add("FLASH|FLASH", DQ_GetBusinessFlashMessage());

            var errors = oServerResponse.AllKeys.ToDictionary(key => key, key => oServerResponse[key]);
            var user = new CredentialsDto()
            {
                Username = credentials.Username,
                Password = credentials.Password,
                ClientType = credentials.ClientType,
                IsAuthenticated = credentials.IsAuthenticated,
                IsFirstLogin = credentials.IsFirstLogin,
                SessionID = credentials.SessionID,
            };
            return new LoginUserResponse()
            {
                Credentials = user,
                Errors = errors
            };
        }

        public string DQ_GetBusinessErrorMessage()
        {
            string empty = string.Empty;
            return DQ_GetNotificationMessages(0);
        }

        public string DQ_GetBusinessFlashMessage()
        {
            string empty = string.Empty;
            return DQ_GetNotificationMessages(-2);
        }

        public string DQ_GetTechnicalErrorMessage()
        {
            string empty = string.Empty;
            return DQ_GetNotificationMessages(-1);
        }

        public string DQ_GetBusinessNotificationMessage()
        {
            string empty = string.Empty;
            return DQ_GetNotificationMessages(1);
        }

        public string DQ_GetNotificationMessages(int i__Type)
        {
            StringBuilder stringBuilder = new StringBuilder();
            string result = string.Empty;
            DataTable dataTable = null;
            string empty = string.Empty;
            if (GlobalOperatorDS != null && GlobalOperatorDS.Tables.Contains("NOTIFICATION"))
            {
                dataTable = GlobalOperatorDS.Tables["NOTIFICATION"];
                foreach (DataRow row in dataTable.Rows)
                {
                    if (row["NOTIFICATION_DESC"] != null && row["NOTIFICATION_TYPE"] != null && Convert.ToInt32(row["NOTIFICATION_TYPE"].ToString()) == i__Type)
                    {
                        empty = row["NOTIFICATION_DESC"].ToString() + "<br>";
                        empty = empty.Replace("\r", "");
                        empty = empty.Replace("\n", "");
                        stringBuilder.AppendLine(empty);
                    }
                }
            }

            if (stringBuilder != null)
            {
                result = stringBuilder.ToString();
            }

            return result;
        }
    }
}


//this.DQ_AddParam("TASK_NAME", "DQWebAuthentication");
//this.DQ_AddParam("CONVERTER_NAME", "Conv_DQWebAuthentication");