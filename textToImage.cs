using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
namespace ImageAndBytes
{
    class Program {
        static void Main(string[] args) {
            Console.WriteLine("Press 1 to write new data to Bitmap");
            Console.WriteLine("Press 2 to read the data from Bitmap");
            string Answer = Console.ReadLine();
            if (Answer == "1") {
                WriteDataToBitmap();
            }
            if (Answer == "2") {
                ReadFromBitmap();
            }
            if (Answer == "3")
            {
                WhatIThoughtWouldWork();
            }
            if (Answer == "4")
            {
                MakeARedLine();
            }
            if (Answer == "5")
            {
                ChangeSentenceIntoImage();
            }
            if (Answer == "6")
            {
                ChangeImageIntoSentence();
            }
            Console.ReadLine();
        }
        private static void WriteDataToBitmap() {
            string[] ShortStory = File.ReadAllText("texttext.txt").Split(' ');
            List<char> ListOfCharacters = new List<char>();
            foreach (string Word in ShortStory) {
                string AddSpace = Word + " ";
                foreach (char Character in AddSpace) {
                    ListOfCharacters.Add(Character);
                }
            }
            int CountOfCharacters = Convert.ToInt32(Math.Sqrt((ListOfCharacters.Count / 3))) + 1;
            Bitmap bitmap = new Bitmap(CountOfCharacters, CountOfCharacters, CountOfCharacters, System.Drawing.Imaging.PixelFormat.Format24bppRgb, IntPtr.Zero);
            int ColumnIterator = 0;
            int RowIterator = 0;
            byte[] RGBVector3 = new byte[3];
            int i = 0;
            foreach (byte CharacterByte in ListOfCharacters) {
                if (RowIterator > (CountOfCharacters - 1)) {
                    RowIterator = 0;
                    ColumnIterator++;
                }
                try {
                    RGBVector3[i] = CharacterByte;
                    i++;
                }
                catch {
                    bitmap.SetPixel(RowIterator, ColumnIterator, Color.FromArgb(RGBVector3[0], RGBVector3[1], RGBVector3[2]));
                    i = 0;
                    RGBVector3[i] = CharacterByte;
                    i++;
                    RowIterator++;
                }
            }
            bitmap.Save("testbitmap2.bmp");
        }

        private static void ReadFromBitmap() {
            string OriginalFile = "testbitmap.bmp";
            Bitmap bitmap = new Bitmap(OriginalFile);
            int FileWidth = bitmap.Width;
            int FileHeight = bitmap.Height;
            for (int i = 0; i < FileHeight; i++) {
                for (int j = 0; j < FileWidth; j++) {
                    Color pixelcolor = bitmap.GetPixel(j, i);
                    if (pixelcolor.R != 0) {
                        Console.Write("{0}{1}{2}", (char)pixelcolor.R, (char)pixelcolor.G, (char)pixelcolor.B);
                    }
                }
            }
        }

        private static void WhatIThoughtWouldWork() {
            string Story = "This is where the story would have gone";
            string[] SplitStory = Story.Split(' ');
            List<byte> CharacterBytes = new List<byte>();
            foreach(string Word in SplitStory) {
                foreach(byte Character in Word) {
                    CharacterBytes.Add(Character);
                }
            }
            byte[] ByteArray = new byte[CharacterBytes.Count];
            int i = 0;
            foreach(byte CurrentByte in CharacterBytes) {
                ByteArray[i] = CurrentByte;
                i++;
            }
            File.WriteAllBytes("TestByteArray.png", ByteArray);
        }

        private static void MakeARedLine()
        {
            Bitmap bitmap = new Bitmap(
                50, 
                50, 
                50, 
                System.Drawing.Imaging.PixelFormat.Format24bppRgb, 
                IntPtr.Zero
                );
            bitmap.SetPixel(23, 25, Color.Red);
            bitmap.SetPixel(24, 25, Color.Red);
            bitmap.SetPixel(25, 25, Color.Red);
            bitmap.SetPixel(26, 25, Color.Red);
            bitmap.Save("RedLine.bmp");
        }

        private static void ChangeSentenceIntoImage()
        {
            string Story = "This is where the story would have gone";
            string[] SplitStory = Story.Split(' ');
            int RowIncrement = 0;
            int ColumnIncrement = 0;
            Bitmap bitmap = new Bitmap(
                8,
                8,
                8,
                System.Drawing.Imaging.PixelFormat.Format24bppRgb,
                IntPtr.Zero
                );
            foreach (string Word in SplitStory) {
                string WordPlusSpace = Word + " "; 
                foreach (byte CharacterByte in WordPlusSpace) {
                    bitmap.SetPixel(
                        RowIncrement, 
                        ColumnIncrement, 
                        Color.FromArgb(CharacterByte, 0, 0)
                        );
                    RowIncrement++;
                }
                RowIncrement = 0;
                ColumnIncrement++;
            }
            bitmap.Save("SentenceAsImage.bmp");
        }

        private static void ChangeImageIntoSentence()
        {
            string OriginalFile = "SentenceAsImage.bmp";
            Bitmap bitmap = new Bitmap(OriginalFile);
            int FileWidth = bitmap.Width;
            int FileHeight = bitmap.Height;
            for (int i = 0; i < FileHeight; i++) {
                for (int j = 0; j < FileWidth; j++) {
                    Color pixelcolor = bitmap.GetPixel(j, i);
                    if (pixelcolor.R != 0) {
                        Console.Write((char)pixelcolor.R);
                    }
                }
            }
        }
    }
}
