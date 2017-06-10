using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ModelDLL.CheckerColor;
using System.Xml.Linq;

namespace ModelDLL
{
    class UpdateCreatorParser
    {

        private const int WHITE_BAR_POSITION_IN_UPDATE_MESSAGE = 25;
        private const int WHITE_TARGET_POSITION_IN_UPDATE_MESSAGE = 28;
        private const int BLACK_BAR_POSITION_IN_UPDATE_MESSAGE = 26;
        private const int BLACK_TARGET_POSITION_IN_UPDATE_MESSAGE = 27;

        internal static string CreateXmlForGameBoardState(GameBoardState state, string rootTag)
        {
            if(state == null)
            {
                throw new ArgumentNullException("state");
            }
            if(rootTag == null)
            {
                throw new ArgumentNullException("rootTag");
            }

            //Separating each element of the array with a space, and removes trailing spaces
            var board = state.getMainBoard().Select(i => i + " ").Aggregate((a, b) => a + b).Trim();
            
            //Wrap the board with tags
            board = "<board>" + board + "</board>";


            int whiteGoal = state.GetCheckersOnPosition(White.BearOffPositionID());
            int whiteBar = state.GetCheckersOnPosition(White.GetBar());

            //The agreed format is that black checkers on bar and target should be represented as negative
            int blackGoal = state.GetCheckersOnPosition(Black.BearOffPositionID());
            blackGoal = Math.Min(blackGoal, blackGoal * -1);

            int blackBar = state.GetCheckersOnPosition(Black.GetBar());
            blackBar = Math.Min(blackBar, blackBar * -1);

            //Wrap each of the above four values in their own tags
            var rest = String.Format("<whiteGoal>{0}</whiteGoal><whiteBar>{1}</whiteBar><blackGoal>{2}</blackGoal><blackBar>{3}</blackBar>",
                                      whiteBar, whiteGoal, blackBar, blackGoal);

            //Wrapping the entire message in the supplied root tags
            if (rootTag == "")
            {
                return board + rest;
            }
            else
            {
                return String.Format("<{0}>" + board + rest + "</{0}>", rootTag);
            }
        }

        internal static string GenerateXmlForDice(List<int> dice)
        {
            return "<dice>" + dice.Aggregate("", (a, b) => a + " " + b).Trim() + "</dice>";
        }

        internal static GameBoardState ParseGameBoardState(string xml)
        {
            XDocument xdoc = XDocument.Parse(xml);

            //Parsing the array representing positions between 1 and 24
            var boardTag = xdoc.Descendants("board");
            if(boardTag.Count() == 0)
            {
                throw new ArgumentException("There is no tag <board></board> in the input xml");
            }
            var boardString = boardTag.First().Value;
            int[] mainBoard;
            try
            {
                mainBoard = boardString.Split(' ').Select(str => int.Parse(str)).ToArray();
            }
            catch
            {
                throw new ArgumentException("The contents of the <board> tag was not all integers");
            }


            //Parsing the four remaining position
            var whiteGoalTag = xdoc.Descendants("whiteGoal");
            var whiteBarTag = xdoc.Descendants("whiteBar");
            var blackGoalTag = xdoc.Descendants("blackGoal");
            var blackBarTag = xdoc.Descendants("blackBar");
            if( whiteGoalTag.None() || whiteBarTag.None() || blackGoalTag.None() || blackBarTag.None())
            {
                throw new ArgumentException("One of the following tags are missing: <whiteGoal>, <whiteBar>, <blackGoal>, <blackBar>");
            }
            int whiteBar = 0, whiteGoal = 0, blackBar = 0, blackGoal = 0;

            bool succeeded = int.TryParse(whiteGoalTag.First().Value, out whiteGoal) &&
                             int.TryParse(whiteBarTag.First().Value, out whiteBar)   &&
                             int.TryParse(blackGoalTag.First().Value, out blackGoal) &&
                             int.TryParse(blackBarTag.First().Value, out blackBar);

            if (!succeeded)
            {
                throw new ArgumentException("The contents of one of the following tags was not an integer: <whiteGoal>, <whiteBar>, <blackGoal>, <blackBar>");
            }

            //The specification for update says that the checkers on the bar and target position for black
            //should be given in negative numbers, but GameBoardState expects them to be positive
            return new GameBoardState(mainBoard, whiteBar, whiteGoal, Math.Abs(blackBar), Math.Abs(blackGoal));
        }

        internal static string CreateXmlForMove(Move move)
        {
            if(move == null)
            {
                throw new ArgumentNullException("move");
            }
            string first = move.color == White ? "w" : "b";
            int from = AdaptPositionToOutputFormat(move.from);
            int to = AdaptPositionToOutputFormat(move.to);

            return "<move>" + first + " " + from + " " + to + "</move>";
        }

        internal static string GenerateXmlForListOfMoves(List<Move> moves)
        {
            string xml = moves.Select(move => CreateXmlForMove(move)).Aggregate("", (a,b) => a + b);
            return "<moves>" + xml + "</moves>";
        }

        internal static List<Move> ParseListOfMoves(string xml)
        {
            XDocument xdoc = XDocument.Parse(xml);
            var allMoveTags = xdoc.Descendants("move");
            if(allMoveTags.Count() == 0)
            {
                return new List<Move>();
            }

            return allMoveTags.Select(tag => ParseMove(tag.ToString())).ToList();
        }

        internal static Move ParseMove(string xml)
        {
            if(xml == null)
            {
                throw new ArgumentNullException("xml");
            }
            int lengthOfOpeningTag = "<move>".Length;
            int lengthOfClosingTag = "</move>".Length;

            int length = xml.Length - lengthOfOpeningTag - lengthOfClosingTag;
            if(length <= 0)
            {
                return Move.NoMove;
            }

            string move = xml.Substring(lengthOfOpeningTag, length);

            //We expect that the move consits of: color fromPosition toPosition , three components in total
            string[] components = move.Split(' ');
            if(components.Length < 3)
            {
                throw new ArgumentException("Not enough space separated components in input. Expected 3, had " + components.Length);
            }

            //parsing the color of the move
            CheckerColor color;
            string first = components[0];
            if(first == "w" || first == "b")
            {
                color = first == "w" ? White : Black;
            }
            else
            {
                throw new ArgumentException("Input didn't have correct format. Expected either a 'w' or 'b' as the character, got " + components[0]);
            }


            //Parsing the positions of the move
            int from, to;
            if(!int.TryParse(components[1], out from))
            {
                throw new ArgumentException("Expected the component after the first space to be an integer, but was " + components[1]);
            }

            if (!int.TryParse(components[2], out to))
            {
                throw new ArgumentException("Expected the component after the second space to be an integer, but was " + components[2]);
            }


            //The positions for special positions in the communication specification is not 
            //necesarrily the same as those used in this implementation, and might need
            //to be transformed
            from = AdaptPositionFromOutputFormat(from);
            to = AdaptPositionFromOutputFormat(to);

            return new Move(color, from, to);
        }

        internal static List<int> ParseDiceFromXml(string data)
        {
            XDocument xdoc = XDocument.Parse(data);
            var diceTag = xdoc.Descendants("dice");
            if (diceTag.Count() == 0)
            {
                throw new ArgumentException("There is no tag <dice></dice> in the input xml");
            }
            List<int> newMovesLeft;
            try
            {
                var dice = diceTag.ElementAt(0).Value.Split(' ').Select(str => int.Parse(str)).ToArray();
                newMovesLeft = new List<int>(dice);
            }
            catch
            {
                throw new ArgumentException("The contents of the <dice> tag was not all integers");
            }

            return newMovesLeft;
        }

        //the specification for how to number positions is not necesarilly identical to the numbering used 
        //in checker color, and so the values have to be transformed to fit
        private static int AdaptPositionToOutputFormat(int pos)
        {

            if (pos == White.GetBar()) return WHITE_BAR_POSITION_IN_UPDATE_MESSAGE;
            if (pos == White.BearOffPositionID()) return WHITE_TARGET_POSITION_IN_UPDATE_MESSAGE;
            if (pos == Black.GetBar()) return BLACK_BAR_POSITION_IN_UPDATE_MESSAGE;
            if (pos == Black.BearOffPositionID()) return BLACK_TARGET_POSITION_IN_UPDATE_MESSAGE;
            return pos;
        }

        private static int AdaptPositionFromOutputFormat(int pos)
        {
            if (pos == WHITE_BAR_POSITION_IN_UPDATE_MESSAGE) return White.GetBar();
            if (pos == WHITE_TARGET_POSITION_IN_UPDATE_MESSAGE) return White.BearOffPositionID();
            if (pos == BLACK_BAR_POSITION_IN_UPDATE_MESSAGE) return Black.GetBar();
            if (pos == BLACK_TARGET_POSITION_IN_UPDATE_MESSAGE) return Black.BearOffPositionID();
            return pos;
        }
    }
}
