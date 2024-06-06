using Entities;
using Entities.IActionResponseDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLC.PolicyComponent
{
    public interface IBLCPolicy
    {
        GetPolicyDetailsResponse DQ_GetPIPolicyDetails(DoOpMainParams parameters);
    }
}
