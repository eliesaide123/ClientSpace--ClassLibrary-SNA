using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClientSpaceApi.Models
{
    public class CredentialsDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ClientType { get; set; } = "P";
        public bool IsFirstLogin { get; set; } = false;
        public bool IsAuthenticated { get; set; } = false;
        public string SessionID { get; set; }
    }
}