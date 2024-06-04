using BLC.Service;
using Entities;
using Entities.IActionResponseDTOs;
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
                var checkRolesResponse = new CheckRolesResponse();

                //if (this.GlobalOperatorDS.Tables["NOTIFICATION"].Rows.Count > 0)
                //{
                //    var errors = CommonFunctions.TransformDataToDictionary("NOTIFICATION", GlobalOperatorDS);
                //    checkRolesResponse.Error = true;
                //    //checkRolesResponse.NOTIFICATION = CommonFunctions.CreateNotificationDto(errors);
                //    return JsonConvert.SerializeObject(checkRolesResponse);
                //}
                if (this.GlobalOperatorDS.Tables["UserIdent"] != null)
                {
                    if (this.GlobalOperatorDS.Tables["UserIdent"].Rows.Count == 1)
                    {
                        var oUserIdent = new cUserIdent();
                        oUserIdent.UserName = this.GlobalOperatorDS.Tables["UserIdent"].Rows[0]["FullName"].ToString();
                        oUserIdent.Pin = this.GlobalOperatorDS.Tables["UserIdent"].Rows[0]["Pin"].ToString();
                        oUserIdent.Role = this.GlobalOperatorDS.Tables["UserIdent"].Rows[0]["Role"].ToString();
                        oUserIdent.Language = this.GlobalOperatorDS.Tables["UserIdent"].Rows[0]["Language"].ToString();
                        oUserIdent.LoggedDate = DateTime.Now.ToShortDateString();

                        _sessionManager.SetSessionValue("DQUserIdent", JsonConvert.SerializeObject(oUserIdent));
                        return JsonConvert.SerializeObject(oUserIdent);
                    }
                }
            }
            return JsonConvert.SerializeObject(false);
        }

        public void SetRole(string sessionId, string roleId)
        {
            GlobalOperatorDS.Tables.Clear();
            List<DQParam> Params = new List<DQParam>();
            Params.Add(new DQParam() { Name = "TASK_NAME", Value = "SetRoles", Type = "" });
            Params.Add(new DQParam() { Name = "SessionID", Value = sessionId, Type = "Q" });
            Params.Add(new DQParam() { Name = "CONVERTER_NAME", Value = "" });
            Params.Add(new DQParam() { Name = "ROLEID", Value = roleId, Type="Q" });
            Params.Add(new DQParam() { Name = "PAGE_MODE", Value = "REAL" });

            DataTable dt = new DataTable("VARIABLES");
            dt.Columns.Add("ATTRIBUTE", typeof(string));
            dt.Columns.Add("STR_VALUE", typeof(string));
            dt.Columns.Add("VAL_FORMAT", typeof(string));
            dt.Columns.Add("VAR_TYPE", typeof(string));
            DataRow row = dt.NewRow();
            row["ATTRIBUTE"] = "";
            row["STR_VALUE"] = "";
            row["VAL_FORMAT"] = "";
            row["VAR_TYPE"] = "";
            dt.Rows.Add(row);
            GlobalOperatorDS.Tables.Add(dt);

            _callApi.PostApiData("/api/DQ_DoOperation", ref GlobalOperatorDS, Params);
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