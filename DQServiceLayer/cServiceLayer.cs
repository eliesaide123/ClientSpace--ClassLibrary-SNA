using System;
using System.Collections.Generic;
using System.Text;
using Progress.Open4GL.Proxy;
using Progress.Open4GL.DynamicAPI;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using DQDataEngine;
using System.Net;
using Newtonsoft.Json;

namespace DQServiceLayer
{
    public class cServiceLayer
    {
        #region Properties.
        public List<ExtentColumn> My_List_Extent_Columns { get; set; }
        public string My_ConnectionString { get; set; }

        public string SERVICE_SOURCE { get; set; }

        public string PROGRESS_USER { get; set; }

        public string PROGRESS_PWD { get; set; }
        public string APPSERVER_CONNNECTION { get; set; }
        public string RESOLVE_VERSION { get; set; }
        public string RESOLVE_PROC { get; set; }
        public string PARAM_DEF { get; set; }
        public string HAND_SHAKE_ENABLED { get; set; }
        public string APP_DOWN_MSG { get; set; }
        public string PROGRESS_SESSION_MODEL { get; set; }
        #endregion
        #region Constructor
        public cServiceLayer()
        {
            #region Declaration And Initialization Section.
            #endregion
            #region Body Section.
            this.APPSERVER_CONNNECTION  = ConfigurationManager.AppSettings.Get("APPSERVER_CONNNECTION");
            this.RESOLVE_VERSION        = ConfigurationManager.AppSettings.Get("RESOLVE_VERSION");
            this.RESOLVE_PROC           = ConfigurationManager.AppSettings.Get("RESOLVE_PROC");
            this.PARAM_DEF              = ConfigurationManager.AppSettings.Get("PARAM_DEF");
            this.HAND_SHAKE_ENABLED     = ConfigurationManager.AppSettings.Get("HAND_SHAKE_ENABLED");
            this.APP_DOWN_MSG           = ConfigurationManager.AppSettings.Get("APP_DOWN_MSG");
            this.PROGRESS_SESSION_MODEL = ConfigurationManager.AppSettings.Get("PROGRESS_SESSION_MODEL");
            this.SERVICE_SOURCE         =  ConfigurationManager.AppSettings.Get("SERVICE_SOURCE"); 
            this.PROGRESS_USER          = ConfigurationManager.AppSettings.Get("PROGRESS_USER");
            this.PROGRESS_PWD           = ConfigurationManager.AppSettings.Get("PROGRESS_PWD");
            #endregion
        }
        public cServiceLayer(string i_ConnectionString):this()
        {
            #region Declaration And Initialization Section.
            #endregion
            #region Body Section.
            this.My_ConnectionString = i_ConnectionString;            
            #endregion
        }
        #endregion
        public  void RunProgram(ref DataSet io_DataSet)
        {
            #region Declaration and Initialization Section.
            string str_ServiceSource = string.Empty;
            #endregion
            #region Body Section
            try
            {
                switch (this.SERVICE_SOURCE)
                {
                    case "PROGRESS":
                        RunProgressProgram(ref io_DataSet);
                        break;
                    case "PASOEPROGRESS":
                        RunPasoeProgressProgram(ref io_DataSet);
                        break;
                    case "NET":
                        RunDotNetProgram(ref io_DataSet);
                        break;
                    case "SIMULATION":
                        RunSimulationProgram(ref io_DataSet);
                        break;
                    default:
                        RunProgressProgram(ref io_DataSet);
                        break;
                }
            }
            catch (System.Exception ex) {
                //ex.Data.Add("CM_RunProgram", "This is a custom error propagated through the exception object");
                throw;
            }
            
            #endregion
        }
        private string CheckCurrentProgressConnection(DataSet io__DS)
        {
            #region Declaration and Initialization Section.
            string str_Connection = string.Empty;
            DQTools.cTools oTools = new DQTools.cTools();
            DataTable oDT_Parameters;
            #endregion
            #region Body Section.
            if (string.IsNullOrEmpty(this.My_ConnectionString) == false)
            {
                str_Connection = this.My_ConnectionString;
            }
            else
            {
                //str_Connection = System.Configuration.ConfigurationManager.AppSettings.Get("APPSERVER_CONNNECTION");
                str_Connection = this.APPSERVER_CONNNECTION;
            }
            oDT_Parameters = io__DS.Tables["PARAMETERS"];

            if (oDT_Parameters != null)
            {
                if (!string.IsNullOrEmpty(oTools.DQ_GetFieldValueByFilter<string>(oDT_Parameters, "PARAM_VALUE", "PARAM_NAME='DQCONNECTION'")))
                {
                    str_Connection = oTools.DQ_GetFieldValueByFilter<string>(oDT_Parameters, "PARAM_VALUE", "PARAM_NAME='DQCONNECTION'");
                }
            }
            
            
            #endregion
            #region Return Section.
            return str_Connection;
            #endregion
        }

        private void RunProgressProgram(ref DataSet io_DataSet)
        {
            
                #region Declaration and Initialization Section.
                //string str_Connection = string.Empty;
                Connection oConn = null;
                OpenAppObject oOpenAppObject = null;
                ParamArray oParamArray = null;
                //string str_ResolveProc = string.Empty;
                //string str_RESOLVE_VERSION = string.Empty;
                //string str_APP_DOWN_MSG = string.Empty;
                
                DQTools.cTools oTools = new DQTools.cTools();
                Progress.Open4GL.Proxy.OpenProcObject oPOObject = null;
                DataSet oDS_Output = new DataSet();

                DQLogger.DQLogger oDQLogger = new DQLogger.DQLogger();
                Exception oException = null;
                #endregion

                #region Body Section.

                try
                {
                // -----------------
                //str_Connection = CheckCurrentProgressConnection(io_DataSet);
                //str_ResolveProc = System.Configuration.ConfigurationManager.AppSettings.Get("RESOLVE_PROC");
                //str_RESOLVE_VERSION = System.Configuration.ConfigurationManager.AppSettings.Get("RESOLVE_VERSION");
                //str_APP_DOWN_MSG = System.Configuration.ConfigurationManager.AppSettings.Get("APP_DOWN_MSG");
                
                // -----------------

                // -----------------
                oConn = oTools.GetConnection(this.APPSERVER_CONNNECTION);
                if (!string.IsNullOrEmpty(this.PROGRESS_SESSION_MODEL))
                {
                    oConn.SessionModel = Convert.ToInt32(this.PROGRESS_SESSION_MODEL);
                }
                oOpenAppObject = new OpenAppObject(oConn, "MySvc");
                // -----------------

                // -----------------
                if (string.IsNullOrEmpty(this.RESOLVE_VERSION) == true)
                {
                    oParamArray = new ParamArray(1);
                    oParamArray.AddDatasetHandle(0, io_DataSet, ParamArrayMode.INPUT_OUTPUT, new ProDataSetMetaData("", ""));
                }
                else
                {
                    switch (this.RESOLVE_VERSION.ToUpper())
                    {
                        case "V2":
                            oParamArray = new ParamArray(2);
                            oParamArray.AddDatasetHandle(0, io_DataSet, ParamArrayMode.INPUT, new ProDataSetMetaData("", ""));
                            oParamArray.AddDatasetHandle(1, oDS_Output, ParamArrayMode.OUTPUT, new ProDataSetMetaData("", ""));
                            break;
                        default:
                            oParamArray = new ParamArray(2);
                            oParamArray.AddDatasetHandle(0, io_DataSet, ParamArrayMode.INPUT, new ProDataSetMetaData("", ""));
                            oParamArray.AddDatasetHandle(1, oDS_Output, ParamArrayMode.OUTPUT, new ProDataSetMetaData("", ""));
                            break;
                    }
                    
                }
                // -----------------

                // Handling Extent Columns.
                // -------------------
                if (this.My_List_Extent_Columns != null)
                {
                    foreach (var oRow_Extent_Column in this.My_List_Extent_Columns)
                    {
                        if (io_DataSet.Tables.Contains(oRow_Extent_Column.DT_Name) == true)
                        {
                            Progress.Open4GL.ProDataTable.SetExtentColumns
                                    (
                                        io_DataSet.Tables[oRow_Extent_Column.DT_Name],
                                        oRow_Extent_Column.Destination_Field_Name,
                                        oRow_Extent_Column.First_Entry,
                                        oRow_Extent_Column.Extent_Size
                                    );
                        }
                    }
                }
                // -------------------

                // -------------------
                oOpenAppObject.RunProc(this.RESOLVE_PROC, oParamArray);
                // -------------------


                // -------------------
                if (string.IsNullOrEmpty(this.RESOLVE_VERSION) == true)
                {
                    io_DataSet = (DataSet)oParamArray.GetOutputParameter(0);
                }
                else
                {
                    io_DataSet = (DataSet)oParamArray.GetOutputParameter(1);
                }
                // -------------------
                                        
                oOpenAppObject.Dispose();

                if (oConn != null)
                {
                    oConn.ReleaseConnection();
                    oConn.Dispose();
                    oConn = null;
                }

                #endregion
            }
            catch (System.Exception ex)
            {
                //ex.Data.Add("CM_RunProgressProgram", "This is a custom error propagated through the exception object");
                throw new Exception(ex.Message,ex);
                if (string.IsNullOrEmpty(this.APP_DOWN_MSG) == true)
                {
                    throw new Exception("Alert<br><br> The Client Web Space is Currently Unavailable <br><br> Please Try Again Later <br><br> Thank you");
                }
                else
                {
                    throw new Exception(this.APP_DOWN_MSG);
                }
            }
        }
        private void RunPasoeProgressProgram(ref DataSet io_DataSet)
        {

            #region Declaration and Initialization Section.            
            #endregion

            #region Body Section
            try
            {
                var uri  = this.APPSERVER_CONNNECTION;
                var user = this.PROGRESS_USER;
                var pwd   = this.PROGRESS_PWD;
                var responseUri = "";
                HttpWebResponse webResponse = null;
                // var xmlData = JsonConvert.SerializeObject(io_DataSet, Formatting.Indented);
                var xmlData = io_DataSet.GetXml();
                var responseResult = CallRESTService
                                    (
                                        uri,
                                        "POST",
                                        user,
                                        pwd,
                                        Encoding.ASCII.GetBytes(xmlData),
                                        null,
                                        "",
                                        "",
                                        "",
                                        "",
                                        ref responseUri,
                                        ref webResponse
                                    );
                Newtonsoft.Json.Linq.JObject o = JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JObject>(responseResult.ToString());
                io_DataSet = JsonConvert.DeserializeObject<DataSet>(o.First.First.ToString());
            }
            catch(Exception ex)
            {
                //ex.Data.Add("CM_RunProgressProgram", "This is a custom error propagated through the exception object");
                throw new Exception(ex.Message, ex);
                if (string.IsNullOrEmpty(this.APP_DOWN_MSG) == true)
                {
                    throw new Exception("Alert<br><br> The Client Web Space is Currently Unavailable <br><br> Please Try Again Later <br><br> Thank you");
                }
                else
                {
                    throw new Exception(this.APP_DOWN_MSG);
                }
            }
            #endregion
        }

        #region Pasoe
        #region Api Calls
        public object CallRESTService(string requestUri, string method, string userName, string password, byte[] postData, CookieContainer cookieContainer, string userAgent, string acceptHeaderString, string referer, string contentType, ref string responseUri, ref HttpWebResponse _httpWebResponse)
        {
            string result = "";

            try
            {
                if (!string.IsNullOrEmpty(requestUri))
                {
                    HttpWebRequest request = WebRequest.Create(requestUri) as HttpWebRequest;

                    if (request != null)
                    {
                        request.KeepAlive = true;
                        System.Net.Cache.RequestCachePolicy cachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.BypassCache);
                        request.CachePolicy = cachePolicy;
                        request.Expect = null;
                        if (!string.IsNullOrEmpty(method))
                            request.Method = method;
                        if (!string.IsNullOrEmpty(acceptHeaderString))
                            request.Accept = acceptHeaderString;
                        if (!string.IsNullOrEmpty(referer))
                            request.Referer = referer;
                        if (!string.IsNullOrEmpty(contentType))
                            request.ContentType = contentType;
                        if (!string.IsNullOrEmpty(userAgent))
                            request.UserAgent = userAgent;
                        if (!string.IsNullOrEmpty(userName))
                            request.Credentials = BuildCredentials(requestUri, userName, password, "BASIC");
                        if (cookieContainer != null)
                            request.CookieContainer = cookieContainer;
                        request.Timeout = 30000;

                        if (request.Method == "POST")
                        {
                            if (postData != null)
                            {
                                request.ContentLength = postData.Length;

                                using (System.IO.Stream dataStream = request.GetRequestStream())
                                {
                                    dataStream.Write(postData, 0, postData.Length);
                                }
                            }
                        }

                        using (HttpWebResponse httpWebResponse = request.GetResponse() as HttpWebResponse)
                        {
                            _httpWebResponse = httpWebResponse;
                            if (httpWebResponse != null)
                            {
                                responseUri = httpWebResponse.ResponseUri.AbsoluteUri;
                                if (cookieContainer != null)
                                    cookieContainer.Add(httpWebResponse.Cookies);

                                using (System.IO.StreamReader streamReader = new System.IO.StreamReader(httpWebResponse.GetResponseStream()))
                                {
                                    result = streamReader.ReadToEnd();
                                }

                                return result;
                            }
                        }
                    }
                }

                responseUri = null;
                return null;
            }
            catch (WebException ex0)
            {
                using (System.IO.StreamReader streamReader = new System.IO.StreamReader(ex0.Response.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();
                }
                responseUri = null;
                throw new Exception(result);
            }
        }
        private ICredentials BuildCredentials(string siteurl, string username, string password, string authtype)
        {
            NetworkCredential cred;

            if (username.Contains(@"\"))
            {
                string domain = username.Substring(0, username.IndexOf(@"\"));
                username = username.Substring(username.IndexOf(@"\") + 1);
                cred = new NetworkCredential(username, password, domain);
            }
            else
                cred = new NetworkCredential(username, password);

            CredentialCache cache = new CredentialCache();

            if (authtype.Contains(":"))
                authtype = authtype.Substring(authtype.IndexOf(":") + 1);

            cache.Add(new Uri(siteurl), authtype, cred);
            return cache;
        }
        #endregion

        #endregion
        private void RunDotNetProgram(ref DataSet io_DataSet)
        {
            #region Declaration And Initialization Section.
            Resolver.Resolver oResolver = null;            
            #endregion
            #region Body Section.
            oResolver = new Resolver.Resolver();
            oResolver.Invoke_Task(io_DataSet);
            #endregion
        }
        private void RunSimulationProgram(ref DataSet io_DataSet)
        {
            #region Declaration & Initialization Section.
            DQTools.cTools  oTools                      = new DQTools.cTools();
            string          str_Random_Rows_Nbr         = ConfigurationManager.AppSettings["Random_Rows_Nbr"];
            string          str_DataEngine_Connection   = ConfigurationManager.AppSettings["DataEngine_Connection"];
            string          str_App_Name                = ConfigurationManager.AppSettings["APP_NAME"];
            #endregion

            #region SaveDSinDB Section.
            DQDataEngine.cDataEngine oDataEngine = new cDataEngine();
            if (!string.IsNullOrEmpty(str_DataEngine_Connection))
            {
                oDataEngine.ConnectionString = str_DataEngine_Connection;
                oDataEngine.AppName          = str_App_Name;
                oDataEngine.SaveDSinDB(io_DataSet);
                oDataEngine.FillDS(io_DataSet);
            }
            
            #endregion

            #region Body Section.
            //try
            //{
            //    for (int i__Counter = 0; i__Counter < io_DataSet.Tables.Count; i__Counter++)
            //    {
            //        DataTable oDT = io_DataSet.Tables[i__Counter];
            //        if ((oDT.TableName != "ERRORS") && (oDT.TableName != "CREDENTIALS") && (oDT.TableName != "PARAMETERS") && (oDT.TableName != "SYSVAR"))
            //        {
            //            if (!string.IsNullOrEmpty(str_Random_Rows_Nbr))
            //            {
            //                oTools.DQ_GenerateRandomData(ref oDT, Int32.Parse(str_Random_Rows_Nbr));
            //            }
            //            else
            //            {
            //                oTools.DQ_GenerateRandomData(ref oDT, 5);
            //            }
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    throw;
            //}
            #endregion
        }

        #region ProDataSet
        #region RunProgram
        public void RunProgram(ref Progress.Open4GL.ProDataSet io_ProDataSet)
        {
            try
            {
                #region Declaration and Initialization Section.
                string str_ServiceSource = string.Empty;
                #endregion
                #region Body Section.
                RunProgressProgram(ref io_ProDataSet);
                #endregion
            }
            catch (System.Exception ex)
            {
                throw;
            }
        }
        #endregion
        #region RunProgressProgram
        private void RunProgressProgram(ref Progress.Open4GL.ProDataSet io_ProDataSet)
        {
            try
            {
                #region Declaration and Initialization Section.
                //string str_Connection = string.Empty;
                Connection oConn = null;
                OpenAppObject oOpenAppObject = null;
                ParamArray oParamArray = null;
                //string str_ResolveProc = string.Empty;
                DQTools.cTools oTools = new DQTools.cTools();
                Progress.Open4GL.Proxy.OpenProcObject oPOObject = null;
                DQLogger.DQLogger oDQLogger = new DQLogger.DQLogger();
                Exception oException = null;
                #endregion

                #region Body Section.
                //str_Connection = System.Configuration.ConfigurationManager.AppSettings.Get("APPSERVER_CONNNECTION");
                //str_ResolveProc = System.Configuration.ConfigurationManager.AppSettings.Get("RESOLVE_PROC");
                oConn = oTools.GetConnection(this.APPSERVER_CONNNECTION);
                oParamArray = new ParamArray(1);
                oOpenAppObject = new OpenAppObject(oConn, "MySvc");

                // Handling Extent Columns.
                // -------------------
              
                if (this.My_List_Extent_Columns != null)
                {
                    foreach (var oRow_Extent_Column in this.My_List_Extent_Columns)
                    {

                        if (io_ProDataSet.Tables.Contains(oRow_Extent_Column.DT_Name) == true)
                        {
                            Progress.Open4GL.ProDataTable.SetExtentColumns
                                    (
                                        io_ProDataSet.Tables[oRow_Extent_Column.DT_Name],
                                        oRow_Extent_Column.Destination_Field_Name,
                                        oRow_Extent_Column.First_Entry,
                                        oRow_Extent_Column.Extent_Size
                                    );
                        }
                    }
                }
                // -------------------

                oParamArray.AddDatasetHandle(0, io_ProDataSet, ParamArrayMode.INPUT_OUTPUT, new ProDataSetMetaData("", ""));
                oOpenAppObject.RunProc(this.RESOLVE_PROC, oParamArray);

                io_ProDataSet = (Progress.Open4GL.ProDataSet)oParamArray.GetOutputParameter(0);
                #endregion
            }
            catch (System.Exception ex)
            {
                //ex.Data.Add("CM_RunProgressProgram", "This is a custom error propagated through the exception object");
                throw;
            }
        }
        #endregion        
        #endregion
    }
    public class ExtentColumn
    {
        #region Properties.
        public string DT_Name { get; set; }
        public string Destination_Field_Name { get; set; }
        public string First_Entry { get; set; }
        public Int32  Extent_Size { get; set; }
        #endregion
    }
}
