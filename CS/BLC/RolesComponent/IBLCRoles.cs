using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLC.RolesComponent
{
    public interface IBLCRoles
    {
        void DQ_SetMasterPageRole();
        string DQ_GetLocalValueforKey(string i__Key);
    }
}
