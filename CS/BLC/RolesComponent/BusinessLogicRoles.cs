using BLC.Service;
using Entities;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        public DataSet GlobalOperatorDS;
        private readonly SessionManager _sessionManager;
        public BusinessLogicRoles(IHttpContextAccessor httpContextAccessor)
        {
            _callApi = new ServiceCallApi();
            GlobalOperatorDS = new DataSet();
            _sessionManager = new SessionManager(httpContextAccessor);
        }

        public string DQ_CheckRoles(CredentialsDto credentials)
        {
            GlobalOperatorDS.Tables.Clear();
            List<DQParam> Params = new List<DQParam>();
            Params.Add(new DQParam() { Name = "TASK_NAME", Value = "CheckRoles", Type = "" });
            Params.Add(new DQParam() { Name = "SessionID", Value = credentials.SessionID, Type = "Q" });
            Params.Add(new DQParam() { Name = "CONVERTER_NAME", Value = "" });
            Params.Add(new DQParam() { Name = "ROLEID", Value = "MR" });
            Params.Add(new DQParam() { Name = "PAGE_MODE", Value = "REAL" });

            DQ_GetUserAccount_Add_Codes_ExtraFields();

            _callApi.PostApiData("/api/DQ_DoOperation", ref GlobalOperatorDS, Params);

            if (this.GlobalOperatorDS != null)
            {
                if (this.GlobalOperatorDS.Tables["UserIdent"] != null)
                {
                    if (this.GlobalOperatorDS.Tables["NOTIFICATION"].Rows.Count > 0)
                    {
                        return JsonConvert.SerializeObject(false);
                    }

                        if (this.GlobalOperatorDS.Tables["UserIdent"].Rows.Count == 1)
                    {
                        var oUserIdent = new cUserIdent();
                        oUserIdent.UserName = this.GlobalOperatorDS.Tables["UserIdent"].Rows[0]["FullName"].ToString();
                        oUserIdent.Pin = this.GlobalOperatorDS.Tables["UserIdent"].Rows[0]["Pin"].ToString();
                        oUserIdent.Role = this.GlobalOperatorDS.Tables["UserIdent"].Rows[0]["Role"].ToString();
                        oUserIdent.Language = this.GlobalOperatorDS.Tables["UserIdent"].Rows[0]["Language"].ToString();
                        oUserIdent.LoggedDate = DateTime.Now.ToShortDateString();

                        _sessionManager.SetSessionValue("DQUserIdent", JsonConvert.SerializeObject(oUserIdent));
                        return JsonConvert.SerializeObject(true);
                    }
                }
            }
            return JsonConvert.SerializeObject(false);
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