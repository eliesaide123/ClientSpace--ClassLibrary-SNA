using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Person
    {
        public Int32 PIN { get; set; }
        public string? Age { get; set; }
        public string? Marital { get; set; }
        public string? Per_Title { get; set; }
        public string? FirstName { get; set; }
        public string? Father { get; set; }
        public string? Family { get; set; }
        public string? FullName { get; set; }
        public string? Profession { get; set; }
        public string? Address { get; set; }
        public string? EntityType { get; set; }
        public int? DOB_Day { get; set; }
        public int? DOB_Month { get; set; }
        public int? DOB_Year { get; set; }
        public bool? HasRequest { get; set; }
        public bool? HasUnpaid { get; set; }
        public bool? HasClaims { get; set; }
        public bool? HasRenewal { get; set; }
        public bool? HasFresh { get; set; }
        public bool? KYC { get; set; }
        public bool? ShowProfile { get; set; }
        public bool? ShowMissing { get; set; }
        public bool? AgentSOA { get; set; }
        public bool? RPSEnabled { get; set; }
        public string? YearMonth { get; set; } 
        public string? KYCMSG { get; set; }
        public string? CONVERT_DATA { get; set; }
    }
}
