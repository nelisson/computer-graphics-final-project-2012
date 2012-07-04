namespace FinalProjectCG
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using Ninja;

    /// <summary>
    ///   This is the main type for your game
    /// </summary>
    public class MainGame : Game
    {
        private Ninja.Ninja _ninja;
        SpriteBatch mBatch;

        public MainGame()
        {
            new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            mBatch = new SpriteBatch(GraphicsDevice);
            _ninja = new Ninja.Ninja(GraphicsDevice);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                Exit();

            _ninja.Update(GamePad.GetState(PlayerIndex.One));

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _ninja.Effect.Alpha = 1.0f;
            _ninja.Effect.DiffuseColor = new Vector3(5.0f, 5.0f, 5.0f);

            _ninja.Effect.AmbientLightColor = new Vector3(0.75f, 0.75f, 0.75f);
            _ninja.Effect.DirectionalLight0.Enabled = true;
            _ninja.Effect.DirectionalLight0.DiffuseColor = new Vector3(1, 0.5f, 0.5f);
            _ninja.Effect.DirectionalLight0.Direction =
                Vector3.Normalize(new Vector3(1.0f, -1.0f, -1.0f));

            _ninja.Effect.DirectionalLight1.Enabled = true;
            _ninja.Effect.DirectionalLight1.DiffuseColor =
                new Vector3(0.5f, 0.5f, 0.5f);
            _ninja.Effect.DirectionalLight1.Direction =
                Vector3.Normalize(new Vector3(-1.0f, -1.0f, 1.0f));
            _ninja.Effect.DirectionalLight1.SpecularColor =
                new Vector3(0.5f, 0.5f, 0.5f);

            _ninja.Effect.LightingEnabled = true;
            _ninja.Effect.World = Matrix.CreateScale(10);

            _ninja.Effect.View = Matrix.CreateLookAt(new Vector3(0f, 100f, -200), new Vector3(0, 50, 0),
                                                     new Vector3(0, 1, 0));
            _ninja.Effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45),
                                                                           (float) GraphicsDevice.Viewport.Width/
                                                                           GraphicsDevice.Viewport.Height, 1f, 10000);

            _ninja.Render(gameTime);
            base.Draw(gameTime);
        }
    }
}