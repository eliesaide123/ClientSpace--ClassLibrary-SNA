using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BLC
{
    public class SessionManager : ISessionManager
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SessionManager(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public void SetSessionValue(string key, string value)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(value);
            _httpContextAccessor.HttpContext.Session.Set(key, byteArray);
        }
    }
}
