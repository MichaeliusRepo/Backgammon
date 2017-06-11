using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelDLL;

namespace Backgammon.AI_Networking
{
    internal class TcpInstance
    {
        private RemotePlayer RemotePlayer;

        private static TcpInstance instance;
        internal static TcpInstance Instance
        {
            get
            {
                if (instance == null)
                    instance = new TcpInstance();
                return instance;
            }
        }

        private TcpInstance() { } // Make singleton.

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
