using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelDLL;

namespace Backgammon.AI_Networking
{
    internal class AIInstance
    {
#pragma warning disable 0649
        private NaiveAI WhiteAI;
        private NaiveAI BlackAI;
#pragma warning restore 0649

        private static AIInstance instance;
        internal static AIInstance Instance
        {
            get
            {
                if (instance == null)
                    instance = new AIInstance();
                return instance;
            }
        }

        private AIInstance() { } // Make constructor private for singleton.


        internal void Move(BackgammonGame model, CheckerColor c)
        {
            Move(model, c, (c == CheckerColor.White) ? WhiteAI : BlackAI);
        }

        private void Move(BackgammonGame model, CheckerColor c, NaiveAI ai)
        {
            if (ai == null)
                ai = new NaiveAI(model, c);
            ai.MakeMove();
        }

        private void NintyPercentJunk()
        {

        }

    }
}
