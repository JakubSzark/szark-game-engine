using System.Collections.Generic;
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
        public Dictionary<Key, bool> keys = new Dictionary<Key, bool>();
        public Dictionary<Key, bool> lastKeys = new Dictionary<Key, bool>();

        public bool this[Key key, Input poll]
        {
            get
            {
                if (!keys.ContainsKey(key) ||
                    !lastKeys.ContainsKey(key))
                    return false;

                var current = keys[key];
                var last = lastKeys[key];

                return poll switch
                {
                    Input.Hold => current,
                    Input.Release => !last && current != last,
                    Input.Once => current && current != last,
                    _ => false
                };
            }
        }

        internal void Update()
        {
            foreach (var pair in keys)
            {
                if (!lastKeys.ContainsKey(pair.Key))
                    lastKeys.Add(pair.Key, pair.Value);
                else lastKeys[pair.Key] = pair.Value;
            }
        }

        internal void OnKeyboardEvent(Key key, Action action)
        {
            bool pressed = action == Action.Press || action == Action.Repeat;
            if (keys.ContainsKey(key)) keys[key] = pressed;
            else keys.Add(key, pressed);
        }
    }
}
