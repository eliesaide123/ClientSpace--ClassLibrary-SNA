using Entities;
using Entities.IActionResponseDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.RolesComponent
{
    public interface IRolesDAL
    {
        public CheckRolesResponse DQ_CheckRoles(CredentialsDto credentials, string jsonPath);
        public void SetRole(string sessionId, string roleId, string jsonPath);
    }
}
