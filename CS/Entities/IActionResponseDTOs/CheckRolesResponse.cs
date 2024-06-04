using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.IActionResponseDTOs
{
    public class CheckRolesResponse
    {
        public bool? Error { get; set; }
        public NotificationDto? NOTIFICATION { get; set; }
        public CheckRolesSuccessResponseDto? SUCCESS { get; set; }
    }
}
