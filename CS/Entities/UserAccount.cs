using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class UserAccount : QuestionAnswer
    {
        public string? Username {  get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
        public string? Mobile { get; set; }
        public int UserLang {  get; set; }
        public string? ContactScenario {  get; set; }
        public string? RegType { get; set; }

    }
}
