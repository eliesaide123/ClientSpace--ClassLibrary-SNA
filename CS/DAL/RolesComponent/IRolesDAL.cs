using Entities;
using Entities.IActionResponseDTOs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.RolesComponent
{
    public interface IRolesDAL
    {
        DataSet DQ_CheckRoles(CredentialsDto credentials, string jsonPath);
        void SetRole(string sessionId, string roleId, string jsonPath);
    }
}
