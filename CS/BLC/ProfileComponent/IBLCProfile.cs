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
        GetPortfolioResponse DQ_GetPortfolio(DoOpMainParams parameters);
    }
}
