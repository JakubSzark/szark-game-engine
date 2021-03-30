using System;

using Szark;
using Szark.Graphics;

namespace Template
{
    class Program : Szark.Game
    {
        public Program() : base("New Window", 800, 800, 8, false) { }

        // Called when game is created
        protected override void OnCreated() { }

        // Called every frame
        protected override void OnRender(Canvas gfx, float deltaTime) { }

        // Called once window is closed
        protected override void OnDestroy() { }

        static void Main() => new Program().Run();
    }
}
