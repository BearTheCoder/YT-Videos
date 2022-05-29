using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
namespace JumpKingMacro
{
    internal class User32
    {
        [DllImport("user32.dll")]
        internal static extern short GetAsyncKeyState(int vKey); //Detects keyboard input.
        [DllImport("user32.dll")]
        internal static extern short SendInput(int vKey); //Detects keyboard input.
        [DllImport("user32.dll")]
        internal static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo); //Triggers mouse.
        [DllImport("user32.dll")]
        internal static extern bool SetCursorPos(int x, int y); // (0,0) = Top-Left; (1920, 1080) = Bottom-Right
        [DllImport("user32.dll")]
        internal static extern bool GetCursorPos(out Point P); // (0,0) = Top-Left; (1920, 1080) = Bottom-Right
    }
}
