using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.JSONResponseDTOs
{
    public class GetUserAccountResponse
    {
        public UserAccount? UserAccount { get; set; }
        public string[]? Questions {  get; set; }
    }
}
