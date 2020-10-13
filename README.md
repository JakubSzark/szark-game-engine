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
- Entity Component System (TODO)

## Example Code
This code creates a simple window with static.
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
        private readonly Random random = new Random();
        private readonly List<(int, int)> coins = new List<(int, int)>();
        private Vector position;
        private int wallet;

        // We setup our window configuration in the base constructor
        public ExampleGame() : base("Example", 1280, 720, 8, false) { }

        // This method is called when the Game is created
        protected override void OnCreated()
        {
            // Here we setup error logging
            ErrorRecieved += s => Console.WriteLine($"[Error]: {s}");

            // Spawn all the coins
            for (int i = 0; i < ScreenWidth; i++)
                for (int j = 0; j < ScreenHeight; j++)
                    if (random.Next(50) == 0) coins.Add((i, j));
        }

        // This method is called once per frame
        protected override void OnRender(Canvas gfx, double deltaTime)
        {
            // Clear the screen
            gfx.Fill(Color.Black);

            // Draw all the coins
            for (int i = 0; i < coins.Count; i++)
            {
                if ((int)position.x == coins[i].Item1 &&
                    (int)position.y == coins[i].Item2)
                {
                    coins.RemoveAt(i);
                    wallet++;
                    continue;
                }

                gfx.Draw(coins[i].Item1, coins[i].Item2, Color.Yellow);
            }

            // Move the Player
            if (Keyboard[Key.W, Input.Hold]) position.y -= 1;
            if (Keyboard[Key.S, Input.Hold]) position.y += 1;
            if (Keyboard[Key.A, Input.Hold]) position.x -= 1;
            if (Keyboard[Key.D, Input.Hold]) position.x += 1;

            // Draw the player
            gfx.Draw((int)position.x, (int)position.y, Color.Green);

            // Draw wallet
            Text.DrawString(gfx, 0, 0, $"{wallet}", Color.White);

            // Win Text!
            if (coins.Count == 0)
                Text.DrawString(gfx, 20, 20, "You win!", Color.White);
        }

        static void Main() => new ExampleGame().Run();
    }
}
```
