using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class BaseResultDto
    {
        public string? SessionID { get; set; }
        public List<ErrorDescriptor>? Errors { get; set; }
    }


    public partial class ErrorDescriptor
    {
        public string? Code { get; set; }
        public string? Description { get; set; }
    }

}