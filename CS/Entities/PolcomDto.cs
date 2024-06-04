using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    //public class PolcomDto
    //{
    //    public string? Product {  get; set; }
    //    public string? GrpCode { get; set; }
    //    public Int32 Pol_serno { get; set; }
    //    public string? ProductName { get; set; }
    //    public string? HolderName { get; set; }
    //    public Int32 InsPIN { get; set; }
    //    public string? PolicyNo { get; set; }
    //    public string? PolicyType { get; set; }
    //    public Int32 Policy_No { get; set; }
    //    public string? Agt_code { get; set; }
    //    public DateTime FirstInception { get; set; }
    //    public DateTime Inception { get; set; }
    //    public DateTime Expiry { get; set; }
    //    public bool ClaimDetail { get; set; }
    //    public Int32 ClaimPrevYears { get; set; }
    //    public Int32 OrderBy { get; set; }
    //    public Int32 CertNo { get; set; }
    //    public decimal Net_Frq { get; set; }
    //    public decimal Total_Premium { get; set; }
    //    public string? Cur_Code { get; set; }
    //    public string? Tabs { get; set; }
    //    public string? Template { get; set; }
    //    public string? AsAgreed { get; set; }
    //    public bool HasRequest { get; set; }
    //    public bool HasFresh { get; set; }
    //    public bool Disable_View { get; set; }
    //    public bool OpenCover { get; set; }
    //    public string? Pay_Mode { get; set; }
    //    public string? Pay_Frq { get; set; }
    //    public string? ContractYr { get; set; }
    //    public string? ContractPrd { get; set; }
    //    public string? PaymentYr { get; set; }
    //    public string? PaymentPrd { get; set; }
    //    public string? PaymentExpiry { get; set; }
    //    public string? ContractLabel { get; set; }
    //    public string? PaymentLabel { get; set; }
    //    public string? Status_Code { get; set; }
    //    public string? Agt_Phone { get; set; }
    //    public string? Agt_Email { get; set; }
    //    public string? FirstLifeInsured { get; set; }
    //    public string? SecondLifeInsured { get; set; }
    //    public string? FirstInsLbl { get; set; }
    //    public string? SecondInsLbl { get; set; }
    //    public string? CreditCardNo { get; set; }
    //    public string? CardExpiry { get; set; }
    //    public string? BAS_Policy{ get; set; }
    //    public string? POS_Policy { get; set; }
    //    public string? PayerName { get; set; }
    //    public string? CreditCardlbl { get; set; }
    //    public string? LegalAddress { get; set; }
    //    public string? GCondRef { get; set; }
    //    public string? PolLang { get; set; }
    //    public string? SendSms { get; set; }
    //    public string? PH_Phone { get; set; }
    //    public string? PH_eMail { get; set; }
    //    public string? PolicyYr { get; set; }
    //    public string? PrintCS { get; set; }
    //    public bool GrpPolicy { get; set; }
    //    public string? CONVERT_DATA { get; set; }
    //}

    public class PolcomPortfolioDto {
        public Int32 Pol_serno { get; set; }
        public string? PolicyType { get; set; }
        public string? PolicyNo { get; set; }
        public string? ProductName { get; set; }
        public string? HolderName { get; set; }
        public DateTime Inception { get; set; }
        public DateTime Expiry { get; set; }
        public string? Status_Code { get; set; }
        public Int32 OrderBy { get; set; }
        public Int32 CertNo { get; set; }
        public string? Pay_Frq { get; set; }
        public decimal Total_Premium { get; set; }
        public string? Cur_Code { get; set; }
        public string? Tabs { get; set; }
        public string? Template { get; set; }
        public string? AsAgreed { get; set; }
        public bool HasRequest { get; set; }
        public bool HasFresh { get; set; }
        public bool Disable_View { get; set; }
        public bool OpenCover { get; set; }
        public string? CONVERT_DATA { get; set; }

    }

    public class PolcomPolicyDetailsDto
    {
        public string? Product { get; set; }
        public string? GrpCode { get; set; }
        public Int32 Pol_serno { get; set; }
        public string? HolderName { get; set; }
        public string? ProductName { get; set; }
        public Int32 InsPIN { get; set; }
        public string? PolicyNo { get; set; }
        public Int32 Policy_No { get; set; }
        public string? Agt_code { get; set; }
        public DateTime FirstInception { get; set; }
        public DateTime Inception { get; set; }
        public DateTime Expiry { get; set; }
        public bool ClaimDetail { get; set; }
        public Int32 ClaimPrevYears { get; set; }
        public decimal Net_Frq { get; set; }
        public string? Cur_Code { get; set; }
        public string? Pay_Mode { get; set; }
        public string? Pay_Frq { get; set; }
        public string? ContractYr { get; set; }
        public string? ContractPrd { get; set; }
        public string? PaymentYr { get; set; }
        public string? PaymentPrd { get; set; }
        public string? PaymentExpiry { get; set; }
        public string? ContractLabel { get; set; }
        public string? PaymentLabel { get; set; }
        public string? Status_Code { get; set; }
        public string? Agt_Phone { get; set; }
        public string? Agt_Email { get; set; }
        public string? FirstLifeInsured { get; set; }
        public string? SecondLifeInsured { get; set; }
        public string? FirstInsLbl { get; set; }
        public string? SecondInsLbl { get; set; }
        public string? CreditCardNo { get; set; }
        public string? CardExpiry { get; set; }
        public string? BAS_Policy { get; set; }
        public string? POS_Policy { get; set; }
        public string? PayerName { get; set; }
        public string? CreditCardlbl { get; set; }
        public string? LegalAddress { get; set; }
        public string? GCondRef { get; set; }
        public string? PolLang { get; set; }
        public string? SendSms { get; set; }
        public string? PH_Phone { get; set; }
        public string? PH_eMail { get; set; }
        public string? PolicyYr { get; set; }
        public string? PrintCS { get; set; }
        public bool GrpPolicy { get; set; }
        public string? CONVERT_DATA { get; set; }
    }
}
