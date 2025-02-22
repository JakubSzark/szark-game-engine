﻿using System;
using System.Runtime.CompilerServices;
using System.Reflection;

using Szark.ECS;
using Szark.Graphics;
using Szark.Input;
using Szark.Math;

namespace Szark
{
    public abstract class Game
    {
        /// <summary>
        /// The singleton instance of Game
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Get<T>() where T : Game
        {
            if (_instance == null)
                throw new ApplicationException("Game internal instance is null!");

            return (T)_instance;
        }

        /// <summary>
        /// Callback for any errors that occur
        /// </summary>
        public event Action<string>? ErrorRecieved;

        /// <summary>
        /// Width of the GLFW window
        /// </summary>
        public uint WindowWidth { get; private set; }

        /// <summary>
        /// Height of the GLFW window
        /// </summary>
        public uint WindowHeight { get; private set; }

        /// <summary>
        /// The pixel width of the Game screen
        /// </summary>
        public int ScreenWidth { get; private set; }

        /// <summary>
        /// The pixel height of the Game screen
        /// </summary>
        public int ScreenHeight { get; private set; }

        /// <summary>
        /// The size of each pixel on screen
        /// </summary>
        public int PixelSize { get; private set; }

        /// <summary>
        /// The title of the Game window
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Whether or not the Game window is fullscreen
        /// </summary>
        public bool IsFullscreen { get; private set; }

        /// <summary>
        /// Sets whether window vsync is enabled
        /// </summary>
        public bool VSync
        {
            get => vsyncEnabled; set
            {
                Core.SetVSync(value);
                vsyncEnabled = value;
            }
        }

        public Mouse Mouse { get; private set; }
        public Keyboard Keyboard { get; private set; }
        public Cursor Cursor { get; private set; }

        public EntityManager EntityManager { get; private set; }

        private static Game? _instance;

        private Canvas? canvas;
        private Texture? drawTarget;
        private Vec2 renderOffset;
        private IntPtr window;

        private uint drawTargetID;
        private uint? customShader;

        private bool vsyncEnabled = true;

        // Window callbacks
        private readonly MouseCallback mouseCallback;
        private readonly WindowEventCallback windowEventCallback;
        private readonly ScrollCallback scrollCallback;
        private readonly CursorCallback cursorCallback;
        private readonly ErrorCallback errorCallback;
        private readonly KeyCallback keyCallback;

        public Game(string title, uint width, uint height, uint pixelSize, bool fullscreen)
        {
            if (_instance == null) _instance = this;
            else if (_instance != this)
                throw new Exception("Cannot have two Game instances!");

            Cursor = new Cursor();
            Keyboard = new Keyboard();
            Mouse = new Mouse();

            Title = title;
            WindowWidth = width;
            WindowHeight = height;
            PixelSize = (int)pixelSize;

            ScreenWidth = (int)width / (int)pixelSize;
            ScreenHeight = (int)height / (int)pixelSize;
            IsFullscreen = fullscreen;

            // Callbacks are required to be members
            mouseCallback = new MouseCallback(Mouse.OnMouseEvent);
            scrollCallback = new ScrollCallback(Mouse.OnScrollEvent);
            windowEventCallback = new WindowEventCallback(OnWindowEvent);
            cursorCallback = new CursorCallback(Cursor.OnCursorEvent);
            keyCallback = new KeyCallback(Keyboard.OnKeyboardEvent);
            errorCallback = new ErrorCallback(Error);

            EntityManager = new EntityManager(Assembly.GetCallingAssembly());

            SetupCallbacks();
        }

        /// <summary>
        /// Runs the Game and begins calls on virtual methods
        /// </summary>
        public void Run()
        {
            window = Core.Create(Title, WindowWidth,
                WindowHeight, IsFullscreen);
            if (window.ToInt64() == 0) return;
            Core.Show(window);
        }

        /// <summary>
        /// Stops the Game and closes the window
        /// </summary>
        public void Stop() => Core.Close(window);

        /// <summary>
        /// Compiles and uses a custom shader for the Canvas.
        /// Any shader errors are sent to the error callback.
        /// </summary>
        /// <param name="vertexSrc">The Vertex Shader</param>
        /// <param name="fragSrc">The Fragment Shader</param>
        public Shader SetCustomShader(string vertexSrc, string fragSrc)
        {
            Shader shader = Core.CompileShader(vertexSrc, fragSrc);
            customShader = shader.ID;
            return shader;
        }

        /// <summary>
        /// Called when the Game window has been created
        /// </summary>
        protected virtual void OnCreated() { }

        /// <summary>
        /// Called every frame 
        /// </summary>
        /// <param name="gfx">Canvas for drawing</param>
        /// <param name="deltaTime">Delta time</param>
        protected virtual void OnRender(Canvas canvas, float deltaTime) { }

        /// <summary>
        /// Called when the window is closed
        /// </summary>
        protected virtual void OnDestroy() { }

        internal void Error(string error) =>
            ErrorRecieved?.Invoke(error);

        void OnWindowEvent(IntPtr window, WindowEvent ev)
        {
            switch (ev)
            {
                case WindowEvent.Opened:
                    Core.InitializeRenderer();
                    Core.InitializeAudioContext();
                    InitDrawTarget();
                    OnCreated();
                    break;

                case WindowEvent.Closed:
                    OnDestroy();
                    break;

                case WindowEvent.Render:
                    Render();
                    break;
            }
        }

        void Render()
        {
            // Make sure window stays squared and centered
            int w = (int)WindowWidth, h = (int)WindowHeight;
            int x = (int)renderOffset.X, y = (int)renderOffset.Y;
            Core.SetViewport(x, y, w, h);

            float deltaTime = (float)Core.GetDeltaTime();

            if (canvas != null)
            {
                OnRender(canvas, deltaTime);
                EntityManager.ExecuteSystems(canvas, deltaTime);
            }

            drawTarget?.Update(drawTargetID);

            if (customShader != null)
                Core.UseShader(customShader.Value);
            else
                Core.UseDefaultShader();

            Core.UseTexture(drawTargetID);
            Core.RenderQuad();

            Keyboard.Update();
            Mouse.Update();
        }

        void InitDrawTarget()
        {
            drawTarget = new Texture((uint)ScreenWidth, (uint)ScreenHeight);
            drawTargetID = drawTarget.GenerateID();
            canvas = drawTarget.GetCanvas();

            var monitor = Core.GetPrimaryMonitorRect();

            if (IsFullscreen)
            {
                var offsetX = (monitor.width - WindowWidth) * 0.5f;
                var offsetY = (monitor.height - WindowHeight) * 0.5f;

                renderOffset.X = offsetX;
                renderOffset.Y = offsetY;
            }
        }

        void SetupCallbacks()
        {
            Core.SetErrorCallback(errorCallback);
            Core.SetWindowEventCallback(windowEventCallback);

            Core.SetScrollCallback(scrollCallback);
            Core.SetKeyCallback(keyCallback);
            Core.SetCursorCallback(cursorCallback);
            Core.SetMouseCallback(mouseCallback);
        }
    }
}
