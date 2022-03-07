using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    [Serializable]
    public class NoteType
    {
        public float volume = 1;
        public float delay = 0;
        public int midiKey = 72;
        public float length = 0;
    }

    [Serializable]
    public class Note
    {
        // relative to current row [0,1,2,3]
        public int position;
        public List<NoteType> noteType = new List<NoteType>();
        public bool touchOptional = false;
    }

    [Serializable]
    public class Row
    {
        // row index (start from 0)
        public int position;
        public List<Note> notes = new List<Note>();
    }

    [Serializable]
    public class Song
    {
        public List<Row> rows = new List<Row>();
    }

    public static class NoteConverter
    {
        public static Dictionary<int, Note> ToDictionary(this Song song)
        {
            var dict = new Dictionary<int, Note>();
            foreach (var row in song.rows)
            {
                var position = row.position;
                if (position < 0) continue;

                foreach (var rowNote in row.notes)
                {
                    var index = rowNote.position;
                    if (index < 0 || index >= 4) continue;
                    dict[position * 4 + index] = rowNote;
                }
            }
            return dict;
        }

        public static Song TestSong { get; } = new Song()
        {
            rows = new List<Row>()
            {
                new Row()
                {
                    position = 0,
                    notes = new List<Note>()
                    {
                        new Note()
                        {
                            position = 0,
                            noteType = new List<NoteType>()
                            {
                                new NoteType()
                                {
                                    midiKey = 77
                                }
                            }
                        },
                        new Note()
                        {
                            position = 0,
                            noteType = new List<NoteType>()
                            {
                                new NoteType()
                                {
                                    midiKey = 78
                                }
                            }
                        }
                    }
                },
                new Row()
                {
                    position = 1,
                    notes = new List<Note>()
                    {
                        new Note()
                        {
                            position = 2,
                            touchOptional = true,
                            noteType = new List<NoteType>()
                            {
                                new NoteType()
                                {
                                    midiKey = 77
                                }
                            }
                        },
                        new Note()
                        {
                            position = 3,
                            noteType = new List<NoteType>()
                            {
                                new NoteType()
                                {
                                    midiKey = 82
                                }
                            }
                        }
                    }
                },
                new Row()
                {
                    position = 3,
                    notes = new List<Note>()
                    {
                        new Note()
                        {
                            position = 2,
                            touchOptional = true,
                            noteType = new List<NoteType>()
                            {
                                new NoteType()
                                {
                                    midiKey = 77
                                }
                            }
                        },
                        new Note()
                        {
                            position = 3,
                            noteType = new List<NoteType>()
                            {
                                new NoteType()
                                {
                                    midiKey = 82
                                }
                            }
                        }
                    }
                },
                new Row()
                {
                    position = 4,
                    notes = new List<Note>()
                    {
                        new Note()
                        {
                            position = 1,
                            noteType = new List<NoteType>()
                            {
                                new NoteType()
                                {
                                    midiKey = 77
                                }
                            }
                        },
                        new Note()
                        {
                            position = 3,
                            noteType = new List<NoteType>()
                            {
                                new NoteType()
                                {
                                    midiKey = 80
                                }
                            }
                        }
                    }
                },
                new Row()
                {
                    position = 5,
                    notes = new List<Note>()
                    {
                        new Note()
                        {
                            position = 2,
                            touchOptional = true,
                            noteType = new List<NoteType>()
                            {
                                new NoteType()
                                {
                                    midiKey = 75
                                }
                            }
                        },
                        new Note()
                        {
                            position = 3,
                            touchOptional = true,
                            noteType = new List<NoteType>()
                            {
                                new NoteType()
                                {
                                    midiKey = 80
                                }
                            }
                        }
                    }
                }
            }
        };
    }
}