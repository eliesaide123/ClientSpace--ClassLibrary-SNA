using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class CredentialsDto: SessionDto
    {
        public string? Username { get; set; } = string.Empty;
        public string? Password { get; set; } = string.Empty;
        public string? ClientType { get; set; } = "P";
        public bool? IsFirstLogin { get; set; } = false;
        public bool? IsAuthenticated { get; set; } = false;
        public string? SessionID { get; set; }
    }

    public class ResponseCredentialsDto
    {
        public string? Username { get; set; } = string.Empty;
        public string? Password { get; set; } = string.Empty;
        public string? ClientType { get; set; } = "P";
        public bool? IsFirstLogin { get; set; } = false;
        public bool? IsAuthenticated { get; set; } = false;
    }

}
