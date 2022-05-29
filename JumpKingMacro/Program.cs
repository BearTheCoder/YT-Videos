using JumpKingMacro;
using WindowsInput;
using WindowsInput.Native;
using System.Numerics;
bool FirstTime = true;
while (true){
    if (User32.GetAsyncKeyState(0x2E) != 0 && FirstTime == true) {
        RunMacros();
        FirstTime = false;
    }
    CheckForKeyboardInput();
    CheckForNewGame();
}
static void CheckForKeyboardInput() {
    InputSimulator IS = new InputSimulator();
    VirtualKeyCode VKC = new VirtualKeyCode();
    int SleepTime = 0;
    Char LetterPressed = ' ';
    bool CanJump = false;
    if (User32.GetAsyncKeyState(0x51) != 0) //"Q"
    {
        VKC = VirtualKeyCode.LEFT;
        SleepTime = 550;
        LetterPressed = 'Q';
        CanJump = true;
      
    }
    else if (User32.GetAsyncKeyState(0x57) != 0) //"W"
    {
        VKC = VirtualKeyCode.LEFT;
        SleepTime = 500;
        LetterPressed = 'W';
        CanJump = true;
    }
    else if (User32.GetAsyncKeyState(0x45) != 0) //"E"
    {
        VKC = VirtualKeyCode.LEFT;
        SleepTime = 450;
        LetterPressed = 'E';
        CanJump = true;
    }
    else if (User32.GetAsyncKeyState(0x52) != 0) //"R"
    {
        VKC = VirtualKeyCode.LEFT;
        SleepTime = 400;
        LetterPressed = 'R';
        CanJump = true;
    }
    else if (User32.GetAsyncKeyState(0x54) != 0) //"T"
    {
        VKC = VirtualKeyCode.LEFT;
        SleepTime = 350;
        LetterPressed = 'T';
        CanJump = true;
    }
    else if (User32.GetAsyncKeyState(0x59) != 0) //"Y"
    {
        VKC = VirtualKeyCode.LEFT;
        SleepTime = 300;
        LetterPressed = 'Y';
        CanJump = true;
    }
    else if (User32.GetAsyncKeyState(0x55) != 0) //"U"
    {
        VKC = VirtualKeyCode.LEFT;
        SleepTime = 250;
        LetterPressed = 'U';
        CanJump = true;
    }
    else if (User32.GetAsyncKeyState(0x49) != 0) //"I"
    {
        VKC = VirtualKeyCode.LEFT;
        SleepTime = 200;
        LetterPressed = 'I';
        CanJump = true;
    }
    else if (User32.GetAsyncKeyState(0x41) != 0) //"A"
    {
        VKC = VirtualKeyCode.RIGHT;
        SleepTime = 550;
        LetterPressed = 'A';
        CanJump = true;
    }
    else if (User32.GetAsyncKeyState(0x53) != 0) //"S"
    {
        VKC = VirtualKeyCode.RIGHT;
        SleepTime = 500;
        LetterPressed = 'S';
        CanJump = true;
    }
    else if (User32.GetAsyncKeyState(0x44) != 0) //"D"
    {
        VKC = VirtualKeyCode.RIGHT;
        SleepTime = 450;
        LetterPressed = 'D';
        CanJump = true;
    }
    else if (User32.GetAsyncKeyState(0x46) != 0) //"F"
    {
        VKC = VirtualKeyCode.RIGHT;
        SleepTime = 400;
        LetterPressed = 'F';
        CanJump = true;
    }
    else if (User32.GetAsyncKeyState(0x47) != 0) //"G"
    {
        VKC = VirtualKeyCode.RIGHT;
        SleepTime = 350;
        LetterPressed = 'G';
        CanJump = true;
    }
    else if (User32.GetAsyncKeyState(0x48) != 0) //"H"
    {
        VKC = VirtualKeyCode.RIGHT;
        SleepTime = 300;
        LetterPressed = 'H';
        CanJump = true;
    }
    else if (User32.GetAsyncKeyState(0x4A) != 0) //"J"
    {
        VKC = VirtualKeyCode.RIGHT;
        SleepTime = 250;
        LetterPressed = 'J';
        CanJump = true;
    }
    else if (User32.GetAsyncKeyState(0x4B) != 0) //"K"
    {
        VKC = VirtualKeyCode.RIGHT;
        SleepTime = 200;
        LetterPressed = 'K';
        CanJump = true;
    }
    if (CanJump)
    {
        IS.Keyboard.KeyDown(VirtualKeyCode.SPACE);
        IS.Keyboard.KeyDown(VKC);
        Thread.Sleep(SleepTime);
        IS.Keyboard.KeyUp(VKC);
        IS.Keyboard.KeyUp(VirtualKeyCode.SPACE);
        CanJump = false;
        Console.WriteLine("Button Press : '" +
            LetterPressed + "' Hold for " + SleepTime + "ms");
    }
}
static void RunMacros()
{
    InputSimulator IS = new InputSimulator();
    VirtualKeyCode VKC = new VirtualKeyCode();
    int Counter = 0;
    foreach (Vector2 V2 in JumpArray.J_Array)
    {
        bool Jump = true;
        Console.WriteLine("Move " + Counter + ": " + V2);
        Counter++;
        if (V2.X == 0) { VKC = VirtualKeyCode.LEFT; }
        else if (V2.X == 1) { VKC = VirtualKeyCode.RIGHT; }
        else if (V2.X == 2) { VKC = VirtualKeyCode.LEFT; Jump = false; }
        else if (V2.X == 3) { VKC = VirtualKeyCode.RIGHT; Jump = false; }
        if (Jump) { IS.Keyboard.KeyDown(VirtualKeyCode.SPACE); }
        IS.Keyboard.KeyDown(VKC);
        Thread.Sleep((int)V2.Y);
        IS.Keyboard.KeyUp(VKC);
        if (Jump) { IS.Keyboard.KeyUp(VirtualKeyCode.SPACE); }
        Thread.Sleep((1200));
    }
}
static void CheckForNewGame()
{
    if (User32.GetAsyncKeyState(34) != 0) //PageDown
    {
        InputSimulator IS = new InputSimulator();
        IS.Keyboard.KeyDown(VirtualKeyCode.ESCAPE);
        Thread.Sleep(100);
        IS.Keyboard.KeyUp(VirtualKeyCode.ESCAPE);
        Thread.Sleep(100);
        IS.Keyboard.KeyDown(VirtualKeyCode.DOWN);
        Thread.Sleep(100);
        IS.Keyboard.KeyUp(VirtualKeyCode.DOWN);
        Thread.Sleep(100);
        IS.Keyboard.KeyDown(VirtualKeyCode.DOWN);
        Thread.Sleep(100);
        IS.Keyboard.KeyUp(VirtualKeyCode.DOWN);
        Thread.Sleep(100);
        IS.Keyboard.KeyDown(VirtualKeyCode.RETURN);
        Thread.Sleep(100);
        IS.Keyboard.KeyUp(VirtualKeyCode.RETURN);
        Thread.Sleep(100);
        IS.Keyboard.KeyDown(VirtualKeyCode.DOWN);
        Thread.Sleep(100);
        IS.Keyboard.KeyUp(VirtualKeyCode.DOWN);
        Thread.Sleep(100);
        IS.Keyboard.KeyDown(VirtualKeyCode.RETURN);
        Thread.Sleep(100);
        IS.Keyboard.KeyUp(VirtualKeyCode.RETURN);
        Thread.Sleep(7500);
        IS.Keyboard.KeyDown(VirtualKeyCode.SPACE);
        Thread.Sleep(100);
        IS.Keyboard.KeyUp(VirtualKeyCode.SPACE);
        Thread.Sleep(1000);
        IS.Keyboard.KeyDown(VirtualKeyCode.DOWN);
        Thread.Sleep(100);
        IS.Keyboard.KeyUp(VirtualKeyCode.DOWN);
        Thread.Sleep(100);
        IS.Keyboard.KeyDown(VirtualKeyCode.RETURN);
        Thread.Sleep(100);
        IS.Keyboard.KeyUp(VirtualKeyCode.RETURN);
        Thread.Sleep(100);
        IS.Keyboard.KeyDown(VirtualKeyCode.DOWN);
        Thread.Sleep(100);
        IS.Keyboard.KeyUp(VirtualKeyCode.DOWN);
        Thread.Sleep(100);
        IS.Keyboard.KeyDown(VirtualKeyCode.RETURN);
        Thread.Sleep(100);
        IS.Keyboard.KeyUp(VirtualKeyCode.RETURN);
        Thread.Sleep(10000);
        RunMacros();
    }
}
