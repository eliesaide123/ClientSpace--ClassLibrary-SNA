using AutoMapper;
using BLC.Service;
using DAL.ProfileComponent;
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
        private readonly IProfileDAL _DAL;
        public BusinessLogicProfile(IHttpContextAccessor httpContextAccessor, IMapper mapper, IProfileDAL DAL)
        {
            _callApi = new ServiceCallApi();
            _sessionManager = new SessionManager(httpContextAccessor);
            _mapper = mapper;
            _DAL = DAL;
            jsonPath = CommonFunctions.GetJSONFileLocation();
        }
        public GetUserAccountResponse DQ_GetUserAccount(CredentialsDto credentials)
        {
            DataSet GlobalOperatorDS = _DAL.DQ_GetUserAccount(credentials, jsonPath);


            return CommonFunctions.HandleNotifications<GetUserAccountResponse>(GlobalOperatorDS, "NOTIFICATION", () =>
            {
                return new GetUserAccountResponse() { UserAccount = _mapper.Map<DataSet, UserAccount>(GlobalOperatorDS), Questions = _mapper.Map<DataTable, string[]>(GlobalOperatorDS.Tables["Codes"]) };
            });
            
            
        }

        public GetClientInfoResponse DQ_GetClientInfo(DoOpMainParams parameters)
        {
            DataSet GlobalOperatorDS = _DAL.DQ_GetClientInfo(parameters, jsonPath);

            return CommonFunctions.HandleNotifications<GetClientInfoResponse>(GlobalOperatorDS, "NOTIFICATION", () =>
            {
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
            });
        }


        public GetPortfolioResponse DQ_GetPortfolio(DoOpMainParams parameters)
        {
            DataSet GlobalOperatorDS = _DAL.DQ_GetPortfolio(parameters, jsonPath);

            return CommonFunctions.HandleNotifications<GetPortfolioResponse>(GlobalOperatorDS, "NOTIFICATION", () =>
            {
                var formattedData = CommonFunctions.GetListFromData<PolcomPortfolioDto>("Polcom", GlobalOperatorDS);
                var paramsOutput = CommonFunctions.GetOutputParams(ref GlobalOperatorDS);

                var sendData = new GetPortfolioResponse()
                {
                    Polcom = formattedData,
                    Page_Direction = paramsOutput["PAGING_DIRECTION"].ToString()
                };

                return sendData;
            });
        }
    }
}
