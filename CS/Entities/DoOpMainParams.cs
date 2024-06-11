using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class DoOpMainParams
    {
        // For the JSON file to work properly make sure that the parametized parameters has the value as follows: 
        // "Fill_RoleID" the RoleID property should be written the same way as present in The DTO
        public CredentialsDto? Credentials { get; set; }
        public string? RoleID { get; set; }
        public int? GridSize { get; set; }
        public string? Direction { get; set; }
        public int? StartIndex { get; set; }
        public Int32? PolSerNo { get; set; }
    }
}
