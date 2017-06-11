using System;
using System.Diagnostics;

namespace Backgammon
{
#if WINDOWS || LINUX
    // The main class.
    public static class Program
    {
        // The main entry point for the application.
        [STAThread]
        static void Main()
        {
            using (var game = new Backgammon())
                game.Run();
        }
    }
#endif
}
