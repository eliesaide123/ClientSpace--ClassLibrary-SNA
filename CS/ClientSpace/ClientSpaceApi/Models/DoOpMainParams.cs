using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClientSpaceApi.Models
{
    public class DoOpMainParams
    {
        // For the JSON file to work properly make sure that the parametized parameters has the value as follows: 
        // "Fill_RoleID" the RoleID property should be written the same way as present in The DTO
        public CredentialsDto Credentials { get; set; } = null;
        public string RoleID { get; set; } = null;
        public int GridSize { get; set; } = 0;
        public string Direction { get; set; }
        public int StartIndex { get; set; } = 0;
        public Int32 PolSerNo { get; set; } = 0;
    }
}