namespace FinalProjectCG
{
    using MazeGameLibrary;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using Nine;
    using Nine.Components;
    using Nine.Graphics;
    using Nine.Navigation;
    using Ninja;
    using System;
    using System.Collections.Generic;

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
            new GraphicsDeviceManager(this)
            
            //FullScreen
                //{
                //    PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width,
                //    PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height,
                //    IsFullScreen = true
                //}
                ;

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        private Maze _maze;
        const int countX = 8;
        const int countY = 8;
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

            

            // Create a path graph
            const int widthHeight = countX*countY;
            _pathGraph = new PathGrid(0, 0, widthHeight, widthHeight, countX*3, countY*3);

            _maze = new Maze(countX,countY);
            _maze.Generate(countX,countY,Maze.GenerateMethod.DepthFirstSearch);
            
            var p = _maze.Entry;

            var g = p.X * countX * 3 + p.Y;

            var v = _pathGraph.IndexToPosition(g);

            _ninja.Position = new Vector3(v.X,v.Y,0);

            var grid = _maze.ToGrid();

            var validos = new List<Tuple<int, int>>();

            for (var i = 0; i < grid.GetLength(0); i++)
            {
                for (var j = 0; j < grid.GetLength(1); j++)
                {
                    if (grid[i, j])
                        _pathGraph.Mark(i, j);
                    else
                        validos.Add(Tuple.Create(i,j));
                }    
            }
            _dwarf.PathGraph = _pathGraph;
            _ninja.PathGraph = _pathGraph;

            var r = new Random();

            var cubo = r.Next(validos.Count);

            p = new Point(validos[cubo].Item1, validos[cubo].Item2);

            g = p.X * countX * 3 + p.Y;

            v = _pathGraph.IndexToPosition(g);

            _dwarf.Position = new Vector3(v.X, v.Y, 0);
            

            _dwarf.BottomLimit = _pathGraph.Position;
            _dwarf.TopLimit = _pathGraph.Position + _pathGraph.Size;

            _ninja.BottomLimit = _pathGraph.Position;
            _ninja.TopLimit = _pathGraph.Position + _pathGraph.Size;
        }

        private void OnIndexChanged()
        {
            if (_ninja.Index == (_maze.Exit.X * countX * 3 + _maze.Exit.Y))
                Exit();

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

                var xSize = _pathGraph.Size.X / _pathGraph.SegmentCountX / 2;
                var ySize = _pathGraph.Size.Y / _pathGraph.SegmentCountY / 2;
                //Exit
                {
                    var center = new Vector3(_maze.Exit.X, _maze.Exit.Y, 0);
                    var g = center.X * countX * 3 + center.Y;
                    var p = _pathGraph.IndexToPosition((int)g);
                    center = new Vector3(p, 0);

                    _primitiveBatch.DrawSolidBox(
                        new BoundingBox(center - new Vector3(xSize, ySize, 0), center + new Vector3(xSize, ySize, 0)),
                        null, Color.White);

                    var entry = new Vector3(_maze.Entry.X, _maze.Entry.Y, 0);
                    var index = (int)(entry.X * countX * 3 + entry.Y);
                    var pos = _pathGraph.IndexToPosition(index);
                    center = new Vector3(pos, 0);

                    _primitiveBatch.DrawSolidBox(
                        new BoundingBox(center - new Vector3(xSize, ySize, 0), center + new Vector3(xSize, ySize, 0)),
                        null, Color.Black);
                }
#if DEBUG

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
#endif
                // Draw obstacles
                for (int x = 0; x < _pathGraph.SegmentCountX; x++)
                {
                    for (int y = 0; y < _pathGraph.SegmentCountY; y++)
                    {
                        if (_pathGraph.IsMarked(x, y))
                        {
                            var center = new Vector3(_pathGraph.SegmentToPosition(x, y), 0);

                            _primitiveBatch.DrawSolidBox(
                                new BoundingBox(center - new Vector3(xSize, ySize, 0),
                                                center + new Vector3(xSize, ySize, xSize)),
                                null, Color.Gold);
                        }
                    }
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

            if (mKeys.IsKeyDown(Keys.PageUp) || GamePad.GetState(PlayerIndex.One).Buttons.LeftShoulder == ButtonState.Pressed)
            {
                _ninja.CurrentHealth = 100;
                _dwarf.CurrentHealth = 100;
            }

            
            _ninja.Update(GamePad.GetState(PlayerIndex.One));
            var center = _ninja.Position;
            _ninja.Index = _pathGraph.PositionToIndex(center.X, center.Y);
            _ninja.Update(GamePad.GetState(PlayerIndex.One));

            _dwarf.Update(GamePad.GetState(PlayerIndex.One), _ninja.Position, _ninja.IsAlive);


            if (_ninja.IsAttacking && _ninja.Index == _dwarf.Index)
            {
                if(_dwarfCount == 0)
                    _dwarf.CurrentHealth -= 1f*_ninja.AttackForce;

                _dwarfCount++;

                if (_dwarfCount > CountLimit)
                    _dwarfCount = 0;
            }

            //If the Down Arrowis pressed, decrease the Health bar
            if (_dwarf.IsAttacking && !_ninja.Blocked)
            {
                if(_ninjaCount == 0)
                    _ninja.CurrentHealth -= 1f;

                _ninjaCount++;

                if (_ninjaCount > CountLimit)
                    _ninjaCount = 0;
            }

            base.Update(gameTime);
        }

        private const int CountLimit = 10;

        private int _ninjaCount;
        private int _dwarfCount;

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
            RenderDwarf(gameTime);
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            DrawGraph();            
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
                                                    10, _mHealthBar.Width/2, 44/2),
                         new Rectangle(0, 45, _mHealthBar.Width, 44), Color.Gray);
            //Draw the current health level based on the current Health
            _mBatch.Draw(_mHealthBar, new Rectangle(Window.ClientBounds.Width/2 - _mHealthBar.Width/2,
                                                    10, (int) (_mHealthBar.Width*((double) _ninja.CurrentHealth/100))/2,
                                                    44/2),
                         new Rectangle(0, 45, _mHealthBar.Width, 44), Color.Red);
            //Draw the box around the health bar
            _mBatch.Draw(_mHealthBar, new Rectangle(Window.ClientBounds.Width/2 - _mHealthBar.Width/2,
                                                    10, _mHealthBar.Width/2, 44/2),
                         new Rectangle(0, 0, _mHealthBar.Width, 44), Color.White);



            _mBatch.Draw(_mHealthBar, new Rectangle(Window.ClientBounds.Width / 2 - _mHealthBar.Width / 2,
                                                    30, _mHealthBar.Width / 2, 44 / 2),
                         new Rectangle(0, 45, _mHealthBar.Width, 44), Color.Gray);
            //Draw the current health level based on the current Health
            _mBatch.Draw(_mHealthBar, new Rectangle(Window.ClientBounds.Width / 2 - _mHealthBar.Width / 2,
                                                    30, (int)(_mHealthBar.Width * ((double)_dwarf.CurrentHealth / 100)) / 2,
                                                    44 / 2),
                         new Rectangle(0, 45, _mHealthBar.Width, 44), Color.Red);
            //Draw the box around the health bar
            _mBatch.Draw(_mHealthBar, new Rectangle(Window.ClientBounds.Width / 2 - _mHealthBar.Width / 2,
                                                    30, _mHealthBar.Width / 2, 44 / 2),
                         new Rectangle(0, 0, _mHealthBar.Width, 44), Color.White);

            _mBatch.End();
        }
    }
}