using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Threading;
using System.IO;
using System.Resources;
using System.Xml;
using System.ComponentModel;
using System.Reflection;

namespace DQBasePage
{
    #region Delegate.
    public delegate void DQ_PrePostHandler();
    #endregion

    public partial class DQBasePage : System.Web.UI.Page
    {

        #region Enumeration.
        public enum Enum_PAGE_ACTION
        {
            EDIT,
            VIEW,
            ADD,
            PRINT,
            SEARCH
        };
        public enum Enum_BIND_LEVEL
        {
            ALL,
            FIELDS,
            SECTIONS,
            NONE
            //TABLES,
            //TABLES_AND_FIELDS,
        };

        public enum Enum_PAGE_MODE
        {
            DEV,
            REAL
        };
        #endregion
        #region Members
        private bool m_Is_Transitory_Page;
        private DataSet m_GlobalDS;
        private DataSet m_ExtraFieldsDS;
        private DataSet m_ParametersDS;
        private Enum_PAGE_ACTION m_PageAction;
        private Enum_BIND_LEVEL m_BindLevel;
        private List<Control> m_ControlsToBind;
        private List<String> m_TablesToBind;
        private List<String> m_SectionsToBind;
        private string m_MasterPage;
        private DQLogger.DQLogger m_DQLogger;
        private string m_DQAppRoot;
        private List<string> m_ListOfPrivateDataDTsToClear;
        private bool IsLoginRedirectionEnabled = true;
        private Enum_PAGE_MODE m_PageMode;
        private Int32 _CWS_Version = 1;
        #endregion
        #region Properties
        
        public bool Is_Transitory_Page
        {
            get { return m_Is_Transitory_Page; }
            set { m_Is_Transitory_Page = value; }
        }
        public DataSet DQ_GlobalDS
        {
            get { return m_GlobalDS; }
            set { m_GlobalDS = value; }
        }
        public DataSet DQ_GlobalDS_NewSession { get; set; }
        public Enum_PAGE_ACTION DQ_PageAction
        {
            get { return m_PageAction; }
            set { m_PageAction = value; }
        }
        public Enum_BIND_LEVEL DQ_BindLevel
        {
            get { return m_BindLevel; }
            set { m_BindLevel = value; }
        }
        public List<Control> DQ_ControlsToBind
        {
            get { return m_ControlsToBind; }
            set { m_ControlsToBind = value; }
        }
        public List<String> DQ_TablesToBind
        {
            get { return m_TablesToBind; }
            set { m_TablesToBind = value; }
        }
        public List<String> DQ_SectionsToBind
        {
            get { return m_SectionsToBind; }
            set { m_SectionsToBind = value; }
        }
        public string DQ_MasterPage
        {
            get { return m_MasterPage; }
            set { m_MasterPage = value; }
        }
        public DQLogger.DQLogger DQ_Logger
        {
            get
            {
                return m_DQLogger;
            }
            set
            {
                m_DQLogger = value;
            }
        }
        public string DQ_AppRoot
        {
            get
            {
                if (String.IsNullOrEmpty(m_DQAppRoot))
                {
                    if (System.Configuration.ConfigurationManager.AppSettings["SITE_PATH"] != null)
                    {
                        m_DQAppRoot = System.Configuration.ConfigurationManager.AppSettings["SITE_PATH"].ToString();
                    }                    
                }
                return m_DQAppRoot;
            }
            set
            {
                m_DQAppRoot = value;
            }
        }
        public List<string> DQ_ListOfPrivateDataDTsToClear
        {
            get
            {
                return m_ListOfPrivateDataDTsToClear;
            }
            set
            {
                m_ListOfPrivateDataDTsToClear = value;
            }
        }
        public Boolean DQ_IsLoginRedirectionEnabled
        {
            get
            {
                return IsLoginRedirectionEnabled;
            }
            set
            {
                IsLoginRedirectionEnabled = value; 
            }
        }
        public Enum_PAGE_MODE DQ_PageMode
        {
            get
            {
                return m_PageMode;
            }
            set
            {
                m_PageMode = value;
            }
        }
        public Int32 CWS_VERSION
        {
            get
            {
                if (ConfigurationManager.AppSettings["CWS_VERSION"] != null)
                {
                    _CWS_Version = Convert.ToInt32(ConfigurationManager.AppSettings["CWS_VERSION"].ToString());
                }
                else
                {
                    _CWS_Version = 1;
                }
                return _CWS_Version;
            }            
        }
        public List<DQServiceLayer.ExtentColumn> DQ_List_Of_Extent_Columns { get; set; }
        #endregion
        #region Events
        public event DQ_PrePostHandler DQ_PreScreen2DS;
        public event DQ_PrePostHandler DQ_PostScreen2DS;
        public event DQ_PrePostHandler DQ_PreManageExtraFields;
        public event DQ_PrePostHandler DQ_PostManageExtraFields;
        public event DQ_PrePostHandler DQ_PreManageParameters;
        public event DQ_PrePostHandler DQ_PostManageParameters;
        public event DQ_PrePostHandler DQ_PreRunProgram;
        public event DQ_PrePostHandler DQ_PostRunProgram;
        public event DQ_PrePostHandler DQ_PreDS2ScreenProgram;
        public event DQ_PrePostHandler DQ_PostDS2ScreenProgram;
        #endregion
        #region Constructor
        public DQBasePage()
        {
            #region Initialization Section.
            DQScreenBinder.ScreenBinder oScreenBinder = new DQScreenBinder.ScreenBinder();
            this.DQ_GlobalDS                    = new DataSet();
            this.m_ExtraFieldsDS                = new DataSet();
            this.m_ParametersDS                 = new DataSet();
            this.DQ_ControlsToBind              = new List<Control>();
            this.DQ_BindLevel                   = Enum_BIND_LEVEL.ALL;
            this.DQ_TablesToBind                = new List<string>();
            this.DQ_SectionsToBind              = new List<string>();
            this.DQ_Logger                      = new DQLogger.DQLogger();
            this.DQ_AppRoot                     = string.Empty;
            this.DQ_ListOfPrivateDataDTsToClear = new List<string>();
            this.DQ_PageMode                    = Enum_PAGE_MODE.REAL;
            #endregion
            #region Creating ExtraFields DataTable.
            oScreenBinder.DQ_CreateDTExtraFields(this.m_ExtraFieldsDS);
            #endregion
            #region Adding Parameters Table.
            oScreenBinder.DQ_CreateDTParameters(this.m_ParametersDS);
            oScreenBinder.DQ_AddParametersFromConfig(this.m_ParametersDS);
            #endregion
            #region Adding the Errors DataTable
            oScreenBinder.DQ_AddDTErrors(this.DQ_GlobalDS);
            #endregion           
        }
        #endregion
        #region DoOperation
        public void DQ_DoOperation()
        {
            try
            {
                #region Declaration And Initialization Section.
                DQScreenBinder.ScreenBinder oScreenBinder = new DQScreenBinder.ScreenBinder();
                DQServiceLayer.cServiceLayer oServiceLayer = new DQServiceLayer.cServiceLayer();
                List<string> i_BuiltInDTs = new List<string>();
                #endregion
                #region DQ_ResetGlobalDS.
                DQ_ResetGlobalDS(ref m_GlobalDS);
                #endregion
                #region DQ_PrepareControlsToBind.
                oScreenBinder.DQ_ControlsToBind = DQ_PrepareControlsToBind();
                #endregion
                #region Screen2DS
                #region PreScreen2DS.
                if (DQ_PreScreen2DS != null)
                {
                    DQ_PreScreen2DS();
                    DQ_HandleEvent(ref DQ_PreScreen2DS);
                }
                #endregion
                #region Actual Screen2DS
                oScreenBinder.DQ_Screen2DS(this.Page, m_GlobalDS);
                #endregion
                #region PostScreen2DS
                if (DQ_PostScreen2DS != null)
                {
                    DQ_PostScreen2DS();
                    DQ_HandleEvent(ref DQ_PostScreen2DS);
                }
                #endregion
                #endregion
                #region ManageExtraFields.
                #region PreManageExtraFields
                if (DQ_PreManageExtraFields != null)
                {
                    DQ_PreManageExtraFields();
                    DQ_HandleEvent(ref DQ_PreManageExtraFields);
                }
                #endregion
                #region Actual ManageExtraFields
                oScreenBinder.DQ_MergeExtraFields(m_GlobalDS, this.m_ExtraFieldsDS, true);
                #endregion
                #region PostManageExtraFields
                if (DQ_PostManageExtraFields != null)
                {
                    DQ_PostManageExtraFields();
                    DQ_HandleEvent(ref DQ_PostManageExtraFields);
                }
                #endregion
                #endregion
                #region ManageParameters.
                #region DQ_AddRequiredParameters
                DQ_AddRequiredParameters();
                #endregion
                #region PreManageParameters
                if (DQ_PreManageParameters != null)
                {
                    DQ_PreManageParameters();
                    DQ_HandleEvent(ref DQ_PreManageParameters);
                }
                #endregion
                #region Actual Manage Parameters.
                oScreenBinder.DQ_AddDTParameters(m_GlobalDS, this.m_ParametersDS);
                #endregion
                #region PostManageParameters.
                if (DQ_PostManageParameters != null)
                {
                    DQ_PostManageParameters();
                    DQ_HandleEvent(ref DQ_PostManageParameters);
                }
                #endregion
                #endregion
                #region ClearUnusedPrivateData
                if (m_ListOfPrivateDataDTsToClear != null)
                {
                    foreach (string str_Entry in m_ListOfPrivateDataDTsToClear)
                    {
                        DQ_RemoveRowAt(this.DQ_GlobalDS, str_Entry, 0);
                    }
                }
                #endregion
                
                #region RunProgram.
                #region DQ_HandShake_Via_Operator
                if (DQ_HandShake_Via_Operator())
                {
                    #region PreRunProgram.
                    if (DQ_PreRunProgram != null)
                    {
                        DQ_PreRunProgram();
                        DQ_HandleEvent(ref DQ_PreRunProgram);
                    }
                    #endregion
                    #region Actual RunProgram.
                    oServiceLayer.My_List_Extent_Columns = this.DQ_List_Of_Extent_Columns;
                    try
                    {
                        oServiceLayer.RunProgram(ref m_GlobalDS);
                    }
                    catch (Exception ex)
                    {
                        if (this.DQ_GlobalDS.Tables.Contains("NOTIFICATION"))
                        {
                            DataRow oDataRow_Notification = this.DQ_GlobalDS.Tables["NOTIFICATION"].NewRow();
                            oDataRow_Notification["NOTIFICATION_TYPE"] = "0";
                            oDataRow_Notification["NOTIFICATION_DESC"] = ex.Message;

                            this.DQ_GlobalDS.Tables["NOTIFICATION"].Rows.Add(oDataRow_Notification);
                        }
                    }
                    #endregion
                    #region PostRunProgram.
                    if (DQ_PostRunProgram != null)
                    {
                        DQ_PostRunProgram();
                        DQ_HandleEvent(ref DQ_PostRunProgram);
                    }
                    #endregion
                    #region DQ_HandleDTErrors
                    DQ_HandleDTErrors();
                    #endregion
                }
                else
                {
                    this.DQ_GlobalDS.Tables["NOTIFICATION"].Clear();
                    this.DQ_GlobalDS.Tables["NOTIFICATION"].ImportRow(this.DQ_GlobalDS_NewSession.Tables["NOTIFICATION"].Rows[0]);
                    DQ_HandleDTErrors();
                }
                #endregion                
                
                #endregion
                #region DS2Screen.
                //#region PreDS2Screen
                //if (DQ_PreDS2ScreenProgram != null) 
                //{ 
                //    DQ_PreDS2ScreenProgram();
                //    DQ_HandleEvent(ref DQ_PreDS2ScreenProgram);
                //}
                //#endregion
                //#region  DS2Screen.
                //oScreenBinder.DQ_DS2Screen(this.Page, m_GlobalDS);
                //#endregion
                //#region PostDS2Screen.
                //if (DQ_PostDS2ScreenProgram != null) 
                //{ 
                //    DQ_PostDS2ScreenProgram();
                //    DQ_HandleEvent(ref DQ_PostDS2ScreenProgram);
                //}
                //#endregion
                #endregion
            }
            catch (System.Exception ex)
            {
                ex.Data.Add("CM_DoOperation", "This is a custom error propagated through the exception object");
                throw;
            }
        }
        #endregion
        #region DQ_BindScreenFromDS
        public void DQ_BindDS2Screen()
        {
            #region Declaration And Initialization Section.
            DQScreenBinder.ScreenBinder oScreenBinder = new DQScreenBinder.ScreenBinder();
            #endregion

            #region Body Section
            oScreenBinder.DQ_DS2Screen(this.Page, m_GlobalDS);
            #endregion
        }
        #endregion
        #region DQ_AddParam
        public void DQ_AddParam(string i_Param_Name, string i_Param_Value)
        {
            #region Declaration And Initialization Section.
            DQScreenBinder.ScreenBinder oScreenBinder = new DQScreenBinder.ScreenBinder();
            #endregion

            #region Body Section.
            oScreenBinder.DQ_AddParam(this.m_ParametersDS, i_Param_Name, i_Param_Value);
            #endregion
        }
        #endregion
        #region DQ_AddParam
        public void DQ_AddParam(string i_Param_Name, string i_Param_Value,string i_Param_Type)
        {
            #region Declaration And Initialization Section.
            DQScreenBinder.ScreenBinder oScreenBinder = new DQScreenBinder.ScreenBinder();
            #endregion

            #region Body Section.
            oScreenBinder.DQ_AddParam(this.m_ParametersDS, i_Param_Name, i_Param_Value, i_Param_Type);
            #endregion
        }
         #endregion
        #region DQ_AddExtraTable.
        private void DQ_AddExtraTable(string i_TableName)
        {
            #region Declaration And Initialization Section.
            DQScreenBinder.ScreenBinder oScreenBinder = new DQScreenBinder.ScreenBinder();
            #endregion
            oScreenBinder.DQ_AddExtraTableToGlobalDS(this.DQ_GlobalDS, i_TableName);
        }
        #endregion
        #region DQ_AddExtraFields
        public void DQ_AddExtraField(string i_TableName, string i_ColumnName, string i_ColumnType, string i_Value)
        {
            #region Declaration And Initialization Section.
            DQScreenBinder.ScreenBinder oScreenBinder = new DQScreenBinder.ScreenBinder();
            #endregion
            oScreenBinder.DQ_AddExtraFieldEntry(this.m_ExtraFieldsDS, i_TableName, i_ColumnName, i_ColumnType, i_Value);
        }
        #endregion
        #region DQ_AddExtraFields
        public void DQ_AddExtraFields(string i_TableName, object i_Instance)
        {
            #region Declaration And Initialization Section.
            DQTools.cTools cTools = new DQTools.cTools();
            PropertyInfo[] oPropertyInfo_List = null;
            #endregion
            #region Body Section.
            oPropertyInfo_List = i_Instance.GetType().GetProperties();
            Array.Sort(oPropertyInfo_List, new DeclarationOrderComparator());
            if (oPropertyInfo_List != null)
            {
                foreach (var oPropertyInfo in oPropertyInfo_List)
                {
                    string str_Type = string.Empty;
                    if ((oPropertyInfo.PropertyType.FullName.Contains("Nullable") && oPropertyInfo.PropertyType.FullName.Contains("Int32")))
                    {
                         str_Type = "Int32"; 
                    }else
                    {
                         str_Type = oPropertyInfo.PropertyType.Name;
                    }
                    DQ_AddExtraField
                        (
                            i_TableName,
                            oPropertyInfo.Name,
                            str_Type,
                            cTools.GetPropValue(i_Instance,oPropertyInfo.Name) == null ? string.Empty :  cTools.GetPropValue(i_Instance,oPropertyInfo.Name).ToString()
                        );
                }
            }
            #endregion
        }
        #endregion
        #region DQ_AddExtraFields_02
        public void DQ_AddExtraField<T>(string i_TableName, string i_ColumnName, T i_Value)
        {
            #region Declaration And Initialization Section.
            DQScreenBinder.ScreenBinder oScreenBinder = new DQScreenBinder.ScreenBinder();
            #endregion
            oScreenBinder.DQ_AddExtraFieldEntry<T>(this.m_ExtraFieldsDS, i_TableName, i_ColumnName, i_Value);
        }
        #endregion
        #region DQ_AddExtraFields_03
        public void DQ_AddExtraField<T>(string i_TableName, string i_ColumnName)
        {
            #region Declaration And Initialization Section.
            DQScreenBinder.ScreenBinder oScreenBinder = new DQScreenBinder.ScreenBinder();
            #endregion
            oScreenBinder.DQ_AddExtraFieldEntry<T>(this.m_ExtraFieldsDS, i_TableName, i_ColumnName);
        }
        #endregion
        #region DQ_PrepareControlsToBind
        private List<Control> DQ_PrepareControlsToBind()
        {
            #region Declaration And Initialization Section.
            List<Control> oList = null;
            List<Control> oIntermediate_List = null;
            DQScreenBinder.ScreenBinder oScreenBinder = new DQScreenBinder.ScreenBinder();
            #endregion
            #region Body Section.
            switch (this.DQ_BindLevel)
            {
                case Enum_BIND_LEVEL.ALL:
                    oList = null; // In this case, the DQScreenBinder.DQ_IsControlToBind always returns true.
                    break;
                case Enum_BIND_LEVEL.NONE:
                    oList = new List<Control>(); // In this case the list of controls is empty.
                    break;
                case Enum_BIND_LEVEL.FIELDS:
                    oList = this.DQ_ControlsToBind;
                    break;
                case Enum_BIND_LEVEL.SECTIONS:
                    oList = new List<Control>();
                    oIntermediate_List = new List<Control>();
                    foreach (String str_SectionName in this.DQ_SectionsToBind)
                    {
                        oScreenBinder.DQ_FilerControls_By_PrivateDataValue(this.Page, "DQ_SectionName", str_SectionName, oIntermediate_List, ref oList);
                    }
                    break;
                //case Enum_BIND_LEVEL.TABLES:
                //         oList              = new List<Control>();
                //         oIntermediate_List = new List<Control>();
                //         foreach(String str_TableName in this.DQ_TablesToBind)
                //         {
                //             oScreenBinder.DQ_FilerControls_By_PrivateDataValue(this.Page, "DQ_TableName", str_TableName, oIntermediate_List, ref oList);
                //         }
                //         break;
                //case Enum_BIND_LEVEL.TABLES_AND_FIELDS:
                //         oList = null;
                //         break;
                default:
                    oList = this.DQ_ControlsToBind;
                    break;

            }
            #endregion
            #region Return Section.
            return oList;
            #endregion
        }
        #endregion
        #region DQ_HandleEvent.
        private void DQ_HandleEvent(ref DQ_PrePostHandler i_DQ_PrePostHandler)
        {
            DQ_ClearEventSubscriptions(ref i_DQ_PrePostHandler);
        }
        #endregion
        #region DQ_ClearEventDelegates
        private void DQ_ClearEventSubscriptions(ref DQ_PrePostHandler i_DQ_PrePostHandler)
        {
            i_DQ_PrePostHandler = (DQ_PrePostHandler)Delegate.Remove((Delegate)i_DQ_PrePostHandler, (Delegate)i_DQ_PrePostHandler);
        }
        #endregion
        #region DQ_ResetGlobalDS
        private void DQ_ResetGlobalDS(ref DataSet i_DataSet)
        {
            #region Declaration And Initialization Section.
            DQScreenBinder.ScreenBinder oScreenBinder = new DQScreenBinder.ScreenBinder();
            DataSet oDS = new DataSet();
            #endregion
            #region Body Section.
            oScreenBinder.DQ_AddDTErrors(oDS);
            i_DataSet = oDS;
            #endregion

        }
        #endregion
        #region DQ_ResetExtraFieldsDS
        public void DQ_ResetExtraFieldsDS()
        {
            #region Declaration And Initialization Section.
            DQScreenBinder.ScreenBinder oScreenBinder = new DQScreenBinder.ScreenBinder();
            #endregion
            #region Body Section.
            oScreenBinder.DQ_ResetExtraFieldsDS(this.m_ExtraFieldsDS);
            #endregion
        }
        #endregion
        #region DQ_SetMasterPage.
        public void DQ_SetMasterPage()
        {
            string str_DefaultMasterPageInConfigFile = string.Empty;
            if (!String.IsNullOrEmpty(this.DQ_MasterPage))
            {
                this.MasterPageFile = this.DQ_MasterPage;
            }
            else
            {
                str_DefaultMasterPageInConfigFile = ConfigurationManager.AppSettings["DQ_DefaultMasterPage"];
                this.MasterPageFile = str_DefaultMasterPageInConfigFile;
            }
        }
        #endregion
        #region DQ_SetTheme
        private void DQ_SetTheme(HttpContext i__HttpContext)
        {
            if (i__HttpContext.Session["DQCULTURE"] != null)
            {
                DQ_SetThemePerCulture(i__HttpContext.Session["DQCULTURE"].ToString());
            }
            else
            {
                DQ_SetThemePerCulture(string.Empty);
            }
        }
        #endregion
        #region DQ_SetThemePerCulture
        protected void DQ_SetThemePerCulture(string i_Culture)
        {
            #region Declaration And Initialization Section.
            string str_DefaultThemeInConfigFile = ConfigurationManager.AppSettings["DQ_DefaultTheme"];
            #endregion
            #region Body Section.
            if (!string.IsNullOrEmpty(str_DefaultThemeInConfigFile))
            {
                if (!String.IsNullOrEmpty(i_Culture))
                {
                    this.Theme = string.Format("{0}_{1}", str_DefaultThemeInConfigFile, i_Culture);
                }
                else
                {
                    this.Theme = string.Format("{0}_EN", str_DefaultThemeInConfigFile);
                }
            }
            #endregion
        }
        #endregion
        #region DQ_Get_JavaScript_Files_To_Register
        private void DQ_Get_JavaScript_Files_To_Register()
        {
            #region Declaration And Initialization Section.
            int i_counter = 1;
            bool isExists = true;
            string str_CurrentFileName = string.Empty;
            Dictionary<string, string> dic_JSFiles = new Dictionary<string, string>();
            #endregion
            #region Populate the Dictionary.
            while (isExists)
            {
                str_CurrentFileName = ConfigurationManager.AppSettings[String.Format("DQ_JS_{0}", i_counter.ToString())];
                if (!String.IsNullOrEmpty(str_CurrentFileName))
                {
                    dic_JSFiles.Add(String.Format("DQ_JS_{0}", i_counter.ToString()), str_CurrentFileName);
                    i_counter++;
                }
                else
                {
                    isExists = false;
                }
            }
            #endregion
            #region Register Scripts.
            DQ_RegisterJSScripts(dic_JSFiles);
            #endregion
        }
        #endregion
        #region DQ_RegisterJSScripts
        private void DQ_RegisterJSScripts(Dictionary<string, string> i_JSScripts)
        {
            foreach (KeyValuePair<string, string> oKeyPair in i_JSScripts)
            {
                this.Page.ClientScript.RegisterClientScriptInclude(this.GetType(), oKeyPair.Key, this.Page.ResolveClientUrl(oKeyPair.Value));
            }
        }
        #endregion
        #region DQ_ScrambleQueryString
        public string DQ_ScrambleQueryString(string i__Input)
        {
            #region Declaration and Initialization Section.
            DQCrypto.DQCrypto oDQCrypto = new DQCrypto.DQCrypto();
            string str_Return = string.Empty;
            #endregion

            #region Body Section.
            str_Return = oDQCrypto.DQ_ScrambleQueryString(i__Input);
            #endregion

            #region Return Section.
            return str_Return;
            #endregion
        }
        #endregion
        #region DQ_DescrambleQueryString
        public string DQ_DescrambleQueryString(string i__Input)
        {
            #region Declaration and Initialization Section.
            DQCrypto.DQCrypto oDQCrypto = new DQCrypto.DQCrypto();
            string str_Return = string.Empty;
            #endregion

            #region Body Section.
            str_Return = oDQCrypto.DQ_DescrambleQueryString(i__Input);
            #endregion

            #region Return Section.
            return str_Return;
            #endregion
        }
        #endregion
        #region DQ_ParseQueryString
        public NameValueCollection DQ_ParseQueryString(string i__Input)
        {
            #region Declaration and Initialization Section.
            DQTools.cTools oTools = new DQTools.cTools();
            NameValueCollection oNameValueCollection = new NameValueCollection();
            #endregion

            #region Body Section.
            oNameValueCollection = oTools.DQ_ParseQueryString(i__Input);
            #endregion

            #region Return Section.
            return oNameValueCollection;
            #endregion
        }
        #endregion
        #region OnPreInit
        protected override void OnPreInit(EventArgs e)
        {
            #region DQ_SetMasterPage
            DQ_SetMasterPage();
            #endregion
            #region DQ_SetUICulture
            DQ_SetUICulture(this.Context);
            #endregion
            #region Set Default Theme.
            DQ_SetTheme(this.Context);
             #endregion
            base.OnPreInit(e);
        }
        #endregion
        #region onInit
        protected override void OnInit(EventArgs e)
        {
            #region Declaration And Initialization Section.
            string str_RedirectToLoginUrl = string.Empty;
            #endregion
            #region Body Section.
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.AppendHeader("Refresh", (Convert.ToString(Session.Timeout * 60 + 10) + "; URL=") + ResolveUrl("~/RedirectToLogin.htm") + "?msg=Session expired. Please sign in.");
            base.OnInit(e);
            #endregion            
        }
        #endregion
        #region onLoad
        protected override void OnLoad(EventArgs e)
        {
            #region Declaration and Initialization Section.
            DQSessionManager.DQSessionManager oDQSessionManager = new DQSessionManager.DQSessionManager();
            string str_SiteMapSequence_IsApplied = ConfigurationManager.AppSettings["SiteMapSequence_IsApplied"];
            string str_LoginRedirection_IsApplied = ConfigurationManager.AppSettings["LoginRedirection_IsApplied"];
            string str_SessionVariables_IsApplied = ConfigurationManager.AppSettings["SessionVariables_IsApplied"];
            #endregion

            #region RegisterJSMsgByCulture
            RegisterJSMsgByCulture();
            #endregion

            #region Register Required JS Files.
            DQ_Get_JavaScript_Files_To_Register();
            #endregion

            #region Check IsLoggedIn Session Variable.
            DQLogger.DQLogger oDQLogger = new DQLogger.DQLogger();
            Exception oException = null;

            //oException = new Exception(string.Format("IsLoginRedirectionEnabled ={0}", IsLoginRedirectionEnabled.ToString()));       
            //oDQLogger.DQLogException(oException);

            if (IsLoginRedirectionEnabled == true)
            {
                //oException = new Exception(string.Format("str_LoginRedirection_IsApplied ={0}", str_LoginRedirection_IsApplied));
                //oDQLogger.DQLogException(oException);

                if (string.IsNullOrEmpty(str_LoginRedirection_IsApplied))
                {
                    str_LoginRedirection_IsApplied = "0";
                }

                if (str_LoginRedirection_IsApplied == "1")
                {
                    //oException = new Exception("Before oDQSessionManager.DQ_RedirectToLogin(this.Context);");
                    //oDQLogger.DQLogException(oException);

                    oDQSessionManager.DQ_RedirectToLogin(this.Context);

                    //oException = new Exception("After oDQSessionManager.DQ_RedirectToLogin(this.Context);");
                    //oDQLogger.DQLogException(oException);
                }
            }
            #endregion

            #region DQ_CheckDBSessionExistence
            DQ_CheckDBSessionExistence();
            #endregion
            
            #region DQ_SetSiteMapUrls
            if (string.IsNullOrEmpty(str_SiteMapSequence_IsApplied))
            {
                str_SiteMapSequence_IsApplied = "0";
            }

            if (str_SiteMapSequence_IsApplied == "1")
            {
                if (DQ_CheckSiteMapSequence() == true)
                {
                    DQ_SetSiteMapUrls();
                }
                else
                {
                    throw new Exception("Not Allowed Sequence of Pages!");
                }
            }
            #endregion

            #region DQ_AddAjaxFeature
            DQ_AddAjaxFeature();
            #endregion

            #region DQ_AddRequiredJSVariables
            DQ_AddRequiredJSVariables();
            #endregion

            #region DQ_SetSessionTimeOut
            DQ_SetSessionTimeOut();
            #endregion

            #region DQ_RedirectByEnvironment
            DQ_RedirectByEnvironment();
            #endregion 
            
            #region DQ_HandShake_Via_DOOPERATION
            //DQ_HandShake_Via_DOOPERATION();
            #endregion

            #region DQ_Set_SessionVariables
            //if (string.IsNullOrEmpty(str_SessionVariables_IsApplied))
            //{
            //    str_SessionVariables_IsApplied = "0";
            //}

            //if (str_SessionVariables_IsApplied == "1")
            //{
            //    oDQSessionManager.DQ_Set_SessionVariables(this.Context);
            //}
            #endregion

            #region DQ_Set_Role
            DQ_Set_Role();
            #endregion

            base.OnLoad(e);
        }
        #endregion
        #region DQ_RedirectByEnvironment
        private void DQ_RedirectByEnvironment()
        {
            #region Declaration And Initialization Section.
            string str_CompanyName = string.Empty;
            string str_Path = string.Empty;
            string str_PageName = string.Empty;
            string str_RedirectTo = string.Empty;
            string str_Querystring = string.Empty;
            FileInfo oFileInfo = null;
            #endregion
            #region Body Section.
           
            str_Path        = System.Web.HttpContext.Current.Request.Url.AbsolutePath;
            oFileInfo       = new FileInfo(str_Path);
            str_PageName    = oFileInfo.Name;
            str_Querystring = System.Web.HttpContext.Current.Request.QueryString.ToString();
            str_CompanyName = DQ_GetConfigEntry("CompanyName");
            str_RedirectTo  = this.AppRelativeTemplateSourceDirectory + str_CompanyName + "/" + str_PageName;

            oFileInfo = new FileInfo(Server.MapPath(str_RedirectTo));
            if ((oFileInfo.Exists))
            {
                if (!(string.IsNullOrEmpty(str_Querystring)))
                {
                    str_RedirectTo = str_RedirectTo + "?" + str_Querystring;
                }              

                Response.Redirect(str_RedirectTo);
                            
            }
            #endregion
            
        }
        #endregion
        #region DQ_Set_Role
        private void DQ_Set_Role()
        {
            #region Declaration And Initialization Section.
            string str_Querystring_Scrambled = string.Empty;
            string str_Plain_Query_string = string.Empty;
            NameValueCollection oCollection = new NameValueCollection();
            string str_SessionVariables_IsApplied = ConfigurationManager.AppSettings["SessionVariables_IsApplied"];
            DQSessionManager.DQSessionManager oDQSessionManager = new DQSessionManager.DQSessionManager();
            DQLogger.DQLogger oDQLogger = new DQLogger.DQLogger();
            #endregion
            #region Body Section.
            str_Querystring_Scrambled = HttpContext.Current.Request.QueryString.ToString();
            if (!string.IsNullOrEmpty(str_Querystring_Scrambled))
            {
                try
                {
                    str_Plain_Query_string = DQ_DescrambleQueryString(str_Querystring_Scrambled.Replace("Criteria=",""));
                    if (!string.IsNullOrEmpty(str_Plain_Query_string))
                    {
                        oCollection = DQ_ParseQueryString(str_Plain_Query_string);
                        if (oCollection != null)
                        {
                            if (oCollection["RoleID"] != null)
                            {
                                if (HttpContext.Current.Session["DQ_ROLEID"].ToString() != oCollection["RoleID"])
                                {
                                    // ----------------
                                    HttpContext.Current.Session["DQ_ROLEID"] = oCollection["RoleID"];
                                    // ----------------


                                    // User Ident is now filled by CheckRoles Task.
                                    // ----------------
                                    //HttpContext.Current.Session["DQUserIdent"] = null;
                                    // ----------------

                                    // ----------------
                                    DQ_AddParam("TASK_NAME", "SetRoles");
                                    DQ_AddParam("SessionID", Session.SessionID, "Q");

                                    DQ_AddExtraField("VARIABLES", "ATTRIBUTE", "System.String");
                                    DQ_AddExtraField("VARIABLES", "STR_VALUE", "System.String");
                                    DQ_AddExtraField("VARIABLES", "VAL_FORMAT", "System.String");
                                    DQ_AddExtraField("VARIABLES", "VAR_TYPE", "System.String");

                                    DQ_DoOperation();
                                    // ----------------


                                    // ----------------
                                    if (m_GlobalDS.Tables["NOTIFICATION"].Rows.Count > 0)
                                    {
                                        // -------------
                                        StringBuilder oSB = new StringBuilder();
                                        oSB.AppendLine("<table>");
                                        foreach (DataRow oDR in m_GlobalDS.Tables["NOTIFICATION"].Rows)
                                        {
                                            oSB.AppendLine(string.Format("<tr><td>{0}</td></tr>", oDR["NOTIFICATION_DESC"].ToString()));
                                        }
                                        oSB.AppendLine("</table>");
                                        HttpContext.Current.Session["DQ_ErrorMessage"] = oSB.ToString();
                                        // -------------

                                        this.Server.Transfer("~/Exception/DQException.aspx");
                                       

                                        // -------------
                                        this.Session["DQ_ErrorMessage"] = oSB.ToString();
                                        Exception oException = new Exception(oSB.ToString());
                                        oDQLogger.DQLogException(oException);
                                        DQ_BusinessErrorNotification();
                                        // -------------
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }
            #endregion
        }
        #endregion
        #region DQ_GetNotificationMessages
        public string DQ_GetNotificationMessages(Int32 i__Type)
        {
            #region Declaration and Initialization Section.
            StringBuilder oSBNotificationMessage = new StringBuilder();
            string        str_ReturnValue        = string.Empty;
            DataTable     oDTErrors              = null;
            string        str_Temp               = string.Empty;
            #endregion

            #region Body Section.
            if (this.DQ_GlobalDS != null)
            {
                if (this.DQ_GlobalDS.Tables.Contains("NOTIFICATION"))
                {
                    oDTErrors = this.DQ_GlobalDS.Tables["NOTIFICATION"];
                    foreach (DataRow oDR in oDTErrors.Rows)
                    {
                        if (oDR["NOTIFICATION_DESC"] != null)
                        {
                            if (oDR["NOTIFICATION_TYPE"] != null)
                            {
                                if (Convert.ToInt32(oDR["NOTIFICATION_TYPE"].ToString()) == i__Type)
                                {
                                    str_Temp = oDR["NOTIFICATION_DESC"].ToString() + "<br>";
                                    str_Temp = str_Temp.Replace("\r", "");
                                    str_Temp = str_Temp.Replace("\n", "");
                                    oSBNotificationMessage.AppendLine(str_Temp);
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            #region Return Section.
            if (oSBNotificationMessage != null)
            {
                str_ReturnValue = oSBNotificationMessage.ToString();
            }
            return str_ReturnValue;
            #endregion
        }
         #endregion
        #region DQ_GetBusinessErrorMessage
        public string DQ_GetBusinessErrorMessage()
        {
            #region Declaration and Initialization Section.
            string str_ReturnValue = string.Empty;
            #endregion

            #region Body Section.
            str_ReturnValue = DQ_GetNotificationMessages(0);
            #endregion

            #region Return Section. 
            return str_ReturnValue;
            #endregion
        }
        #endregion
        #region DQ_GetBusinessFlashMessage
        public string DQ_GetBusinessFlashMessage()
        {
            #region Declaration and Initialization Section.
            string str_ReturnValue = string.Empty;
            #endregion

            #region Body Section.
            str_ReturnValue = DQ_GetNotificationMessages(-2);
            #endregion

            #region Return Section.
            return str_ReturnValue;
            #endregion
        }
         #endregion
        #region DQ_GetTechnicalErrorMessage
        public string DQ_GetTechnicalErrorMessage()
        {
            #region Declaration and Initialization Section.
            string str_ReturnValue = string.Empty;
            #endregion

            #region Body Section.
            str_ReturnValue = DQ_GetNotificationMessages(-1);
            #endregion

            #region Return Section.
            return str_ReturnValue;
            #endregion
        }
         #endregion
        #region DQ_GetBusinessNotificationMessage
        public string DQ_GetBusinessNotificationMessage()
        {
            #region Declaration and Initialization Section.
            string str_ReturnValue = string.Empty;
            #endregion

            #region Body Section.
            str_ReturnValue = DQ_GetNotificationMessages(1);
            #endregion

            #region Return Section.
            return str_ReturnValue;
            #endregion
        }
        #endregion
        #region DQ_HandleDTErrors
        private void DQ_HandleDTErrors()
        {
            #region Declaration and Initialization Section.
            DataTable oDT_Errors = null;
            StringBuilder oSB = new StringBuilder();
            Boolean IsErrorExists = false;
            DQLogger.DQLogger oDQLogger = new DQLogger.DQLogger();
            #endregion
            #region Body Section.
            if (this.DQ_GlobalDS.Tables.Contains("NOTIFICATION"))
            {
                // Get Errors Data Table
                // ---------------
                oDT_Errors = this.DQ_GlobalDS.Tables["NOTIFICATION"];
                // ---------------

                // Remove entries with  = 999999
                // ---------------
                if (oDT_Errors.Rows != null)
                {
                    for (int i = oDT_Errors.Rows.Count - 1; i >= 0; i--)
                    {
                        DataRow dr = oDT_Errors.Rows[i];
                        if (dr["NOTIFICATION_TYPE"].ToString() == "999999")
                            dr.Delete();
                    }
                }
                oDT_Errors.AcceptChanges();
                // ---------------

                // Check if error exists
                // ---------------
                if (oDT_Errors.Rows.Count > 0)
                {
                    if (this.Context.Session != null)
                    {
                        this.Context.Session["DQ_NOTIFICATION"] = oDT_Errors;
                    }
                    IsErrorExists = true;
                }
                // ---------------


                // Prepare Message
                // ---------------
                oSB.AppendLine("<table>");
                foreach (DataRow oDR in oDT_Errors.Rows)
                {
                    oSB.AppendLine(string.Format("<tr><td>{0}</td></tr>", oDR["NOTIFICATION_DESC"].ToString()));
                }
                oSB.AppendLine("</table>");
                // ---------------
            }

            // In case of error existence
            // Log it [DQLogException]
            // Fire DQ_BusinessErrorNotification
            // ---------------
            if (IsErrorExists == true)
            {                
                this.Session["DQ_ErrorMessage"] = oSB.ToString();
                Exception oException = new Exception(oSB.ToString());                
                DQ_BusinessErrorNotification();
                try
                {
                    oDQLogger.DQLogException(oException);
                }
                catch (Exception ex02)
                {
                    Console.WriteLine(ex02.Message);
                }
                if (Is_Transitory_Page == true)
                {
                    this.Server.Transfer("~/Exception/DQException.aspx");
                }
            }
            // ---------------

            #endregion

        }
        #endregion
        #region DQ_HandleDTErrors_2
        private void DQ_HandleDTErrors_2()
        {
            #region Declaration and Initialization Section.
            DataTable oDT_Errors = null;
            StringBuilder oSB = new StringBuilder();
            Boolean IsErrorExists = false;
            DQLogger.DQLogger oDQLogger = new DQLogger.DQLogger();
            #endregion
            #region Body Section.
            if (DQ_GlobalDS_NewSession.Tables.Contains("NOTIFICATION"))
            {
                // Get Errors Data Table
                // ---------------
                oDT_Errors = DQ_GlobalDS_NewSession.Tables["NOTIFICATION"];
                // ---------------

                // Remove entries with  = 999999
                // ---------------
                if (oDT_Errors.Rows != null)
                {
                    for (int i = oDT_Errors.Rows.Count - 1; i >= 0; i--)
                    {
                        DataRow dr = oDT_Errors.Rows[i];
                        if (dr["NOTIFICATION_TYPE"].ToString() == "999999")
                            dr.Delete();
                    }
                }
                oDT_Errors.AcceptChanges();
                // ---------------

                // Check if error exists
                // ---------------
                if (oDT_Errors.Rows.Count > 0)
                {
                    this.Context.Session["DQ_NOTIFICATION"] = oDT_Errors;
                    IsErrorExists = true;
                }
                // ---------------


                // Prepare Message
                // ---------------
                oSB.AppendLine("<table>");
                foreach (DataRow oDR in oDT_Errors.Rows)
                {
                    oSB.AppendLine(string.Format("<tr><td>{0}</td></tr>", oDR["NOTIFICATION_DESC"].ToString()));
                }
                oSB.AppendLine("</table>");
                // ---------------
            }

            // In case of error existence
            // Log it [DQLogException]
            // Fire DQ_BusinessErrorNotification
            // ---------------
            if (IsErrorExists == true)
            {
                this.Session["DQ_ErrorMessage"] = oSB.ToString();
                Exception oException = new Exception(oSB.ToString());
                DQ_BusinessErrorNotification();
                try
                {
                    oDQLogger.DQLogException(oException);
                }
                catch (Exception ex02)
                {
                    Console.WriteLine(ex02.Message);
                }
                //this.Server.Transfer("~/Exception/DQException.aspx");
            }
            // ---------------

            #endregion

        }
        #endregion
        #region DQ_CheckSiteMapSequence
        protected bool DQ_CheckSiteMapSequence()
        {
            string strPageURL = this.Request.Url.PathAndQuery;
            string strParentNodeUrl;

            if ((strPageURL.IndexOf("/Login/DQLogin.aspx") == -1) && (strPageURL.IndexOf("/Logout/DQLogout.aspx") == -1))
            {
                if ((SiteMap.CurrentNode.ParentNode != null) && (Session["DQ_SiteMapURLs"] != null))
                {
                    strParentNodeUrl = SiteMap.CurrentNode.ParentNode.Url;

                    if (Session["DQ_SiteMapURLs"].ToString().IndexOf(strParentNodeUrl) == -1)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        #endregion
        #region DQ_SetSiteMapUrls
        protected void DQ_SetSiteMapUrls()
        {
            #region Declaration and Initialization Section.
            string strPageURL = string.Empty;
            string strPageName = string.Empty;
            string[] arrSiteMapURLs = null;
            #endregion
            #region Body Section.
            //strPageURL = this.Request.Url.ToString();
            strPageURL = this.Request.Url.PathAndQuery;
            if ((strPageURL.IndexOf("/Login/DQLogin.aspx") == -1) && (strPageURL.IndexOf("/Logout/DQLogout.aspx") == -1))
            {
                //strPageURL = strPageURL.Replace("http://localhost", "");
                //strPageName = strPageURL.Substring(0, strPageURL.LastIndexOf(".aspx") + 5);
                strPageName = this.Request.Url.AbsolutePath;

                if ((Session["DQ_SiteMapURLs"] == null) || (Session["DQ_SiteMapURLs"].ToString().IndexOf(strPageURL) == -1 && Session["DQ_SiteMapURLs"].ToString().IndexOf(strPageName) == -1))
                {
                    Session["DQ_SiteMapURLs"] += strPageURL + "~";
                }
                else if ((Session["DQ_SiteMapURLs"] == null) || (Session["DQ_SiteMapURLs"].ToString().IndexOf(strPageURL) == -1 && Session["DQ_SiteMapURLs"].ToString().IndexOf(strPageName) != -1))
                {
                    arrSiteMapURLs = Session["DQ_SiteMapURLs"].ToString().Split(new Char[] { '~' });
                    for (int j = 0; j <= arrSiteMapURLs.Length - 1; j++)
                    {
                        if (arrSiteMapURLs[j].IndexOf(strPageName) != -1)
                        {
                            arrSiteMapURLs[j] = strPageURL;
                            break;
                        }
                    }
                    Session["DQ_SiteMapURLs"] = string.Join("~", arrSiteMapURLs);
                }
            }
            #endregion

        }
        #endregion
        #region DQ_AddRequiredParameters
        private void DQ_AddRequiredParameters()
        {
            #region Declaration and Initialization Section.
            #endregion
            #region Body Section.

            // ----------------------
            if (Session["DQ_SessionID"] != null)
            {
                this.DQ_AddParam("SessionID", Session["DQ_SessionID"].ToString(),"Q");
            }
            else
            {
                this.DQ_AddParam("SessionID", Session.SessionID,"Q");
            }
            // ----------------------

            // ----------------------
            if (Session["DQ_ROLEID"] != null)
            {
                this.DQ_AddParam("ROLEID", Session["DQ_ROLEID"].ToString(), "Q");
            }            
            // ----------------------

            // ----------------------
            //if (Session["$Service$"] != null)
            //{
            //    this.DQ_AddParam("$Service$", Session["$Service$"].ToString(), "Q");
            //}  
            // ----------------------

            // ----------------------
            //if (Session["$ValMod$"] != null)
            //{
            //    this.DQ_AddParam("$ValMod$", Session["$ValMod$"].ToString(), "Q");
            //}
            // ----------------------

            this.DQ_AddParam("PAGE_MODE", this.DQ_PageMode.ToString());
            #endregion
        }
        #endregion
        #region DQ_GetParameter
        public string DQ_GetParameter(string i__Param_Name)
        {
            #region Declaration and Initialization Section.
            string str__ReturnValue = string.Empty;
            DataRow[] oDataRowCollection = null;
            DataRow oDataRow = null;
            DataTable oDT_Parameters = new DataTable();
            string str_Filer = string.Empty;
            #endregion

            #region Body Section.
            if (!string.IsNullOrEmpty(i__Param_Name))
            {
                oDT_Parameters = this.DQ_GlobalDS.Tables["PARAMETERS"];
                str_Filer = string.Format("PARAM_NAME='{0}'", i__Param_Name);

                if (oDT_Parameters != null)
                {
                    oDataRowCollection = oDT_Parameters.Select(string.Format("PARAM_NAME='{0}'", i__Param_Name));

                    if (oDataRowCollection != null)
                    {
                        if (oDataRowCollection.Length == 1)
                        {
                            oDataRow = oDataRowCollection[0];
                            if (oDataRow["PARAM_VALUE"] != null)
                            {
                                str__ReturnValue = oDataRow["PARAM_VALUE"].ToString();
                            }
                        }
                    }
                }
            }
            #endregion

            #region Return Section.
            return str__ReturnValue;
            #endregion
        }
        #endregion
        #region DQ_ParseClientData
        public NameValueCollection DQ_ParseClientData(string i__Input)
        {
            #region Declaration and Initialization Section.
            DQTools.cTools oTools = new DQTools.cTools();
            NameValueCollection oNameValueCollection = new NameValueCollection();
            #endregion

            #region Body Section.
            oNameValueCollection = oTools.DQ_ParseClientData(i__Input);
            #endregion

            #region Return Section.
            return oNameValueCollection;
            #endregion
        }
        #endregion
        #region DQ_PrepareServerResponse
        public string DQ_PrepareServerResponse(NameValueCollection i__NameValueCollection)
        {
            #region Declaration and Initialization Section.
            string str_ReturnValue = string.Empty;
            DQTools.cTools oTools = new DQTools.cTools();
            #endregion

            #region Body Region.
            str_ReturnValue = oTools.DQ_PrepareServerResponse(i__NameValueCollection);
            #endregion

            #region Return Section.
            return str_ReturnValue;
            #endregion
        }
        #endregion
        #region DQ_BusinessErrorNotification
        private void DQ_BusinessErrorNotification()
        {
            #region Declaration and Initialization Section.
            StringBuilder       oSB                  = new StringBuilder();
            ClientScriptManager oClientScriptManager = this.Page.ClientScript;
            string              str_SITE_PATH        = ConfigurationManager.AppSettings["SITE_PATH"];
            string              str_ExceptionPageUrl = string.Empty;
            string str_BusinessErrorMessage = string.Empty;
            string str_BusinessNotificationMessage = string.Empty;
            string str_BusinessFlashMessage = string.Empty;
            string str_TechnicalErrorMessage = string.Empty;

            #endregion
            #region Body Section.

            // ---------------
            str_BusinessErrorMessage = this.DQ_GetBusinessErrorMessage();
            str_BusinessNotificationMessage = this.DQ_GetBusinessNotificationMessage();
            str_BusinessFlashMessage = this.DQ_GetBusinessFlashMessage();
            str_TechnicalErrorMessage = this.DQ_GetTechnicalErrorMessage();
            // ---------------

            if (!string.IsNullOrEmpty(str_BusinessErrorMessage))
            {
                oSB.AppendLine("js_ShowBusinessError('" + CleanErrorMsgContent(this.DQ_GetBusinessErrorMessage()) + "');");
            }

            if (!string.IsNullOrEmpty(str_BusinessNotificationMessage))
            {
                oSB.AppendLine("js_ShowBusinessNotification('" + CleanErrorMsgContent(this.DQ_GetBusinessNotificationMessage()) + "');");
            }

            if (!string.IsNullOrEmpty(str_BusinessFlashMessage))
            {
                oSB.AppendLine("js_ShowBusinessFlashMessage('" + CleanErrorMsgContent(this.DQ_GetBusinessFlashMessage()) + "');");
            }

            if (!string.IsNullOrEmpty(str_SITE_PATH))
            {
                str_ExceptionPageUrl = str_SITE_PATH + "/Exception/DQException.aspx";
                if (!string.IsNullOrEmpty(str_ExceptionPageUrl))
                {
                    if (!string.IsNullOrEmpty(CleanErrorMsgContent(str_TechnicalErrorMessage)))
                    {
                        // --------------
                        Session["DQ_BusinessMessage"] = "";
                        if (!string.IsNullOrEmpty(str_BusinessErrorMessage))
                        {
                            Session["DQ_BusinessMessage"] = Session["DQ_BusinessMessage"] + str_BusinessErrorMessage;
                        }
                        if (!string.IsNullOrEmpty(str_BusinessNotificationMessage))
                        {
                            Session["DQ_BusinessMessage"] = Session["DQ_BusinessMessage"] + str_BusinessNotificationMessage;
                        }
                        if (!string.IsNullOrEmpty(str_BusinessFlashMessage))
                        {
                            Session["DQ_BusinessMessage"] = Session["DQ_BusinessMessage"] + str_BusinessFlashMessage;
                        }
                        // --------------

                        //oSB.AppendLine("js_HandleTechnicalErrors('" + str_ExceptionPageUrl + "');");
                        try
                        {
                            //this.Server.Transfer("~/Exception/DQException.aspx");
                            Response.Redirect("~/Exception/DQException.aspx");
                        }
                        catch (ThreadAbortException ex)
                        {

                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(oSB.ToString()))
            {
                if (!oClientScriptManager.IsStartupScriptRegistered("BusinessErrorNotification"))
                {
                    oClientScriptManager.RegisterStartupScript(this.GetType(), "BusinessErrorNotification", oSB.ToString(), true);                    
                }
            }
            #endregion
        }
        #endregion
        #region DQ_AddAjaxFeature
        private void DQ_AddAjaxFeature()
        {
            if (!(this.DesignMode))
            {
                if ((this.DQ_Ajax_Enabled))
                {
                    //-----------------------------------------------------------
                    if ((this.DQ_Ajax_onRequest_Enabled))
                    {
                        if ((!string.IsNullOrEmpty(this.DQ_Ajax_onRequest_Sender)) & (!string.IsNullOrEmpty(this.DQ_Ajax_onRequest_Receiver)))
                        {
                            Register_JS_CallBack_Handler("onRequest", this.DQ_Ajax_onRequest_Sender, this.DQ_Ajax_onRequest_Receiver, this.DQ_Ajax_onRequest_ErrorHandler, this.DQ_Ajax_onRequest_HandleLengthyProcess, this.DQ_Ajax_onRequest_NoSenderValue);
                        }
                    }
                    //-----------------------------------------------------------
                }
            }
        }
        #endregion
        #region DQ_FillCodesDropDownList
        public void DQ_FillCodesDropDownList(Control i_Page, DataTable i_DataTable, SortedList<string, string> i_SortedList)
        {
            #region Declaration and Initialization Section.
            DQTools.cTools oTools = new DQTools.cTools();
            #endregion

            #region Body Section.
            oTools.DQ_FillCodesDropDownList(i_Page, i_DataTable, i_SortedList);
            #endregion
        }
        #endregion
        #region DQ_GetInnerHTMLContent
        public string DQ_GetInnerHTMLContent(WebControl i__Control)
        {
            #region Declaration and Initialization Section.
            DQTools.cTools oTools = new DQTools.cTools();
            string str_ReturnValue = string.Empty;
            #endregion

            #region Body Section.
            str_ReturnValue = oTools.DQ_GetInnerHTMLContent(i__Control);
            #endregion

            #region Return Section.
            return str_ReturnValue;
            #endregion
        }
        #endregion
        #region DQ_AddRequiredJSVariables
        protected void DQ_AddRequiredJSVariables()
        {
            #region Declaration and Initialization Section.
            StringBuilder oSB = new StringBuilder();
            ClientScriptManager oClientScriptManager = this.Page.ClientScript;
            #endregion

            #region Body Section.
            oSB.AppendLine(string.Format("var js_Global_AppRoot = '{0}';", this.DQ_AppRoot));
            if (!oClientScriptManager.IsStartupScriptRegistered("js_Global_Variables"))
            {
                oClientScriptManager.RegisterStartupScript(this.GetType(), "js_Global_Variables", oSB.ToString(), true);
            }
            #endregion
        }
        #endregion
        #region DQ_GetFieldValue
        public static T DQ_GetFieldValue<T>(DataSet i__DS, string i__TableName, String i__ColumnName, Int32 i__RowNbr)
        {
            #region Declaration and Initialization Section.
            T oReturnValue = default(T);
            DQTools.cTools oTools = new DQTools.cTools();
            #endregion

            #region Body Section.
            oReturnValue = oTools.DQ_GetFieldValue<T>(i__DS, i__TableName, i__ColumnName, i__RowNbr);
            #endregion

            #region Return Section.
            return oReturnValue;
            #endregion
        }
        #endregion
        #region DQ_GetFieldValue
        public static T DQ_GetFieldValue<T>(DataTable i__DT, String i__ColumnName, Int32 i__RowNbr)
        {
            #region Declaration and Initialization Section.
            T oReturnValue = default(T);
            DQTools.cTools oTools = new DQTools.cTools();
            #endregion

            #region Body Section.
            oReturnValue = oTools.DQ_GetFieldValue<T>(i__DT, i__ColumnName, i__RowNbr);
            #endregion

            #region Return Section.
            return oReturnValue;
            #endregion
        }
        #endregion
        #region DQ_SetFieldValue
        public static void DQ_SetFieldValue<T>(DataSet i__DS, string i__TableName, String i__ColumnName, Int32 i__RowNbr, T i__Value)
        {
            #region Declaraiton and Initialization Section.
            DQTools.cTools oTools = new DQTools.cTools();
            #endregion

            #region Body Section.
            oTools.DQ_SetFieldValue<T>(i__DS, i__TableName, i__ColumnName, i__RowNbr, i__Value);
            #endregion
        }
        #endregion
        #region DQ_SetFieldValue
        public static void DQ_SetFieldValue<T>(DataTable i__DT, String i__ColumnName, Int32 i__RowNbr, T i__Value)
        {
            #region Declaraiton and Initialization Section.
            DQTools.cTools oTools = new DQTools.cTools();
            #endregion

            #region Body Section.
            oTools.DQ_SetFieldValue<T>(i__DT, i__ColumnName, i__RowNbr, i__Value);
            #endregion
        }
        #endregion
        #region DQ_GetFieldValueByFilter
        public static T DQ_GetFieldValueByFilter<T>(DataSet i__DS, string i__TableName, String i__ColumnName, string i__FilterExpression)
        {
            #region Declaration and Initialization Section.
            T oReturnValue = default(T);
            DQTools.cTools oTools = new DQTools.cTools();
            #endregion

            #region Body Section.
            oReturnValue = oTools.DQ_GetFieldValueByFilter<T>(i__DS, i__TableName, i__ColumnName, i__FilterExpression);
            #endregion

            #region Return Section.
            return oReturnValue;
            #endregion
        }
        #endregion
        #region DQ_CheckDBSessionExistence
        private void DQ_CheckDBSessionExistence()
        {
            #region Declaration and Initialization Section.
            string str_DBSession_CheckExistence = string.Empty;
            string str_DBSessionNonExistence_Url = string.Empty;
            DQSessionManager.DQSessionManager oSessionManager = new DQSessionManager.DQSessionManager();
            Boolean b_IsSessionExists = false;
            #endregion

            #region Body Section.
            if (ConfigurationManager.AppSettings["DBSession_CheckExistence"] != null)
            {
                str_DBSession_CheckExistence = ConfigurationManager.AppSettings["DBSession_CheckExistence"].ToString();
            }
            if (ConfigurationManager.AppSettings["DBSessionNonExistence_Url"] != null)
            {
                str_DBSessionNonExistence_Url = ConfigurationManager.AppSettings["DBSessionNonExistence_Url"].ToString();
            }

            if ((!string.IsNullOrEmpty(str_DBSession_CheckExistence)) && (!string.IsNullOrEmpty(str_DBSessionNonExistence_Url)))
            {
                if (str_DBSession_CheckExistence == "1")
                {
                    b_IsSessionExists = oSessionManager.DQ_CheckDBSessionExistence(this.Context);
                    if (b_IsSessionExists == false)
                    {
                        this.Response.Redirect(str_DBSessionNonExistence_Url);
                    }
                }
            }
            #endregion
        }
        #endregion
        #region DQ_SetConnection
        public void DQ_SetConnection(string i__Connection)
        {
            this.DQ_AddParam("DQCONNECTION", "AppServer://openEdge:5163/netprogress");
        }
        #endregion
        #region DQ_SetUICulture
        public void DQ_SetUICulture(HttpContext i__HttpContext)
        {
            #region Declaration and Initialization Section.
            string                                  str__Culture        = string.Empty;        
            #endregion
            #region Body Section.
            if (i__HttpContext != null)
            {
                if (i__HttpContext.Session["DQCULTURE"] != null)
                {
                    str__Culture = i__HttpContext.Session["DQCULTURE"].ToString();
                }
                else
                {
                    str__Culture = "en";
                }

                System.Globalization.CultureInfo oCultureInfo = System.Globalization.CultureInfo.CreateSpecificCulture(str__Culture);
                oCultureInfo.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";

                Thread.CurrentThread.CurrentUICulture = oCultureInfo;
            }
            #endregion
        }
        #endregion
        #region DQ_GetInvariantDate
        public DateTime DQ_GetInvariantDate(DateTime i__DateTime)
        {
            #region Declaration And Initialization Section.
            DateTime        oDateTime   = DateTime.Now;
            DQTools.cTools  oTools      = new DQTools.cTools();
            #endregion
            #region Body Section.
            oDateTime = oTools.DQ_GetInvariantDate(i__DateTime);
            #endregion
            #region Return Section.
            return oDateTime;
            #endregion
        }
        #endregion
        #region DQ_GetGlobalValueForKey
        public string DQ_GetGlobalValueForKey(string i__ResourceFile, string i__Key)
        {
            #region Declaration And Initialization Section.
            string str_Value = string.Empty;
            string str_ResourcePath = string.Empty;
            string str_CultureName = string.Empty;
            string str_ApplicationPath = string.Empty;
            #endregion
            #region Body Section.
            if (!String.IsNullOrEmpty(i__ResourceFile))
            {
                str_CultureName = GetCultureName();
                str_ApplicationPath = Request.PhysicalApplicationPath;
                if (str_CultureName == "en")
                {
                    str_ResourcePath = string.Format("{0}{1}{2}.resx", str_ApplicationPath, "App_GlobalResources\\", i__ResourceFile);
                }
                else
                {
                    str_ResourcePath = string.Format("{0}{1}{2}.{3}.resx", str_ApplicationPath, "App_GlobalResources\\", i__ResourceFile, str_CultureName);
                }
                if ((File.Exists(str_ResourcePath)))
                {
                    if ((GetGlobalResourceObject(i__ResourceFile, i__Key) != null))
                    {
                        str_Value = GetGlobalResourceObject(i__ResourceFile, i__Key).ToString();
                    }
                }
            }
            #endregion
            #region Return Section.
            return str_Value;
            #endregion
        }
        #endregion
        #region DQ_GetLocalValueforKey
        public string DQ_GetLocalValueforKey(String i__Key)
        {
            #region Declaration And Initialization Section.
            string str_ReturnValue = string.Empty;
            #endregion
            #region Body Section.
            str_ReturnValue = GetLocalResourceObject(i__Key).ToString();
            #endregion
            #region Return Section.
            return str_ReturnValue;
            #endregion
        }
         #endregion
        #region DQ_GetLocalValueforKey_Deleted
        public string DQ_GetLocalValueforKey_Deleted(String i__Key)
        {
            
            #region Declaration And Initiazation Section.
            ResXResourceReader oResReader;
            IDictionaryEnumerator IDE_Resource;
            string str_Value = string.Empty;
            string str_ResourcePath = string.Empty;
            #endregion
            #region Body Section.

            str_ResourcePath = DQ_GetApp_LocalResources_Path(this.Context);
            if ((File.Exists(str_ResourcePath)))
            {
                oResReader = new ResXResourceReader(str_ResourcePath);
                IDE_Resource = oResReader.GetEnumerator();

                while (IDE_Resource.MoveNext())
                {
                    if ((IDE_Resource.Key.ToString() == i__Key))
                    {
                        str_Value = IDE_Resource.Value.ToString();
                    }
                }
                oResReader.Close();
            }

            #endregion
            #region Return Section.
            return str_Value;
            #endregion
        }
        #endregion
        #region DQ_GetApp_LocalResources_Path
        public string DQ_GetApp_LocalResources_Path(HttpContext i__HttpContext)
        {
            #region Declaration And Initialization Section.
            string str_oldLocalResPath = string.Empty;
            string str_NewLocalPath = string.Empty;
            string str_CutlureName = string.Empty;
            string str_PageName = string.Empty;
            int i_Index = -1;
            string str_PageFolder = string.Empty;
            #endregion
            #region Body Section.
            if (i__HttpContext != null)
            {
                str_oldLocalResPath = i__HttpContext.Request.PhysicalPath;
                str_CutlureName = GetCultureName();
                i_Index = str_oldLocalResPath.LastIndexOf("\\");
                str_PageName = str_oldLocalResPath.Substring(i_Index + 1, str_oldLocalResPath.Length - i_Index - 1);
                str_PageFolder = str_oldLocalResPath.Substring(0, i_Index + 1);
                if (str_CutlureName == "en")
                {
                    str_NewLocalPath = string.Format("{0}{1}{2}.resx", str_PageFolder, "App_LocalResources\\", str_PageName);
                }
                else
                {
                    str_NewLocalPath = string.Format("{0}{1}{2}.{3}.resx", str_PageFolder, "App_LocalResources\\", str_PageName, str_CutlureName);
                }
            }
            #endregion
            #region Return Section.
            return str_NewLocalPath;
            #endregion
        }
        #endregion
        #region GetCultureName
        public string GetCultureName()
        {
            #region Declaration And Initialization Section.
            string str_ReturnValue = string.Empty;
            string str_LanguageName = string.Empty;
            int i_Index = -1;
            #endregion
            #region Body Section.
            str_ReturnValue = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
            i_Index = str_ReturnValue.IndexOf("-");
            if (i_Index >= 0)
            {
                str_ReturnValue = str_ReturnValue.Substring(0, i_Index);
            }
            #endregion
            #region Return Section.
            return str_ReturnValue;
            #endregion
        }
        #endregion
        #region DQ_SetLocalRessourceEntry
        public void DQ_SetLocalRessourceEntry(string i__Key, string i__Value)
        {

            #region Declaration And Initialization Section.
            string str_ResourcePath = string.Empty;
            ResXResourceReader oResxReader;
            IDictionaryEnumerator IDE_Resource;
            ResXResourceWriter oResxWriter;
            bool bl_Found = false;
            #endregion


            #region Body Section.
            str_ResourcePath = DQ_GetApp_LocalResources_Path(this.Context);
            if ((File.Exists(str_ResourcePath)))
            {
                oResxReader = new ResXResourceReader(str_ResourcePath);
                IDE_Resource = oResxReader.GetEnumerator();
                oResxWriter = new ResXResourceWriter(str_ResourcePath);

                while (IDE_Resource.MoveNext())
                {
                    if ((IDE_Resource.Key.ToString() == i__Key))
                    {
                        bl_Found = true;
                        oResxWriter.AddResource(IDE_Resource.Key.ToString(), i__Value);
                    }
                    else
                    {
                        oResxWriter.AddResource(IDE_Resource.Key.ToString(), IDE_Resource.Value);
                    }

                }

                if ((bl_Found == false))
                {
                    oResxWriter.AddResource(i__Key, i__Value);
                }

                oResxWriter.Generate();
                oResxWriter.Close();
                oResxReader.Close();
            }
            #endregion
        }
        #endregion
        #region DQ_SetGlobalRessourceEntry
        public void DQ_SetGlobalRessourceEntry(string i__Key, string i__Value, string i__ResourceFile)
        {

            #region Declaration And Initialization Section.
            string str_ResourcePath = string.Empty;
            ResXResourceReader o_ResxReader;
            IDictionaryEnumerator IDE_Resource;
            ResXResourceWriter o_ResxWriter;
            bool bl_Found = false;
            string str_ApplicationPath = string.Empty;
            string str_CultureName = string.Empty;
            #endregion


            #region Body Section.

            str_ApplicationPath = Request.PhysicalApplicationPath;
            str_CultureName = GetCultureName();
            if (str_CultureName == "en")
            {
                str_ResourcePath = string.Format("{0}{1}{2}.resx", str_ApplicationPath, "App_GlobalResources\\", i__ResourceFile);
            }
            else
            {
                str_ResourcePath = string.Format("{0}{1}{2}.{3}.resx", str_ApplicationPath, "App_GlobalResources\\", i__ResourceFile, str_CultureName);
            }

            if ((File.Exists(str_ResourcePath)))
            {
                o_ResxReader = new ResXResourceReader(str_ResourcePath);
                IDE_Resource = o_ResxReader.GetEnumerator();
                o_ResxWriter = new ResXResourceWriter(str_ResourcePath);

                while (IDE_Resource.MoveNext())
                {
                    if ((IDE_Resource.Key.ToString() == i__Key))
                    {
                        bl_Found = true;
                        o_ResxWriter.AddResource(IDE_Resource.Key.ToString(), i__Value);
                    }
                    else
                    {
                        o_ResxWriter.AddResource(IDE_Resource.Key.ToString(), IDE_Resource.Value);
                    }

                }

                if ((bl_Found == false))
                {
                    o_ResxWriter.AddResource(i__Key, i__Value);
                }

                o_ResxWriter.Generate();
                o_ResxWriter.Close();
                o_ResxReader.Close();

            }
            #endregion
        }
        #endregion
        #region RegisterJsMsgByCulture
        protected void RegisterJSMsgByCulture()
        {
            #region Declaration And Initialization Section.
            ClientScriptManager oClientScriptManager                = this.Page.ClientScript;
            string              str_Script                          = string.Empty;
            string              str_CultureName                     = GetCultureName();
            string              str_JS_GLOBALIZATION_SUPPORTED      = ConfigurationManager.AppSettings["JS_GLOBALIZATION_SUPPORTED"];
            #endregion
            #region Body Section.
            str_Script = "<?xml version=\"1.0\"?><rootElement><GeneralMessage Key=\"Messages_General_ChoseThe\" Value=\"Choose the %1\"></GeneralMessage><GeneralMessage Key=\"Messages_General_TypeThe\" Value=\"Type the %1\"></GeneralMessage><GeneralMessage Key=\"Messages_General_SelectThe\" Value=\"Select the %1\"></GeneralMessage><GeneralMessage Key=\"Messages_General_IsRequired\" Value=\"%1 is required\"></GeneralMessage><GeneralMessage Key=\"Messages_General_MinLength\" Value=\"%1 should be at least %2 characters long\"></GeneralMessage><GeneralMessage Key=\"Messages_General_NotValidDate\" Value=\"%1 is not valid date.\"></GeneralMessage><GeneralMessage Key=\"Messages_General_NotValidTime\" Value=\"%1 is not a valid time\"></GeneralMessage><GeneralMessage Key=\"Messages_General_NotValidEmail\" Value=\"%1 is not a valid email address\"></GeneralMessage><GeneralMessage Key=\"Messages_General_WrongPhone\" Value=\"%1 is not a valid phone number\"></GeneralMessage><GeneralMessage Key=\"Messages_General_Alphabetic\" Value=\"%1 should be alphabetic\"></GeneralMessage><GeneralMessage Key=\"Messages_General_AlphabeticPlus\" Value=\"%1 should be alphabeticplus\"></GeneralMessage><GeneralMessage Key=\"Messages_General_Alphanumeric\" Value=\"%1 should be Alphanumeric\"></GeneralMessage><GeneralMessage Key=\"Messages_General_Percentage\" Value=\"%1 should be a percentage\"></GeneralMessage><GeneralMessage Key=\"Messages_General_Sign\" Value=\"%1 has not a valid sign\"></GeneralMessage><GeneralMessage Key=\"Messages_General_AlphabetiSpecial\" Value=\"%1 should be AlphabetiSpecial\"></GeneralMessage><GeneralMessage Key=\"Messages_General_MinMax\" Value=\"%1 should be in the following range [ %2 , %3 ]\"></GeneralMessage><GeneralMessage Key=\"Messages_General_Integer\" Value=\"%1 should be an integer\"></GeneralMessage><GeneralMessage Key=\"Messages_General_Decimal\" Value=\"%1 should be Decimal\"></GeneralMessage><GeneralMessage Key=\"Messages_General_Uncommon\" Value=\"The Source and Destination Modules involved in the paste did not have the same tasks.\"></GeneralMessage><GeneralMessage Key=\"Messages_General_NegativeInteger\" Value=\"%1 should be a Negative Integer\"></GeneralMessage><GeneralMessage Key=\"Messages_General_NegativeDecimal\" Value=\"%1 should be a Negative Decimal\"></GeneralMessage><GeneralMessage Key=\"Messages_General_PhoneInternational\" Value=\"%1 is not a valid Phone International\"></GeneralMessage><GeneralMessage Key=\"Messages_General_PhoneLocal\" Value=\"%1 is not a valid  Local Phone\"></GeneralMessage><GeneralMessage Key=\"Messages_General_Mobile\" Value=\"%1 si not a valid Mobile number\"></GeneralMessage><GeneralMessage Key=\"Messages_General_PositiveInteger\" Value=\"%1 should be a positive integer\"></GeneralMessage><GeneralMessage Key=\"Messages_General_GreaterThan\" Value=\"%1 should be equal or  greater than %2\"></GeneralMessage><GeneralMessage Key=\"Messages_General_LessThan\" Value=\"%1 should be equal or  less than %2\"></GeneralMessage><GeneralMessage Key=\"Messages_General_NotValidFormat\" Value=\"%1 has not a valid format\"></GeneralMessage><GeneralMessage Key=\"Messages_General_ForExample\" Value=\"( Ex :  %1 )\"></GeneralMessage></rootElement>"; 
            if (!string.IsNullOrEmpty(str_JS_GLOBALIZATION_SUPPORTED))
            {
                if (str_JS_GLOBALIZATION_SUPPORTED == "1")
                {
                    GetJSMessages();
                    str_CultureName = GetCultureName();
                    str_Script = string.Format("<script language=\"javascript\">var sXmlGeneralMessages = \'{0}\';</script>", this.Session[string.Format("DQ_JS_MSG_{0}", str_CultureName.ToUpper())].ToString());                   
                }              
            }
            if (!oClientScriptManager.IsClientScriptBlockRegistered("JS_MSG"))
            {
                oClientScriptManager.RegisterClientScriptBlock(this.GetType(), "JS_MSG", str_Script);
            }
            #endregion
        }
        #endregion
        #region GetJSMessages
        protected void GetJSMessages()
        {
            #region Declaration And Initialization Section.
            string str_CultureName = string.Empty;
            string str_FilePath = string.Empty;
            StringBuilder oSB = new StringBuilder();
            #endregion
            #region Body Section.
            str_CultureName = GetCultureName();
            if (this.Session[string.Format("DQ_JS_MSG_{0}",str_CultureName.ToUpper())] == null)
            {                
                if (str_CultureName == "en")
                {
                    str_FilePath = @"~\Includes\Messages\Messages.xml";
                }
                else
                {
                    str_FilePath = string.Format(@"~\Includes\Messages\Messages_{0}.xml", str_CultureName);
                }

                //ResXResourceReader oResXResourceReader = new ResXResourceReader(Server.MapPath(str_FilePath));
                XmlDocument oXmlDocument = new XmlDocument();
                oXmlDocument.Load(Server.MapPath(str_FilePath));
                XmlNodeList oXmlNodeList = oXmlDocument.SelectNodes("Messages/Message"); 

                oSB.Append("<?xml version=\"1.0\" encoding=\"utf-8\" ?><rootElement>");
                //foreach (DictionaryEntry oDictionaryEntry in oResXResourceReader)
                foreach(XmlNode oNode in oXmlNodeList)
                {
                    //oSB.Append(string.Format("<GeneralMessage Key=\"{0}\" Value=\"{1}\"></GeneralMessage>", oDictionaryEntry.Key.ToString(), oDictionaryEntry.Value.ToString()));
                    oSB.Append(string.Format("<GeneralMessage Key=\"{0}\" Value=\"{1}\"></GeneralMessage>", oNode.Attributes["Key"].Value.ToString(), oNode.Attributes["Value"].Value.ToString()));
                }
                oSB.Append("</rootElement>");

                this.Session.Add(string.Format("DQ_JS_MSG_{0}", str_CultureName.ToUpper()), oSB.ToString());
            }
            #endregion
        }
        #endregion
        #region DQ_RemoveRowAt
        public void DQ_RemoveRowAt(DataSet i__DS, string i__TableName, Int32 i__RowPosition)
        {
            #region Declaration And Initialization Section.
            DQTools.cTools oTools = new DQTools.cTools(); 
            #endregion
            #region Body Section.
            oTools.DQ_RemoveRowAt(i__DS, i__TableName, i__RowPosition);
            #endregion
        }
        #endregion
        #region DQ_Format
        public string DQ_Format<T>(T i__Value, string i__Format)
        {
            #region Declaration And Initialization Section.
            string str_ReturnValue = string.Empty;              
            #endregion
            #region Body Section.
            str_ReturnValue = Microsoft.VisualBasic.Strings.Format(i__Value, i__Format);
            #endregion
            #region Return Section.
            return str_ReturnValue;
            #endregion
        }
        #endregion
        #region DQ_GetConfigEntry
        public string DQ_GetConfigEntry(string i__Key)
        {
            #region Declaration And Initialization Section.
            string str_ReturnValue = string.Empty;
            #endregion
            #region Body Section.
            if (System.Configuration.ConfigurationManager.AppSettings[i__Key] != null)
            {
                str_ReturnValue = System.Configuration.ConfigurationManager.AppSettings[i__Key].ToString();
            }
            #endregion
            #region Return Section.
            return str_ReturnValue;
            #endregion
        }
        #endregion
        #region DQ_SetMasterPageControl
        public void DQ_SetMasterPageControl(Page i_Page,string i_ControlName,string i_PropertyName,object i_Value)
        {
            #region Declaration And Initialization Section.
            MasterPage      oMasterPage  = i_Page.Master;  
            DQTools.cTools  oTools       = new DQTools.cTools();
            Control         oControl     = null;   
            #endregion
            #region Body Section.
            if (oMasterPage != null)
            {
                oControl = oMasterPage.FindControl(i_ControlName);
                if (oControl != null)
                {
                    oTools.SetPropValue(oControl, i_PropertyName, i_Value);
                }
            }
            #endregion
        }
        #endregion
        #region DQ_SetSessionTimeOut
        private void DQ_SetSessionTimeOut()
        {
            #region Declaration And Initialization Section.
            Int32 i_CUSTON_SESSION_TIMEOUT = 600;
            #endregion
            #region Body Section.
            if (ConfigurationManager.AppSettings["CUSTON_SESSION_TIMEOUT"] != null)
            {
                i_CUSTON_SESSION_TIMEOUT = Convert.ToInt32(ConfigurationManager.AppSettings["CUSTON_SESSION_TIMEOUT"].ToString());
            }
            this.Session.Timeout = i_CUSTON_SESSION_TIMEOUT;            
            #endregion
        }
        #endregion
        #region DQ_HandShake_Via_Operator
        private bool DQ_HandShake_Via_Operator()
        {
            #region Declaration And Initialization Section.
            NameValueCollection oParams = new NameValueCollection();
            DQOperator.DQOperator oDQOperator = new DQOperator.DQOperator();
            DataSet oDS = new DataSet();
            string str_HAND_SHAKE_ENABLED = string.Empty;
            DQLogger.DQLogger oDQLogger = new DQLogger.DQLogger();
            bool is_Return_Value = true;
            #endregion
            #region Body Section.

            if (ConfigurationManager.AppSettings["HAND_SHAKE_ENABLED"] != null)
            {
                str_HAND_SHAKE_ENABLED = ConfigurationManager.AppSettings["HAND_SHAKE_ENABLED"].ToString().ToUpper();
                if 
                        (
                            (!string.IsNullOrEmpty(str_HAND_SHAKE_ENABLED)) &&
                            (str_HAND_SHAKE_ENABLED != "0") &&
                            (Session["ALREADY_DQNewSession_CALLED"] == null)
                        )
                {
                                      
                    // ----------------
                    oParams.Add("TASK_NAME", "DQNewSession");
                    oParams.Add("SessionID", Session.SessionID);
                    oDQOperator.DQ_DoOperation(ref oDS, oParams);
                    // ----------------

                   
                    // ----------------
                    if (oDS.Tables["NOTIFICATION"].Rows.Count > 0)
                    {
                        Session["ALREADY_DQNewSession_CALLED"] = null;

                        StringBuilder oSB = new StringBuilder();
                        oSB.AppendLine("<table>");
                        foreach (DataRow oDR in oDS.Tables["NOTIFICATION"].Rows)
                        {
                            oSB.AppendLine(string.Format("<tr><td>{0}</td></tr>", oDR["NOTIFICATION_DESC"].ToString()));
                        }
                        oSB.AppendLine("</table>");
                        HttpContext.Current.Session["DQ_ErrorMessage"] = oSB.ToString();

                        this.Session["DQ_ErrorMessage"] = oSB.ToString();
                        Exception oException = new Exception(oSB.ToString());
                        oDQLogger.DQLogException(oException);
                        DQ_BusinessErrorNotification();
                        Session["DQ_ErrorMessage"] = oSB.ToString();
                        Session["DQ_BusinessMessage"] = oSB.ToString();
                        if (IsCallback == false)
                        {
                            throw new Exception(oSB.ToString());
                        }
                        else
                        {
                            is_Return_Value = false;
                        }
                    }
                    else
                    {
                        Session["ALREADY_DQNewSession_CALLED"] = true;
                    }
                    // ----------------
                }
            }           
            #endregion    
            #region Return Section.
            this.DQ_GlobalDS_NewSession = oDS;
            return is_Return_Value;
            #endregion
        }
        #endregion
        #region DQ_HandShake_Via_DOOPERATION
        private void DQ_HandShake_Via_DOOPERATION()
        {
            #region Declaration And Initialization Section.
            NameValueCollection oParams = new NameValueCollection();
            DQOperator.DQOperator oDQOperator = new DQOperator.DQOperator();
            DataSet oDS = new DataSet();
            string str_HAND_SHAKE_ENABLED = string.Empty;
            DQLogger.DQLogger oDQLogger = new DQLogger.DQLogger();
            #endregion
            #region Body Section.

            if (ConfigurationManager.AppSettings["HAND_SHAKE_ENABLED"] != null)
            {
                str_HAND_SHAKE_ENABLED = ConfigurationManager.AppSettings["HAND_SHAKE_ENABLED"].ToString().ToUpper();
                if
                        (
                            (!string.IsNullOrEmpty(str_HAND_SHAKE_ENABLED)) &&
                            (str_HAND_SHAKE_ENABLED != "0") &&
                            (Session["ALREADY_DQNewSession_CALLED"] == null)
                        )
                {
                    
                    // ----------------
                    DQ_AddParam("TASK_NAME", "DQNewSession");
                    DQ_AddParam("SessionID", Session.SessionID, "Q");
                    DQ_DoOperation();
                    // ----------------


                    // ----------------
                    if (m_GlobalDS.Tables["NOTIFICATION"].Rows.Count > 0)
                    {

                        StringBuilder oSB = new StringBuilder();
                        oSB.AppendLine("<table>");
                        foreach (DataRow oDR in oDS.Tables["NOTIFICATION"].Rows)
                        {
                            oSB.AppendLine(string.Format("<tr><td>{0}</td></tr>", oDR["NOTIFICATION_DESC"].ToString()));
                        }
                        oSB.AppendLine("</table>");
                        HttpContext.Current.Session["DQ_ErrorMessage"] = oSB.ToString();

                        this.Session["DQ_ErrorMessage"] = oSB.ToString();
                        Exception oException = new Exception(oSB.ToString());
                        oDQLogger.DQLogException(oException);
                        DQ_BusinessErrorNotification();
                    }
                    else
                    {
                        Session["ALREADY_DQNewSession_CALLED"] = true;
                    }
                    // ----------------
                }
            }
            #endregion
        }
        #endregion
        #region FixDot
        public string FixDot(string NumericVal)
        {
            #region Declaration And Initialization Section
            DQTools.cTools oTools           = new DQTools.cTools();
            string         str_ReturnValue  = string.Empty;
            #endregion
            #region Body Section
            str_ReturnValue = oTools.FixDot(NumericVal);
            #endregion
            #region Return Section
            return str_ReturnValue;
            #endregion
        }
           #endregion
        #region InvFixDot
        public string InvFixDot(string NumericVal)
        {
            #region Declaration And Initialization Section.
            string         str_ReturnValue = string.Empty;
            DQTools.cTools oTools          = new DQTools.cTools(); 
            #endregion
            #region Body Section.
            str_ReturnValue = oTools.InvFixDot(NumericVal);
            #endregion
            #region Return Section.
            return str_ReturnValue;
            #endregion
        }
        #endregion
        #region CleanErrorMsgContent
        private string CleanErrorMsgContent(string i__Input)
        {
            #region Declaration And Initialization Section.
            string str_ReturnValue = string.Empty;
            #endregion
            #region Body Section.
            str_ReturnValue = i__Input.Replace("\r\n","");
            #endregion
            #region Return Section.
            return str_ReturnValue;
            #endregion
        }
        #endregion
        #region SetDivScrollingHeight
        public void SetDivScrollingHeight(HtmlGenericControl div_CtrlID, Int32 i_NbOfRows)
        {
            #region Declaration And Initialization Section.
            #endregion
            #region Body Section.
            if (div_CtrlID != null)
            {
                if (i_NbOfRows > 0)
                {
                    if (div_CtrlID.Style != null)
                    {
                        div_CtrlID.Style.Add("overflow-y", "scroll");
                        div_CtrlID.Style.Add("height", Convert.ToString((20 * i_NbOfRows) + 27));
                    }
                }
            }
            #endregion
        }
        #endregion
      

    }

    #region Action_Result
    public partial class Base_Action_Result
    {
        public List<DQExceptionEntry> List_Exceptions { get; set; }
        public List<DQParameter> List_Params { get; set; }
    }

    public partial class DQExceptionEntry
    {
        #region Properties.
        public Int32 NOTIFICATION_SEQ { get; set; }
        public string NOTIFICATION_CODE { get; set; }
        public string NOTIFICATION_DESC { get; set; }
        public string NOTIFICATION_DBDESC { get; set; }
        public string NOTIFICATION_INPRG { get; set; }
        public Int32 NOTIFICATION_TYPE { get; set; }
        #endregion
    }

    
    #endregion
    #region CollectionHelper
    public class CollectionHelper
    {
        private CollectionHelper()
        {
        }

        public static DataTable ConvertTo<T>(IList<T> list)
        {
            DataTable table = CreateTable<T>();
            Type entityType = typeof(T);
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);

            foreach (T item in list)
            {
                DataRow row = table.NewRow();

                foreach (PropertyDescriptor prop in properties)
                {
                    row[prop.Name] = prop.GetValue(item);
                }

                table.Rows.Add(row);
            }

            return table;
        }

        public static IList<T> ConvertTo<T>(IList<DataRow> rows)
        {
            IList<T> list = null;

            if (rows != null)
            {
                list = new List<T>();

                foreach (DataRow row in rows)
                {
                    T item = CreateItem<T>(row);
                    list.Add(item);
                }
            }

            return list;
        }

        public static IList<T> ConvertTo<T>(DataTable table)
        {
            if (table == null)
            {
                return null;
            }

            List<DataRow> rows = new List<DataRow>();

            foreach (DataRow row in table.Rows)
            {
                rows.Add(row);
            }

            return ConvertTo<T>(rows);
        }

        public static T CreateItem<T>(DataRow row)
        {
            T obj = default(T);
            if (row != null)
            {
                obj = Activator.CreateInstance<T>();

                foreach (DataColumn column in row.Table.Columns)
                {
                    PropertyInfo prop = obj.GetType().GetProperty(column.ColumnName);
                    try
                    {
                        object value = row[column.ColumnName];

                        switch (prop.PropertyType.FullName)
                        {
                            case "System.Int16":
                            case "System.Nullable`1[[System.Int16, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]":
                            case "System.Nullable`1[[System.Int16, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]":
                                if (value is DBNull)
                                {
                                    value = 0;
                                }
                                value = (Int16?)(value);
                                break;
                            case "System.Int32":
                            case "System.Nullable`1[[System.Int32, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]":
                            case "System.Nullable`1[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]":
                                if (value is DBNull)
                                {
                                    value = 0;
                                }
                                value = (Int32?)(value);
                                break;
                            case "System.Int64":
                            case "System.Nullable`1[[System.Int64, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]":
                            case "System.Nullable`1[[System.Int64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]":
                                if (value is DBNull)
                                {
                                    value = 0;
                                }
                                value = (Int64?)(value);
                                break;
                            case "System.Boolean":
                                value = Convert.ToBoolean(value);
                                break;
                            case "System.Nullable`1[[System.Boolean, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]":
                                value = (bool?)(value);
                                break;
                            case "System.Nullable`1[[System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]":
                                value = (bool?)(value);
                                break;
                            case "System.Double":
                            case "System.Nullable`1[[System.Double, mscorlib, Version=2.0.0.0, Culture=neutral,PublicKeyToken=b77a5c561934e089]]":
                            case "System.Nullable`1[[System.Double, mscorlib, Version=4.0.0.0, Culture=neutral,PublicKeyToken=b77a5c561934e089]]":
                                value = Convert.ToDouble(value);
                                break;
                            case "System.Decimal":
                            case "System.Nullable`1[[System.Decimal, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]":
                            case "System.Nullable`1[[System.Decimal, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]":
                                value = Convert.ToDecimal(value);
                                break;
                            case "System.Byte":
                            case "System.Nullable`1[[System.Byte, mscorlib, Version=2.0.0.0, Culture=neutral,PublicKeyToken=b77a5c561934e089]]":
                            case "System.Nullable`1[[System.Byte, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]":
                            case "System.Nullable`1[[System.Byte, mscorlib, Version=4.0.0.0, Culture=neutral,PublicKeyToken=b77a5c561934e089]]":
                            case "System.Nullable`1[[System.Byte, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]":

                                value = Convert.ToByte(value);
                                break;
                            default:
                                value = Convert.ChangeType(value, prop.PropertyType);
                                break;
                        }

                        prop.SetValue(obj, value, null);
                    }
                    catch(Exception e)
                    {
                        // You can log something here
                        throw;
                    }
                }
            }

            return obj;
        }

        public static DataTable CreateTable<T>()
        {
            Type entityType = typeof(T);
            DataTable table = new DataTable(entityType.Name);
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entityType);

            foreach (PropertyDescriptor prop in properties)
            {
                table.Columns.Add(prop.Name, prop.PropertyType);
            }

            return table;
        }
    }
    #endregion
    #region Parameter
    public partial class DQParameter
    {
        #region Properties.
        public string PARAM_NAME { get; set; }
        public string PARAM_VALUE { get; set; }
        public string PARAM_TYPE { get; set; }
        #endregion
    }
    #endregion
    public class DeclarationOrderComparator : IComparer
    {
        int IComparer.Compare(Object x, Object y)
        {
            PropertyInfo first = x as PropertyInfo;
            PropertyInfo second = y as PropertyInfo;
            if (first.MetadataToken < second.MetadataToken)
                return -1;
            else if (first.MetadataToken > second.MetadataToken)
                return 1;

            return 0;
        }
    }
}