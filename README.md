# Szark Engine V2
The newest and simplest version of the szark engine. Made with C++ and C#

## What is it?
Szark Engine V2 is a sequel to the last version. 
I focus on simplicity and no package dependencies.
The engine is made up of two parts: Core and Runtime. 
The Core is written in C++ and contains things GLFW, OpenGL, OpenAL, etc...
The Runtime is made with C# and contains the meat of what will
be interacted with to make games.

## How to Install
- Make sure you have .NET 5.0 SDK Installed
- Download an Empty Project from the Releases Page

## Features
- Drawing pixels in the window
- Drawing bitmap files (.bmp)
- Playing audio (.wav)

## Example Game
This example is game about collecting coins until there are none left.
```c#
using System.Collections.Generic;
using System;

using Szark.Math;
using Szark.Graphics;
using Szark.Input;

namespace Example
{
    class ExampleGame : Szark.Game
    {
        private Vector player;
        private readonly List<Vector> coins = new List<Vector>();
        private int coinsCollected;

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
        protected override void OnRender(Canvas gfx, float deltaTime)
        {
            // Clear the screen
            gfx.Fill(Color.Black);

            // Draw all the coins
            for (int i = 0; i < coins.Count; i++)
            {
                // Check if player is in same position
                if (Vector.Distance(player, coins[i]) < 1)
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
            gfx.DrawString(-1, 0, $"{coinsCollected}", Color.White, -1);

            // Win Text!
            if (coins.Count == 0)
                gfx.DrawString(20, 20, "You win!", Color.White);
        }

        static void Main() => new ExampleGame().Run();
    }
}
```

## Other Examples
### Raycaster
<img src="https://i.imgur.com/SPTGHfe.gif" width="400">

## Acknowledgments
[olcPixelGameEngine](https://github.com/OneLoneCoder/olcPixelGameEngine) by [Javidx9]
