# Szark Game Engine
## What is it?
**Szark Game Engine** is a GUI-less game engine created with **C++** and **C#** which allows for a way to easily make visuals or games.

## How to Install
1. Grab the OpenAL Install: https://www.openal.org/downloads/
2. Make sure you have .NET 5.0+ SDK Installed
3. Download an Empty Project from the Releases Page

## Features
- You can draw on the screen with the Canvas API
- Importing and using images and audio files
- Custom shaders for the screen
- Math functions and structures

## Documentation
Currently there is no wiki, but looking at the examples and the source code can help you get started!

## Basic Usage
<img src="https://i.imgur.com/v06ZBLK.png" width="200"></img>
### This example is game about collecting coins until there are none left.
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
        private Vec2 player;
        private readonly List<Vec2> coins = new List<Vec2>();
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
            gfx.DrawString(-1, 0, $"{coinsCollected}", Color.White, -1);

            // Win Text!
            if (coins.Count == 0)
                gfx.DrawString(20, 20, "You win!", Color.White);
        }

        static void Main() => new ExampleGame().Run();
    }
}
```

## Examples
<img src="https://i.imgur.com/SPTGHfe.gif" width="400"></img><img src="https://i.imgur.com/xqOdddD.png" width="400"></img>
<img src="https://i.imgur.com/8JcMVde.png" width="400"></img><img src="https://i.imgur.com/g03ZFz5.png" width="400"></img>

## Acknowledgments
[olcPixelGameEngine](https://github.com/OneLoneCoder/olcPixelGameEngine) by [Javidx9]
