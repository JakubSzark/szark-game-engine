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
```
