using Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLC.RolesComponent
{
    public interface IBLCRoles
    {
        public string DQ_CheckRoles(CredentialsDto credentials);
        public string SetRole(string sessionId, string roleId);
        public void DQ_GetUserAccount_Add_Codes_ExtraFields();
    }
}
