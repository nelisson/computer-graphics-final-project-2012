namespace FinalProjectCG
{
    using System;
    using System.Collections.Generic;
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
        private readonly List<Dwarf.Dwarf> _dwarves = new List<Dwarf.Dwarf>();
        private SpriteBatch _mBatch;
        private Texture2D _mHealthBar;
        private Ninja.Ninja _ninja;

        #region Nine
        // An A* seach algorithm.
        private readonly GraphSearch _graphSearch = new GraphSearch();

        // A list of nodes containing the result path.
        private readonly List<int> _path = new List<int>();
        private Input _input;
        private PathGrid _pathGraph;
        private PrimitiveBatch _primitiveBatch;

        // Start node of the path.
        private int? _start;
        #endregion

        public MainGame()
        {
            new GraphicsDeviceManager(this);
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
            _dwarves.Add(new Dwarf.Dwarf(GraphicsDevice));
            _ninja.Animations.Fire(NinjaAnimation.Idle2);
            _mHealthBar = Content.Load<Texture2D>("HealthBar");

            Components.Add(new FrameRate(GraphicsDevice, Content.Load<SpriteFont>("Calibri")));
            Components.Add(new InputComponent(Window.Handle));

            _primitiveBatch = new PrimitiveBatch(GraphicsDevice);

            _ninja.IndexChanged += OnIndexChanged;

            foreach (var dwarf in _dwarves)
            {
                dwarf.IndexChanged += OnIndexChanged;
            }

            // Handle input events
            _input = new Input();
            _input.MouseDown += OnMouseDown;


            // Create a path graph
            _pathGraph = new PathGrid(0, 0, 9, 9, 3, 3);


            // Create some random obstacles
            var random = new Random();

            for (int i = 0; i < 4; i++)
            {
                _pathGraph.Mark(random.Next(_pathGraph.SegmentCountX),
                               random.Next(_pathGraph.SegmentCountY));
            }

        }

        private void OnIndexChanged()
        {
            _path.Clear();
            _graphSearch.Search(_pathGraph, _dwarves[0].Index, _ninja.Index, _path);
        }

        /// <summary>
        ///   Handle mouse down event.
        /// </summary>
        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (!_start.HasValue)
                {
                    // Get start path graph node.
                    _start = GetPickedNode(e.X, e.Y);
                    _path.Clear();
                }
                else
                {
                    int? end = GetPickedNode(e.X, e.Y);

                    if (end.HasValue)
                    {
                        // Search from start to end.
                        // NOTE: the path return from A* search is from end to start.
                        _path.Clear();
                        _graphSearch.Search(_pathGraph, _start.Value, end.Value, _path);
                        _start = null;
                    }
                }
            }
        }

        /// <summary>
        ///   Helper method to get picked path graph node from screen coordinates.
        /// </summary>
        private int? GetPickedNode(int x, int y)
        {
            // Gets the pick ray from current mouse cursor
            Ray ray = GraphicsDevice.Viewport.CreatePickRay(x, y, _ninja.Effect.View, _ninja.Effect.Projection);
            // Test ray against ground plane
            float? distance = ray.Intersects(new Plane(Vector3.UnitZ, 0));

            if (distance.HasValue)
            {
                Vector3 target = ray.Position + ray.Direction * distance.Value;

                // Make sure we don't pick an obstacle.
                if (!_pathGraph.IsMarked(target.X, target.Y))
                    return _pathGraph.PositionToIndex(target.X, target.Y);
            }

            return null;
        }

        /// <summary>
        ///   This is called when the game should draw itself.
        /// </summary>
        void DrawGraph()
        {
            _primitiveBatch.Begin(_ninja.Effect.View, _ninja.Effect.Projection);
            {
                // Draw grid
                _primitiveBatch.DrawGrid(_pathGraph, null, Color.Gray);

                //Draw node where ninja is placed
                {
                    var p = _pathGraph.IndexToPosition(_ninja.Index);

                    var center = new Vector3(p,0);

                    _primitiveBatch.DrawSolidBox(new BoundingBox(center - new Vector3(1.5f, 1.5f, 0), center + new Vector3(1.5f, 1.5f, 0)), null, Color.IndianRed);
                }

                //Draw node where dwarf  is placed
                {
                    foreach (var dwarf in _dwarves)
                    {
                        var p = _pathGraph.IndexToPosition(dwarf.Index);

                        var center = new Vector3(p, 0);

                        _primitiveBatch.DrawSolidBox(new BoundingBox(center - new Vector3(1.5f, 1.5f, 0), center + new Vector3(1.5f, 1.5f, 0)), null, Color.Green);
                    }
                }

                // Draw obstacles
                for (int x = 0; x < _pathGraph.SegmentCountX; x++)
                {
                    for (int y = 0; y < _pathGraph.SegmentCountY; y++)
                    {
                        if (_pathGraph.IsMarked(x, y))
                        {
                            var center = new Vector3(_pathGraph.SegmentToPosition(x, y), 0);

                            _primitiveBatch.DrawSolidBox(center + new Vector3(0,0,1), Vector3.One * 2, null, Color.Gold);
                        }
                    }
                }

                // Draw start node
                if (_start.HasValue)
                {
                    _primitiveBatch.DrawSolidSphere(new Vector3(_pathGraph.IndexToPosition(_start.Value), 0), 0.5f, 4, null,
                                                   Color.Honeydew);
                }

                // Draw path
                for (int i = 0; i < _path.Count - 1; i++)
                {
                    var point1 =
                        new Vector3(
                            _pathGraph.SegmentToPosition(_path[i] % _pathGraph.SegmentCountX, _path[i] / _pathGraph.SegmentCountX),
                            0);
                    var point2 =
                        new Vector3(
                            _pathGraph.SegmentToPosition(_path[i + 1] % _pathGraph.SegmentCountX,
                                                        _path[i + 1] / _pathGraph.SegmentCountX), 0);

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

            //If the Down Arrowis pressed, decrease the Health bar
            if (mKeys.IsKeyDown(Keys.PageDown))
            {
                _ninja.CurrentHealth -= 1;
            }
            _ninja.Update(GamePad.GetState(PlayerIndex.One));

            foreach (var dwarf in _dwarves)
            {
                dwarf.Update(GamePad.GetState(PlayerIndex.One));

                var c = dwarf.Position;

                dwarf.Index = _pathGraph.PositionToIndex(c.X, c.Y);
            }

            var center = _ninja.Position;

            _ninja.Index = _pathGraph.PositionToIndex(center.X, center.Y);

            if (mKeys.IsKeyDown(Keys.Y))
            {
                
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
            _ninja.Effect.World = Matrix.CreateScale(0.2f)*Matrix.CreateRotationX(MathHelper.PiOver2)*Matrix.CreateRotationZ(_ninja.Rotation)*
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
            foreach (var dwarf in _dwarves)
            {
                dwarf.Effect.World = Matrix.CreateScale(0.025f) * Matrix.CreateRotationX(MathHelper.PiOver2) * Matrix.CreateRotationZ(dwarf.Rotation)
                    *Matrix.CreateTranslation(dwarf.Position);

                dwarf.Effect.View = _ninja.Effect.View;

                dwarf.Effect.Projection = _ninja.Effect.Projection;

                dwarf.Render(gameTime);
            }
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