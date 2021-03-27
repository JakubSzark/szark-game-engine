using System.Runtime.InteropServices;
using Szark.Math;

namespace Szark.Input
{
    /// <summary>
    /// Interop type for GLFW mouse button input.
    /// Contains the button and the action.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct MouseAction
    {
        public int Button;
        public Action Action;
    }

    /// <summary>
    /// This class is used to check for mouse button input.
    /// Use the indexing operator to check for input.
    /// </summary>
    public class Mouse
    {
        public bool this[int button, Input poll]
        {
            get
            {
                if (button != current.Button)
                    return false;

                return poll switch
                {
                    Input.Hold => current.Action == Action.Press
                        || current.Action == Action.Repeat,
                    Input.Release => last.Action == Action.Press
                        && current.Action != last.Action,
                    Input.Once => current.Action == Action.Press
                        && current.Action != last.Action,
                    _ => false
                };
            }
        }

        public Vec2 Wheel { get; private set; }

        private MouseAction current, last;

        internal void Update() => last = current;
        internal void OnMouseEvent(int button, Action action) =>
            (current.Button, current.Action) = (button, action);

        internal void OnScrollEvent(double dx, double dy) =>
            Wheel = new Vec2((float)dx, (float)dy);
    }
}
