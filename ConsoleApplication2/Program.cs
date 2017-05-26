using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelDLL;
using System.Text.RegularExpressions;

namespace ConsoleApplication2
{
    class Program : Player
    {
        private BackgammonGame bg;
        private PlayerInterface player;
        private PlayerInterface aiInterface;
        private NaiveAI nai;

        static void Main(string[] args)
        {
            new Program();
        }

        public Program()
        {
            run();
        }

        private void run()
        {
            bg = new BackgammonGame(BackgammonGame.DefaultGameBoard, new RealDice());
            Console.WriteLine("Pip w/b: " + bg.GetGameBoardState().GetPip(CheckerColor.White) + "/" + bg.GetGameBoardState().GetPip(CheckerColor.Black));


            //TODO: Make all this code happen in the the public Program() constructor. Include the AI. 
            //Try to run the game with an ai. Implement better ais, have them play agains each other.
            
            //Create the player
            player = new PlayerInterface(bg, CheckerColor.White, this);

            //Create the ai
            aiInterface = new PlayerInterface(bg, CheckerColor.Black, null);
            nai = new NaiveAI(aiInterface);
            aiInterface.SetPlayerIfNull(nai);

            Move();




            //Console.WriteLine(bg.GetGameBoardState().Stringify());
            //Console.WriteLine("Moves left: " + string.Join(",", bg.GetMovesLeft()));
            //Console.Write("Your move>");
            
        }

        public void TurnEnded()
        {
            
        }

        public void TurnStarted()
        {
            var s = Console.ReadLine();
            var ar = s.Split(' ');
            Console.WriteLine("ar: " + string.Join(",", ar));

            while (!s.Equals("quit"))
            {
                Console.Write("type quit to quit >");
                s = Console.ReadLine();
            }
        }

        public void Move()
        {
            Console.WriteLine(bg.GetGameBoardState().Stringify());
            Console.WriteLine("Pip w/b: " + bg.GetGameBoardState().GetPip(CheckerColor.White) + "/" + bg.GetGameBoardState().GetPip(CheckerColor.Black));
            Console.WriteLine("Moves left: " + string.Join(",", bg.GetMovesLeft()));
            Console.Write("Make a move>");
            var s = Console.ReadLine();
            while (!HasCorrectFormat(s) || !LegalMove(s))
            {
                Console.Write("Make a move>");
                s = Console.ReadLine();
            }
            
            var ar = s.Split(' ');
            int a = Convert.ToInt32(ar[0]);
            int b = Convert.ToInt32(ar[1]);
            bg.Move(CheckerColor.White, a, b);
            if (player.IsMyTurn())
            {
                Move();
            }
            else
            {
                nai.TurnStarted();
                Move();
            }
        }

        public bool HasCorrectFormat(string s)
        {
            Regex regex = new Regex(@"^\d+$");
            var ar = s.Split(' ');
            if (ar.Length != 2)
            {
                Console.WriteLine("Wrong number of spaces in input");
                return false;
            }
            if(!regex.IsMatch(ar[0]) || !regex.IsMatch(ar[1]))
            {
                Console.WriteLine("Input does not only consist of numbers");
                return false;
            }
            return true;
        }

        public bool LegalMove(string s)
        {
            int a, b;
            int.TryParse(s.Split(' ')[0], out a);
            int.TryParse(s.Split(' ')[1], out b);
            bool result = bg.GetLegalMovesFor(CheckerColor.White, a).Contains(b);
            if (!result)
            {
                Console.WriteLine("The specified move is not legal");
            }
            return result;
        }
    }
}
