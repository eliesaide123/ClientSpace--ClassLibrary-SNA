using Entities.IActionResponseDTOs;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.LoginComponent
{
    public interface ILoginDAL
    {
        LoginUserResponse Authenticate(CredentialsDto credentials, string jsonPath);
    }
}
