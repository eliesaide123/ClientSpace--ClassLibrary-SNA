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
        CredentialsDto Authenticate(CredentialsDto credentials);
        LoginUserResponse IsFirstLogin(CredentialsDto credentials);

        public string DQ_GetBusinessErrorMessage();

        public string DQ_GetBusinessFlashMessage();

        public string DQ_GetTechnicalErrorMessage();

        public string DQ_GetBusinessNotificationMessage();

        public string DQ_GetNotificationMessages(int i__Type);
    }
}
