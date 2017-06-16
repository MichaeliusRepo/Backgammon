using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelDLL;

namespace Backgammon.AI_Networking
{
    internal class TCPManager
    {
        private RemotePlayer RemotePlayer;

        private static TCPManager instance;
        internal static TCPManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new TCPManager();
                return instance;
            }
        }

        private TCPManager() { } // Make singleton.

        internal void Instantiate(BackgammonGame model, CheckerColor color)
        {
            RemotePlayer = new RemotePlayer(model, null,  color);
        }

        internal void MakeMove(BackgammonGame model, CheckerColor color)
        {
            if (RemotePlayer == null)
                Instantiate(model, color);
            RemotePlayer.MakeMove();
        }


    }
}
