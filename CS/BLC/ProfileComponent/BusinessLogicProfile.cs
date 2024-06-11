using AutoMapper;
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
        private readonly IMapper _mapper;
        private readonly string jsonPath;
        public BusinessLogicProfile(IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _callApi = new ServiceCallApi();
            _sessionManager = new SessionManager(httpContextAccessor);
            _mapper = mapper;
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
            return new GetUserAccountResponse() { UserAccount = _mapper.Map<DataSet, UserAccount>(GlobalOperatorDS), Questions = _mapper.Map<DataTable, string[]>(GlobalOperatorDS.Tables["Codes"]) };
        }

        public GetClientInfoResponse DQ_GetClientInfo(DoOpMainParams parameters)
        {
            this.GlobalOperatorDS = new DataSet();

            var taskName = "GetClientInfo";
            List<DQParam> Params = new List<DQParam>();
            
            CommonFunctions.ConstructTask(parameters, jsonPath, taskName, ref Params, ref GlobalOperatorDS);

            _callApi.PostApiData("/api/DQ_DoOperation", ref GlobalOperatorDS, Params);
            RemoveFirstRowPersons();

            if (this.GlobalOperatorDS.Tables["NOTIFICATION"].Rows.Count > 0)
            {
                return new GetClientInfoResponse() { Errors = CommonFunctions.GetNotifications("NOTIFICATION", GlobalOperatorDS) };
            }

            var outputParams = CommonFunctions.GetOutputParams(ref GlobalOperatorDS);

            _sessionManager.SetSessionValue("DQ_OnlineSales", outputParams["OnlineSales"]);

            if (!string.IsNullOrEmpty(outputParams["OnlineAgt"]))
            {
                _sessionManager.SetSessionValue("DQ_OnlineAgt", outputParams["OnlineAgt"].Replace("^", "="));
            }

            if (!string.IsNullOrEmpty(outputParams["AgtCode"]))
            {
                _sessionManager.SetSessionValue("AgtCode", outputParams["AgtCode"]);
            }

            var person = _mapper.Map<DataSet, Person>(GlobalOperatorDS);
            var codes = CommonFunctions.GetListFromData<CodesClientInfoDto>("Codes", GlobalOperatorDS);
            var products = CommonFunctions.GetListFromData<ProductClientInfoDto>("Product", GlobalOperatorDS);
            return new GetClientInfoResponse() { Person = person, Products = products, Codes = codes };
        }


        public GetPortfolioResponse DQ_GetPortfolio(DoOpMainParams parameters)
        {
            this.GlobalOperatorDS = new DataSet();
            var taskName = "GetPortfolio";
            List<DQParam> Params = new List<DQParam>();

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
    }
}
