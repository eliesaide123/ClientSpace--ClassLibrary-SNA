using BLC.Service;
using Entities;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
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
        public string DQ_GetUserAccount(CredentialsDto credentials)
        {
            GlobalOperatorDS.Tables.Clear();

            List<DQParam> Params = new List<DQParam>();
            Params.Add(new DQParam() { Name = "TASK_NAME", Value = "GetUserAccount", Type = "" });
            Params.Add(new DQParam() { Name = "SessionID", Value = credentials.SessionID, Type = "Q" });
            Params.Add(new DQParam() { Name = "CONVERTER_NAME", Value = "" });
            Params.Add(new DQParam() { Name = "PAGE_MODE", Value = "REAL" });

            DQ_GetUserAccount_TPIDENT_ExtraFields();
            DQ_GetUserAccount_TPVALIDSET_ExtraFields();
            DQ_GetUserAccount_Add_Codes_ExtraFields();

            _callApi.PostApiData("/api/DQ_DoOperation", ref GlobalOperatorDS, Params);
            RemoveFirstRows();

            var userAccount = new UserAccount()
            {
                Username = GlobalOperatorDS.Tables["TPIDENT"].Rows[0]["TP-UserId"].ToString(),
                Password = GlobalOperatorDS.Tables["TPIDENT"].Rows[0]["TP-Pwd"].ToString(),
                Email = GlobalOperatorDS.Tables["TPIDENT"].Rows[0]["TP-Email"].ToString(),
                Mobile = GlobalOperatorDS.Tables["TPIDENT"].Rows[0]["TP-Mobile"].ToString(),
                UserLang = Convert.ToInt32(GlobalOperatorDS.Tables["TPIDENT"].Rows[0]["TP-UserLang"]),
                ContactScenario = GlobalOperatorDS.Tables["TPIDENT"].Rows[0]["TP-ContactScenario"].ToString(),
                RegType = GlobalOperatorDS.Tables["TPIDENT"].Rows[0]["TP-RegType"].ToString(),
                Question = GlobalOperatorDS.Tables["TPVALIDSET"].Rows[0]["TP-Question"].ToString(),
                Answer = GlobalOperatorDS.Tables["TPVALIDSET"].Rows[0]["TP-Answer"].ToString(),
            };

            string[] questions = ExtractEngFullValues();

            return JsonConvert.SerializeObject(new { userAccount, questions });
        }

        public string DQ_GetClientInfo(string sessionId, string roleId)
        {
            GlobalOperatorDS.Tables.Clear();

            List<DQParam> Params = new List<DQParam>();
            Params.Add(new DQParam() { Name = "TASK_NAME", Value = "GetClientInfo", Type = "" });
            Params.Add(new DQParam() { Name = "SessionID", Value = sessionId, Type = "Q" });
            Params.Add(new DQParam() { Name = "CONVERTER_NAME", Value = "Conv_GetClientInfo" });
            Params.Add(new DQParam() { Name = "PAGE_MODE", Value = "REAL" });
            Params.Add(new DQParam() { Name = "OnlineSales", Value = "", Type="O" });
            Params.Add(new DQParam() { Name = "OnlineAgt", Value = "", Type="O" });
            Params.Add(new DQParam() { Name = "AgtCode", Value = "", Type="O" });
            Params.Add(new DQParam() { Name = "ROLEID", Value = roleId, Type="Q" });

            DQ_GetClientInfo_ExtraFields_Persons();
            DQ_GetHolderProduct_ExtraFields_Product();
            DQ_GetClientInfo_ExtraFields_Codes();

            _callApi.PostApiData("/api/DQ_DoOperation", ref GlobalOperatorDS, Params);
            RemoveFirstRows();

            //_sessionManager.SetSessionValue("DQ_OnlineSales", ""); //DQ_GetParameter("OnlineSales");

            //if (!string.IsNullOrEmpty(DQ_GetParameter("OnlineAgt")))
            //{
            //    Session["DQ_OnlineAgt"] = DQ_GetParameter("OnlineAgt").Replace("^", "=");
            //}

            //if (!string.IsNullOrEmpty(DQ_GetParameter("AgtCode")))
            //{
            //    Session["AgtCode"] = DQ_GetParameter("AgtCode");
            //}
            return string.Empty;
        }

        public void DQ_GetClientInfo_ExtraFields_Persons()
        {
            DataTable personsTable = new DataTable("Persons");

            personsTable.Columns.Add("PIN", typeof(int));
            personsTable.Columns.Add("Age", typeof(string));
            personsTable.Columns.Add("Marital", typeof(string));
            personsTable.Columns.Add("Per_Title", typeof(string));
            personsTable.Columns.Add("FirstName", typeof(string));
            personsTable.Columns.Add("Father", typeof(string));
            personsTable.Columns.Add("Family", typeof(string));
            personsTable.Columns.Add("FullName", typeof(string));
            personsTable.Columns.Add("Profession", typeof(string));
            personsTable.Columns.Add("Address", typeof(string));
            personsTable.Columns.Add("EntityType", typeof(string));
            personsTable.Columns.Add("DOB_Day", typeof(int));
            personsTable.Columns.Add("DOB_Month", typeof(int));
            personsTable.Columns.Add("DOB_Year", typeof(int));
            personsTable.Columns.Add("HasRequest", typeof(bool));
            personsTable.Columns.Add("HasUnpaid", typeof(bool));
            personsTable.Columns.Add("HasClaims", typeof(bool));
            personsTable.Columns.Add("HasRenewal", typeof(bool));
            personsTable.Columns.Add("HasFresh", typeof(bool));
            personsTable.Columns.Add("KYC", typeof(bool));
            personsTable.Columns.Add("ShowProfile", typeof(bool));
            personsTable.Columns.Add("ShowMissing", typeof(bool));
            personsTable.Columns.Add("AgentSOA", typeof(bool));
            personsTable.Columns.Add("RPSEnabled", typeof(bool));
            personsTable.Columns.Add("YearMonth", typeof(string));
            personsTable.Columns.Add("KYCMSG", typeof(string));
            personsTable.Columns.Add("CONVERT_DATA", typeof(string));

            DataRow row = personsTable.NewRow();
            foreach (DataColumn column in personsTable.Columns)
            {
                if (column.DataType == typeof(string))
                {
                    row[column.ColumnName] = string.Empty;
                }
                else if (column.DataType == typeof(int))
                {
                    row[column.ColumnName] = 0; // Default value for int
                }
                else if (column.DataType == typeof(bool))
                {
                    row[column.ColumnName] = false; // Default value for bool
                }
                // Add additional type checks if needed
            }
            personsTable.Rows.Add(row);

            GlobalOperatorDS.Tables.Add(personsTable);
        }
        public void DQ_GetHolderProduct_ExtraFields_Product()
        {
            DataTable productTable = new DataTable("Product");

            productTable.Columns.Add("Product", typeof(string));
            productTable.Columns.Add("Prod_Desc", typeof(string));

            DataRow row = productTable.NewRow();
            row["Product"] = string.Empty;
            row["Prod_Desc"] = string.Empty;

            productTable.Rows.Add(row);

            GlobalOperatorDS.Tables.Add(productTable);
        }

        public void DQ_GetClientInfo_ExtraFields_Codes()
        {
            DataTable codesTable = new DataTable("Codes");

            codesTable.Columns.Add("Code", typeof(string));
            codesTable.Columns.Add("Eng_Full", typeof(string));

            DataRow row = codesTable.NewRow();
            row["Code"] = string.Empty;
            row["Eng_Full"] = string.Empty;

            codesTable.Rows.Add(row);

            GlobalOperatorDS.Tables.Add(codesTable);
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

        public void RemoveFirstRows()
        {
            // Remove the first row of each DataTable if they exist, excluding specific tables
            foreach (DataTable table in GlobalOperatorDS.Tables)
            {
                if (table.Rows.Count > 0 && table.TableName != "PARAMETERS" && table.TableName != "NOTIFICATION")
                {
                    table.Rows[0].Delete();
                    table.AcceptChanges();
                }
            }
        }

        public string[] ExtractEngFullValues()
        {
            // Use LINQ to filter rows where "Tbl_Name" is the specified value and select "Eng_Full"
            var query = from row in GlobalOperatorDS.Tables["Codes"].AsEnumerable()
                        where row.Field<string>("Tbl_Name") == "_CSQuestions"
                        select row.Field<string>("Eng_Full");

            // Convert the result to an array
            return query.ToArray();
        }
    }
}
