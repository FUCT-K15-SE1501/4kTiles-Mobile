using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public class Note
    {
        // relative to current row [1,2,3,4]
        public int Position { get; set; }
        // [C,D,E,F,G,A,B] implement pitch
        public char NoteType { get; set; }
        public bool IsSilent { get; set; } = false;
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