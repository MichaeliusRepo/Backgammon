using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelDLL
{
    class Program
    {
        static void Main(string[] args)
        {

            BackgammonGame bg = new BackgammonGame(BackgammonGame.DefaultGameBoard, new RealDice());
            Player naiWhite = new GreedyAI(null);
            bg.ConnectPlayer(CheckerColor.White, naiWhite);

            Player naiBlack = new NaiveAI(null);
            bg.ConnectPlayer(CheckerColor.Black, naiBlack);


            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
           
                
            bg.StartGame();
            
            stopwatch.Stop();

            long t = stopwatch.ElapsedMilliseconds;
            double perSec = 1000 / t;
            double goal =  (100000.0 * t / 1000)/60/60;
            Console.WriteLine("Game is over in " + t + " milliseconds. That's about " + perSec + " runs a second and it does 10k in " + goal + "hours");
            var s = Console.ReadLine();

        }
    }
}
