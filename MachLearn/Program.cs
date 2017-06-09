using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using ModelDLL;
using static ModelDLL.CheckerColor;

namespace MachLearn
{
    class Program
    {
        private string LocalDirectory => AppDomain.CurrentDomain.BaseDirectory;
        private static int Modulo;

        static void Main(string[] args)
        {
            Console.WriteLine("Print data every ...th game: (100 for quick test, type 1000 for overnight data mining))");
            Modulo = int.Parse(Console.ReadLine());
            MineData();
            //PitMachVsNaive1000();
        }

        static void MineData()
        {
            int gameCount = 0, BlackWins = 0, WhiteWins = 0;
            while (true)
                try
                {
                    if (gameCount % Modulo == 0)
                        PrintData(gameCount, WhiteWins, BlackWins);
                    if (new MachPlayer(new BackgammonGame(BackgammonGame.DefaultGameBoard, new RealDice())).Run() == White)
                        WhiteWins++;
                    else
                        BlackWins++;
                    gameCount++;
                }
                catch (Exception) { Console.WriteLine("Game" + gameCount + " crashed :,("); }
        }

        static void PitMachVsNaive1000()
        {
            Console.WriteLine("Mining data...");
            int gameCount = 0, BlackWins = 0, WhiteWins = 0;
            PrintData(gameCount, BlackWins, WhiteWins);
            while (gameCount != 1000)
            {
                Console.WriteLine(gameCount);
                if (PitMachVsNaive(gameCount) == White)
                    WhiteWins++;
                else
                    BlackWins++;
                gameCount++;
            }
            PrintData(gameCount, BlackWins, WhiteWins);
            Console.WriteLine("W/B wins: " + WhiteWins + "/" + BlackWins);
            Console.ReadLine();
        }

        static CheckerColor PitMachVsNaive(int gameCount)
        {
            var game = new BackgammonGame(BackgammonGame.DefaultGameBoard, new RealDice());
            Player black = new MachPlayer(game);
            Player white = new NaiveAI(game, White);
            while (!TemporalDifference.GameOver(game.GetGameBoardState()))
                if (game.playerToMove() == White)
                    white.MakeMove();
                else
                    black.MakeMove();
            return (game.GetGameBoardState().getCheckersOnTarget(White) == 15) ? White : Black;
        }

        static void PrintData(int gameCount, int BlackWins, int WhiteWins)
        {
            Console.WriteLine(gameCount);
            string[] contents = new string[3] { String.Join("     ", TemporalDifference.w),
                String.Join("     ", TemporalDifference.EligibilityTraces),
                " W/B wins: " + WhiteWins + "/" + BlackWins};
            File.WriteAllLines(GetFilePath(gameCount), contents);
        }

        static string GetFilePath(int gameCount)
        {
            return AppDomain.CurrentDomain.BaseDirectory + "/MLdataSets/" + gameCount + ".txt";
        }
    }
}
