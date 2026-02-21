using System;

namespace RadicalRacer
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new RadicalRacerMissionControl())
            {
                game.Run();
            }
        }
    }
}
