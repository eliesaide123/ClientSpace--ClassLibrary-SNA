using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.JSONResponseDTOs
{
    public class GetClientInfoResponseDto
    {
        public Person Person { get; set; }
        public string[][] Products { get; set; }
        public string[][] Codes { get; set; }

    }
}
