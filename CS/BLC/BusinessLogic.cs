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

namespace BLC
{
    public class BusinessLogic : IBLC
    {
        private readonly ServiceCallApi _callApi;
        public BusinessLogic() {
            _callApi = new ServiceCallApi();
        }
        public bool Authenticate(CredentialsDto credentials)
        {
            DataSet OperatorDS = new DataSet();
            NameValueCollection Params = new NameValueCollection();
            Params["TASK_NAME"] = "DQWebAuthentication";
            Params["CONVERTER_NAME"] = "Conv_DQWebAuthentication";

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

            credentialsTable.Rows.Add(row);
            OperatorDS.Tables.Add(credentialsTable);

            var data = _callApi.PostApiDataAsync("/api/DQ_DoOperation", OperatorDS, Params);

            //if (DQOperator.DQOperator.DQ_GlobalDS.Tables["Credentials"] != null)
            //{
            //    if (this.DQ_GlobalDS.Tables["Credentials"].Rows.Count == 1)
            //    {
            //        credentials.IsAuthenticated = Convert.ToBoolean(this.DQ_GlobalDS.Tables["Credentials"].Rows[0]["IsAuthenticated"].ToString());
            //        credentials.IsFirstLogin = Convert.ToBoolean(this.DQ_GlobalDS.Tables["Credentials"].Rows[0]["IsFirstLogin"].ToString());
            //    }
            //}


            return false;
        }
    }
}


//this.DQ_AddParam("TASK_NAME", "DQWebAuthentication");
//this.DQ_AddParam("CONVERTER_NAME", "Conv_DQWebAuthentication");