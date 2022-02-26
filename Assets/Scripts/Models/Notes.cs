using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public class NoteType
    {
        private float? _volume;

        public int MidiKey { get; set; }
        public float Delay { get; set; } = 0;
        public float? Length { get; set; }
        public float? Volume 
        {
            get => _volume;
            set 
            {
                if (value < 0)
                    _volume = 0;
                else if (value > 1)
                    _volume = 1;
                else
                    _volume = value;
            }
        }
    }

    public class Note
    {
        // relative to current row [1,2,3,4]
        public int Position { get; set; }
        // [C,D,E,F,G,A,B] implement pitch
        public List<NoteType> NoteType { get; set; }
        public bool TouchOptional { get; set; } = false;
    }

    public class Row
    {
        // row index
        public int Position { get; set; }
        public List<Note> Notes { get; set; }
    }

    // public class Song
    // {
    //     public List<Row> Rows { get; set; }
    // }
}