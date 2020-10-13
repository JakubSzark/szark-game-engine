# Szark Engine V2
The newest and simplest version of the szark engine. Made with C++ and C#

## What is it?
Szark Engine V2 is a sequel to my last version. 
I focus on simplicity and no dependencies.
The engine is made up of two parts: Core and Runtime. 
The Core is written in C++ and contains things GLFW, OpenGL, OpenAL, etc...
The Runtime is made with C# and contains the meat of what a user will 
be interacting with to make their games.

## How to Install
- Download latest release dll's
- Attach dll's to c# project

## Features
- Drawing pixels in the window
- Drawing bitmap files (.bmp)
- Playing Audio (.wav)

## Example Game
This example is game about collecting coins until there are none left.
```c#
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
                if (player.x == coins[i].x && player.y == coins[i].y)
                {
                    wallet++;
                    coins.RemoveAt(i);
                    continue;
                }

                gfx.Draw((int)coins[i].x, (int)coins[i].y, Color.Yellow);
            }

            // Move the Player
            if (Keyboard[Key.W, Input.Hold]) player.y -= 1;
            if (Keyboard[Key.S, Input.Hold]) player.y += 1;
            if (Keyboard[Key.A, Input.Hold]) player.x -= 1;
            if (Keyboard[Key.D, Input.Hold]) player.x += 1;

            // Draw the player
            gfx.Draw((int)player.x, (int)player.y, Color.Green);

            // Draw total coins collected
            Text.DrawString(gfx, 0, 0, $"{wallet}", Color.White);

            // Win Text!
            if (coins.Count == 0)
                Text.DrawString(gfx, 20, 20, "You win!", Color.White);
        }

        static void Main() => new ExampleGame().Run();
    }
}
```
