using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.IActionResponseDTOs
{
    public class LoginUserResponse: BaseResultDto
    {
        public CredentialsDto? Credentials { get; set; }
    }
}
