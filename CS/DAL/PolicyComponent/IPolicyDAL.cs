using Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.PolicyComponent
{
    public interface IPolicyDAL
    {
        DataSet DQ_GetPIPolicyDetails(DoOpMainParams parameters, string jsonPath);
    }
}
