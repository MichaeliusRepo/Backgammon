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
        static void Main(string[] args)
        {
            //MineData();
            //MineData2();
            PitMachVsNaive1000();
        }

        static void MineData2()
        {
            int gameCount = 0, MachWins = 0, NaiveWins = 0;
            while (true)
                try
                {
                    //if (gameCount % 100 == 0)
                    PrintData(gameCount, MachWins, NaiveWins);
                    if (LearnVsNaive(gameCount) is MachAI) MachWins++;
                    else NaiveWins++;
                    gameCount++;
                }
                catch (Exception e) { Console.WriteLine("Game " + gameCount + " crashed :,(    " + e.Message); }
        }

        static Player LearnVsNaive(int gameCount)
        {
            Player white, black;
            var game = new BackgammonGame(BackgammonGame.DefaultGameBoard, new RealDice());
            if (gameCount % 2 == 0)
            {
                white = new MachAI(game);
                black = new NaiveAI(game, Black);
            }
            else
            {
                black = new MachAI(game);
                white = new NaiveAI(game, White);
            }
            while (!TemporalDifference.GameOver(game.GetGameBoardState()))
                if (game.playerToMove() == White)
                    white.MakeMove();
                else
                    black.MakeMove();
            TemporalDifference.UpdateWeights(game.GetGameBoardState(), game.GetGameBoardState(), (gameCount % 2 == 0) ? White : Black);
            return (game.GetGameBoardState().getCheckersOnTarget(White) == 15) ? white : black;
        }

        static void MineData()
        {
            Console.WriteLine("Print data every ...th game: (100 for quick test, type 1000 for overnight data mining))");
            int Modulo = int.Parse(Console.ReadLine());

            int gameCount = 0, BlackWins = 0, WhiteWins = 0;
            while (true)
                try
                {
                    if (gameCount % Modulo == 0)
                        PrintData(gameCount, WhiteWins, BlackWins);
                    if (new MachAI(new BackgammonGame(BackgammonGame.DefaultGameBoard, new RealDice())).Run() == White)
                        WhiteWins++;
                    else
                        BlackWins++;
                    gameCount++;
                }
                catch (Exception e) { Console.WriteLine("Game " + gameCount + " crashed :,(    " + e.Message); }
        }

        static void PitMachVsNaive1000()
        {
            int gameCount = 0, BlackWins = 0, WhiteWins = 0;
            while (gameCount != 1000)
            {
                Console.WriteLine("W/B wins: " + WhiteWins + "/" + BlackWins);
                if (PitMachVsNaive(gameCount) == White)
                    WhiteWins++;
                else
                    BlackWins++;
                gameCount++;
            }
            Console.WriteLine("W/B wins: " + WhiteWins + "/" + BlackWins);
            Console.ReadLine();
        }

        static CheckerColor PitMachVsNaive(int gameCount)
        { // MachAI 
            var game = new BackgammonGame(BackgammonGame.DefaultGameBoard, new RealDice());
            Player white = new MachAI(game);
            Player black = new NaiveAI(game, Black);
            while (!TemporalDifference.GameOver(game.GetGameBoardState()))
                if (game.playerToMove() == White)
                    white.MakeMove();
                else
                    black.MakeMove();
            return (game.GetGameBoardState().getCheckersOnTarget(White) == 15) ? White : Black;
        }

        static void PrintData(int gameCount, int Wins, int Losses)
        {
            Console.WriteLine(gameCount);
            string[] contents = new string[5] { String.Join("|", TemporalDifference.thetaWhite),
                String.Join("|", TemporalDifference.etWhite),
                String.Join("|", TemporalDifference.thetaBlack),
                String.Join("|", TemporalDifference.etBlack),
                " W/L ratio: " + Losses + "/" + Wins};
            File.WriteAllLines(GetFilePath(gameCount), contents);
        }

        static string GetFilePath(int gameCount)
        {
            return AppDomain.CurrentDomain.BaseDirectory + "/MLdataSets/" + gameCount + ".txt";
        }
    }
}
