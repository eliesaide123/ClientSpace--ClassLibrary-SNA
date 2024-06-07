using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.IActionResponseDTOs
{
    public class GetClientInfoResponse:BaseResultDto
    {
        public Person? Person { get; set; }
        public List<ProductClientInfoDto>? Products { get; set; }
        public List<CodesClientInfoDto>? Codes { get; set; }

    }
}
