using System;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
namespace ColorPalletCreator {
    class ColorObject {
        internal Color MyColor;
        internal int Occurence;
        internal ColorObject(Color color) {
            MyColor = color;
            Occurence = 1;
        }
    }
    class Program {
        static int Tolerance = 40;
        static void Main(string[] args) {
            Bitmap ImageBM = new Bitmap(Image.FromFile(@"C:\Users\aaron\Downloads\Pallet2.jpg"));
            List<ColorObject> ColorObjects = new List<ColorObject>();
            List<Color> ActivePalletColors = new List<Color>();
            List<Color> CheckedColors = new List<Color>();
            Console.WriteLine("Checking Pixels...");
            for (int i = 0; i < ImageBM.Width; i++) {
                for (int j = 0; j < ImageBM.Height; j++) {
                    Color PixelColor = ImageBM.GetPixel(i, j);
                    if (ActivePalletColors.Contains(PixelColor) == false) {
                        if (CheckedColors.Contains(PixelColor) == false) {
                            if (i == 0 && j == 0) {
                                ColorObjects.Add(new ColorObject(PixelColor));
                                ActivePalletColors.Add(PixelColor);
                                CheckedColors.Add(PixelColor);
                                continue;
                            }
                            bool AddValue = true;
                            foreach (Color C in ActivePalletColors) {
                                int C_TotalValue = C.R + C.G + C.B;
                                int PC_TotalValue = PixelColor.R + PixelColor.G + PixelColor.B;
                                if (CalculateDifference(C_TotalValue, PC_TotalValue) < Tolerance) {
                                    if (CheckRGBDifference(C, PixelColor) == false) {
                                        CheckedColors.Add(PixelColor);
                                        AddValue = false;
                                        break;
                                    }
                                }
                            }
                            if (AddValue) {
                                CheckedColors.Add(PixelColor);
                                ColorObjects.Add(new ColorObject(PixelColor));
                                ActivePalletColors.Add(PixelColor);
                            }
                        }
                    }
                    else {
                        int Index = ActivePalletColors.IndexOf(PixelColor);
                        ColorObjects[Index].Occurence++;
                    }
                }
            }
            List<ColorObject> SortedList = new List<ColorObject>();
            SortedList = ColorObjects.OrderByDescending(o => o.Occurence).ToList();
            CreatePallet(SortedList);
            Console.WriteLine("Complete!");
            Console.ReadLine();
        }
        private static float CalculateDifference(int Value1, int Value2) {
            return MathF.Sqrt(MathF.Pow(Value1 - Value2, 2));
        }
        private static bool CheckRGBDifference(Color CO_Color, Color PC_Color) {
            float R_Difference = CalculateDifference(CO_Color.R, PC_Color.R);
            float G_Difference = CalculateDifference(CO_Color.G, PC_Color.G);
            float B_Difference = CalculateDifference(CO_Color.B, PC_Color.B);
            if (R_Difference > Tolerance || G_Difference > Tolerance || B_Difference > Tolerance) {
                return true;
            }
            return false;
        }
        private static void CreatePallet(List<ColorObject> SortedList) {
            Bitmap PalletImage = new Bitmap(1000, 1000, 1000, PixelFormat.Format24bppRgb, IntPtr.Zero);
            int k = 0;
            for (int i = 0; i < 1000; i++){
                for (int j = 0; j < 1000; j++) {
                    if (k == -1) { k = 0; }
                    if (j % 200 == 0 && j != 0) { k++; }
                    PalletImage.SetPixel(i, j, SortedList[k].MyColor);
                }
                k -= 4;
                if (i % 200 == 0 && i != 0) { k += 5; }
            }
            PalletImage.Save("testbitmap2.bmp");
        }
    }
}
