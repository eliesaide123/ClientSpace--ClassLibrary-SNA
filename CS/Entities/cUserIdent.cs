using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class cUserIdent:BaseResultDto
    {
        #region Properties
        public string? UserName { get; set; }
        public string? Pin { get; set; }
        public string? Role { get; set; }
        public string? LoggedDate { get; set; }
        public string? RoleID { get; set; }
        public string? Language { get; set; }
        #endregion
    }

}
