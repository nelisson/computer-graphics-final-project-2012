﻿namespace FinalProjectCG
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
        SpriteBatch _mBatch;
        Texture2D _mHealthBar;

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
            _mBatch = new SpriteBatch(GraphicsDevice);
            _ninja = new Ninja.Ninja(GraphicsDevice);
            _ninja.Animations.Fire(NinjaAnimation.Idle2);
            _mHealthBar = Content.Load<Texture2D>("HealthBar");
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

            KeyboardState mKeys = Keyboard.GetState();

            if (mKeys.IsKeyDown(Keys.Up))
            {
                _ninja.CurrentHealth += 1;
            }

            //If the Down Arrowis pressed, decrease the Health bar
            if (mKeys.IsKeyDown(Keys.Down))
            {
                _ninja.CurrentHealth -= 1;
            }

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


            _mBatch.Begin();

            //Draw the negative space for the health bar

            _mBatch.Draw(_mHealthBar, new Rectangle(Window.ClientBounds.Width / 2 - _mHealthBar.Width / 2,

                 30, _mHealthBar.Width, 44), new Rectangle(0, 45, _mHealthBar.Width, 44), Color.Gray);

            //Draw the current health level based on the current Health
            _mBatch.Draw(_mHealthBar, new Rectangle(Window.ClientBounds.Width / 2 - _mHealthBar.Width / 2,
                 30, (int)(_mHealthBar.Width * ((double)_ninja.CurrentHealth / 100)), 44),
                 new Rectangle(0, 45, _mHealthBar.Width, 44), Color.Red);

            //Draw the box around the health bar

            _mBatch.Draw(_mHealthBar, new Rectangle(Window.ClientBounds.Width / 2 - _mHealthBar.Width / 2,

                30, _mHealthBar.Width, 44), new Rectangle(0, 0, _mHealthBar.Width, 44), Color.White);

            _mBatch.End();


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