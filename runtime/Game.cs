using System;
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
        /// Called when an error occurs
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
        /// Render offset of the Game screen
        /// </summary>
        public Vector RenderOffset { get; private set; }

        public Mouse Mouse { get; private set; }
        public Keyboard Keyboard { get; private set; }
        public Cursor Cursor { get; private set; }

        private Canvas? canvas;
        private Texture? drawTarget;
        private IntPtr window;

        private uint drawTargetID;

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
            if (Core.InitializeLibraries())
            {
                window = Core.Create(Title, WindowWidth,
                    WindowHeight, IsFullscreen);
                if (window.ToInt64() == 0) return;

                Core.Show(window);
                Core.TerminateLibraries();
            }
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
        protected virtual void OnRender(Canvas gfx, double deltaTime) { }

        /// <summary>
        /// Called when the window is closed
        /// </summary>
        protected virtual void OnDestroy() { }

        void OnError(string error) => ErrorRecieved?.Invoke(error);

        void OnWindowEvent(IntPtr window, Core.WindowEvent ev)
        {
            switch (ev)
            {
                case Core.WindowEvent.Opened:
                    Core.InitializeRenderer();
                    Core.InitializeAudioContext();
                    drawTarget = new Texture(ScreenWidth, ScreenHeight);
                    drawTargetID = drawTarget.GenerateID();
                    canvas = drawTarget.GetCanvas();

                    var rect = Core.GetPrimaryMonitorRect();

                    RenderOffset = new Vector()
                    {
                        x = IsFullscreen ? (rect.width - WindowWidth) * 0.5f : 0,
                        y = IsFullscreen ? (rect.height - WindowHeight) * 0.5f : 0
                    };

                    OnCreated();
                    break;

                case Core.WindowEvent.Closed:
                    OnDestroy();
                    break;

                case Core.WindowEvent.Render:
                    // Make sure window stays squared and centered
                    Core.SetViewport((int)RenderOffset.x, (int)RenderOffset.y,
                        (int)WindowWidth, (int)WindowHeight);

                    if (canvas != null)
                        OnRender(canvas, Core.GetDeltaTime());
                    if (drawTarget != null)
                        drawTarget.Update(drawTargetID);

                    Core.UseDefaultShader();
                    Core.UseTexture(drawTargetID);
                    Core.RenderQuad();

                    Keyboard.Update();
                    Mouse.Update();
                    break;
            }
        }

        void SetupCallbacks()
        {
            Core.SetErrorCallback(OnError);
            Core.SetWindowEventCallback(OnWindowEvent);

            Core.SetScrollCallback(Mouse.OnScrollEvent);
            Core.SetKeyCallback(Keyboard.OnKeyboardEvent);
            Core.SetCursorCallback(Cursor.OnCursorEvent);
            Core.SetMouseCallback(Mouse.OnMouseEvent);
        }
    }
}
