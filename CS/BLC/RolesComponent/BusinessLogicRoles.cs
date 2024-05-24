using BLC.Service;
using Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLC.RolesComponent
{
    public class BusinessLogicRoles : IBLCRoles
    {
        private readonly ServiceCallApi _callApi;
        public DataSet GlobalOperatorDS;
        private readonly SessionManager _sessionManager;
        public BusinessLogicRoles(IHttpContextAccessor httpContextAccessor)
        {
            _callApi = new ServiceCallApi();
            GlobalOperatorDS = new DataSet();
            _sessionManager = new SessionManager(httpContextAccessor);
        }
        public void DQ_SetMasterPageRole()
        {
            cUserIdent oUserIdent = new cUserIdent();

            if (_sessionManager.GetSessionValue("DQUserIdent") != null)
            {
                oUserIdent = (cUserIdent)Convert.ChangeType(_sessionManager.GetSessionValue("DQUserIdent"), oUserIdent.GetType());
            }

            oUserIdent.Role = DQ_GetLocalValueforKey("Role_Desc");

            _sessionManager.SetSessionValue("DQ_ROLEID" ,"MR");
        }

        public string DQ_GetLocalValueforKey(string i__Key)
        {
            string empty = string.Empty;
            return ""; /*GetLocalResourceObject(i__Key).ToString();*/
        }
    }
}