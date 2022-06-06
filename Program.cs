using System;
using Microsoft.Xna.Framework;

namespace MetaBalls
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (Game game = new Game())
            {
                game.Run();
            }
        }
    }
}
