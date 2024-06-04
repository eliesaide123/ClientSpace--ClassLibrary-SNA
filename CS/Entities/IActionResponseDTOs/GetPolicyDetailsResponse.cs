using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.IActionResponseDTOs
{
    public class GetPolicyDetailsResponse
    {
        public List<PolcomPolicyDetailsDto>? Polcom { get;set; }
        public List<CodesPolicyDetailsDto>? Codes { get;set; }
    }
}
