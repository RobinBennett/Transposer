using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Toub.Sound.Midi;

namespace Transposer
{
    public partial class Form1 : Form
    {
        // List<Note> myNotes = LoadNotes();
        List<Note> myNotes = OdetoJoy();

        int playPosition = 0;
        int currentNoteDuration = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private List<Note> Transpose(List<Note> source, int change)
        {
            var target = new List<Note>();

            foreach (var loopNote in source)
            {
                target.Add(loopNote.Transpose(change));
            }

            return target;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

            var transposed = Transpose(myNotes, 5);

            DrawMusic(myNotes, panel1, 20);
            DrawMusic(transposed, panel2, -5);
        }

        private void DrawMusic(List<Note> myNotes, Panel thePanel, int NoteOffset)
        {
            Graphics g = thePanel.CreateGraphics();
            DrawMusic(myNotes, g, NoteOffset);
        }

        private void DrawMusic(List<Note> myNotes, Graphics g, int NoteOffset)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            Pen myPen = new Pen(Color.Black, 1f);
            Pen thickPen = new Pen(Color.Black, 2f);
            Pen reallyThickPen = new Pen(Color.Black, 5f);
            Brush mybrush = new SolidBrush(Color.Black);

            int topMargin = 50;
            int leftMargin = 50;
            int rightMargin = 50;
            int lineHeight = 10;

            string clef = "BaseClef";
            if (NoteOffset == 20) clef = "TrebleClef";
            g.DrawImage(imageList1.Images[clef], leftMargin - 5, topMargin - 10, 40, 65);


            // Draw lines
            for (int lineNum = 0; lineNum < 5; lineNum++)
            {
                g.DrawLine(myPen, leftMargin, topMargin + lineNum * lineHeight, panel1.Width - rightMargin, topMargin + lineNum * lineHeight);
            }


            // Draw notes
            int position = leftMargin + 40;

            foreach (Note loopNote in myNotes)
            {
                int notegap = (int)loopNote.Duration * 8;


                int noteHeight = NoteOffset - ((int)loopNote.Name + loopNote.Octave * 7) * (lineHeight / 2) + topMargin;


                //ledger lines
                if (noteHeight < topMargin)
                {
                    int ledgerHeight = topMargin;
                    while (ledgerHeight > noteHeight)
                    {
                        g.DrawLine(myPen, position - 3, ledgerHeight, position + 15, ledgerHeight);
                        ledgerHeight = ledgerHeight - lineHeight;
                    }

                }
                else if (noteHeight > topMargin + lineHeight * 4)
                {
                    int ledgerHeight = topMargin + lineHeight * 4;
                    while (ledgerHeight < noteHeight + 10)
                    {
                        g.DrawLine(myPen, position - 3, ledgerHeight, position + 15, ledgerHeight);
                        ledgerHeight = ledgerHeight + lineHeight;
                    }
                }

                //sharps and flats
                if (loopNote.Accidental == Sharp.Sharp)
                {
                    g.DrawImage(imageList2.Images[2], position - 15, noteHeight - 5);
                }
                if (loopNote.Accidental == Sharp.Flat)
                {
                    g.DrawImage(imageList2.Images[0], position - 15, noteHeight - 5);
                }

                // blobs    
                switch (loopNote.Duration)
                {
                    case Length.Minim:
                    case Length.SemiBreve:
                        g.DrawEllipse(thickPen, position, noteHeight + 1, 12, 8);
                        break;

                    case Length.Crochet:
                    case Length.Quaver:
                    case Length.SemiQuaver:
                    case Length.DemiSemiQuaver:
                        g.FillEllipse(mybrush, position, noteHeight + 1, 12, 8);
                        break;
                }
                // lines
                // if its not a semibreve 
                if (loopNote.Duration != Length.SemiBreve)
                {
                    if (noteHeight < topMargin + (lineHeight * 2))
                    {
                        //ones that go down
                        g.DrawLine(thickPen, position, noteHeight + 5, position, noteHeight + 29);
                        if (loopNote.Duration == Length.Quaver | loopNote.Duration == Length.SemiQuaver | loopNote.Duration == Length.DemiSemiQuaver)
                        {
                            g.DrawLine(thickPen, position, noteHeight + 29, position + 12, noteHeight + 15);
                            if (loopNote.Duration == Length.SemiQuaver)
                            {
                                g.DrawLine(thickPen, position, noteHeight + 32, position + 12, noteHeight + 18);
                            }
                        }
                    }
                    else
                    {
                        //sticks that go up
                        g.DrawLine(thickPen, position + 12, noteHeight + 3, position + 12, noteHeight - 27);
                        if (loopNote.Duration == Length.Quaver | loopNote.Duration == Length.SemiQuaver | loopNote.Duration == Length.DemiSemiQuaver)
                        {
                            g.DrawLine(thickPen, position + 12, noteHeight - 27, position + 22, noteHeight - 15);
                            if (loopNote.Duration == Length.SemiQuaver)
                            {
                                g.DrawLine(thickPen, position + 12, noteHeight - 22, position + 22, noteHeight - 10);
                            }
                        }
                    }

                }

                if (loopNote.Dotted == true)
                {
                    g.FillEllipse(mybrush, position + 15, noteHeight + 3, 4, 4);
                    position = position + (notegap / 2);

                }
                position = position + notegap;
            }
        }
        private static List<Note> Somewhere()
        {
            List<Note> myNotes = new List<Note>();
            myNotes.Add(new Note(Letter.B, Length.Minim, -1, Sharp.Flat));
            myNotes.Add(new Note(Letter.A, Length.Minim, 0, Sharp.Flat));
            myNotes.Add(new Note(Letter.G, Length.Crochet, -1, Dotted: true));
            myNotes.Add(new Note(Letter.E, Length.Quaver, -1, Sharp.Flat));
            myNotes.Add(new Note(Letter.C, Length.Minim, -1));
            myNotes.Add(new Note(Letter.C, Length.Crochet, 0));
            myNotes.Add(new Note(Letter.B, Length.Crochet, 0, Sharp.Flat));
            myNotes.Add(new Note(Letter.A, Length.Crochet, 0, Sharp.Flat));
            myNotes.Add(new Note(Letter.G, Length.Crochet, -1, Dotted: true));
            myNotes.Add(new Note(Letter.E, Length.Quaver, -1, Sharp.Flat));
            myNotes.Add(new Note(Letter.C, Length.Minim, -1));

            //myNotes.Add(new Note(Letter.D, Length.Crochet, -1, Dotted: true));
            //myNotes.Add(new Note(Letter.E, Length.Quaver, -1, Sharp.Flat));
            //myNotes.Add(new Note(Letter.D, Length.Crochet, -1));
            //myNotes.Add(new Note(Letter.E, Length.Crochet, -1, Sharp.Flat));
            //myNotes.Add(new Note(Letter.E, Length.Crochet, -1));
            //myNotes.Add(new Note(Letter.D, Length.Crochet, -1, Dotted: true));
            //myNotes.Add(new Note(Letter.B, Length.Quaver, -1, Sharp.Flat));
            //myNotes.Add(new Note(Letter.C, Length.Minim, -1));
            //myNotes.Add(new Note(Letter.A, Length.Crochet, 0, Sharp.Flat, Dotted: true));
            //myNotes.Add(new Note(Letter.A, Length.Quaver, 0, Sharp.Flat));
            //myNotes.Add(new Note(Letter.B, Length.Minim, -1, Sharp.Flat));
            //myNotes.Add(new Note(Letter.E, Length.Quaver, -2, Sharp.Flat));
            //myNotes.Add(new Note(Letter.D, Length.Crochet, -2, Dotted: true));
            //myNotes.Add(new Note(Letter.D, Length.Minim, -2));
            return myNotes;
        }

        private static List<Note> Titanic()
        {
            List<Note> myNotes = new List<Note>();
            myNotes.Add(new Note(Letter.C, Length.Crochet, -1));
            myNotes.Add(new Note(Letter.A, Length.Minim, 0));
            myNotes.Add(new Note(Letter.G, Length.Crochet, -1));
            myNotes.Add(new Note(Letter.F, Length.Quaver, -1));
            myNotes.Add(new Note(Letter.E, Length.Quaver, -1));
            myNotes.Add(new Note(Letter.E, Length.Minim, -1));
            myNotes.Add(new Note(Letter.A, Length.Crochet, 0));
            myNotes.Add(new Note(Letter.B, Length.Crochet, 0, Sharp.Flat));
            myNotes.Add(new Note(Letter.E, Length.Minim, -1));
            myNotes.Add(new Note(Letter.E, Length.Crochet, -1, Sharp.Flat));
            myNotes.Add(new Note(Letter.D, Length.Crochet, -1));
            myNotes.Add(new Note(Letter.C, Length.Crochet, -1));
            myNotes.Add(new Note(Letter.F, Length.Minim, -1));
            myNotes.Add(new Note(Letter.C, Length.Crochet, -1));
            myNotes.Add(new Note(Letter.C, Length.Crochet, -1));
            myNotes.Add(new Note(Letter.D, Length.Minim, -1));
            myNotes.Add(new Note(Letter.E, Length.Crochet, -1));
            myNotes.Add(new Note(Letter.D, Length.Minim, -1));
            myNotes.Add(new Note(Letter.C, Length.Minim, -1));
            return myNotes;
        }

        private static List<Note> Valkyries()
        {
            List<Note> myNotes = new List<Note>();
            myNotes.Add(new Note(Letter.F, Length.Quaver, -2,Sharp.Sharp));
            myNotes.Add(new Note(Letter.B, Length.Quaver, -1, Dotted:true));
            myNotes.Add(new Note(Letter.F, Length.SemiQuaver, -2));
            myNotes.Add(new Note(Letter.B, Length.Quaver, -1));
            myNotes.Add(new Note(Letter.D, Length.Crochet, -1, Dotted:true));
            myNotes.Add(new Note(Letter.B, Length.Crochet, -1, Dotted:true));
          
            return myNotes;
        }
        private static List<Note> OdetoJoy()
        {
            List<Note> myNotes = new List<Note>();
            myNotes.Add(new Note(Letter.A, Length.Quaver, -1));
            myNotes.Add(new Note(Letter.A, Length.Quaver, -1));
            myNotes.Add(new Note(Letter.B, Length.Quaver, -1, Sharp.Flat));
            myNotes.Add(new Note(Letter.C, Length.Quaver, -1));
            myNotes.Add(new Note(Letter.C, Length.Quaver, -1));
            myNotes.Add(new Note(Letter.B, Length.Quaver, -1, Sharp.Flat));
            myNotes.Add(new Note(Letter.A, Length.Quaver, -1));
            myNotes.Add(new Note(Letter.G, Length.Quaver, -2));
            myNotes.Add(new Note(Letter.F, Length.Quaver, -2));
            myNotes.Add(new Note(Letter.F, Length.Quaver, -2));
            myNotes.Add(new Note(Letter.G, Length.Quaver, -2));
            myNotes.Add(new Note(Letter.A, Length.Quaver, -1));
            myNotes.Add(new Note(Letter.A, Length.Quaver, -1));
            myNotes.Add(new Note(Letter.G, Length.Quaver, -2));
            myNotes.Add(new Note(Letter.G, Length.Crochet, -2));

            myNotes.Add(new Note(Letter.A, Length.Quaver, -1));
            myNotes.Add(new Note(Letter.A, Length.Quaver, -1));
            myNotes.Add(new Note(Letter.B, Length.Quaver, -1, Sharp.Flat));
            myNotes.Add(new Note(Letter.C, Length.Quaver, -1));
            myNotes.Add(new Note(Letter.C, Length.Quaver, -1));
            myNotes.Add(new Note(Letter.B, Length.Quaver, -1, Sharp.Flat));
            myNotes.Add(new Note(Letter.A, Length.Quaver, -1));
            myNotes.Add(new Note(Letter.G, Length.Quaver, -2));
            myNotes.Add(new Note(Letter.F, Length.Quaver, -2));
            myNotes.Add(new Note(Letter.F, Length.Quaver, -2));
            myNotes.Add(new Note(Letter.G, Length.Quaver, -2));
            myNotes.Add(new Note(Letter.A, Length.Quaver, -1));
            myNotes.Add(new Note(Letter.G, Length.Quaver, -2));
            myNotes.Add(new Note(Letter.F, Length.Quaver, -2));
            myNotes.Add(new Note(Letter.F, Length.Crochet, -2));
            return myNotes;
        }


        private static List<Note> WallaceAndGromit()
        {
            List<Note> myNotes = new List<Note>();
            myNotes.Add(new Note(Letter.E, Length.Crochet, -1));
            myNotes.Add(new Note(Letter.D, Length.Quaver, -1));
            myNotes.Add(new Note(Letter.C, Length.Quaver, -1));
            myNotes.Add(new Note(Letter.E, Length.Crochet, -1));
            myNotes.Add(new Note(Letter.D, Length.Quaver, -1));
            myNotes.Add(new Note(Letter.C, Length.Quaver, -1));
            myNotes.Add(new Note(Letter.E, Length.Quaver, -1));
            myNotes.Add(new Note(Letter.B, Length.Quaver, -1));
            myNotes.Add(new Note(Letter.B, Length.Minim, -1, Dotted:true));

            myNotes.Add(new Note(Letter.F, Length.Quaver, -1));
            myNotes.Add(new Note(Letter.E, Length.Quaver, -1));
            myNotes.Add(new Note(Letter.F, Length.Quaver, -1));
            myNotes.Add(new Note(Letter.G, Length.Crochet, -1));
            myNotes.Add(new Note(Letter.F, Length.Quaver, -1));
            myNotes.Add(new Note(Letter.E, Length.Quaver, -1));
            myNotes.Add(new Note(Letter.C, Length.Quaver, -1));

            myNotes.Add(new Note(Letter.E, Length.Crochet, -1));
            myNotes.Add(new Note(Letter.D, Length.Quaver, -1));
            myNotes.Add(new Note(Letter.C, Length.Quaver, -1));
            myNotes.Add(new Note(Letter.E, Length.Crochet, -1));
            myNotes.Add(new Note(Letter.D, Length.Quaver, -1));
            myNotes.Add(new Note(Letter.C, Length.Quaver, -1));
            myNotes.Add(new Note(Letter.E, Length.Quaver, -1));
            myNotes.Add(new Note(Letter.B, Length.Quaver, -1));
            myNotes.Add(new Note(Letter.B, Length.Minim, -1, Dotted: true));

            myNotes.Add(new Note(Letter.F, Length.Quaver, -1));
            myNotes.Add(new Note(Letter.E, Length.Quaver, -1));
            myNotes.Add(new Note(Letter.F, Length.Quaver, -1));
            myNotes.Add(new Note(Letter.G, Length.Crochet, -1));
            myNotes.Add(new Note(Letter.F, Length.Quaver, -1));
            myNotes.Add(new Note(Letter.G, Length.Quaver, -1));
            myNotes.Add(new Note(Letter.C, Length.Quaver, 0));
            return myNotes;
        }



        private static List<Note> MusicOfTheNight()
        {
            List<Note> myNotes = new List<Note>();
            myNotes.Add(new Note(Letter.E, Length.Quaver, -1));
            myNotes.Add(new Note(Letter.E, Length.Quaver, -1));
            myNotes.Add(new Note(Letter.D, Length.Quaver, -1));
            myNotes.Add(new Note(Letter.E, Length.Quaver, -1));
            myNotes.Add(new Note(Letter.F, Length.Crochet, -1));
            myNotes.Add(new Note(Letter.C, Length.Quaver, -1));
            myNotes.Add(new Note(Letter.B, Length.Quaver, -1));

            myNotes.Add(new Note(Letter.E, Length.Quaver, -1));
            myNotes.Add(new Note(Letter.E, Length.Quaver, -1));
            myNotes.Add(new Note(Letter.D, Length.Quaver, -1));
            myNotes.Add(new Note(Letter.E, Length.Quaver, -1));
            myNotes.Add(new Note(Letter.F, Length.Crochet, -1));
            myNotes.Add(new Note(Letter.C, Length.Crochet, -1));

            myNotes.Add(new Note(Letter.E, Length.Quaver, -1));
            myNotes.Add(new Note(Letter.E, Length.Quaver, -1));
            myNotes.Add(new Note(Letter.D, Length.Quaver, -1));
            myNotes.Add(new Note(Letter.E, Length.Quaver, -1));
            myNotes.Add(new Note(Letter.F, Length.Quaver, -1));
            myNotes.Add(new Note(Letter.C, Length.Quaver, -1));
            myNotes.Add(new Note(Letter.A, Length.Quaver, -1));
            myNotes.Add(new Note(Letter.F, Length.Quaver, -2, Sharp.Sharp));
            myNotes.Add(new Note(Letter.B, Length.Crochet, -1));

            myNotes.Add(new Note(Letter.B, Length.Crochet, 0));
            myNotes.Add(new Note(Letter.B, Length.Quaver, 0));
            myNotes.Add(new Note(Letter.F, Length.Quaver, -1));
            myNotes.Add(new Note(Letter.F, Length.Quaver, -1, Sharp.Sharp));
            myNotes.Add(new Note(Letter.F, Length.Quaver, -1));
            myNotes.Add(new Note(Letter.E, Length.Quaver, -1));
            myNotes.Add(new Note(Letter.D, Length.Quaver, -1));
            myNotes.Add(new Note(Letter.G, Length.Quaver, -2));
            myNotes.Add(new Note(Letter.A, Length.Quaver, -1));
            myNotes.Add(new Note(Letter.G, Length.SemiBreve, -2));

            return myNotes;
        }

        private static List<Note> LoadNotes()
        {
            List<Note> myNotes = new List<Note>();

            // Hark the herald...
            myNotes.Add(new Note(Letter.C, Length.Crochet, -1));
            myNotes.Add(new Note(Letter.F, Length.Crochet, -1));
            myNotes.Add(new Note(Letter.F, Length.Crochet, -1, Dotted: true));
            myNotes.Add(new Note(Letter.E, Length.Quaver, -1, Sharp.Sharp));
            myNotes.Add(new Note(Letter.F, Length.Crochet, -1, Sharp.Flat));
            myNotes.Add(new Note(Letter.A, Length.Crochet, 0));
            myNotes.Add(new Note(Letter.A, Length.Crochet, 0));
            myNotes.Add(new Note(Letter.G, Length.Minim, -1, Dotted: true));

            // Ride of the Valkyries
            myNotes.Add(new Note(Letter.D, Length.Quaver, -1, Dotted: true));
            myNotes.Add(new Note(Letter.A, Length.SemiQuaver, -1));
            myNotes.Add(new Note(Letter.D, Length.Quaver, -1));
            myNotes.Add(new Note(Letter.F, Length.Crochet, -1, Dotted: true));
            myNotes.Add(new Note(Letter.D, Length.Crochet, -1, Dotted: true));
            myNotes.Add(new Note(Letter.F, Length.Quaver, -1, Dotted: true));
            myNotes.Add(new Note(Letter.D, Length.SemiQuaver, -1));
            myNotes.Add(new Note(Letter.F, Length.Quaver, -1));
            myNotes.Add(new Note(Letter.A, Length.Crochet, 0, Dotted: true));
            myNotes.Add(new Note(Letter.F, Length.Crochet, -1, Dotted: true));
            return myNotes;
        }

        private static List<Note> LoadScale()
        {
            List<Note> myNotes = new List<Note>();

            //myNotes.Add(new Note(Letter.A, Length.Quaver, -1));
            //myNotes.Add(new Note(Letter.B, Length.Quaver, -1));
            //myNotes.Add(new Note(Letter.C, Length.Quaver, -1));
            //myNotes.Add(new Note(Letter.D, Length.Quaver, -1));
            //myNotes.Add(new Note(Letter.E, Length.Quaver, -1, Sharp.Flat)); 
            //myNotes.Add(new Note(Letter.E, Length.Quaver, -1));
            //myNotes.Add(new Note(Letter.F, Length.Quaver, -1));
            //myNotes.Add(new Note(Letter.G, Length.Quaver, -1));

            myNotes.Add(new Note(Letter.C, Length.Quaver, 0));
            myNotes.Add(new Note(Letter.D, Length.Quaver, 0));
            myNotes.Add(new Note(Letter.E, Length.Quaver, 0));
            myNotes.Add(new Note(Letter.F, Length.Quaver, 0));
            myNotes.Add(new Note(Letter.G, Length.Quaver, 0));
            myNotes.Add(new Note(Letter.A, Length.Quaver, 0));
            myNotes.Add(new Note(Letter.B, Length.Quaver, 0));
            myNotes.Add(new Note(Letter.C, Length.Crochet, 1));
            //this bit plays a scale but does not write one

            //myNotes.Add(new Note(Letter.A, Length.Quaver, 1));
            //myNotes.Add(new Note(Letter.B, Length.Quaver, 1));
            //myNotes.Add(new Note(Letter.C, Length.Quaver, 1));
            //myNotes.Add(new Note(Letter.D, Length.Quaver, 1));
            //myNotes.Add(new Note(Letter.E, Length.Quaver, 1));
            //myNotes.Add(new Note(Letter.F, Length.Quaver, 1));
            //myNotes.Add(new Note(Letter.G, Length.Quaver, 1));

            return myNotes;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            MidiPlayer.OpenMidi();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            MidiPlayer.CloseMidi();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG|All files (*.*)|*.*";

            var result = saveFileDialog1.ShowDialog(this);
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                var bitmap = new Bitmap(this.panel2.Width, this.panel2.Height);
                var g = Graphics.FromImage(bitmap);
                var transposed = Transpose(myNotes, 5);
                DrawMusic(transposed, g, -5);


                bitmap.Save(saveFileDialog1.FileName, ImageFormat.Png);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            playPosition = 0;
            currentNoteDuration = 0;
            timer1.Enabled = true;
            timer1.Interval = 50;

            button1.Enabled = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (currentNoteDuration == 0)
            {
                if (playPosition >= 1)
                {
                    Note previousNote = myNotes[playPosition - 1];
                    MidiPlayer.Play(new NoteOff(0, 1, previousNote.ToString(), 100));
                }

                Note currentNote = myNotes[playPosition];
                MidiPlayer.Play(new NoteOn(0, 1, currentNote.ToString(), 100));

                Console.WriteLine(currentNote.ToString());

                currentNoteDuration = (int)currentNote.Duration;
                if (currentNote.Dotted) currentNoteDuration *= (int)1.5;
                currentNoteDuration--;

                playPosition++;
                if (playPosition >= myNotes.Count)
                {
                    timer1.Enabled = false;
                    button1.Enabled = true;
                }
            }
            else
            {
                currentNoteDuration--;
            }
        }
    }
}
