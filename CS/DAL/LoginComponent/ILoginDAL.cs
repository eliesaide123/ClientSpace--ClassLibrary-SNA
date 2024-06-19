using Entities.IActionResponseDTOs;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace DAL.LoginComponent
{
    public interface ILoginDAL
    {
        DataSet Authenticate(CredentialsDto credentials, string jsonPath);
    }
}
