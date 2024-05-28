using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLC.ProfileComponent
{
    public interface IBLCProfile
    {
        string DQ_GetUserAccount(CredentialsDto credentials);
        public string DQ_GetClientInfo(string sessionId, string roleId);
        public void DQ_GetClientInfo_ExtraFields_Codes();
        public void DQ_GetHolderProduct_ExtraFields_Product();
        public void DQ_GetClientInfo_ExtraFields_Persons();
        void DQ_GetUserAccount_TPIDENT_ExtraFields();
        void DQ_GetUserAccount_TPVALIDSET_ExtraFields();
        void DQ_GetUserAccount_Add_Codes_ExtraFields();
        void RemoveFirstRows();
        public void RemoveFirstRowPersons();
        string[] ExtractEngFullValues();
        string SortingDS();
    }
}
