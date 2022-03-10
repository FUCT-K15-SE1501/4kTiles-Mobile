using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    [Serializable]
    public class SongInfo
    {
        public int songId;
        public string songName;
        public string author;
        public int creatorId;
        public string creatorName;
        public int bpm;
        public string notes;
        public DateTime releaseDate;
        public DateTime updatedDate;
        public List<string> genres;
    }
    [Serializable]
    public class SongListResponse
    {
        public int errorCode;
        public List<SongInfo> data;
    }
    [Serializable]
    public class SongResponse
    {
        public int errorCode;
        public SongInfo data;
    }
}
