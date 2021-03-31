using Szark.Math;

namespace Szark.Input
{
    /// <summary>
    /// This class is in charge of the mouse cursor.
    /// </summary>
    public class Cursor
    {
        /// <summary>
        /// Scaled and clamped screen position
        /// </summary>
        public Vec2 Position { get; private set; }

        /// <summary>
        /// Raw GLFW cursor position
        /// </summary>
        public Vec2 RawPosition { get; private set; }

        internal void OnCursorEvent(double x, double y)
        {
            var game = Game.Get<Game>();

            RawPosition = new Vec2((float)x, (float)y);

            var posX = game.IsFullscreen ? x - game.WindowWidth * 0.5f : x;
            var posY = game.IsFullscreen ? y - game.WindowHeight * 0.5f : y;

            Position = new Vec2()
            {
                X = Mathf.Clamp((float)posX / game.PixelSize, 0, game.ScreenWidth),
                Y = Mathf.Clamp((float)posY / game.PixelSize, 0, game.ScreenHeight)
            };
        }
    }
}
