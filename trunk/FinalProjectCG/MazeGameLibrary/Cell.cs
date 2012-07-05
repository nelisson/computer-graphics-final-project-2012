namespace MazeGameLibrary
{
    using System;
    using Microsoft.Xna.Framework;

    /// <summary>
    ///   Represents a maze cell
    /// </summary>
    public struct Cell
    {
        #region Paths enum

        public enum Paths
        {
            Up,
            Down,
            Right,
            Left,
            None
        }

        #endregion

        public bool[,] ToGrid()
        {
            var grid = new[,] { { true, false, true }, { false, false, false }, { true, false, true } };

            grid[0, 1] = UpWall;
            grid[1, 0] = LeftWall;
            grid[2, 1] = DownWall;
            grid[1, 2] = RightWall;

            return grid;
        }

        /// <summary>
        ///   Gets or sets a value whether the cell has an intact down wall
        /// </summary>
        public bool DownWall;

        /// <summary>
        ///   Gets or sets a value whether the cell has an intact left wall
        /// </summary>
        public bool LeftWall;

        public Paths Path;

        /// <summary>
        ///   Gets or sets a pointer to the previous Cell in the found path chain
        /// </summary>
        public unsafe Cell* Previous;

        /// <summary>
        ///   /// Gets or sets a value whether the cell has an intact right wall
        /// </summary>
        public bool RightWall;

        /// <summary>
        ///   Gets or sets a value whether the cell has an intact up wall
        /// </summary>
        public bool UpWall;

        /// <summary>
        ///   Gets or sets a value whether the cell has been visited already
        /// </summary>
        public bool Visited;


        private readonly Point position;

        /// <summary>
        ///   Initializes a new instance of maze cell with locations
        /// </summary>
        /// <param name="_position"> The location on the 2d array </param>
        public unsafe Cell(Point _position)
        {
            position = _position;

            // initially, all walls are intact
            LeftWall = true;
            RightWall = true;
            UpWall = true;
            DownWall = true;
            Path = Paths.None;

            // must be initialized, since it is a member of a struct
            Visited = false;
            Previous = null;
        }

        /// <summary>
        ///   Provides indexing to the boolean fields in the cell
        /// </summary>
        /// <param name="index"> 0 leftW, 1 rightW, 2 UpW, 3 downW, 4 visited </param>
        /// <returns> </returns>
        public bool this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return LeftWall;
                    case 1:
                        return RightWall;
                    case 2:
                        return UpWall;
                    case 3:
                        return DownWall;
                    case 4:
                        return Visited;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        LeftWall = value;
                        break;
                    case 1:
                        RightWall = value;
                        break;
                    case 2:
                        UpWall = value;
                        break;
                    case 3:
                        DownWall = value;
                        break;
                    case 4:
                        Visited = value;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        ///   The current location on the two-dimensional container
        /// </summary>
        public Point Position
        {
            get { return position; }
        }

        /// <summary>
        ///   The current location on the two-dimensional container
        /// </summary>
        public Point GridPosition
        {
            get { return new Point(position.Y * 3 + 1, position.X * 3 + 1); }
        }

        /// <summary>
        ///   Reset a cell so that all walls are intact and not visited
        /// </summary>
        public void Reset()
        {
            for (int i = 0; i < 4; i++)
            {
                this[i] = true;
            }
            Visited = false;
        }
    }
}