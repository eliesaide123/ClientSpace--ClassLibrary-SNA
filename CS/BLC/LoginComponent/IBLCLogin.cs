using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using Entities.IActionResponseDTOs;

namespace BLC.LoginComponent
{
    public interface IBLCLogin
    {
        void GetSession(string sessionId);
        LoginUserResponse Authenticate(CredentialsDto credentials);
    }
}
