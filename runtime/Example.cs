using Szark.Graphics;
using System;

namespace Example
{
    class ExampleGame : Szark.Game
    {
        private Random random;

        public ExampleGame() : base("Example", 1280, 720, 8, false) { }

        protected override void OnCreated()
        {
            ErrorRecieved += s => Console.WriteLine($"[Error]: {s}");
            random = new Random();
        }

        protected override void OnRender(Canvas gfx, double deltaTime)
        {
            for (int i = 0; i < ScreenWidth; i++)
            {
                for (int j = 0; j < ScreenHeight; j++)
                {
                    var value = (byte)random.Next(255);
                    gfx.Draw(i, j, new Color(value, value, value));
                }
            }
        }

        static void Main() => new ExampleGame().Run();
    }
}
