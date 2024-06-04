using Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLC
{
    public static class CommonFunctions
    {
        public static Dictionary<string, object> TransformDataToDictionary(string tblName, DataSet GlobalOperatorDS)
        {
            DataTable dataTable = GlobalOperatorDS.Tables[tblName];
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();

            foreach (DataRow row in dataTable.Rows)
            {
                Dictionary<string, object> rowDict = new Dictionary<string, object>();
                foreach (DataColumn column in dataTable.Columns)
                {
                    rowDict[column.ColumnName] = row[column];
                }
                rows.Add(rowDict);
            }

            var result = new Dictionary<string, object>
            {
                { dataTable.TableName, rows }
            };

            return result;
        }
        public static string TransformDataToJson(string tblName, DataSet GlobalOperatorDS)
        {
            var result = TransformDataToDictionary(tblName, GlobalOperatorDS);
            return JsonConvert.SerializeObject(result, Newtonsoft.Json.Formatting.Indented);
        }

        //public static NotificationDto CreateNotificationDto(Dictionary<string, object> dictionary)
        //{
        //    var notification = new NotificationDto();

        //    if (dictionary.TryGetValue("NOTIFICATION", out object notificationObj) && notificationObj is Dictionary<string, object> notificationDict)
        //    {
        //        notificationDict.TryGetValue("NOTIFICATION_SEQ", out object seqValue);
        //        notification.NOTIFICATION_SEQ = seqValue as string;

        //        notificationDict.TryGetValue("NOTIFICATION_CODE", out object codeValue);
        //        notification.NOTIFICATION_CODE = codeValue as string;

        //        notificationDict.TryGetValue("NOTIFICATION_DESC", out object descValue);
        //        notification.NOTIFICATION_DESC = descValue as string;

        //        notificationDict.TryGetValue("NOTIFICATION_DBDESC", out object dbDescValue);
        //        notification.NOTIFICATION_DBDESC = dbDescValue as string;

        //        notificationDict.TryGetValue("NOTIFICATION_INPRG", out object inPrgValue);
        //        notification.NOTIFICATION_INPRG = inPrgValue as string;

        //        notificationDict.TryGetValue("NOTIFICATION_TYPE", out object typeValue);
        //        notification.NOTIFICATION_TYPE = typeValue as string;
        //    }

        //    return notification;
        //}



    }
}
