using System.Runtime.InteropServices;

namespace Szark.Input
{
    /// <summary>
    /// Interop type for GLFW keyboard callback.
    /// Holds the key that was used and the action.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct KeyAction
    {
        public Key Key;
        public Action Action;
    }

    /// <summary>
    /// This is the type of input we are trying to
    /// check for with the any input device with buttons.
    /// </summary>
    public enum Input
    {
        Hold,
        Release,
        Once,
    }

    /// <summary>
    /// This class is used to check for keyboard input.
    /// Use the indexing operator to check for key input.
    /// </summary>
    public class Keyboard
    {
        public bool this[Key key, Input poll]
        {
            get
            {
                if (key != current.Key) 
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

        private KeyAction current, last;

        internal void Update() => last = current;
        internal void OnKeyboardEvent(Key key, Action action) =>
            (current.Key, current.Action) = (key, action);
    }
}
