using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Transposer
{
    public enum Letter
    {
        A = 0,
        B = 1,
        C = 2,
        D = 3,
        E = 4,
        F = 5,
        G = 6
    };

    public enum Length
    {
        SemiBreve = 32,
        Minim = 16,
        Crochet = 8,
        Quaver = 4,
        SemiQuaver = 2,
        DemiSemiQuaver = 1
    };

    public enum Sharp
    {
        Flat = -1,
        Natural = 0,
        Sharp = 1
    };

    class Note
    {
        public int Octave;
        public Letter Name;
        public Length Duration;
        public bool Dotted;
        public Sharp Accidental;

        /// <summary>
        /// Creators
        /// </summary>
        public Note() { }
        public Note(Letter Name, Length Duration, int Octave, Sharp Accidental = Sharp.Natural, bool Dotted = false)
        {
            this.Name = Name;
            this.Duration = Duration;
            this.Octave = Octave;
            this.Dotted = Dotted;
            this.Accidental = Accidental;
        }

        public override string ToString()
        {
            string output;
            switch (this.Accidental)
            {
                case (Sharp.Sharp):
                    output = this.Name + "#";
                    break;
                default:
                    output = this.Name.ToString();
                    break;
                case (Sharp.Flat):
                    output = (this.Name -1) + "#";
                    break;
            }

            return output + (this.Octave + 5).ToString();
        }

        public Note Transpose(int Change)
        {
            var newNote = new Note();

            newNote.Duration = this.Duration;
            newNote.Octave = this.Octave;
            newNote.Dotted = this.Dotted;

            int value = this.GetValue() + Change;

            while (value >= 12)
            {
                value -= 12;
                newNote.Octave++;
            }

            while (value < 0)
            {
                value += 12;
                newNote.Octave--;
            }

            switch (value)
            {
                case 0:
                    newNote.Name = Letter.A;
                    break;
                case 1:
                case 2:
                    newNote.Name = Letter.B;
                    break;
                case 3:
                    newNote.Name = Letter.C;
                    break;
                case 4:
                case 5:
                    newNote.Name = Letter.D;
                    break;
                case 6:
                case 7:
                    newNote.Name = Letter.E;
                    break;
                case 8:
                    newNote.Name = Letter.F;
                    break;
                case 9:
                case 10:
                    newNote.Name = Letter.G;
                    break;
                case 11:
                    newNote.Name = Letter.A;
                    newNote.Octave++;
                    break;
                default:
                    break;
            }

            switch (value)
            {
                case 1:
                case 4:
                case 6:
                case 9:
                case 11:
                    newNote.Accidental = Sharp.Flat;
                    break;
                default:
                    break;
            }

            return newNote;
        }

        public int GetValue()
        {
            int value = (int)this.Name;

            switch (this.Name)
            {
                case Letter.A:
                    value = 0;
                    break;
                case Letter.B:
                    value = 2;
                    break;
                case Letter.C:
                    value = 3;
                    break;
                case Letter.D:
                    value = 5;
                    break;
                case Letter.E:
                    value = 7;
                    break;
                case Letter.F:
                    value = 8;
                    break;
                case Letter.G:
                    value = 10;
                    break;
            }

            if (this.Accidental == Sharp.Sharp) value += 1;
            if (this.Accidental == Sharp.Flat) value -= 1;
            return value;
        }

    }
}
