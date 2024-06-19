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

namespace DAL.PolicyComponent
{
    public class PolicyDAL:IPolicyDAL
    {
        private readonly ServiceCallApi _callApi;
        private DataSet GlobalOperatorDS;
        private readonly SessionManager _sessionManager;
        private readonly IMapper _mapper;

        public PolicyDAL(IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _callApi = new ServiceCallApi();
            GlobalOperatorDS = new DataSet();
            _sessionManager = new SessionManager(httpContextAccessor);
            _mapper = mapper;
        }

        public DataSet DQ_GetPIPolicyDetails(DoOpMainParams parameters, string jsonPath)
        {
            this.GlobalOperatorDS = new DataSet();
            var taskName = "GetPIPolicyDetails";

            CommonFunctions.CallDoOperation(_callApi, taskName, parameters, jsonPath, ref GlobalOperatorDS);
            return GlobalOperatorDS;
        }
    }
}
