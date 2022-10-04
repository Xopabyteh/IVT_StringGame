using System.Diagnostics;
using System.Net.Mime;
using System.Xml.Linq;

namespace IVT_StringGame;

class FastTypeGame
{
    private Stopwatch sw;
    private int lives;
    private int difficultyLives;
    public FastTypeGame()
    {
        sw = new Stopwatch();
    }
    public void Play()
    {

        //Intro
        SlowWriteLine("Hey, this is a game about typing fast.");
        SlowWriteLine("Lets start by getting your name!");

        //Name
        SlowWrite("Name: ");
        var name = ReadNotNull();
        SlowWriteLine($"Wow, {name} is such a lovely name!");

        //Choose difficulty
        SlowWriteLine("Now lets choose your difficulty..");
        var difficulty = Prompt("Hard", "Harder");
        switch (difficulty.choiceIndex)
        {
            case 0:
                difficultyLives = 5;
                SlowWriteLine("Lame... But lets play.");
                break;
            case 1:
                difficultyLives = 1;
                SlowWriteLine("Now that's how you game!");
                break;
            default:
                throw new InvalidOperationException("Prompt answer out of bounds");
        }

        if (name.ToLower() == "honza")
        {
            difficultyLives *= 2;
        }
        lives = difficultyLives;
        //Game
        SlowWriteLine($"You now have {lives} lives");
        SlowWriteLine("Ok, let's start with a simple word... maybe your name!");
        SlowWriteLine("You will see the sentence/word and you will have to type over it.");
        SlowWriteLine("Messing up will lose you a life.");
        SlowWriteLine("Try to get the best time!");
        SlowWriteLine("Case sensitive!!!");

        if (!PlayForString(name))
        {
            SlowWriteLine("Oh did you just lose on the first level...................... :'(");
            return;
        }
        SlowWriteLine($"Let's go up a level now!");

        if (!PlayForString("He ate a steak"))
        {
            SlowWriteLine("It's just the second level and you've died alredy");
            return;
        }
        SlowWriteLine("That must have been beginners luck, harder!");

        
        if (!PlayForString("Today is a lovely day with no mistakes, only happy little accidents"))
        {
            return;
        }
        SlowWriteLine("Happy day!");

        if (!PlayForString("Hard weird palindromic words help affluent aggrandize a persons mind"))
        {
            SlowWriteLine("It was the last level, you were so close :(");
            return;
        }
        SlowWriteLine("Do you even know what those words mean?");
        SlowWriteLine("It doesn't matter, you won. Good job!");

        Prompt("Quit","Leave","Exit");
        Environment.Exit(0);
    }

    const int PLAY_SPACE_SPACING_TOP = 2;

    /// <summary>
    /// Returns true when the player passed and false when he dies
    /// </summary>
    /// <param name="playString"></param>
    /// <returns></returns>
    private bool PlayForString(string playString)
    {
        //Ready
        Console.WriteLine("Get ready!");
        Thread.Sleep(1000);
        Console.WriteLine("3");
        Thread.Sleep(1000);
        Console.WriteLine("2");
        Thread.Sleep(1000);
        Console.WriteLine("1");
        Thread.Sleep(1000);
        Console.WriteLine("Go!");

        var curPos = Console.GetCursorPosition();
        Console.SetCursorPosition(0, curPos.Top + PLAY_SPACE_SPACING_TOP);
        Console.Write(playString);
        Console.SetCursorPosition(0, curPos.Top + PLAY_SPACE_SPACING_TOP);

        sw.Restart();
        int playStringIndex = 0;
        while (true)
        {
            LogLives(lives,playString.Length);
            var key = Console.ReadKey(true).KeyChar;
            
            //Don't allow functional characters
            if((int)key < 32)
                continue;

            var wordLetter = playString[playStringIndex];
            if ((int)key == (int)wordLetter)
            {
                Console.BackgroundColor = ConsoleColor.DarkGreen;
            } 
            else
            {
                //Wrong playString
                lives--;

                if (lives <= 0)
                {
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.Clear();
                    SlowWriteLine("Oh noes, you died.");
                    var deathChoice = Prompt("Play from start", "More lives please");
                    switch (deathChoice.choiceIndex)
                    {
                        case 0:
                            return false;
                        case 1:
                            SlowWriteLine("\nYou may continue.");
                            lives += difficultyLives;
                            return PlayForString(playString);
                    }
                    
                    continue;
                }
                Console.BackgroundColor = ConsoleColor.Red;
            }

            Console.Write(key);



            playStringIndex++;
            if (playStringIndex == playString.Length)
                break;
        }
        Console.BackgroundColor = ConsoleColor.Black;
        Console.WriteLine();
        SlowWriteLine($"Wow! You did it in {sw.Elapsed.TotalMilliseconds}ms");
        sw.Stop();
        return true;
    }

    private void LogLives(int lives, int wordLength)
    {
        var curPos = Console.GetCursorPosition();
        Console.SetCursorPosition(wordLength+1, curPos.Top - 1);
        Console.BackgroundColor = ConsoleColor.DarkRed;
        Console.Write($"{lives}HP");
        Console.BackgroundColor = ConsoleColor.Black;
        Console.SetCursorPosition(curPos.Left, curPos.Top);
    }
    private const int SLOW_PRINT_DELAY = 38;
    private const int SLOW_PRINT_DELAY_LN = 214;

    private Dictionary<char, int> specialWaitSymbols = new()
    {
        {'.', SLOW_PRINT_DELAY * 4},
        {'!', SLOW_PRINT_DELAY * 2},
        {',', SLOW_PRINT_DELAY * 3},
    };

    private bool WriteColorSwitch;
    private const int SHOWN_MESSAGES_UNTIL_SCROLL = 2;
    private void SlowWrite(string? s)
    {
        var curPos = Console.GetCursorPosition();

        WriteColorSwitch = !WriteColorSwitch;
        Console.ForegroundColor = WriteColorSwitch ? ConsoleColor.White : ConsoleColor.Gray;
        foreach (var ch in s)
        {
            Console.Write(ch);
            Thread.Sleep(specialWaitSymbols.ContainsKey(ch) ? specialWaitSymbols[ch] : SLOW_PRINT_DELAY);
        }
    }
    private void SlowWriteLine(string? s)
    {
        SlowWrite(s);
        Thread.Sleep(SLOW_PRINT_DELAY_LN);
        Console.WriteLine();
    }

    private string ReadNotNull()
    {
        string? s = string.Empty;
        while (true)
        {
            s = Console.ReadLine();
            if (string.IsNullOrEmpty(s))
            {
                Console.WriteLine("Must not be empty!");
                continue;
            }
            break;
        }

        return s!;
    }

    private const int promptCurPosOffsetTop = 1;
    private const int promptCurPosOffsetLeft = 1;
    /// <summary>
    /// The choiceIndex is 0 based but the text output is 1 based
    /// </summary>
    /// <param name="choices"></param>
    /// <returns></returns>
    private (int choiceIndex, string choice) Prompt(params string[] choices)
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
        int res;
        while (true)
        {
            key = Console.ReadKey(true);
            if(!int.TryParse(key.KeyChar.ToString(),out res))
                continue;
            if(res > choices.Length || res <= 0)
                continue;

            break;
        }

        res--; //Transform to 0 based
        return (res, choices[res]);
    }
}