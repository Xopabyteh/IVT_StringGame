namespace IVT_StringGame;

class FastTypeGame
{
    private int lives = 10;
    public void Play()
    {
        Prompt("ANO", "NE", "UMUM");
        Console.ReadLine();
        //SlowWriteLine("Hey, this is a game about typing fast\nLets start by getting your name!\nName: ");
        //var name = ReadNotNull();

        //SlowWriteLine($"Wow, {name} is such a lovely name\nNow lets choose your difficulty");


    }

    private const int SlowPrintDelay = 10;
    private void SlowWriteLine(string? s)
    {
        foreach (var ch in s)
        {
            Console.Write(ch);
            Thread.Sleep(SlowPrintDelay);
        }
        Console.WriteLine();
    }

    private string ReadNotNull()
    {
        string? s = string.Empty;
        while (string.IsNullOrEmpty(s))
        {
            s = Console.ReadLine();
        }

        return s;
    }

    private const int promptCurPosOffsetTop = 1;
    private const int promptCurPosOffsetLeft = 1;
    private int Prompt(params string[] choices)
    {
        (int Left, int Top) originalCurPos = Console.GetCursorPosition();
        (int Left, int Top) pointerCurPos = originalCurPos;

        for (var i = 0; i < choices.Length; i++)
        {
            var choice = choices[i];
            Console.Write((i + 1).ToString().PadLeft(promptCurPosOffsetLeft + choice.Length));
            pointerCurPos = (pointerCurPos.Left + promptCurPosOffsetLeft + choice.Length, pointerCurPos.Top + promptCurPosOffsetTop);
            Console.SetCursorPosition(pointerCurPos.Left, pointerCurPos.Top);
            Console.Write(choice);
            pointerCurPos = (pointerCurPos.Left + promptCurPosOffsetLeft + choice.Length, pointerCurPos.Top - promptCurPosOffsetTop);
            Console.SetCursorPosition(pointerCurPos.Left, pointerCurPos.Top);

        }
        Console.SetCursorPosition(0, originalCurPos.Top + promptCurPosOffsetTop + 1);
        
        ConsoleKeyInfo key;
        while (true)
        {
            key = Console.ReadKey();
            //if(int.TryParse())
        }
        return 1;
    }
}