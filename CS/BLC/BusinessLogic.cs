using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using System.Data;
using System.Collections.Specialized;
using BLC.Service;
using RestSharp;
using System.Net;
using Newtonsoft.Json;

namespace BLC
{
    public class BusinessLogic : IBLC
    {
        private readonly ServiceCallApi _callApi;
        public BusinessLogic() {
            _callApi = new ServiceCallApi();
        }
        public async void GetSession(string sessionId)
        {
            DataSet OperatorDS = new DataSet();
            List<DQParam> Params = new List<DQParam>();
            Params.Add(new DQParam() { Name = "TASK_NAME", Value = "DQNewSession", Type = "" });
            Params.Add(new DQParam() { Name = "SessionID", Value = sessionId, Type= ""});
            _callApi.PostApiData("/api/DQ_DoOperation", ref OperatorDS, Params);

        }
        public CredentialsDto Authenticate(CredentialsDto credentials)
        {
            DataSet OperatorDS = new DataSet();
            List<DQParam> Params = new List<DQParam>();
            Params.Add(new DQParam() { Name = "TASK_NAME", Value = "DQWebAuthentication", Type = "" });
            Params.Add(new DQParam() { Name = "CONVERTER_NAME", Value= "Conv_DQWebAuthentication", Type="" });
            Params.Add(new DQParam() { Name = "SessionID", Value= credentials.SessionID, Type="Q" });

            // Add your logic to create and populate the DataSet
            DataTable credentialsTable = new DataTable("Credentials");
            credentialsTable.Columns.Add("User_ID", typeof(string));
            credentialsTable.Columns.Add("Password", typeof(string));
            credentialsTable.Columns.Add("ClientType", typeof(string));
            credentialsTable.Columns.Add("SessionID", typeof(string));
            credentialsTable.Columns.Add("IsAuthenticated", typeof(bool));
            credentialsTable.Columns.Add("StationNo", typeof(string));
            credentialsTable.Columns.Add("IsFirstLogin", typeof(bool));

            DataRow row = credentialsTable.NewRow();
            row["User_ID"] = credentials.Username;
            row["Password"] = credentials.Password;
            row["ClientType"] = credentials.ClientType;
            row["SessionID"] = credentials.SessionID;
            row["IsAuthenticated"] = credentials.IsAuthenticated;
            row["IsFirstLogin"] = credentials.IsFirstLogin;

            credentialsTable.Rows.Add(row);
            OperatorDS.Tables.Add(credentialsTable);

            _callApi.PostApiData("/api/DQ_DoOperation", ref OperatorDS, Params);

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
                Username = OperatorDS.Tables["Credentials"].Rows[0]["User_ID"].ToString(),
                Password = OperatorDS.Tables["Credentials"].Rows[0]["Password"].ToString(),
                ClientType = OperatorDS.Tables["Credentials"].Rows[0]["ClientType"].ToString(),
                SessionID = OperatorDS.Tables["Credentials"].Rows[0]["SessionID"].ToString(),
                IsAuthenticated = Convert.ToBoolean(OperatorDS.Tables["Credentials"].Rows[0]["IsAuthenticated"].ToString()),
                IsFirstLogin = Convert.ToBoolean(OperatorDS.Tables["Credentials"].Rows[0]["IsFirstLogin"].ToString())
            };
        }
    }
}


//this.DQ_AddParam("TASK_NAME", "DQWebAuthentication");
//this.DQ_AddParam("CONVERTER_NAME", "Conv_DQWebAuthentication");