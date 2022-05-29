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
    }
}
