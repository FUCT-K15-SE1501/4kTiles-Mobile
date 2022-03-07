using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    [Serializable]
    public class Login
    {
        public string password;
        public string email;
    }
    [Serializable]
    public class LoginResponse
    {
        public string data;
    }
}
