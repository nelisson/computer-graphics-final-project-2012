﻿namespace FinalProjectCG
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using Nine;
    using Nine.Components;
    using Nine.Graphics;
    using Nine.Navigation;
    using Ninja;

    /// <summary>
    ///   This is the main type for your game
    /// </summary>
    public class MainGame : Game
    {
        private Dwarf.Dwarf _dwarf;
        private SpriteBatch _mBatch;
        private Texture2D _mHealthBar;
        private Ninja.Ninja _ninja;

        #region Nine

        // An A* seach algorithm.
        private readonly GraphSearch _graphSearch = new GraphSearch();

        private PathGrid _pathGraph;
        private PrimitiveBatch _primitiveBatch;

        // Start node of the path.

        #endregion

        public MainGame()
        {
            new GraphicsDeviceManager(this);
            
            //FullScreen
                //{
                //    PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width,
                //    PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height,
                //    IsFullScreen = true
                //};

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        /// <summary>
        ///   LoadContent will be called once per game and is the place to load all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            _mBatch = new SpriteBatch(GraphicsDevice);
            _ninja = new Ninja.Ninja(GraphicsDevice);
            _dwarf = new Dwarf.Dwarf(GraphicsDevice);
            _ninja.Animations.Fire(NinjaAnimation.Idle2);
            _mHealthBar = Content.Load<Texture2D>("HealthBar");

            Components.Add(new FrameRate(GraphicsDevice, Content.Load<SpriteFont>("Calibri")));
            Components.Add(new InputComponent(Window.Handle));

            _primitiveBatch = new PrimitiveBatch(GraphicsDevice);

            _ninja.IndexChanged += OnIndexChanged;

            _dwarf.IndexChanged += OnIndexChanged;

            const int countX = 5;
            const int countY = 5;

            // Create a path graph
            const int widthHeight = (countX*countY)/3;
            const float gridPos = -(widthHeight/2 + 0.5f);
            _pathGraph = new PathGrid(gridPos, gridPos, widthHeight, widthHeight, countX, countY);
            _dwarf.PathGraph = _pathGraph;

            // Create some random obstacles
            //var random = new Random();

            //for (int i = 0; i < 4; i++)
            //{
            //    _pathGraph.Mark(random.Next(_pathGraph.SegmentCountX),
            //                    random.Next(_pathGraph.SegmentCountY));
            //}
        }

        private void OnIndexChanged()
        {
            _dwarf.Path.Clear();
            if(_ninja.Index == 0)
            {
                _graphSearch.Search(_pathGraph, _ninja.Index, _dwarf.Index, _dwarf.Path);
                _dwarf.Path.Reverse();
            }
            else
                _graphSearch.Search(_pathGraph, _dwarf.Index, _ninja.Index, _dwarf.Path);
        }

        /// <summary>
        ///   This is called when the game should draw itself.
        /// </summary>
        private void DrawGraph()
        {
            _primitiveBatch.Begin(_ninja.Effect.View, _ninja.Effect.Projection);
            {
                // Draw grid
                _primitiveBatch.DrawGrid(_pathGraph, null, Color.Gray);

                var xSize = _pathGraph.Size.X/_pathGraph.SegmentCountX/2;
                var ySize = _pathGraph.Size.Y/_pathGraph.SegmentCountY/2;

                //Draw node where ninja is placed
                {
                    var p = _pathGraph.IndexToPosition(_ninja.Index);

                    var center = new Vector3(p, 0);

                    _primitiveBatch.DrawSolidBox(
                        new BoundingBox(center - new Vector3(xSize, ySize, 0), center + new Vector3(xSize, ySize, 0)),
                        null, Color.Green);
                }

                //Draw node where dwarf  is placed
                {
                    
                    var p = _pathGraph.IndexToPosition(_dwarf.Index);

                    var center = new Vector3(p, 0);

                    _primitiveBatch.DrawSolidBox(
                        new BoundingBox(center - new Vector3(xSize, ySize, 0), center + new Vector3(xSize, ySize, 0)),
                        null, Color.IndianRed);
                }

                // Draw obstacles
                for (int x = 0; x < _pathGraph.SegmentCountX; x++)
                {
                    for (int y = 0; y < _pathGraph.SegmentCountY; y++)
                    {
                        if (_pathGraph.IsMarked(x, y))
                        {
                            var center = new Vector3(_pathGraph.SegmentToPosition(x, y),0.5f);

                            _primitiveBatch.DrawSolidBox(center, Vector3.One*xSize, null, Color.Gold);
                        }
                    }
                }

                // Draw path
                for (int i = 0; i < _dwarf.Path.Count - 1; i++)
                {
                    var point1 =
                        new Vector3(
                            _pathGraph.SegmentToPosition(_dwarf.Path[i] % _pathGraph.SegmentCountX,
                                                         _dwarf.Path[i] / _pathGraph.SegmentCountX),
                            0);
                    var point2 =
                        new Vector3(
                            _pathGraph.SegmentToPosition(_dwarf.Path[i + 1] % _pathGraph.SegmentCountX,
                                                         _dwarf.Path[i + 1] / _pathGraph.SegmentCountX), 0);

                    _primitiveBatch.DrawLine(point1, point2, null, Color.White);
                }
            }
            _primitiveBatch.End();
        }

        /// <summary>
        ///   Allows the game to run logic such as updating the world, checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime"> Provides a snapshot of timing values. </param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                Exit();

            KeyboardState mKeys = Keyboard.GetState();

            if (mKeys.IsKeyDown(Keys.PageUp))
            {
                _ninja.CurrentHealth += 1;
            }

            _ninja.Update(GamePad.GetState(PlayerIndex.One));
            var center = _ninja.Position;
            _ninja.Index = _pathGraph.PositionToIndex(center.X, center.Y);
            _ninja.Update(GamePad.GetState(PlayerIndex.One));

            _dwarf.Update(GamePad.GetState(PlayerIndex.One));
            var c = _dwarf.Position;
            _dwarf.Index = _pathGraph.PositionToIndex(c.X, c.Y);
            _dwarf.Update(GamePad.GetState(PlayerIndex.One));

            //If the Down Arrowis pressed, decrease the Health bar
            if (_dwarf.IsAttacking && !_ninja.Blocked)
            {
                _ninja.CurrentHealth -= 1;
            }

            base.Update(gameTime);
        }

        /// <summary>
        ///   This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime"> Provides a snapshot of timing values. </param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            RenderNinja(gameTime);
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            DrawGraph();
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            RenderDwarf(gameTime);
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            RenderHealthBar();

            base.Draw(gameTime);
        }

        private void RenderNinja(GameTime gameTime)
        {
            _ninja.Effect.World = Matrix.CreateScale(0.2f)*Matrix.CreateRotationX(MathHelper.PiOver2)*
                                  Matrix.CreateRotationZ(_ninja.Rotation)*
                                  Matrix.CreateTranslation(_ninja.Position);

            _ninja.Effect.View = Matrix.CreateLookAt(_ninja.Position + new Vector3(0f, -5, 4),
                                                     _ninja.Position,
                                                     new Vector3(0, 1, 0));

            _ninja.Effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45),
                                                                           (float) GraphicsDevice.Viewport.Width/
                                                                           GraphicsDevice.Viewport.Height, 1f, 10000);


            _ninja.Render(gameTime);
        }

        private void RenderDwarf(GameTime gameTime)
        {
            _dwarf.Effect.World = Matrix.CreateScale(0.025f)*Matrix.CreateRotationX(MathHelper.PiOver2)*
                                  Matrix.CreateRotationZ(_dwarf.Rotation)
                                  *Matrix.CreateTranslation(_dwarf.Position);

            _dwarf.Effect.View = _ninja.Effect.View;

            _dwarf.Effect.Projection = _ninja.Effect.Projection;

            _dwarf.Render(gameTime);
        }

        private void RenderHealthBar()
        {
            _mBatch.Begin();

            //Draw the negative space for the health bar

            _mBatch.Draw(_mHealthBar, new Rectangle(Window.ClientBounds.Width/2 - _mHealthBar.Width/2,
                                                    30, _mHealthBar.Width/2, 44/2),
                         new Rectangle(0, 45, _mHealthBar.Width, 44), Color.Gray);

            //Draw the current health level based on the current Health
            _mBatch.Draw(_mHealthBar, new Rectangle(Window.ClientBounds.Width/2 - _mHealthBar.Width/2,
                                                    30, (int) (_mHealthBar.Width*((double) _ninja.CurrentHealth/100))/2,
                                                    44/2),
                         new Rectangle(0, 45, _mHealthBar.Width, 44), Color.Red);

            //Draw the box around the health bar

            _mBatch.Draw(_mHealthBar, new Rectangle(Window.ClientBounds.Width/2 - _mHealthBar.Width/2,
                                                    30, _mHealthBar.Width/2, 44/2),
                         new Rectangle(0, 0, _mHealthBar.Width, 44), Color.White);

            _mBatch.End();
        }
    }
}