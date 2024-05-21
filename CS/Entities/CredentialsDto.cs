using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class CredentialsDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ClientType { get; set; } = "PH";
        public string UserID { get; set; } = string.Empty;
        public bool IsFirstLogin { get; set; } = false;
        public bool IsAuthenticated { get; set; } = false;
        public string? SessionID { get; set; }
    }
}
