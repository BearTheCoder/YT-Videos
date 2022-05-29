using System;
using System.Windows;
using System.Numerics;
using System.Drawing;
using System.Threading;
using System.Diagnostics;
using System.IO;
namespace LeafRevBlowWPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            //Start Main Function
            InitializeComponent();
            DeleteAllImages();
            bool Run = false;
            int PixelDist = 200;
            Vector2 PixelLocation = new Vector2(-PixelDist, 0);
            Stopwatch SW = new Stopwatch();
            SW.Start();
            while (true)
            {
                Run = TurnOnTurnOff(Run, SW);
                //DisplayMousePosition();
                if (Run)
                {
                    PixelLocation = MouseMoveMacro(Run, PixelLocation, PixelDist);
                    GetScreenShot(SW);
                    CheckForFailSafe();
                }
            }
            //End Main Function

            static bool TurnOnTurnOff(bool RunLocal, Stopwatch LocalSW)
            {
                if (User32.GetAsyncKeyState(0x49) != 0) // "I" Key
                {
                    RunLocal = true;
                    LocalSW.Restart();
                }
                else if (User32.GetAsyncKeyState(0x4F) != 0) //"O" Key
                {
                    RunLocal = false;
                    LocalSW.Stop();
                    
                }
                return RunLocal;
            }
            static Vector2 MouseMoveMacro(bool RunLocal, Vector2 PixelLocationLocal, int PixelDistLocal)
            {
                int x = (int)PixelLocationLocal.X;
                int y = (int)PixelLocationLocal.Y;
                if (RunLocal)
                {
                    User32.SetCursorPos(x, y);
                    x += PixelDistLocal;
                    if (x > 1900)
                    {
                        y += PixelDistLocal;
                        x = -PixelDistLocal;
                    }
                    if (y > 1060)
                    {
                        y = 0;
                        x = -PixelDistLocal;
                    }
                    Thread.Sleep(PixelDistLocal / 4);
                }
                if (!RunLocal)
                {
                    x = -PixelDistLocal;
                    y = 0;
                }
                return new Vector2(x, y);
            }
            static void GetScreenShot(Stopwatch SWLocal)
            {
                if (SWLocal.ElapsedMilliseconds > 3000)
                {
                    Bitmap BM = new Bitmap((int)SystemParameters.VirtualScreenWidth, (int)SystemParameters.VirtualScreenHeight);
                    Graphics g = Graphics.FromImage(BM);
                    g.CopyFromScreen((int)SystemParameters.VirtualScreenLeft, (int)SystemParameters.VirtualScreenTop, 0, 0, BM.Size);
                    string FileName = "Images/Screenshot" + DateTime.Now.ToString("MM_dd_yyyy_HH_mm_ss") + ".png";
                    BM.Save(FileName);
                    ReadPixels(FileName);
                    SWLocal.Restart();
                }
            }
            static void ReadPixels(string LocalFileName)
            {
                Bitmap BM = new Bitmap(System.Drawing.Image.FromFile(LocalFileName));
                for(int i = 0; i < 1550; i++)
                {
                    for (int j = 1900; j < BM.Height; j++)
                    {
                        Color PixelColor = BM.GetPixel(i, j);
                        if (PixelColor.R == 255 && PixelColor.G == 0 && PixelColor.B == 0)
                        {
                            if (i > 200 && j > 2090)
                            {
                                if (i < 260)
                                {
                                    GreenLeafUpgrades(i, j);
                                }
                                else
                                {
                                    GoldenLeafUpgrades(i, j);
                                }
                                goto LoopEnd;
                            }
                        }
                    }
                }
            LoopEnd:
                Trace.WriteLine("end");
            }
            static void GreenLeafUpgrades(int LocalI, int LocalJ)
            {
                //Thread Wait Time
                int WaitTime = 65;

                //Open Window
                User32.SetCursorPos(LocalI, LocalJ - 1080);
                Thread.Sleep(WaitTime);
                ClickMouse(WaitTime);

                BuyUpgrades(WaitTime, 250); //Buy Upgrades

                //Exit Window
                User32.SetCursorPos(1678, 144);
                Thread.Sleep(WaitTime);
                ClickMouse(WaitTime);
            }
            static void GoldenLeafUpgrades(int LocalI, int LocalJ)
            {
                //Thread Wait Time
                int WaitTime = 65;

                //Open Window
                User32.SetCursorPos(LocalI, LocalJ - 1080);
                Thread.Sleep(WaitTime);
                ClickMouse(WaitTime);

                BuyUpgrades(WaitTime, 250); //Buy Upgrades
                ScrollWindow(WaitTime, 898); //Scroll Down
                BuyUpgrades(WaitTime, 300); //Buy Upgrades Shifted
                ScrollWindow(WaitTime, 218); //Scroll Up

                //Exit Window
                User32.SetCursorPos(1678, 144);
                Thread.Sleep(WaitTime);
                ClickMouse(WaitTime);
            }
            static void BuyUpgrades(int WaitTimeLocal, int LocalLocation){
                for (int k = 0; k < 7; k++)
                {
                    User32.SetCursorPos(1230, LocalLocation + (k * 87));
                    Thread.Sleep(WaitTimeLocal);
                    for (int M = 0; M < 3; M++)
                    {
                        ClickMouse(WaitTimeLocal);
                    }
                }
            }
            static void ScrollWindow(int WaitTimeLocal, int LocalY)
            {
                User32.SetCursorPos(1678, LocalY);
                for (int k = 0; k < 5; k++)
                {
                    ClickMouse(WaitTimeLocal);
                }
            }
            static void ClickMouse(int WaitTimeLocal)
            {
                User32.mouse_event(0x02, 0, 0, 0, 0);
                Thread.Sleep(WaitTimeLocal / 2);
                User32.mouse_event(0x04, 0, 0, 0, 0);
                Thread.Sleep(WaitTimeLocal / 2);
            }
            static void DeleteAllImages()
            {
                string[] Files = Directory.GetFiles(@"Images");
                foreach (string filePath in Files)
                {
                    File.Delete(filePath);
                }
            }
            static void DisplayMousePosition()
            {
                //Get Mouse Position
                System.Drawing.Point NewPoint = new System.Drawing.Point();
                User32.GetCursorPos(out NewPoint);
                Trace.WriteLine(NewPoint.ToString());
            }
            static void CheckForFailSafe()
            {
                if (User32.GetAsyncKeyState(34) != 0) // Page Down
                {
                    Environment.Exit(0);
                }
            }
        }
    }
}
