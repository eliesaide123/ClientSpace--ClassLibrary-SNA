using AutoMapper;
using BLC.Service;
using BLC;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Entities.IActionResponseDTOs;

namespace DAL.ProfileComponent
{
    public class ProfileDAL : IProfileDAL
    {
        private readonly ServiceCallApi _callApi;
        private DataSet GlobalOperatorDS;
        private readonly SessionManager _sessionManager;
        private readonly IMapper _mapper;

        public ProfileDAL(IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _callApi = new ServiceCallApi();
            GlobalOperatorDS = new DataSet();
            _sessionManager = new SessionManager(httpContextAccessor);
            _mapper = mapper;
        }

        public DataSet DQ_GetUserAccount(CredentialsDto credentials, string jsonPath)
        {
            this.GlobalOperatorDS = new DataSet();

            var taskName = "GetUserAccount";

            var doOpParams = new DoOpMainParams() { Credentials = credentials };

            CommonFunctions.CallDoOperation(_callApi, taskName, doOpParams, jsonPath, ref GlobalOperatorDS);
            CommonFunctions.RemoveFirstRows(ref GlobalOperatorDS);

            return GlobalOperatorDS;
        }

        public DataSet DQ_GetClientInfo(DoOpMainParams parameters, string jsonPath)
        {
            this.GlobalOperatorDS = new DataSet();

            var taskName = "GetClientInfo";

            CommonFunctions.CallDoOperation(_callApi, taskName, parameters, jsonPath, ref GlobalOperatorDS);
            CommonFunctions.RemoveFirstRowFromTblName(ref GlobalOperatorDS, "Persons");

            return GlobalOperatorDS;
        }
        public DataSet DQ_GetPortfolio(DoOpMainParams parameters, string jsonPath)
        {
            this.GlobalOperatorDS = new DataSet();
            var taskName = "GetPortfolio";
            CommonFunctions.CallDoOperation(_callApi, taskName, parameters, jsonPath, ref GlobalOperatorDS);
            return GlobalOperatorDS;
        }
    }
}
