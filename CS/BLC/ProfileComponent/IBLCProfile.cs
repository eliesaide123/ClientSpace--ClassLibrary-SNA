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
        void DQ_GetUserAccount_TPIDENT_ExtraFields();
        void DQ_GetUserAccount_TPVALIDSET_ExtraFields();
        void DQ_GetUserAccount_Add_Codes_ExtraFields();
        void RemoveFirstRows();
        string[] ExtractEngFullValues();
    }
}
