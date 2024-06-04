using Entities;
using Entities.IActionResponseDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLC.ProfileComponent
{
    public interface IBLCProfile
    {
        GetUserAccountResponse DQ_GetUserAccount(CredentialsDto credentials);
        public GetClientInfoResponse DQ_GetClientInfo(DoOpMainParams parameters);
        GetPortfolioResponse GetPortfolio(DoOpMainParams parameters);
        public void DQ_GetClientInfo_ExtraFields_Codes();
        public void DQ_GetHolderProduct_ExtraFields_Product();
        public void DQ_GetClientInfo_ExtraFields_Persons();
        void DQ_GetUserAccount_TPIDENT_ExtraFields();
        void DQ_GetUserAccount_TPVALIDSET_ExtraFields();
        void DQ_GetUserAccount_Add_Codes_ExtraFields();
        void DQ_GetPortfolio_ExtraFields_Polcom();
    }
}
