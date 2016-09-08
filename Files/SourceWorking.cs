using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineLearningPlatformer
{
    class Program
    {
        static void Main(string[] args)
        {
            Graphics.SetUpWindow(); // Initialise and configure game window.

            GetRunningArguments(); // Allow settings to be changed before starting.

            while (true) // Loop forever.
            {
                Machine.Generation += 1;

                PlayGame();

                //Console.Clear();
                //Console.Write("\n\tYou Are Dead\n\t");
                //System.Threading.Thread.Sleep(20);

            }
        }

        public static void PlayGame()
        {
            Console.Clear();

            GameVars.Reset();

            while (Player.Alive) // Loop for as long as player is alive (from Player.Alive bool == true).
            {

                if (GameVars.PlayerControl) // If the boolean represent acceptance of user input is 'true', call GetPlayerInput (for debugging).
                {
                    Controls.GetPlayerInput();  // For debugging purposes, allows user input and changes Players 'NextDirection' string. (Also handles other inputs, such as 'P' for pause menu.)
                }
                else if (GameVars.PlayingBack)
                {
                    Machine.PlaybackBestInputs();
                }
                else // If the boolean is 'false', call method for the machine to 'play' the game without user input. 
                {
                    Controls.CheckPause(); // Manual check for 'P' key to pause, as GetPlayerInput is not called to handle it.
                    Machine.LearnFromRandom(); // Method for computer to choose random direction for player.
                }

                Controls.HandlePlayerDirection(); // Run method to handle input, given as NextDirection string.

                Physics.CalculateVerticalMotion(); // Method to calculate vertical motion - checks falling due to gravity, and upward motion due to V_Speed (jumping).

                if (GameVars.GraphicsOn) // Boolean to decide whether game graphics should be rendered - graphics OFF improves runspeed significantly due to Console.Write functions being very slow.
                {
                    Graphics.DrawLevelToScreen(); // Graphics method to draw the correct level portion to screen, relative to player position in-level and on-screen.
                    Graphics.DrawPlayerToScreen(); // Graphics method to draw the player's sprite to the on-screen X and Y co-ordinates.
                }

                if (GameVars.DrawGameInfo)
                {
                    Graphics.DrawInfo(); // Write program data to info bar on screen. (ie Fitness, Frame No. etc.)
                }

                System.Threading.Thread.Sleep(GameVars.TickRate); // Tick every X milliseconds (Wait between advancing frame, and running game loop logic).

                GameVars.CurrentFrame += 1; //Advance Frame counter after tick.
                CalculateFitness(); // Determine 'fitness' value of current level run, dependant on rightmost distance travelled, and the current frame ('time' taken).

            }

        }

        public static void CalculateFitness() // Determine the 'fitness' value of the current level run - a score of effectiveness completing the level, used to compare different input logs.
        {
            Player.Fitness = (Player.RightMost_X - (GameVars.CurrentFrame / 2) + 100); // Fitness is calculate as: ((Rightmost Horizontal distance - (The number of frames taken / 2)) + 100);
        }

        public static void GetRunningArguments()
        {
            Console.CursorVisible = true;

            string ArgInput = "";

            while (!(ArgInput.Contains("start")))
            {
                Console.Clear();

                Console.WriteLine("PAUSED: Enter Arguments -\n Include variable name to toggle state:\n\nPlayerControl:\t{0}\n\nGraphicsOn:\t{1}\n\nDrawGameInfo:\t{2}\n\nInclude 'Start' to begin.\n\nInput:\n", GameVars.PlayerControl, GameVars.GraphicsOn, GameVars.DrawGameInfo);

                ArgInput = Console.ReadLine();
                ArgInput = ArgInput.ToLower();

                if (ArgInput.Contains("playercontrol"))
                {
                    GameVars.PlayerControl = !(GameVars.PlayerControl);
                    if (GameVars.PlayerControl == true) { GameVars.TickRate = 50; }
                    else { GameVars.TickRate = 0; };
                }

                if (ArgInput.Contains("graphicson"))
                {
                    GameVars.GraphicsOn = !(GameVars.GraphicsOn);
                }

                if (ArgInput.Contains("drawgameinfo"))
                {
                    GameVars.DrawGameInfo = !(GameVars.DrawGameInfo);
                }

                if (ArgInput.Contains("playback"))
                {
                    GameVars.Reset();

                    GameVars.PlayerControl = false;
                    GameVars.GraphicsOn = true;
                    GameVars.PlayingBack = !(GameVars.PlayingBack);

                }
            }

            Console.CursorVisible = false;

            Console.Clear();
        }
    }

    class Player
    {
        // Variables containing the two-dimensional co-ordinates of the player in the level.
        public static int X_Pos = 12;       //
        public static int Y_Pos = 12;       //
        // ---------------------------------//

        public static char Sprite = 'P'; // The symbol that will represent the player on-screen.

        // Variables containing the two-dimensional co-ordinates of the player on the screen. (This will differ to level co-ords if screen has scrolled.)
        public static int X_ScreenPos = 12; //
        public static int Y_ScreenPos = 12; //
        // ---------------------------------//

        public static bool Alive = true; // Boolean to keep status of player for main game loop.

        public static int V_Speed = 0; // Stores value for vertical 'momentum' - Used for jumping as this takes place over multiple frames, unlike walking.

        public static bool IsStanding = false; // Boolean to keep status of whether player is touching the floor. (Immediate tile below is solid)

        // ------------------------------
        public static int RightMost_X = 0;

        public static int Fitness = 0;

        // ------------------------------

        public static string NextDirection = "None";
    }

    class Graphics //   Class to contain all methods related to on-screen display.
    {
        public static void DrawPlayerToScreen() // Draw the current sprite of the player to its respective on-screen position.
        {
            Console.SetCursorPosition(Player.X_ScreenPos, Player.Y_ScreenPos); // Set cursor to Player's on-screen position.

            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;

            Console.Write(Player.Sprite); // Draw character to represent player.

            Console.ResetColor(); // Set Console Colour to default for next method after drawing player colours.
        }

        public static void SetUpWindow() // To run upon game start - Configures console window.
        {
            Console.SetWindowSize(40, 21); // Sets the size of the game window.
            Console.SetBufferSize(Console.WindowWidth, Console.WindowHeight); // Matches screen buffer size to size of console window (Removes scroll-bar).

            Console.CursorVisible = false; // Hides the cursor from the user (prevents flickering).
            Console.Clear(); // Resets the console display by clearing console window and buffer of text.

            Console.Title = "Machine Learning - Finley Warman A-Level Computing"; // Sets the window header text.
        }

        public static void DrawInfo()
        {
            //Console.BackgroundColor = ConsoleColor.White;
            //Console.ForegroundColor = ConsoleColor.Black;

            //Console.SetCursorPosition(0, Console.WindowHeight - 1); // Place cursor to bottom left to clear line with colours.

            /*
            for(int i = 0; i < Console.WindowWidth - 1; i++)
            {
                {
                    Console.Write(' ');
                }
            }
            */

            Console.SetCursorPosition(0, Console.WindowHeight - 2);  // Place cursor to bottom left to write information.

            try
            {
                Console.Write(" Fit.: {1}, Frm.: {0}, Gen.: {2}", GameVars.CurrentFrame, Player.Fitness, Machine.Generation);
            }
            catch { }; // Catches any exceptions, in the case that the string to write exceeds the width of the console - prevents crashing.

            Console.SetCursorPosition(0, Console.WindowHeight - 1);  // Place cursor to bottom left to write information.

            try
            {
                Console.Write(" HiFit.: {1}, Inp.: {0}    ", Player.NextDirection, Machine.HighestFitness);
            }
            catch { }; // Catches any exceptions, in the case that the string to write exceeds the width of the console - prevents crashing.

            //Console.ResetColor();
        }

        public static void DrawLevelToScreen() // To draw portion of level to avaible screen space, relative to player position.
        {
            for (int Y = 0; (Y < Console.WindowHeight) && (Y < GameVars.CurrentLevel.Length); Y++) // Iterate through each string in string array (Vertical level components)
            {
                for (int X = 0; (X < Console.WindowWidth) && (X < GameVars.CurrentLevel[Y].Length); X++) // Iterate through each character for relative vertical string (Horizontal)
                {
                    Console.SetCursorPosition(X, Y);

                    if (Player.X_Pos - Player.X_ScreenPos <= 0)
                    {

                        Console.Write(GameVars.CurrentLevel[Y][X]);

                    }

                    else if ((Player.X_Pos - Player.X_ScreenPos) > (GameVars.CurrentLevel[0].Length - Console.WindowWidth))
                    {

                        Console.Write(GameVars.CurrentLevel[Y][X + (GameVars.CurrentLevel[0].Length - Console.WindowWidth)]);

                    }

                    else
                    {
                        Console.Write(GameVars.CurrentLevel[Y][X + Player.X_Pos - Player.X_ScreenPos]); //
                        // Uses the difference between the players true coordinates and screen coordinates                           //
                        // to find the 'origin' to draw the level from - draw the correct level portion.                             //
                    }



                }
            };
        }

        public static void DrawInputDirection()
        {
            Console.SetCursorPosition(Console.WindowWidth - 6, 0);
            Console.Write(Player.NextDirection);
        }

    }

    class Physics // Class to control all methods related to in-game motion.
    {
        public static void CheckGravity() // Will move the player towards the ground as long as the player is not in contact with it.
        {
            if (!(Player.IsStanding)) // Uses the boolean value of the player class to see if the player is on the ground.
            {
                Player.Y_Pos += 1; // Reduce vertical position of player by 1 for this frame if not touching the floor.
                Player.Y_ScreenPos += 1; //Also move the player vertically downward on-screen.
            };

        }

        public static void CheckStanding() // Method to check if the tile directly below the player is solid. (Changes the Player 'IsStanding' Bool)
        {
            if (Player.Y_Pos + 1 >= GameVars.CurrentLevel.Length)
            {
                Player.IsStanding = false;
                Player.Alive = false;
            }
            else if (GameVars.CurrentLevel[Player.Y_Pos + 1][Player.X_Pos] == ' ') //If the tile below is a space character (empty) the the player is not standing.
            {
                Player.IsStanding = false;
            }
            else // Otherwise the floor below is solid, so change IsStanding to true.
            {
                Player.IsStanding = true;
            }

        }

        public static bool CheckAdjacentSolid(string direction) // Method to check if a tile to the left or right (Depending on string arguments)
        {
            if (direction == "Right")
            {
                if ((Player.X_Pos + 1) < GameVars.CurrentLevel[0].Length & GameVars.CurrentLevel[Player.Y_Pos][Player.X_Pos + 1] != ' ')
                {
                    return true;
                }

            }
            if (direction == "Left")
            {
                if (GameVars.CurrentLevel[Player.Y_Pos][Player.X_Pos - 1] != ' ')
                {
                    return true;
                }
            }
            if (direction == "Up")
            {
                if (GameVars.CurrentLevel[Player.Y_Pos - 1][Player.X_Pos] != ' ')
                {
                    return true;
                }
            }
            return false;


        }

        public static void CalculateVerticalMotion()
        {
            CheckStanding();

            if (Player.V_Speed > 0 & CheckAdjacentSolid("Up") == false)
            {
                Player.V_Speed -= 1;
                Player.Y_Pos -= 1;
                Player.Y_ScreenPos -= 1;

            }

            else
            {
                Player.V_Speed = 0;
                CheckGravity();
            }
        }

    }

    class GameVars // Class to store Game Variables
    {
        public static string[] CurrentLevel = Levels.Lvl_1;

        public static int CurrentFrame = 0;

        public static int TickRate = 0;

        public static bool PlayerControl = false;

        public static bool GraphicsOn = true;

        public static bool DrawGameInfo = true;

        public static bool PlayingBack = false;

        public static void Reset()
        {
            CurrentFrame = 0;

            // Reset Player Variables:

            // Variables containing the two-dimensional co-ordinates of the player in the level.
            Player.X_Pos = 12;       //
            Player.Y_Pos = 12;       //
            // ---------------------------------//

            Player.Sprite = 'P'; // The symbol that will represent the player on-screen.

            // Variables containing the two-dimensional co-ordinates of the player on the screen. (This will differ to level co-ords if screen has scrolled.)
            Player.X_ScreenPos = 12; //
            Player.Y_ScreenPos = 12; //
            // ---------------------------------//

            Player.Alive = true; // Boolean to keep status of player for main game loop.

            Player.V_Speed = 0; // Stores value for vertical 'momentum' - Used for jumping as this takes place over multiple frames, unlike walking.

            Player.IsStanding = false; // Boolean to keep status of whether player is touching the floor. (Immediate tile below is solid)

            // ------------------------------
            Player.RightMost_X = 0;

            Player.Fitness = 0;

            // ------------------------------

            Player.NextDirection = "None";
        }
    }




    class Controls // Class to control user input (For Debug only)
    {
        public static string[] ControlList = { "Right", "Left", "Up", "None" };

        public static void GetPlayerInput()
        {

            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                switch (key.Key)
                {
                    case ConsoleKey.P:

                        Program.GetRunningArguments(); //Pause
                        break;

                    case ConsoleKey.RightArrow:

                        Player.NextDirection = "Right";
                        break;

                    case ConsoleKey.LeftArrow:

                        Player.NextDirection = "Left";
                        break;

                    case ConsoleKey.UpArrow:

                        Player.NextDirection = "Up";
                        break;

                    default:
                        break;
                }
            }
            else
            {
                Player.NextDirection = "None";
            }

        }

        public static void HandlePlayerDirection()
        {
            if (Player.NextDirection == "Right")
            {
                if (!(Physics.CheckAdjacentSolid("Right")))
                {
                    if (Player.X_Pos < GameVars.CurrentLevel[0].Length)
                    {

                        Player.X_Pos += 1;

                        if ((Player.X_ScreenPos + Player.X_Pos) < GameVars.CurrentLevel[0].Length)
                        {
                            if (Player.X_ScreenPos < (Console.WindowWidth * 0.60))
                            {
                                Player.X_ScreenPos += 1;
                            }
                        }
                        else
                        {
                            Player.X_ScreenPos += 1;
                        }



                        if (Player.X_Pos > Player.RightMost_X)
                        {
                            Player.RightMost_X = Player.X_Pos;
                        }

                    }
                }
            }
            if (Player.NextDirection == "Left")
            {
                if (!(Physics.CheckAdjacentSolid("Left")))
                {
                    if (Player.X_Pos > 0)
                    {

                        Player.X_Pos -= 1;
                        if (Player.X_Pos - Player.X_ScreenPos >= 0)
                        {

                            if ((Player.X_ScreenPos > (Console.WindowWidth * 0.40)))
                            {
                                Player.X_ScreenPos -= 1;
                            }
                        }
                        else
                        {

                            Player.X_ScreenPos -= 1;
                        }


                    }
                }
            }

            if (Player.NextDirection == "Up")
            {
                if (Player.IsStanding)
                {
                    Player.V_Speed = 5;
                }
            }

        }

        public static void CheckPause()
        {
            if (Console.KeyAvailable) { if (Console.ReadKey(true).Key == ConsoleKey.P) { Program.GetRunningArguments(); } } //Hold P to pause and enter argument entry screen.
        }



    }


    class Levels // Class to store string arrays containing level information. (Each level should be <= 20 high.)
    {
        public static string[] Lvl_1 =
        {
            "▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓",
            "▓      ▓▓▓▓▓▓▓▓▓▓▓▓           ▓▓▓▓▓▓                          ▓▓▓▓▓▓▓              ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓             ▓▓▓               ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓                          #",
            "▓         ▓▓▓▓▓                ▓▓▓▓                            ▓▓▓▓▓               ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓              ▓▓▓                      ▓▓▓▓▓▓▓▓▓▓▓▓▓▓                                #",
            "▓          ▓▓▓                  ▓▓                                                    ▓▓▓▓▓▓▓▓▓▓▓▓▓▓               ▓▓▓                                                                    #",
            "▓                               ▓▓                 ▓▓▓                                ▓▓▓▓▓▓▓▓▓▓▓                  ▓▓▓                                                                    #",
            "▓                               ▓▓               ▓▓▓▓▓▓                                 ▓▓▓▓▓▓                     ▓▓                                                                     #",
            "▓                                              ▓▓▓▓▓▓▓▓▓                                 ▓▓▓▓                      ▓                                                                      #",
            "▓                                                                                         ▓▓                                                                                              #",
            "▓                                                                                                                                                                                         #",
            "▓                                                ▓▓▓▓▓▓▓▓▓▓                                                        ▓▓                                                                     #",
            "▓                                           ▓▓▓▓   ▓▓▓▓▓                                                           ▓▓                                                                     #",
            "▓                                 ▓▓▓▓▓▓▓▓                                                                  ▓▓▓▓▓▓▓▓▓▓                          ▓▓▓▓▓▓                                    #",
            "▓                              ▓▓▓▓▓▓                                  ▓▓                                ▓▓▓▓▓▓▓                                ▓▓▓▓▓▓                                    #",
            "▓                       ▓▓▓                                            ▓▓                          ▓▓▓                                           ▓▓▓▓▓                               ▓▓▓▓▓#",
            "▓▓▓                                                                    ▓▓                ▓▓▓▓▓▓▓                                                 ▓▓▓▓                             ▓▓▓▓▓▓▓▓#",
            "▓▓▓▓▓▓                                                             ▓▓▓▓▓▓                                                                                   ▓▓                            #",
            "▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓                         ▓▓▓▓▓▓▓▓▓▓▓              ▓▓▓▓▓▓▓▓                                                                              ▓▓▓▓                             #",
            "▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓           ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓          ▓▓▓▓▓▓▓▓▓▓                                                ▓▓▓                                       ▓▓▓▓▓▓▓▓▓▓▓▓       #",
            "▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓    ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓    ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓    ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓  ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓",
            "▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓    ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓    ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓    ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓  ▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓",

        };

        public static string[] DebugLevel =
        {


                        "▓                                                                               #",
                        "▓                                                                               #",
                        "▓                                                                               #",
                        "▓                                                                               #",
                        "▓                                                                               #",
                        "▓                                                                               #",
                        "▓                                                                               #",
                        "▓                                                                               #",
                        "▓                                                                               #",
                        "▓                                                                               #",
                        "▓                                                                               #",
                        "▓                                                                               #",
                        "▓                                                                               #",
                        "▓                                                                               #",
                        "▓                                                                               #",
                        "▓                                                                               #",
                        "▓                                                                               #",
                        "▓                                                                               #",
                        "▓                                                                               #",
                        "▓▓▓▓▓▓▓▓▓▓▓▓.▓▓▓▓▓▓▓▓▓▓▓▓▓▓.▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓.▓▓▓▓▓▓▓▓▓.▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓▓.▓▓▓▓#",


        };


    }

    class Machine
    {
        public static List<string> inputlist = new List<string>();

        public static List<string> bestinputset = new List<string>();

        public static int Generation = 0;

        public static List<string> MachineControlList = new List<String> { "Right", "Left", "Up", "None" };

        public static int HighestFitness = 0;


        // Recording Input variables.

        public static bool RecordingInputs = false;

        public static int RecordCounter = 0;

        public static List<string> CurrentInputRecord = new List<String>();

        public static int RecordFitness = 0;

        public static int RecordStartFrame = 0;

        public static int RecordMaxFitness = 0;

        public static string CurrentCellCapture = "";



        public class InputSet
        {

            public List<string> Inputs { get; set; }

            public int MaxFitness { get; set; }

        }

        public static Dictionary<string, InputSet> KnownCells = new Dictionary<string, InputSet>();


        public static void LearnFromCellCapture()
        {
            string CellCapture = CaptureArea4x4();

            Random MutationRand = new Random();

            // Have probability of 'mutating'. Probability is dependant on Highest fitness achieved for cell, out of maximum possible fitness from sets.



            if (!(KnownCells.ContainsKey(CellCapture)))
            {
                if (RecordingInputs == false)
                {
                    RecordStartFrame = GameVars.CurrentFrame;

                    RecordingInputs = true;
                }
                if (RecordCounter <= 5)
                {

                    RecordCounter += 1;

                    CurrentInputRecord.Add(Player.NextDirection);

                    RecordFitness = (Player.RightMost_X - ((GameVars.CurrentFrame - RecordStartFrame) / RecordCounter) + 100); // Fitness is calculate as: ((Rightmost Horizontal distance - (The number of frames taken / 2)) + 100);
                }
                else
                {
                    if (RecordFitness > RecordMaxFitness)
                    {

                        RecordFitness = RecordMaxFitness;

                        InputSet InputsAndHighFit = new InputSet();

                        InputsAndHighFit.Inputs = CurrentInputRecord;

                        InputsAndHighFit.MaxFitness = RecordMaxFitness;

                        KnownCells.Add(CurrentCellCapture, InputsAndHighFit);
                    }

                    RecordingInputs = false;

                    RecordCounter = 0;

                    CurrentInputRecord.Clear();
                    
                    RecordFitness = 0;

                    RecordStartFrame = 0;

                    RecordMaxFitness = 0;

                    CurrentCellCapture = "";
                }

                //KnownCells.Add(CellCapture,);

            };




        }

        public static void LearnFromRandom()
        {
            CaptureArea4x4();

            Random rand = new Random();

            Player.NextDirection = MachineControlList[rand.Next(MachineControlList.Count)];

            inputlist.Add(Player.NextDirection);

            if (Player.Fitness > HighestFitness)
            {
                HighestFitness = Player.Fitness;
                foreach (string inp in inputlist)
                {
                    MachineControlList.Add(inp);
                }

                bestinputset = inputlist;

                inputlist.Clear();
            }

            if (Player.Fitness < 0)
            {
                Player.Alive = false;


                inputlist.Clear();

            }


        }

        public static void PlaybackBestInputs()
        {
            try
            {
                Player.NextDirection = bestinputset[GameVars.CurrentFrame];

            }
            catch
            {
                GameVars.PlayingBack = false;
                Player.Alive = false;
            }
        }

        // Capture 8 cell area surrounding player

        public static string CaptureArea4x4()
        {
            string CellArea = "";

            for (int y = Player.Y_Pos - 2; y <= Player.Y_Pos + 2; y++)
            {
                for (int x = Player.X_Pos - 2; x <= Player.X_Pos + 2; x++)
                {
                    if (!(x == Player.X_Pos & y == Player.Y_Pos))
                    {
                        try
                        {
                            CellArea += GameVars.CurrentLevel[y][x];
                        }
                        catch
                        {
                            CellArea += " ";
                        }
                    }
                }
            }

            return CellArea;
        }

    }


}

