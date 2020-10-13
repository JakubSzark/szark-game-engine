using System.Collections.Generic;
using System;

using Szark.Math;
using Szark.Graphics;
using Szark.Input;
using Szark;

namespace Example
{
    class ExampleGame : Szark.Game
    {
        private Vector player;
        private readonly List<Vector> coins = new List<Vector>();
        private int wallet;

        // We setup our window configuration in the base constructor
        public ExampleGame() : base("Example", 1280, 720, 8, false) { }

        // This method is called when the Game is created
        protected override void OnCreated()
        {
            // Here we setup error logging
            ErrorRecieved += s => Console.WriteLine($"[Error]: {s}");

            // Spawn all the coins
            Random random = new Random();
            for (int i = 0; i < ScreenWidth; i++)
                for (int j = 0; j < ScreenHeight; j++)
                    if (random.Next(50) == 0) coins.Add(new Vector(i, j));
        }

        // This method is called once per frame
        protected override void OnRender(Canvas gfx, double deltaTime)
        {
            // Clear the screen
            gfx.Fill(Color.Black);

            // Draw all the coins
            for (int i = 0; i < coins.Count; i++)
            {
                // Check if player is in same position
                if (player.X == coins[i].X && player.Y == coins[i].Y)
                {
                    wallet++;
                    coins.RemoveAt(i);
                    continue;
                }

                gfx.Draw((int)coins[i].X, (int)coins[i].Y, Color.Yellow);
            }

            // Move the Player
            if (Keyboard[Key.W, Input.Hold]) player.Y -= 1;
            if (Keyboard[Key.S, Input.Hold]) player.Y += 1;
            if (Keyboard[Key.A, Input.Hold]) player.X -= 1;
            if (Keyboard[Key.D, Input.Hold]) player.X += 1;

            // Draw the player
            gfx.Draw((int)player.X, (int)player.Y, Color.Green);

            // Draw total coins collected
            gfx.DrawString(0, 0, $"{wallet}", Color.White);

            // Win Text!
            if (coins.Count == 0)
                gfx.DrawString(20, 20, "You win!", Color.White);
        }

        static void Main() => new ExampleGame().Run();
    }
}
