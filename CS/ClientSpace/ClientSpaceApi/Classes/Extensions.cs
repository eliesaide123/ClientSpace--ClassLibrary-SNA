using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace ClientSpaceApi.Classes
{
    public static class Extensions
    {
        public static NameValueCollection ToNameValueCollection(this IDictionary<string, string> dict)
        {
            var nameValueCollection = new NameValueCollection();
            foreach (var kvp in dict)
            {
                nameValueCollection.Add(kvp.Key, kvp.Value);
            }
            return nameValueCollection;
        }
    }
}