using Szark.Input;
using Szark.Graphics;
using Szark.Math;

using System;
using System.Collections.Generic;

namespace Example
{
    class CoinGame : Szark.Game
    {
        private Vec2 player;
        private readonly List<Vec2> coins = new List<Vec2>();
        private int coinsCollected;

        // We setup our window configuration in the base constructor
        public CoinGame() : base("Example", 800, 800, 8, false) { }

        // This method is called when the Game is created
        protected override void OnCreated()
        {
            // Here we setup error logging
            ErrorRecieved += s => Console.WriteLine($"[Error]: {s}");

            // Place the player in the center
            player = new Vec2(ScreenWidth / 2, ScreenHeight / 2);

            // Spawn all the coins
            Random random = new Random();
            for (int i = 0; i < ScreenWidth; i++)
                for (int j = 0; j < ScreenHeight; j++)
                    if (random.Next(50) == 0) coins.Add(new Vec2(i, j));
        }

        // This method is called once per frame
        protected override void OnRender(Canvas gfx, float deltaTime)
        {
            // Clear the screen
            gfx.Fill(Color.Black);

            // Draw all the coins
            for (int i = 0; i < coins.Count; i++)
            {
                // Check if player is in same position
                if (Vec2.Distance(player, coins[i]) < 1)
                {
                    coinsCollected++;
                    coins.RemoveAt(i);
                    continue;
                }

                gfx.Draw(coins[i], Color.Yellow);
            }

            // Move the Player
            float speed = 50 * deltaTime;
            if (Keyboard[Key.W, Input.Hold]) player.Y -= speed;
            if (Keyboard[Key.S, Input.Hold]) player.Y += speed;
            if (Keyboard[Key.A, Input.Hold]) player.X -= speed;
            if (Keyboard[Key.D, Input.Hold]) player.X += speed;

            gfx.Draw(player, Color.Green); // Draw the player

            // Draw total coins collected
            gfx.DrawString(4, 4, $"{coinsCollected}", Color.White, -1);

            // Win Text!
            if (coins.Count == 0)
                gfx.DrawString(20, 20, "You win!", Color.White);
        }
    }
}