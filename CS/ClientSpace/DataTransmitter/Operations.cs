using System;
using System.Collections.Specialized;
using System.Data;
using DQBasePage;
using static DQBasePage.DQBasePage;

namespace DataTransmitter
{
    public class Operations
    {
        private DQBasePage.DQBasePage _basePage;

        public Operations()
        {
            _basePage = new DQBasePage.DQBasePage();
        }

        public void Check_Credentials(ref DataSet OperatorDS, NameValueCollection Params)
        {
            _basePage.DQ_AddParam("TASK_NAME", "DQWebAuthentication");
            _basePage.DQ_AddParam("CONVERTER_NAME", "Conv_DQWebAuthentication");

            _basePage.DQ_AddExtraField<String>("Credentials", "User_ID", i__UserName);
            _basePage.DQ_AddExtraField<String>("Credentials", "Password", i__Password);
            _basePage.DQ_AddExtraField<String>("Credentials", "ClientType", i__ClientType);
            _basePage.DQ_AddExtraField<String>("Credentials", "SessionID", _basePage.Session.SessionID);
            _basePage.DQ_AddExtraField<Boolean>("Credentials", "IsAuthenticated");
            _basePage.DQ_AddExtraField<String>("Credentials", "StationNo");
            _basePage.DQ_AddExtraField<Boolean>("Credentials", "IsFirstLogin");

            _basePage.DQ_BindLevel = Enum_BIND_LEVEL.NONE;
            _basePage.DQ_DoOperation();


            if (_basePage.DQ_GlobalDS.Tables["Credentials"] != null)
            {
                if (_basePage.DQ_GlobalDS.Tables["Credentials"].Rows.Count == 1)
                {
                    Is_Authenticated = Convert.ToBoolean(_basePage.DQ_GlobalDS.Tables["Credentials"].Rows[0]["IsAuthenticated"].ToString());
                    i__IsFirstLogin = Convert.ToBoolean(_basePage.DQ_GlobalDS.Tables["Credentials"].Rows[0]["IsFirstLogin"].ToString());
                }
            }
        }
    }
}
