using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    [Serializable]
    public class Leaderboard
    {
        public int accountId;
        public string userName;
        public int bestScore;
    }

    [Serializable]
    public class LeaderboardResponse
    {
        public List<Leaderboard> data;
    }
}
