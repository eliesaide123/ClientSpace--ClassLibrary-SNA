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
 ///<summary>
 ///This Custom control inherits from the ASP.NET TextBox Control.
 ///</summary>
namespace DQBasePage
{
    public partial class DQBasePage : System.Web.UI.Page, ICallbackEventHandler 
    {
        #region "AJAX Feature"
        #region "Members"
        private bool m_DQ_Ajax_Enabled;
        private string m_DQ_CallBackArgs;
        private bool m_DQ_Ajax_onRequest_Enabled;
        private bool m_DQ_Ajax_onRequest_HandleLengthyProcess;
        private string m_DQ_Ajax_onRequest_Sender;
        private string m_DQ_Ajax_onRequest_Receiver;
        private string m_DQ_Ajax_onRequest_NoSenderValue;
        private string m_DQ_Ajax_onRequest_ErrorHandler;
        #endregion
        #region "Properties"
        ///<summary>
        ///This Property Indicates if this control is AJAX Enabled.
        ///</summary>
        public bool DQ_Ajax_Enabled
        {
            get
            {
                return m_DQ_Ajax_Enabled;
            }
            set
            {
                m_DQ_Ajax_Enabled = value;
              }
        }
        ///<summary>
        ///This Property Used To Send Data To Client Side JS Function.
        ///</summary>
        public string DQ_CallBackArgs
        {
            get
            {
                return m_DQ_CallBackArgs;
            }
            set
            {
                m_DQ_CallBackArgs = value;
            }
        }
        ///<summary>
        ///This Property Indicates if this control is onRequest AJAX Enabled.
        ///</summary>
       public bool DQ_Ajax_onRequest_Enabled
        {
            get
            {
                return m_DQ_Ajax_onRequest_Enabled;
            }
            set
            {
                m_DQ_Ajax_onRequest_Enabled = value;
            }
        }
        ///<summary>
        ///This Property Indicates if this control is Handling the lengthy onRequest Process.
        ///</summary>
        public bool DQ_Ajax_onRequest_HandleLengthyProcess
        {
            get
            {
                return m_DQ_Ajax_onRequest_HandleLengthyProcess;
            }
            set
            {
                m_DQ_Ajax_onRequest_HandleLengthyProcess = value;                
            }
        }
        ///<summary>
        ///This Property Indicates the name of JS function that prepares the string to be sent to server side.
        ///</summary>
       
        public string DQ_Ajax_onRequest_Sender
        {
            get
            {
                return m_DQ_Ajax_onRequest_Sender;
            }
            set
            {
                m_DQ_Ajax_onRequest_Sender = value;                
            }
        }
        ///<summary>
        ///This Property Indicates the name of JS function that handles the received string from Server Side.
        ///</summary>
       
        public string DQ_Ajax_onRequest_Receiver
        {
            get
            {
                return m_DQ_Ajax_onRequest_Receiver;
            }
            set
            {
                m_DQ_Ajax_onRequest_Receiver = value;
            }
        }
        ///<summary>
        ///This Property Indicates the specific value that stops sending data to the server side.
        ///</summary>
       
        public string DQ_Ajax_onRequest_NoSenderValue
        {
            get
            {
                return m_DQ_Ajax_onRequest_NoSenderValue;
            }
            set
            {
                m_DQ_Ajax_onRequest_NoSenderValue = value;
            }
        }
        ///<summary>
        ///This Property Indicates The JS function that should be Called When A Server Side Exception Occured.
        ///</summary>
        
        public string DQ_Ajax_onRequest_ErrorHandler
        {
            get
            {
                return m_DQ_Ajax_onRequest_ErrorHandler;
            }
            set
            {
                m_DQ_Ajax_onRequest_ErrorHandler = value;                
            }
        }
        #endregion
        #region "Delegate"
        public delegate void AsyncEventHandler(object sender, AsyncArgs e);
        #endregion
        #region "Events"
        #region "DQ_AjaxonRequestEvent"
        private static readonly object DQ_AjaxonRequestEventKey = new object();
        public event AsyncEventHandler DQ_AjaxonRequestEvent
        {
            add { Events.AddHandler(DQ_AjaxonRequestEventKey, value); }
            remove { Events.RemoveHandler(DQ_AjaxonRequestEventKey, value); }
            
        }
        #endregion
        #endregion
        #region "Events Triggers"
        #region "Trigger_DQ_AjaxonRequest"
        protected void Trigger_DQ_Ajax_onRequest(AsyncArgs e)
        {
            AsyncEventHandler oAsyncClick = (AsyncEventHandler)Events[DQ_AjaxonRequestEventKey];
            oAsyncClick.Invoke(this, e);
            //DQ_AjaxonRequestEvent(this, e);
             //if ((oAsyncClick != null)) 
             //{
             //    DQ_AjaxonRequestEvent(this, e);
             //}            
        }
        #endregion
        #endregion
        #region "ICallBackEventHandler"
        public string  GetCallbackResult()
        {
            return DQ_CallBackArgs;
        }
        public void  RaiseCallbackEvent(string eventArgument)
        {
            int idx_EventSrc = eventArgument.IndexOf("__EventSrc");
            string str_EventSrc = eventArgument.Substring(idx_EventSrc, eventArgument.Length - idx_EventSrc);
            eventArgument = eventArgument.Replace(str_EventSrc, "");
            str_EventSrc = str_EventSrc.Replace("__EventSrc", "");
            switch (str_EventSrc)
            {
                case "_onRequest":
                    Trigger_DQ_Ajax_onRequest(new AsyncArgs(eventArgument));
                    break;
            }
        }
        #endregion
        #endregion
        #region "Register_JS_CallBack_Handler"
        private void Register_JS_CallBack_Handler(string i__EventName, string i__SenderFunction, string i__ReceiverFunction, string i__ErrorHandlerFunction, bool i__HandleLengthyProcess, string i_NoSenderValue)
        {
            string str_ClientID = "PageLevel";
            ClientScriptManager oClientScriptManager = Page.ClientScript;
            string str_ccbReference = oClientScriptManager.GetCallbackEventReference(this, "args", "js_Handle_Server_Response_" + i__EventName + "_" + str_ClientID, "", "js_Handle_Server_Error_" + i__EventName + "_" + str_ClientID, true);
            StringBuilder str_CallBackScript = new StringBuilder();

            if ((string.IsNullOrEmpty(i_NoSenderValue)))
            {
                i_NoSenderValue = "@#$";
            }


            str_CallBackScript.AppendLine("<script language=\"javascript\" type=\"text/javascript\">");
            str_CallBackScript.AppendLine("function js_CallServer_" + i__EventName + "_" + str_ClientID + "()");
            str_CallBackScript.AppendLine("{");
            str_CallBackScript.AppendLine("try");
            str_CallBackScript.AppendLine("{");
            str_CallBackScript.AppendLine("var args = null;");
            str_CallBackScript.AppendLine("args = " + i__SenderFunction + "();");
            str_CallBackScript.AppendLine("if ( (args != null) && (args != '" + i_NoSenderValue + "'))");
            str_CallBackScript.AppendLine("{");
            str_CallBackScript.AppendLine("args = args + '__EventSrc_' + '" + i__EventName + "';");
            str_CallBackScript.AppendLine(str_ccbReference + ";");
            if ((i__HandleLengthyProcess))
            {
                str_CallBackScript.AppendLine("js_AttachWaitBehavior('" + str_ClientID + "','" + System.Web.HttpContext.Current.Request.ApplicationPath + "/Includes/Images/Ajax-loader.gif',0);");
                //str_CallBackScript.AppendLine("js_SetDisability_ControlsByForm(true,'');")
            }
            str_CallBackScript.AppendLine("}");
            str_CallBackScript.AppendLine("}");
            str_CallBackScript.AppendLine("catch(e)");
            str_CallBackScript.AppendLine("{");
            str_CallBackScript.AppendLine("alert('js_CallServer_" + i__EventName + "_" + str_ClientID + ":' + e.message" + ");");
            str_CallBackScript.AppendLine("}");
            str_CallBackScript.AppendLine("}");
            str_CallBackScript.AppendLine("");

            str_CallBackScript.AppendLine("function js_Handle_Server_Response_" + i__EventName + "_" + str_ClientID + "(i__ServerResponse) ");
            str_CallBackScript.AppendLine("{");
            str_CallBackScript.AppendLine("try");
            str_CallBackScript.AppendLine("{");
            if ((i__HandleLengthyProcess))
            {
                //str_CallBackScript.AppendLine("js_SetDisability_ControlsByForm(false,'');")
                str_CallBackScript.AppendLine("js_RemoveWaitBehavior('" + str_ClientID + "');");
            }
            str_CallBackScript.AppendLine(i__ReceiverFunction + "(i__ServerResponse);");
            str_CallBackScript.AppendLine("}");
            str_CallBackScript.AppendLine("catch(e)");
            str_CallBackScript.AppendLine("{");
            str_CallBackScript.AppendLine("alert('js_Handle_Server_Response_" + i__EventName + "_" + str_ClientID + "():' + e.message)");
            str_CallBackScript.AppendLine("}");
            str_CallBackScript.AppendLine("}");

            str_CallBackScript.AppendLine("function js_Handle_Server_Error_" + i__EventName + "_" + str_ClientID + "(i__Error,i__Context) ");
            str_CallBackScript.AppendLine("{");
            str_CallBackScript.AppendLine("try");
            str_CallBackScript.AppendLine("{");

            if ((i__HandleLengthyProcess))
            {
                str_CallBackScript.AppendLine("js_RemoveWaitBehavior('" + str_ClientID + "');");
            }

            if (!(string.IsNullOrEmpty(i__ErrorHandlerFunction)))
            {
                str_CallBackScript.AppendLine(i__ErrorHandlerFunction + "(i__Error);");
            }
            str_CallBackScript.AppendLine("}");
            str_CallBackScript.AppendLine("catch(e)");
            str_CallBackScript.AppendLine("{");
            str_CallBackScript.AppendLine("alert('js_Handle_Server_Error_" + i__EventName + "_" + str_ClientID + "():' + e.message)");
            str_CallBackScript.AppendLine("}");
            str_CallBackScript.AppendLine("}");


            str_CallBackScript.AppendLine("</script>");

            if (!(oClientScriptManager.IsStartupScriptRegistered("js_CallBack_" + i__EventName + "_" + str_ClientID)))
            {
                oClientScriptManager.RegisterStartupScript(this.GetType(), "js_CallBack_" + i__EventName + "_" + str_ClientID, str_CallBackScript.ToString());
            }
        }
        #endregion
    }
     public class AsyncArgs : EventArgs
     {
         public string _Text;
         public string Text 
         {
             get { return _Text; }
             set { _Text = value; }
         }
         public AsyncArgs(string i_Text)
         {
             Text = i_Text;
         }        
     }
}