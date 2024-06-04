using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.IActionResponseDTOs
{
    public class CheckRolesResponse : BaseResultDto
    {
        public bool? Error { get; set; }
        public cUserIdent? SUCCESS { get; set; }
    }
}
