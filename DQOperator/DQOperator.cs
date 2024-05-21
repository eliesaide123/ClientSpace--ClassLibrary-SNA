using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Data;

namespace DQOperator
{
    public class DQOperator
    {
        #region Members.
        #endregion
        #region Properties
        #endregion
        #region DQ_DoOperation
        public void DQ_DoOperation(ref DataSet i__OperatorDS,NameValueCollection i__Params)
        {
            #region Declaration and Initialization Section.
            DQScreenBinder.ScreenBinder  oScreenBinder = new DQScreenBinder.ScreenBinder();
            DQServiceLayer.cServiceLayer oServiceLayer = new DQServiceLayer.cServiceLayer(); 
            #endregion
            #region Add Common Tables [Ex: ERRORS, PARAMETERS]
            oScreenBinder.DQ_PrepareGeneralDS(ref i__OperatorDS);
            #endregion
            #region Add Parameters.
            foreach (string oKey  in i__Params.AllKeys)
            {
                oScreenBinder.DQ_AddParam(i__OperatorDS, oKey, i__Params[oKey]);
            }
            #endregion
            #region Call the Corresponding Program.
            oServiceLayer.RunProgram(ref i__OperatorDS);
            #endregion
        }
        #endregion
        #region DQ_DoOperation
        public void DQ_DoOperation(ref DataSet i__OperatorDS, List<DQParam> i_Params)
        {
            #region Declaration and Initialization Section.
            DQScreenBinder.ScreenBinder oScreenBinder = new DQScreenBinder.ScreenBinder();
            DQServiceLayer.cServiceLayer oServiceLayer = new DQServiceLayer.cServiceLayer();
            #endregion
            #region Add Common Tables [Ex: ERRORS, PARAMETERS]
            oScreenBinder.DQ_PrepareGeneralDS(ref i__OperatorDS);
            #endregion
            #region Add Parameters.
            foreach (DQParam param in i_Params)
            {
                oScreenBinder.DQ_AddParam(i__OperatorDS, param.Name, param.Value,param.Type);
            }
            #endregion
            #region Call the Corresponding Program.
            oServiceLayer.RunProgram(ref i__OperatorDS);
            #endregion
        }
        #endregion
        #region DQ_GetParameter
        public string DQ_GetParameter(DataSet i_DS, string i__Param_Name)
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
                oDT_Parameters = i_DS.Tables["PARAMETERS"];
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
    }

    public partial class DQParam
    {
        #region Properties
        public string Name { get; set; }
        public string Value { get; set; }
        public string Type { get; set; }
        #endregion
    }
}
