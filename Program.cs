using System;
namespace NastyEngine
{
    class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new Engine(fullscreen: false, pixelPerfect: false, gameWidth: 384 * 4, gameHeight: 216 * 4, screenWidth: 384 * 4, screenHeight: 216 * 4, startScene: new DemoScene(), title: "NASTY ENGINE"))
                game.Run();
        }
    }
}
