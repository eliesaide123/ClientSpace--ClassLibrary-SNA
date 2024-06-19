using Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ProfileComponent
{
    public interface IProfileDAL
    {
        DataSet DQ_GetUserAccount(CredentialsDto credentials, string jsonPath);
        DataSet DQ_GetClientInfo(DoOpMainParams parameters, string jsonPath);
        DataSet DQ_GetPortfolio(DoOpMainParams parameters, string jsonPath);
    }
}
