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

            Console.WriteLine("What color do you want to play?");
            //var humanPlayer = Console.ReadLine();

            CheckerColor humanColor = CheckerColor.White;
            bool done = false;
            while (!done)
            {
                var humanPlayer = Console.ReadLine();
                if (humanPlayer == "white")
                {
                    humanColor = CheckerColor.White;
                    done = true;
                    Console.WriteLine("White chosen");
                }
                    
                else if (humanPlayer == "black")
                {
                    humanColor = CheckerColor.Black;
                    done = true;
                    Console.WriteLine("Black chosen");
                }
                else
                {
                    Console.WriteLine("Plase try again");
                }
            }


            Dice dice = new RealDice();
            BackgammonGame game = new BackgammonGame(BackgammonGame.DefaultGameBoard, dice);


            
            RealClient client = new RealClient(null);
            RemotePlayer remotePlayer = new RemotePlayer(game, client, humanColor.OppositeColor());
            client.player = remotePlayer;

            NaiveAI ai = new NaiveAI(game, CheckerColor.White);


            Player whitePlayer = humanColor == CheckerColor.White ? (Player)ai : (Player)remotePlayer;
            Player blackPlayer = humanColor == CheckerColor.Black ? (Player)ai : (Player)remotePlayer;

            if(humanColor.OppositeColor() == CheckerColor.Black)
            {
                client.SendDataToPlayer("");
            }

            Console.WriteLine("Game is ready to start. Press enter when both players are connected");
            Console.ReadLine();

            while (!game.GameIsOver())
            {
                (game.playerToMove() == CheckerColor.White ? whitePlayer : blackPlayer).MakeMove();
            }

            Console.WriteLine("Game is over");





            /*Dice dice1 = new RealDice();
            Dice dice2 = new RealDice();


            FakeServer server = new FakeServer(null, null);

            FakeClient client1 = new FakeClient(null, server);
            server.client1 = client1;

            FakeClient client2 = new FakeClient(null, server);
            server.client2 = client2;

            //Setting up game one
            BackgammonGame game1 = new BackgammonGame(BackgammonGame.DefaultGameBoard, dice1);

            NaiveAI game1WhitePlayer = new NaiveAI(game1, CheckerColor.White);
            //game1.ConnectPlayer(CheckerColor.White, game1WhitePlayer);

            RemotePlayer game1BlackPlayer = new RemotePlayer(game1, client1, CheckerColor.Black);
            client1.player = game1BlackPlayer;

            View view1 = new ConsoleView(game1, "Game 1 View");
            game1.ConnectView(view1);



            //Setting up game two
            BackgammonGame game2 = new BackgammonGame(BackgammonGame.DefaultGameBoard, dice2);

            RemotePlayer game2WhitePlayer = new RemotePlayer(game2, client2, CheckerColor.White);
            client2.player = game2WhitePlayer;

            NaiveAI game2BlackPlayer = new NaiveAI(game2, CheckerColor.Black);
            //game2.ConnectPlayer(CheckerColor.Black, game2BlackPlayer);

            View view2 = new ConsoleView(game2, "Game 2 View");
            game2.ConnectView(view2);


            //dice1.QueueRandomRoll();
            while (!(game1.GameIsOver() || game2.GameIsOver()))
            {
                if(game1.NumberOfTurnsMade == 56)
                {
                    Console.WriteLine("stop");
                }
                while(game1.playerToMove() == CheckerColor.White)
                {
                    game1WhitePlayer.MakeMove();
                }
                game1BlackPlayer.MakeMove();


                Console.WriteLine("turn for game 1: " + game1.NumberOfTurnsMade);
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

                if (game1.GameIsOver()) break;

                while (game2.playerToMove() == CheckerColor.Black)
                {
                    game2BlackPlayer.MakeMove();
                }
                game2WhitePlayer.MakeMove();
               
            }

            Console.Write("Game is over. Enter for next round>");
            Console.ReadLine();*/



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
