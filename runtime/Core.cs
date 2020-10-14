using System.Runtime.InteropServices;
using System;

using Szark.Audio;
using Szark.Graphics;
using Szark.Input;

namespace Szark
{
    /// <summary>
    /// GLFW Action type
    /// </summary>
    internal enum Action
    {
        Release,
        Press,
        Repeat
    }

    internal enum WindowEvent
    {
        Opened,
        Closed,
        Render,
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct Point { public double x, y; }

    [StructLayout(LayoutKind.Sequential)]
    internal struct Rect { public int x, y, width, height; }

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void KeyCallback(Key key, Action action);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void ErrorCallback([MarshalAs(UnmanagedType.LPStr)] string msg);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void MouseCallback(int button, Action action);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void ScrollCallback(double dx, double dy);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void CursorCallback(double x, double y);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void WindowEventCallback(IntPtr window, WindowEvent ev);

    internal class Core
    {
        const string CorePath = "../../../core/SzarkCore.dll";

        [DllImport(CorePath)]
        internal static extern void SetErrorCallback(ErrorCallback callback);

        [DllImport(CorePath)]
        internal static extern void SetWindowEventCallback(WindowEventCallback callback);

        [DllImport(CorePath)]
        internal static extern void SetKeyCallback(KeyCallback callback);

        [DllImport(CorePath)]
        internal static extern void SetMouseCallback(MouseCallback callback);

        [DllImport(CorePath)]
        internal static extern void SetScrollCallback(ScrollCallback callback);

        [DllImport(CorePath)]
        internal static extern void SetCursorCallback(CursorCallback callback);

        [DllImport(CorePath, CharSet = CharSet.Ansi)]
        internal static extern IntPtr Create(string title, uint width,
            uint height, bool fullscreen);

        [DllImport(CorePath)]
        internal static extern void Show(IntPtr window);

        [DllImport(CorePath)]
        internal static extern void Close(IntPtr window);

        [DllImport(CorePath)]
        internal static extern Point GetCursor();

        [DllImport(CorePath)]
        internal static extern uint GenerateTextureID(
            [MarshalAs(UnmanagedType.LPArray)] Color[] pixels,
            uint width, uint height
        );

        [DllImport(CorePath)]
        internal static extern uint UpdateTexture(
            uint textureID,
            [MarshalAs(UnmanagedType.LPArray)] Color[] pixels,
            uint width, uint height
        );

        [DllImport(CorePath, CharSet = CharSet.Ansi)]
        internal static extern uint CompileShader(string vertexSrc,
            string fragmentSrc);

        [DllImport(CorePath)]
        internal static extern void InitializeRenderer();

        [DllImport(CorePath)]
        internal static extern void UseShader(uint id);

        [DllImport(CorePath)]
        internal static extern void UseDefaultShader();

        [DllImport(CorePath)]
        internal static extern void UseTexture(uint id);

        [DllImport(CorePath)]
        internal static extern void RenderQuad();

        [DllImport(CorePath)]
        internal static extern double GetDeltaTime();

        [DllImport(CorePath)]
        internal static extern Rect GetPrimaryMonitorRect();

        [DllImport(CorePath)]
        internal static extern void SetViewport(int x, int y, int w, int h);

        [DllImport(CorePath)]
        internal static extern void InitializeAudioContext();

        [DllImport(CorePath)]
        internal static extern void PlayAudioClip(uint id, int vol, bool loop);

        [DllImport(CorePath)]
        internal static extern void StopAudioClip(uint id);

        [DllImport(CorePath)]
        internal static extern AudioClip CreateAudioClip(int format,
            [MarshalAs(UnmanagedType.LPArray)] byte[] buffer, uint length, uint freq
        );

        [DllImport(CorePath)]
        internal static extern void DestroyAudioClip(AudioClip clip);

        [DllImport(CorePath)]
        internal static extern void SetVSync(bool enabled);
    }
}
