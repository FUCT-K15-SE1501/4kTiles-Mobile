using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Models
{
    [Serializable]
    public class Profile
    {
        public int accountId;
        public string userName;
    }
    [Serializable]
    public class ProfileResponse
    {
        public Profile data;
    }
}
