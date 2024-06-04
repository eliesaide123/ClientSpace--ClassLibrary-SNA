using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.IActionResponseDTOs
{
    public class LoginUserResponse
    {
        public ResponseCredentialsDto? Credentials { get; set; }
        public Dictionary<string, string>? Errors { get; set; }
    }
}
