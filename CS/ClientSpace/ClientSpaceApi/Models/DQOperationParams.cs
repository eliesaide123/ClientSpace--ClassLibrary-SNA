using DQOperator;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientSpaceApi.Models
{
    public class DQOperationParams
    {
        public string taskName { get; set; }
        public string jsonPath { get; set; }
        public string doOpParams { get; set; }
    }
}
