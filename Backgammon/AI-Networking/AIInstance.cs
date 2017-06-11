using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelDLL;
using MachLearn;

namespace Backgammon.AI_Networking
{
    internal class AIInstance
    {
#pragma warning disable 0649
        private Player WhiteAI;
        private Player BlackAI;
#pragma warning restore 0649
        private bool naive = false;

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

        private void Move(BackgammonGame model, CheckerColor c, Player ai)
        {
            if (ai == null)
                if (naive)
                    ai = new NaiveAI(model, c);
                else
                    ai = new MachAI(model);
            ai.MakeMove();
        }

    }
}
