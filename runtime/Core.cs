using System;
using System.Runtime.InteropServices;
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

    [StructLayout(LayoutKind.Sequential)]
    internal struct Point { public double x, y; }

    [StructLayout(LayoutKind.Sequential)]
    internal struct Rect { public int x, y, width, height; }

    internal class Core
    {
        internal enum WindowEvent
        {
            Opened,
            Closed,
            Render,
        }

        const string CorePath = "../../../core/SzarkCore.dll";

        internal delegate void KeyCallback(Key key, Action action);
        internal delegate void ErrorCallback([MarshalAs(UnmanagedType.LPStr)] string msg);
        internal delegate void MouseCallback(int button, Action action);
        internal delegate void ScrollCallback(double dx, double dy);
        internal delegate void CursorCallback(double x, double y);

        internal delegate void WindowEventCallback(
            IntPtr window, WindowEvent ev
        );

        [DllImport(CorePath)]
        internal static extern void SetErrorCallback(
            [MarshalAs(UnmanagedType.FunctionPtr)] ErrorCallback callback
        );

        [DllImport(CorePath)]
        internal static extern void SetWindowEventCallback(
            [MarshalAs(UnmanagedType.FunctionPtr)] WindowEventCallback callback
        );

        [DllImport(CorePath)]
        internal static extern void SetKeyCallback(
            [MarshalAs(UnmanagedType.FunctionPtr)] KeyCallback callback
        );

        [DllImport(CorePath)]
        internal static extern void SetMouseCallback(
            [MarshalAs(UnmanagedType.FunctionPtr)] MouseCallback callback
        );

        [DllImport(CorePath)]
        internal static extern void SetScrollCallback(
            [MarshalAs(UnmanagedType.FunctionPtr)] ScrollCallback callback
        );

        [DllImport(CorePath)]
        internal static extern void SetCursorCallback(
            [MarshalAs(UnmanagedType.FunctionPtr)] CursorCallback callback
        );

        [DllImport(CorePath)]
        internal static extern bool InitializeLibraries();

        [DllImport(CorePath)]
        internal static extern void TerminateLibraries();

        [DllImport(CorePath)]
        internal static extern IntPtr Create(
            [MarshalAs(UnmanagedType.LPStr)] string title,
            uint width, uint height, bool fullscreen
        );

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

        [DllImport(CorePath)]
        internal static extern uint CompileShader(
            [MarshalAs(UnmanagedType.LPStr)] string vertexSrc,
            [MarshalAs(UnmanagedType.LPStr)] string fragmentSrc
        );

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
        internal static extern uint GenerateAudioClipID(int format,
            [MarshalAs(UnmanagedType.LPArray)] byte[] buffer, uint length, uint freq
        );
    }
}
