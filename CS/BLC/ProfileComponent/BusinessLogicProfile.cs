using BLC.Service;
using Entities;
using Entities.IActionResponseDTOs;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLC.ProfileComponent
{
    public class BusinessLogicProfile : IBLCProfile
    {
        private readonly ServiceCallApi _callApi;
        private DataSet GlobalOperatorDS;
        private readonly SessionManager _sessionManager;
        private readonly string jsonPath;
        public BusinessLogicProfile(IHttpContextAccessor httpContextAccessor)
        {
            _callApi = new ServiceCallApi();
            _sessionManager = new SessionManager(httpContextAccessor);
            if (ConfigurationManager.AppSettings != null && ConfigurationManager.AppSettings["jsonFilePath"] != null)
            {
                jsonPath = ConfigurationManager.AppSettings["jsonFilePath"];
            }
        }
        public GetUserAccountResponse DQ_GetUserAccount(CredentialsDto credentials)
        {
            this.GlobalOperatorDS = new DataSet();

            var taskName = "GetUserAccount";
            List<DQParam> Params = new List<DQParam>();
            
            var doOpParams = new DoOpMainParams() { Credentials = credentials};

            CommonFunctions.ConstructTask(doOpParams, jsonPath, taskName, ref Params, ref GlobalOperatorDS);

            _callApi.PostApiData("/api/DQ_DoOperation", ref GlobalOperatorDS, Params);
            RemoveFirstRows();

            if (this.GlobalOperatorDS.Tables["NOTIFICATION"].Rows.Count > 0)
            {
                return new GetUserAccountResponse() { Errors = CommonFunctions.GetNotifications("NOTIFICATION", GlobalOperatorDS) };
            }

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

            return new GetUserAccountResponse() { UserAccount = userAccount, Questions = questions };
        }

        public GetClientInfoResponse DQ_GetClientInfo(DoOpMainParams parameters)
        {
            this.GlobalOperatorDS = new DataSet();

            var taskName = "GetClientInfo";
            //List<DQParam> Params = CommonFunctions.GetTaskParams(jsonPath, taskName);
            List<DQParam> Params = new List<DQParam>();
            //Params.Add(new DQParam() { Name = "SessionID", Value = parameters.Credentials.SessionID, Type = "Q" });
            //Params.Add(new DQParam() { Name = "ROLEID", Value = parameters.RoleID, Type = "Q" });

            //DataTable tbl_Persons = CommonFunctions.GetTableColumns(jsonPath, taskName, "Persons");
            //CommonFunctions.DefaultRow(ref tbl_Persons, ref GlobalOperatorDS);
            //DataTable tbl_Product = CommonFunctions.GetTableColumns(jsonPath, taskName, "Product");
            //CommonFunctions.DefaultRow(ref tbl_Product, ref GlobalOperatorDS);
            //DataTable tbl_Codes = CommonFunctions.GetTableColumns(jsonPath, taskName, "Codes");
            //CommonFunctions.DefaultRow(ref tbl_Codes, ref GlobalOperatorDS);

            CommonFunctions.ConstructTask(parameters, jsonPath, taskName, ref Params, ref GlobalOperatorDS);

            _callApi.PostApiData("/api/DQ_DoOperation", ref GlobalOperatorDS, Params);
            RemoveFirstRowPersons();

            if (this.GlobalOperatorDS.Tables["NOTIFICATION"].Rows.Count > 0)
            {
                return new GetClientInfoResponse() { Errors = CommonFunctions.GetNotifications("NOTIFICATION", GlobalOperatorDS) };
            }

            //_sessionManager.SetSessionValue("DQ_OnlineSales", ""); //DQ_GetParameter("OnlineSales");

            //if (!string.IsNullOrEmpty(DQ_GetParameter("OnlineAgt")))
            //{
            //    Session["DQ_OnlineAgt"] = DQ_GetParameter("OnlineAgt").Replace("^", "=");
            //}

            //if (!string.IsNullOrEmpty(DQ_GetParameter("AgtCode")))
            //{
            //    Session["AgtCode"] = DQ_GetParameter("AgtCode");
            //}

            var person = SortingDS();
            var codes = CommonFunctions.GetListFromData<CodesClientInfoDto>("Codes", GlobalOperatorDS);
            var products = CommonFunctions.GetListFromData<ProductClientInfoDto>("Product", GlobalOperatorDS);
            return new GetClientInfoResponse() { Person = person, Products = products, Codes = codes };
        }


        public GetPortfolioResponse DQ_GetPortfolio(DoOpMainParams parameters)
        {
            this.GlobalOperatorDS = new DataSet();
            var taskName = "GetPortfolio";
            //List<DQParam> Params = CommonFunctions.GetTaskParams(jsonPath, taskName);
            List<DQParam> Params = new List<DQParam>();
            //Params.Add(new DQParam() { Name = "SessionID", Value = parameters.Credentials.SessionID, Type = "Q" });
            //Params.Add(new DQParam() { Name = "ROLEID", Value = parameters.RoleID, Type = "Q" });
            //Params.Add(new DQParam() { Name = "PAGING_START_INDEX", Value = parameters.StartIndex.ToString(), Type = "O" });
            //Params.Add(new DQParam() { Name = "PAGING_PAGE_SIZE", Value = parameters.GridSize.ToString(), Type = "Q" });
            //Params.Add(new DQParam() { Name = "PAGING_ACTION", Value = parameters.Direction, Type = "O" });

            //DataTable tbl_Polcom = CommonFunctions.GetTableColumns(jsonPath, taskName, "Polcom");
            //CommonFunctions.DefaultRow(ref tbl_Polcom, ref GlobalOperatorDS);

            CommonFunctions.ConstructTask(parameters, jsonPath, taskName, ref Params, ref GlobalOperatorDS);

            _callApi.PostApiData("/api/DQ_DoOperation", ref GlobalOperatorDS, Params);

            if (this.GlobalOperatorDS.Tables["NOTIFICATION"].Rows.Count > 0)
            {
                return new GetPortfolioResponse()
                {
                    Errors = CommonFunctions.GetNotifications("NOTIFICATION", GlobalOperatorDS)
                };
            }

            var formattedData = CommonFunctions.GetListFromData<PolcomPortfolioDto>("Polcom", GlobalOperatorDS);

            var sendData = new GetPortfolioResponse()
            {
                Polcom = formattedData,
            };

            return sendData;
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
        public void RemoveFirstRowPersons()
        {
            // Remove the first row of each DataTable if they exist, excluding specific tables
            foreach (DataTable table in GlobalOperatorDS.Tables)
            {
                if (table.Rows.Count > 0 && table.TableName == "Persons")
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

        public Person SortingDS()
        {
            DataRow row = GlobalOperatorDS.Tables["Persons"].Rows[0];
            var person = new Person()
            {
                PIN = row["PIN"]?.ToString() != string.Empty ? Convert.ToInt32(row["PIN"]) : 0,
                Age = row["Age"]?.ToString() ?? string.Empty,
                Marital = row["Marital"]?.ToString() ?? string.Empty,
                Per_Title = row["Per_Title"]?.ToString() ?? string.Empty,
                FirstName = row["FirstName"]?.ToString() ?? string.Empty,
                Father = row["Father"]?.ToString() ?? string.Empty,
                Family = row["Family"]?.ToString() ?? string.Empty,
                FullName = row["FullName"]?.ToString() ?? string.Empty,
                Profession = row["Profession"]?.ToString() ?? string.Empty,
                Address = row["Address"]?.ToString() ?? string.Empty,
                EntityType = row["EntityType"]?.ToString() ?? string.Empty,
                DOB_Day = row["DOB_Day"]?.ToString() != string.Empty ? Convert.ToInt32(row["DOB_Day"]) : (int?)null,
                DOB_Month = row["DOB_Month"]?.ToString() != string.Empty ? Convert.ToInt32(row["DOB_Month"]) : (int?)null,
                DOB_Year = row["DOB_Year"]?.ToString() != string.Empty ? Convert.ToInt32(row["DOB_Year"]) : (int?)null,
                HasRequest = row["HasRequest"]?.ToString() != string.Empty ? Convert.ToBoolean(row["HasRequest"]) : false,
                HasUnpaid = row["HasUnpaid"]?.ToString() != string.Empty ? Convert.ToBoolean(row["HasUnpaid"]) : false,
                HasClaims = row["HasClaims"]?.ToString() != string.Empty ? Convert.ToBoolean(row["HasClaims"]) : false,
                HasRenewal = row["HasRenewal"]?.ToString() != string.Empty ? Convert.ToBoolean(row["HasRenewal"]) : false,
                HasFresh = row["HasFresh"]?.ToString() != string.Empty ? Convert.ToBoolean(row["HasFresh"]) : false,
                KYC = row["KYC"]?.ToString() != string.Empty ? Convert.ToBoolean(row["KYC"]) : false,
                ShowProfile = row["ShowProfile"]?.ToString() != string.Empty ? Convert.ToBoolean(row["ShowProfile"]) : false,
                ShowMissing = row["ShowMissing"]?.ToString() != string.Empty ? Convert.ToBoolean(row["ShowMissing"]) : false,
                AgentSOA = row["AgentSOA"]?.ToString() != string.Empty ? Convert.ToBoolean(row["AgentSOA"]) : false,
                RPSEnabled = row["RPSEnabled"]?.ToString() != string.Empty ? Convert.ToBoolean(row["RPSEnabled"]) : false,
                YearMonth = row["YearMonth"]?.ToString() ?? string.Empty,
                KYCMSG = row["KYCMSG"]?.ToString() ?? string.Empty,
                CONVERT_DATA = row["CONVERT_DATA"]?.ToString() ?? string.Empty,
            };

            return person;
        }
    }
}
