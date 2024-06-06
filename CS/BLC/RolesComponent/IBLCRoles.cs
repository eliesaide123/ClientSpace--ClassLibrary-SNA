using Entities;
using Entities.IActionResponseDTOs;
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
        public CheckRolesResponse DQ_CheckRoles(CredentialsDto credentials);
        public void SetRole(string sessionId, string roleId);
    }
}
