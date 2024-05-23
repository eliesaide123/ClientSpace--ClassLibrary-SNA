using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLC
{
    public interface ISessionManager
    {
        void SetSessionValue(string key, string value);
    }
}
