using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class CredentialsDto : BaseResultDto
    {
        public string? Username { get; set; } = string.Empty;
        public string? Password { get; set; } = string.Empty;
        public string? ClientType { get; set; } = "PH";
        public string? UserID { get; set; } = string.Empty;
        public bool? IsFirstLogin { get; set; } = false;
        public bool? IsAuthenticated { get; set; } = false;

    }

   
}
