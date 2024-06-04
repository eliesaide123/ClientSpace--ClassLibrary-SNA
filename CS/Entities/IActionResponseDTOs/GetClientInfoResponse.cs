using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.IActionResponseDTOs
{
    public class GetClientInfoResponse
    {
        public Person? Person { get; set; }
        public List<ProductClientnfoDto>? Products { get; set; }
        public List<CodesClientInfoDto>? Codes { get; set; }

    }
}
