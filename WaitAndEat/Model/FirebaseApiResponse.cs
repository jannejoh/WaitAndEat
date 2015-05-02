using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaitAndEat.Model
{
    // Generated with http://json2csharp.com

    public class User
    {
        public string provider { get; set; }
        public string uid { get; set; }
        public string id { get; set; }
        public string md5_hash { get; set; }
        public string sessionKey { get; set; }
        public string email { get; set; }
        public bool isTemporaryPassword { get; set; }
    }

    public class Error
    {
        public string code { get; set; }
        public string message { get; set; }
    }

    public class FirebaseApiResponse
    {
        public User user { get; set; }
        public string token { get; set; }
        public Error error { get; set; }
        
        public bool isError()
        {
            return error != null;
        }
    }

    public class CreateResponse
    {
        public string name { get; set; }
    }
}
