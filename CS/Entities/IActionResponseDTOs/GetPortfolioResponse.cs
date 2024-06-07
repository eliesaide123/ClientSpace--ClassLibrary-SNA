using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.IActionResponseDTOs
{
    public class GetPortfolioResponse : BaseResultDto
    {
        public List<PolcomPortfolioDto>? Polcom { get;set; }
    }
}
