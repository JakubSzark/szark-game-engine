# Szark Engine V2
The newest and simplest version of the szark engine. Made with C++ and C#

## What is it?
Szark engine v2 is a sequel to my last version of szark engine but this time, I focus on simplicity and no dependencies.
I wanted to focus more on making this an actual no-gui game engine and make it really easy to get started. The engine
is made up of two parts: the core and the runtime. The core is written in C++ and contains things windowing, opengl, openal, etc...
The runtime is made with C# and contains the meat of what a user will be interacting with to make their games.

## How to Install
- Download latest release dll's
- Attach dll's to c# project

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

## Libraries Used
- GLFW
- GLAD
- OpenAL
