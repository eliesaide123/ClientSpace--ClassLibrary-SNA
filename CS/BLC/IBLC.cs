using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace BLC
{
    public interface IBLC
    {
        void GetSession(string sessionId);
        CredentialsDto Authenticate(CredentialsDto credentials);
    }
}
