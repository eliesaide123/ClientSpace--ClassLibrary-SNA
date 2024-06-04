using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLC.PolicyComponent
{
    public interface IBLCPolicy
    {
        string DQ_GetPIPolicyDetails(DoOpMainParams parameters);
        void DQ_GetPIPolicyDetails_ExtraFields();
        void DQ_GetPIPolicyDetails_Codes_ExtraFields();
    }
}
