using System;

namespace _3D_Radical_Racer
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (RadicalRacerMissionControl game = new RadicalRacerMissionControl())
            {
                game.Run();
            }
        }
    }
#endif
}

