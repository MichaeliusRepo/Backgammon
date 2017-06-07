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
            Dice dice = new FakeDice(new int[] { 1, 3 });

            FakeServer server = new FakeServer(null, null);

            FakeClient client1 = new FakeClient(null, server);
            server.client1 = client1;

            FakeClient client2 = new FakeClient(null, server);
            server.client2 = client2;

            //Setting up game one
            BackgammonGame game1 = new BackgammonGame(BackgammonGame.DefaultGameBoard, dice);

            NaiveAI game1WhitePlayer = new NaiveAI(null);
            game1.ConnectPlayer(CheckerColor.White, game1WhitePlayer);

            RemotePlayer game1BlackPlayer = new RemotePlayer(game1, client1, CheckerColor.Black);
            client1.player = game1BlackPlayer;

            View view1 = new ConsoleView(game1, "Game 1 View");
            game1.ConnectView(view1);



            //Setting up game two
            BackgammonGame game2 = new BackgammonGame(BackgammonGame.DefaultGameBoard, dice);

            RemotePlayer game2WhitePlayer = new RemotePlayer(game2, client2, CheckerColor.White);
            client2.player = game2WhitePlayer;

            NaiveAI game2BlackPlayer = new NaiveAI(null);
            game2.ConnectPlayer(CheckerColor.Black, game2BlackPlayer);

            View view2 = new ConsoleView(game2, "Game 2 View");
            game2.ConnectView(view2);


            int turn = 0;
            while (!game1.GameIsOver())
            {
                while(game1.NumberOfTurnsMade == turn)
                {
                    game1WhitePlayer.MakeMove();
                }
                game1BlackPlayer.MakeMove();
                turn++;

                if(game1.GetGameBoardState().Stringify() != game2.GetGameBoardState().Stringify())
                {
                    Console.WriteLine("GAME STATE IS NOT EQUAL IN THE TWO GAMES!!!");
                    Console.ReadLine();
                }

                CheckerColor a, b;
                if ((a = game1.playerToMove()) != (b = game2.playerToMove()))
                {
                    Console.WriteLine("PLAYER TO MOVE IS NOT EQUAL IN THE TWO GAMES!");
                    Console.WriteLine(a + " for game1 and " + b + " for game2");
                }


                if(game1.NumberOfTurnsMade != game2.NumberOfTurnsMade)
                {
                    Console.WriteLine("NUMBER OF TURNS MADE ARE NOT EQUAL");
                }

                //turn = game2.NumberOfTurnsMade;
                while(game2.NumberOfTurnsMade == turn)
                {
                    game2BlackPlayer.MakeMove();
                }
                game2WhitePlayer.MakeMove();
                turn++;

                Console.ReadLine();
            }

            Console.Write("Game is over. Enter for next round>");
            Console.ReadLine();



            /*BackgammonGame bg = new BackgammonGame(BackgammonGame.DefaultGameBoard, new RealDice());
            Player naiWhite = new NaiveAI(null);
            bg.ConnectPlayer(CheckerColor.White, naiWhite);

            Player naiBlack = new NaiveAI(null);
            bg.ConnectPlayer(CheckerColor.Black, naiBlack);


            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();


            // bg.StartGame();
            bg.RunGame();
            
            stopwatch.Stop();

            long t = stopwatch.ElapsedMilliseconds;
            double perSec = 1000 / t;
            double goal =  (100000.0 * t / 1000)/60/60;
            Console.WriteLine("Game is over in " + t + " milliseconds. That's about " + perSec + " runs a second and it does 10k in " + goal + "hours");
            var s = Console.ReadLine();*/

        }
    }
}
