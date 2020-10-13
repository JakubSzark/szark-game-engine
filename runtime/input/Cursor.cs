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
        public Vector Position { get; private set; }

        /// <summary>
        /// Raw GLFW cursor position
        /// </summary>
        public Vector RawPosition { get; private set; }

        internal void OnCursorEvent(double x, double y)
        {
            RawPosition = new Vector((float)x, (float)y);

            if (Game.Instance is Game game)
            {
                var posX = game.IsFullscreen ? x - game.WindowWidth * 0.5f : x;
                var posY = game.IsFullscreen ? y - game.WindowHeight * 0.5f : y;

                Position = new Vector()
                {
                    x = Mathf.Clamp((float)posX / game.PixelSize, 0, game.ScreenWidth),
                    y = Mathf.Clamp((float)posY / game.PixelSize, 0, game.ScreenHeight)
                };
            }
        }
    }
}
