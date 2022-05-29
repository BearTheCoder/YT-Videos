using System.Runtime.InteropServices;
while (true){ RunAutoClicker(); }
[DllImport("user32.dll")]
static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo); //Clicks the mouse
[DllImport("user32.dll")]
static extern short GetAsyncKeyState(int vKey); //Detects keyboard input.
static void RunAutoClicker(){
    if (GetAsyncKeyState(0x58) != 0) {
        mouse_event(0x02, 0, 0, 0, 0);
        Thread.Sleep(5);
        mouse_event(0x04, 0, 0, 0, 0);
        Thread.Sleep(5);
    }
    if (GetAsyncKeyState(34) != 0) { Environment.Exit(0); }
}
