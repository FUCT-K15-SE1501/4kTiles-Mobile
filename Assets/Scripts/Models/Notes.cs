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
        // relative to current row [0,1,2,3]
        public int Position { get; set; }
        // [C,D,E,F,G,A,B] implement pitch
        public List<NoteType> NoteType { get; set; }
        public bool TouchOptional { get; set; } = false;
    }

    public class Row
    {
        // row index (start from 0)
        public int Position { get; set; }
        public List<Note> Notes { get; set; }
    }

    public static class NoteConverter
    {
        public static Dictionary<int, Note> ToDictionary(this IEnumerable<Row> rows)
        {
            var dict = new Dictionary<int, Note>();
            foreach (var row in rows)
            {
                var position = row.Position;
                if (position < 0) continue;

                foreach (var rowNote in row.Notes)
                {
                    var index = rowNote.Position;
                    if (index < 0 || index >= 4) continue;
                    dict[position * 4 + index] = rowNote;
                }
            }
            return dict;
        }

        public static List<Row> TestRow { get; } = new List<Row>()
        {
            new Row()
            {
                Position = 0,
                Notes = new List<Note>()
                {
                    new Note()
                    {
                        Position = 0,
                        NoteType = new List<NoteType>()
                        {
                            new NoteType()
                            {
                                MidiKey = 77
                            }
                        }
                    },
                    new Note()
                    {
                        Position = 0,
                        NoteType = new List<NoteType>()
                        {
                            new NoteType()
                            {
                                MidiKey = 78
                            }
                        }
                    }
                }
            },
            new Row()
            {
                Position = 1,
                Notes = new List<Note>()
                {
                    new Note()
                    {
                        Position = 2,
                        TouchOptional = true,
                        NoteType = new List<NoteType>()
                        {
                            new NoteType()
                            {
                                MidiKey = 77
                            }
                        }
                    },
                    new Note()
                    {
                        Position = 3,
                        NoteType = new List<NoteType>()
                        {
                            new NoteType()
                            {
                                MidiKey = 82
                            }
                        }
                    }
                }
            },
            new Row()
            {
                Position = 3,
                Notes = new List<Note>()
                {
                    new Note()
                    {
                        Position = 2,
                        TouchOptional = true,
                        NoteType = new List<NoteType>()
                        {
                            new NoteType()
                            {
                                MidiKey = 77
                            }
                        }
                    },
                    new Note()
                    {
                        Position = 3,
                        NoteType = new List<NoteType>()
                        {
                            new NoteType()
                            {
                                MidiKey = 82
                            }
                        }
                    }
                }
            },
            new Row()
            {
                Position = 4,
                Notes = new List<Note>()
                {
                    new Note()
                    {
                        Position = 1,
                        NoteType = new List<NoteType>()
                        {
                            new NoteType()
                            {
                                MidiKey = 77
                            }
                        }
                    },
                    new Note()
                    {
                        Position = 3,
                        NoteType = new List<NoteType>()
                        {
                            new NoteType()
                            {
                                MidiKey = 80
                            }
                        }
                    }
                }
            },
            new Row()
            {
                Position = 5,
                Notes = new List<Note>()
                {
                    new Note()
                    {
                        Position = 2,
                        TouchOptional = true,
                        NoteType = new List<NoteType>()
                        {
                            new NoteType()
                            {
                                MidiKey = 75
                            }
                        }
                    },
                    new Note()
                    {
                        Position = 3,
                        TouchOptional = true,
                        NoteType = new List<NoteType>()
                        {
                            new NoteType()
                            {
                                MidiKey = 80
                            }
                        }
                    }
                }
            }
        };
    }

    // public class Song
    // {
    //     public List<Row> Rows { get; set; }
    // }
}