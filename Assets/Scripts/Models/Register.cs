using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    [Serializable]
    public class Register
    {
        public string userName;
        public string password;
        public string email;
    }

    [Serializable]
    public class RegisterResponse
    {
        public string data;
    }
}
