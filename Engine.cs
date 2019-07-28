#define NASTY_ENGINE
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NastyEngine.Input;

#if DEBUG

using System.Diagnostics;

#endif

namespace NastyEngine
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Engine : Game
    {
        public static Engine Instance_;

        private GraphicsDeviceManager graphics;
        public static GraphicsDeviceManager Graphics { get { return Instance_.graphics; } }

        public static int GameWidth;
        public static int GameHeight;

        public static int ViewWidth = 48 * 8;
        public static int ViewHeight = 27 * 8;

        public static int ScreenWidth;
        public static int ScreenHeight;

        public static float ScreenRatio;
        public static Vector2 ScreenIndent;

        public static Matrix ScreenMatrix;
        public static Viewport Viewport;

        private bool pixelPerfect;
        public static bool PixelPerfect { get { return Instance_.pixelPerfect; } set { if (Instance_.pixelPerfect) Instance_.UpdateView(); else Instance_.UpdateViewPixelPerfect(); Instance_.pixelPerfect = value; } }

        private bool resizing = false;

        private SceneManager sceneManager;
        private InputManager inputManager;

        private readonly string title;

        public delegate void ClosingWindowDel();

        ClosingWindowDel closingWindow;

        public static void SetClosingDelegate(ClosingWindowDel del) { Instance_.closingWindow = del; }

        public Engine(bool fullscreen = false, bool pixelPerfect = false, int screenWidth = 48 * 8, int screenHeight = 27 * 8, int gameWidth = 48 * 8, int gameHeight = 27 * 8, Scene startScene = null, string title = "title")
        {
            this.title = title;
            // Initialize Scenes

            sceneManager = new SceneManager(startScene);
            GameWidth = gameWidth;
            GameHeight = gameHeight;

            if (Instance_ == null)
                Instance_ = this;

            Content.RootDirectory = "Content";

            graphics = new GraphicsDeviceManager(this);
            graphics.DeviceReset += new EventHandler<EventArgs>(OnGraphicsReset);
            graphics.DeviceCreated += new EventHandler<EventArgs>(OnGraphicsCreate);
            graphics.SynchronizeWithVerticalRetrace = true;
            graphics.PreferMultiSampling = false;
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            graphics.PreferredBackBufferFormat = SurfaceFormat.Color;
            graphics.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;
            graphics.ApplyChanges();

            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += new EventHandler<EventArgs>(Window_ClientSizeChanged);

            ScreenWidth = graphics.PreferredBackBufferWidth = screenWidth;//GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            ScreenHeight = graphics.PreferredBackBufferHeight = screenHeight;//GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            graphics.ApplyChanges();
            if (!graphics.IsFullScreen && fullscreen)
                graphics.ToggleFullScreen();

            IsMouseVisible = true;
            PixelPerfect = pixelPerfect;
        }

        void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            if (Window.ClientBounds.Width > 0 && Window.ClientBounds.Height > 0 && !resizing)
            {
                resizing = true;

                ScreenWidth = graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
                ScreenHeight = graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;

                if (pixelPerfect)
                    UpdateViewPixelPerfect();
                else
                    UpdateView();

                resizing = false;
            }
        }

        void OnGraphicsReset(object sender, EventArgs e)
        {
            if (pixelPerfect)
                UpdateViewPixelPerfect();
            else
                UpdateView();
        }

        void OnGraphicsCreate(object sender, EventArgs e)
        {
            if (pixelPerfect)
                UpdateViewPixelPerfect();
            else
                UpdateView();
        }

        protected override void Initialize()
        {
            Window.Title = title;
            Window.TextInput += TextInputHandler;

            var collisionManager = new CollisionManager(GameWidth, GameHeight);

            var resourceManager = new ResourceManager(Content);

            inputManager = new InputManager();

            base.Initialize();
        }

        private void TextInputHandler(object sender, TextInputEventArgs args)
        {
            KeyInput.InputChar = args.Character;
        }

        protected override void LoadContent()
        {
            ResourceManager.LoadContent();
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            base.OnExiting(sender, args);

            closingWindow?.Invoke();
        }

        protected override void Update(GameTime gameTime)
        {
            inputManager.Update();

#if DEBUG
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            //    Exit();
#endif


            GTime.Delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            GTime.UpdateTime((float)gameTime.ElapsedGameTime.TotalSeconds);

            // if we just changed scenes, init the scene
            SceneManager.InitCurrentScene();
            SceneManager.UpdateCurrentScene();

            CollisionManager.Update();

            base.Update(gameTime);
        }

        public static RenderTarget2D MainImage;
        protected override void Draw(GameTime gameTime)
        {

            // prepare render
            GraphicsDevice.SetRenderTarget(MainImage);
            GraphicsDevice.Viewport = Viewport;
            GraphicsDevice.Clear(SceneManager.GetCurrentScene().clearColor);

            // Main Render

            sceneManager.CurrentScene.RenderBegin();
            sceneManager.CurrentScene.Render();
            CollisionManager.Render();
            sceneManager.CurrentScene.RenderEnd();
            //
            base.Draw(gameTime);
        }

        private void UpdateViewPixelPerfect()
        {
            float screenWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
            float screenHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;

            if (screenWidth / GameWidth > screenHeight / GameHeight)
            {
                ViewWidth = (int)(screenHeight / GameHeight * GameWidth);
                ViewHeight = (int)screenHeight;
            }
            else
            {
                ViewWidth = (int)screenWidth;
                ViewHeight = (int)(screenWidth / GameWidth * GameHeight);
            }

            float scale = (int)(ViewWidth / (float)GameWidth);

            // view size
            ViewWidth = (int)screenWidth;
            ViewHeight = (int)screenHeight;

            ScreenMatrix = Matrix.CreateScale(scale);
            ScreenMatrix.M33 = 1;


            //float rWidth = (float)GraphicsDevice.PresentationParameters.BackBufferWidth / (float)GameWidth;//ratio for width
            //float rHeight = (float)GraphicsDevice.PresentationParameters.BackBufferHeight / (float)GameHeight;//ratio for height
            //
            //float ratioToUse = (int)Math.Floor(rWidth < rHeight ? rWidth : rHeight);
            //ratioToUse = ratioToUse == 0 ? rWidth < rHeight ? rWidth : rHeight : ratioToUse;
            //int gWidth = (int)(GameWidth * ratioToUse);
            //int gHeight = (int)(GameHeight * ratioToUse);
            //
            //int gX = (GraphicsDevice.PresentationParameters.BackBufferWidth / 2) - (gWidth / 2);
            //int gY = (GraphicsDevice.PresentationParameters.BackBufferHeight / 2) - (gHeight / 2);

            //ScreenRatio = ratioToUse;

            // TODO: fix scaling issues
            // make viewport
            Viewport = new Viewport
            {
                X = (int)(screenWidth / 2 - (GameWidth * scale) / 2),
                Y = (int)(screenHeight / 2 - (GameHeight * scale) / 2),
                Width = (int)(GameWidth * scale),
                Height = (int)(GameHeight * scale),
                MinDepth = 0,
                MaxDepth = 1
            };
            MainImage = new RenderTarget2D(GraphicsDevice, (int)screenWidth, (int)screenHeight, false, GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth16);
        }

        private void UpdateView()
        {
            float screenWidth = GraphicsDevice.PresentationParameters.BackBufferWidth;
            float screenHeight = GraphicsDevice.PresentationParameters.BackBufferHeight;

            // view size
            if (screenWidth / GameWidth > screenHeight / GameHeight)
            {
                ViewWidth = (int)(screenHeight / GameHeight * GameWidth);
                ViewHeight = (int)screenHeight;
            }
            else
            {
                ViewWidth = (int)screenWidth;
                ViewHeight = (int)(screenWidth / GameWidth * GameHeight);
            }

            //float aspect = ViewHeight / (float)ViewWidth;
            //ViewHeight -= (int)aspect;

            // create matrix for scaling based on view size
            float scale = ViewWidth / (float)GameWidth;
            ScreenMatrix = Matrix.CreateScale(scale);
            ScreenMatrix.M33 = 1;

            // make viewport
            Viewport = new Viewport
            {
                X = (int)(screenWidth / 2 - ViewWidth / 2),
                Y = (int)(screenHeight / 2 - ViewHeight / 2),
                Width = ViewWidth,
                Height = ViewHeight,
                MinDepth = 0,
                MaxDepth = 1
            };

            MainImage = new RenderTarget2D(GraphicsDevice, (int)screenWidth, (int)screenHeight, false, GraphicsDevice.PresentationParameters.BackBufferFormat, DepthFormat.Depth24Stencil8);
        }

#if DEBUG
        public static void Log(object message)
        {
            Debug.WriteLine(message);
        }
#else
        public static void Log(object message){}
#endif
    }
}
