using BLC.Service;
using Entities;
using Entities.IActionResponseDTOs;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLC.PolicyComponent
{
    public class BusinessLogicPolicy :IBLCPolicy
    {
        private readonly ServiceCallApi _callApi;
        public DataSet GlobalOperatorDS;
        private readonly SessionManager _sessionManager;
        public BusinessLogicPolicy(IHttpContextAccessor httpContextAccessor)
        {
            _callApi = new ServiceCallApi();
            GlobalOperatorDS = new DataSet();
            _sessionManager = new SessionManager(httpContextAccessor);
        }

        public GetPolicyDetailsResponse DQ_GetPIPolicyDetails(DoOpMainParams parameters)
        {
            GlobalOperatorDS.Tables.Clear();
            List<DQParam> Params = new List<DQParam>();
            Params.Add(new DQParam() { Name = "TASK_NAME", Value = "GetPIPolicyDetails" });
            Params.Add(new DQParam() { Name = "SessionID", Value = parameters.Credentials.SessionID, Type = "Q" });
            Params.Add(new DQParam() { Name = "ROLEID", Value = parameters.RoleID, Type = "Q" });
            Params.Add(new DQParam() { Name = "PolSerNo", Value = parameters.PolSerNo.ToString(), Type = "O" });
            Params.Add(new DQParam() { Name = "FundPerfPrefix", Value = "", Type = "O" });
            Params.Add(new DQParam() { Name = "CertNo", Value = "0", Type = "Q" });
            Params.Add(new DQParam() { Name = "CONVERTER_NAME", Value = "Conv_GetPIPolicyDetails" });

            DQ_GetPIPolicyDetails_ExtraFields();
            DQ_GetPIPolicyDetails_Codes_ExtraFields();

            _callApi.PostApiData("/api/DQ_DoOperation", ref GlobalOperatorDS, Params);

            var RES_Polcom = CommonFunctions.GetListFromData<PolcomPolicyDetailsDto>("Polcom", GlobalOperatorDS);
            var RES_Codes = CommonFunctions.GetListFromData<CodesPolicyDetailsDto>("Codes", GlobalOperatorDS);

            var sendResponse = new GetPolicyDetailsResponse() { Polcom = RES_Polcom, Codes = RES_Codes };

            return sendResponse;
        }

        public void DQ_GetPIPolicyDetails_ExtraFields()
        {
            DataTable dataTable = new DataTable("Polcom");

            // Add columns to the DataTable
            dataTable.Columns.Add("Product", typeof(string));
            dataTable.Columns.Add("GrpCode", typeof(string));
            dataTable.Columns.Add("Pol_serno", typeof(Int32));
            dataTable.Columns.Add("HolderName", typeof(string));
            dataTable.Columns.Add("ProductName", typeof(string));
            dataTable.Columns.Add("InsPIN", typeof(Int32));
            dataTable.Columns.Add("PolicyNo", typeof(string));
            dataTable.Columns.Add("Policy_No", typeof(Int32));
            dataTable.Columns.Add("Agt_code", typeof(string));
            dataTable.Columns.Add("FirstInception", typeof(DateTime));
            dataTable.Columns.Add("Inception", typeof(DateTime));
            dataTable.Columns.Add("expiry", typeof(DateTime));
            dataTable.Columns.Add("ClaimDetail", typeof(bool));
            dataTable.Columns.Add("ClaimPrevYears", typeof(Int32));
            dataTable.Columns.Add("Net_Frq", typeof(decimal));
            dataTable.Columns.Add("cur_code", typeof(string));
            dataTable.Columns.Add("Pay_Mode", typeof(string));
            dataTable.Columns.Add("Pay_Frq", typeof(string));
            dataTable.Columns.Add("ContractYr", typeof(string));
            dataTable.Columns.Add("ContractPrd", typeof(string));
            dataTable.Columns.Add("PaymentYr", typeof(string));
            dataTable.Columns.Add("PaymentPrd", typeof(string));
            dataTable.Columns.Add("PaymentExpiry", typeof(string));
            dataTable.Columns.Add("ContractLabel", typeof(string));
            dataTable.Columns.Add("PaymentLabel", typeof(string));
            dataTable.Columns.Add("Status_code", typeof(string));
            dataTable.Columns.Add("Agt_Phone", typeof(string));
            dataTable.Columns.Add("Agt_Email", typeof(string));
            dataTable.Columns.Add("FirstLifeInsured", typeof(string));
            dataTable.Columns.Add("SecondLifeInsured", typeof(string));
            dataTable.Columns.Add("FirstInsLbl", typeof(string));
            dataTable.Columns.Add("SecondInsLbl", typeof(string));
            dataTable.Columns.Add("CreditCardNo", typeof(string));
            dataTable.Columns.Add("CardExpiry", typeof(string));
            dataTable.Columns.Add("BAS-Policy", typeof(string));
            dataTable.Columns.Add("POS-Policy", typeof(string));
            dataTable.Columns.Add("PayerName", typeof(string));
            dataTable.Columns.Add("CreditCardlbl", typeof(string));
            dataTable.Columns.Add("LegalAddress", typeof(string));
            dataTable.Columns.Add("GCondRef", typeof(string));
            dataTable.Columns.Add("PolLang", typeof(string));
            dataTable.Columns.Add("SendSms", typeof(string));
            dataTable.Columns.Add("PH_Phone", typeof(string));
            dataTable.Columns.Add("PH_eMail", typeof(string));
            dataTable.Columns.Add("PolicyYr", typeof(string));
            dataTable.Columns.Add("PrintCS", typeof(string));
            dataTable.Columns.Add("GrpPolicy", typeof(bool));
            dataTable.Columns.Add("CONVERT_DATA", typeof(string));

            GlobalOperatorDS.Tables.Add(dataTable);
        }

        public void DQ_GetPIPolicyDetails_Codes_ExtraFields()
        {
            DataTable dataTable = new DataTable("Codes");

            // Add columns to the DataTable
            dataTable.Columns.Add("Tbl_Name", typeof(string));
            dataTable.Columns.Add("Code", typeof(string));
            dataTable.Columns.Add("Eng_Full", typeof(string));
            dataTable.Columns.Add("Repl_With", typeof(string));

            GlobalOperatorDS.Tables.Add(dataTable);
        }

        
    }
}
