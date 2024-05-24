using BLC.Service;
using Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLC.ProfileComponent
{
    public class BusinessLogicProfile: IBLCProfile
    {
        private readonly ServiceCallApi _callApi;
        public DataSet GlobalOperatorDS;
        private readonly SessionManager _sessionManager;
        public BusinessLogicProfile(IHttpContextAccessor httpContextAccessor)
        {
            _callApi = new ServiceCallApi();
            GlobalOperatorDS = new DataSet();
            _sessionManager = new SessionManager(httpContextAccessor);
        }
        public void DQ_GetUserAccount(CredentialsDto credentials)
        {
            GlobalOperatorDS.Tables.Clear();
            List<DQParam> Params = new List<DQParam>();
            Params.Add(new DQParam() { Name = "TASK_NAME", Value = "GetUserAccount", Type = "" });
            Params.Add(new DQParam() { Name = "SessionID", Value = credentials.SessionID, Type = "Q" });

            DQ_GetUserAccount_TPIDENT_ExtraFields();
            DQ_GetUserAccount_TPVALIDSET_ExtraFields();
            DQ_GetUserAccount_Add_Codes_ExtraFields();

            _callApi.PostApiData("/api/DQ_DoOperation", ref GlobalOperatorDS, Params);
        }

        public void DQ_GetUserAccount_TPIDENT_ExtraFields()
        {
            DataTable dataTable = new DataTable("TPIDENT");

            // Add columns to the DataTable
            dataTable.Columns.Add("TP-UserId", typeof(string));
            dataTable.Columns.Add("TP-Mobile", typeof(string));
            dataTable.Columns.Add("TP-UserName", typeof(string));
            dataTable.Columns.Add("TP-Email", typeof(string));
            dataTable.Columns.Add("TP-UserLang", typeof(string));
            dataTable.Columns.Add("TP-ContactScenario", typeof(string));
            dataTable.Columns.Add("TP-Pwd", typeof(string));
            dataTable.Columns.Add("TP-RegType", typeof(string));

            // Add a row with values
            DataRow row = dataTable.NewRow();
            row["TP-UserId"] = string.Empty;
            row["TP-Mobile"] = string.Empty;
            row["TP-UserName"] = string.Empty;
            row["TP-Email"] = string.Empty;
            row["TP-UserLang"] = string.Empty;
            row["TP-ContactScenario"] = string.Empty;
            row["TP-Pwd"] = string.Empty;
            row["TP-RegType"] = string.Empty;
            dataTable.Rows.Add(row);
            GlobalOperatorDS.Tables.Add(dataTable);
        }

        public void DQ_GetUserAccount_TPVALIDSET_ExtraFields()
        {
            DataTable dataTable = new DataTable("TPVALIDSET");

            // Add columns to the DataTable
            dataTable.Columns.Add("TP-Question", typeof(string));
            dataTable.Columns.Add("TP-Answer", typeof(string));

            // Add rows with the desired fields
            DataRow row = dataTable.NewRow();
            row["TP-Question"] = "";
            row["TP-Answer"] = "";
            dataTable.Rows.Add(row);
            GlobalOperatorDS.Tables.Add(dataTable);
        }

        public void DQ_GetUserAccount_Add_Codes_ExtraFields()
        {
            DataTable dataTable = new DataTable("Codes");

            // Add columns to the DataTable
            dataTable.Columns.Add("Tbl_Name", typeof(string));
            dataTable.Columns.Add("Code", typeof(string));
            dataTable.Columns.Add("Eng_Full", typeof(string));

            DataRow row = dataTable.NewRow();
            row["Tbl_Name"] = "";
            row["Code"] = "";
            row["Eng_Full"] = "";
            dataTable.Rows.Add(row);
            GlobalOperatorDS.Tables.Add(dataTable);
        }
    }
}
