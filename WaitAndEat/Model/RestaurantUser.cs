using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaitAndEat.Model
{
    class RestaurantUser
    {
        private readonly TimeSpan TokenMaxAge = new TimeSpan(24, 0, 0);

        public RestaurantUser(string id, string email, string password, string token)
        {
            this.id = id;
            this.email = email;
            this.password = password;
            this.token = token;
            this.tokenCreated = DateTime.Now;
        }

        public string id { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public string token { get; set; }
        public DateTime tokenCreated { get; set; }

        public bool isTokenExpired() 
        {
            return tokenCreated == null || DateTime.Now.Subtract(tokenCreated) > TokenMaxAge;
        }
    }
}
