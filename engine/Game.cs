using System;
using System.Security.Cryptography.X509Certificates;
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
        public static Game? Instance { get; private set; }

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
        public uint ScreenWidth { get; private set; }

        /// <summary>
        /// The pixel height of the Game screen
        /// </summary>
        public uint ScreenHeight { get; private set; }

        /// <summary>
        /// The size of each pixel on screen
        /// </summary>
        public uint PixelSize { get; private set; }

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

        private Canvas? canvas;
        private Texture? drawTarget;
        private Vec2 renderOffset;
        private IntPtr window;

        private uint drawTargetID;
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
            Cursor = new Cursor();
            Keyboard = new Keyboard();
            Mouse = new Mouse();

            Title = title;
            WindowWidth = width;
            WindowHeight = height;
            PixelSize = pixelSize;

            ScreenWidth = width / pixelSize;
            ScreenHeight = height / pixelSize;
            IsFullscreen = fullscreen;

            // Callbacks are required to be members
            mouseCallback = new MouseCallback(Mouse.OnMouseEvent);
            scrollCallback = new ScrollCallback(Mouse.OnScrollEvent);
            windowEventCallback = new WindowEventCallback(OnWindowEvent);
            cursorCallback = new CursorCallback(Cursor.OnCursorEvent);
            keyCallback = new KeyCallback(Keyboard.OnKeyboardEvent);
            errorCallback = new ErrorCallback(Error);

            if (Instance == null) Instance = this;
            else if (Instance != this)
                throw new Exception("Cannot have two Game instances!");

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
        /// Called when the Game window has been created
        /// </summary>
        protected virtual void OnCreated() { }

        /// <summary>
        /// Called every frame 
        /// </summary>
        /// <param name="gfx">Canvas for drawing</param>
        /// <param name="deltaTime">Delta time</param>
        protected virtual void OnRender(Canvas gfx, float deltaTime) { }

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
            if (canvas != null) OnRender(canvas, deltaTime);
            drawTarget?.Update(drawTargetID);

            Core.UseDefaultShader();
            Core.UseTexture(drawTargetID);
            Core.RenderQuad();

            Keyboard.Update();
            Mouse.Update();
        }

        void InitDrawTarget()
        {
            drawTarget = new Texture(ScreenWidth, ScreenHeight);
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
