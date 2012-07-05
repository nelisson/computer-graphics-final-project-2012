namespace MazeGameLibrary
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Microsoft.Xna.Framework;

    /// <summary>
    ///   Creates, solves and draws mazes
    /// </summary>
    public class Maze
    {
        private const int sleepPeriod = 1000;

        /// <summary>
        ///   Indicates the maze begin
        /// </summary>
        public Cell begin;

        /// <summary>
        ///   Indicates the maze end
        /// </summary>
        public Cell end;

        /// <summary>
        ///   Used to draw the found path
        /// </summary>
        private readonly List<Cell> foundPath = new List<Cell>();

        /// <summary>
        ///   /// Indicates the currnet height the user selects
        /// </summary>
        private int height;

        /// <summary>
        ///   The array that carries all the Cell instances in the maze
        /// </summary>
        public readonly Cell[,] maze;

        /// <summary>
        ///   Used to generate maze
        /// </summary>
        private readonly Random random = new Random();

        /// <summary>
        ///   Indicates the currnet width the user selects
        /// </summary>
        private int width;

        /// <summary>
        ///   Initializes a maze with a maximum size
        /// </summary>
        /// <param name="totalWidth"> The maximum width of the maze </param>
        /// <param name="totalHeight"> The maximum height of the maze </param>
        public Maze(int totalWidth, int totalHeight)
        {
            maze = new Cell[totalHeight,totalWidth];
        }

        /// <summary>
        ///   Indicates the current sleeping time (used to slow operation)
        /// </summary>
        public int Sleep { get; set; }

        /// <summary>
        ///   Indicates the width of one rectangle on the maze
        /// </summary>
        public int unitX
        {
            get { return maze.GetLength(1)/width; }
        }

        /// <summary>
        ///   Indicates the height of one rectangle on the maze
        /// </summary>
        public int unitY
        {
            get { return maze.GetLength(0)/height; }
        }

        private static bool[,] _grid;

        public bool[,] ToGrid()
        {
            if (_grid == null)
            {
                _grid = new bool[width * 3, height * 3];

                foreach (var cell in maze)
                {
                    var position = cell.GridPosition;
                    var bGrid = cell.ToGrid();


                    _grid[position.X - 1, position.Y - 1] = bGrid[0, 0];
                    _grid[position.X - 1, position.Y + 0] = bGrid[0, 1];
                    _grid[position.X - 1, position.Y + 1] = bGrid[0, 2];
                    _grid[position.X + 0, position.Y - 1] = bGrid[1, 0];
                    _grid[position.X + 0, position.Y + 0] = bGrid[1, 1];
                    _grid[position.X + 0, position.Y + 1] = bGrid[1, 2];
                    _grid[position.X + 1, position.Y - 1] = bGrid[2, 0];
                    _grid[position.X + 1, position.Y + 0] = bGrid[2, 1];
                    _grid[position.X + 1, position.Y + 1] = bGrid[2, 2];
                }
            }
            return _grid;
        }

        /// <summary>
        ///   Generates a maze with the specific size
        /// </summary>
        /// <param name="_width"> Number of squares in width </param>
        /// <param name="_height"> Number of squares in height </param>
        /// <param name="method"> indicates the method used to generate the maze </param>
        public void Generate(int _width, int _height, GenerateMethod method)
        {
            initalize(maze, _width, _height);

            switch (method)
            {
                case GenerateMethod.DepthFirstSearch:
                    depthFirstSearchMazeGeneration(maze, width, height);
                    break;
                case GenerateMethod.BreadthFirstSearch:
                    breadthFirstSearchMazeGeneration(maze, width, height);
                    break;
            }
        }

        /// <summary>
        ///   Resets a maze array
        /// </summary>
        /// <param name="arr"> The maze array </param>
        /// <param name="_width"> Number of squares in width </param>
        /// <param name="_height"> Number of squares in height </param>
        private void initalize(Cell[,] arr, int _width, int _height)
        {
            width = _width;
            height = _height;

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    arr[i, j] = new Cell(new Point(j, i));
                }
            }
        }

        /// <summary>
        ///   Generate a maze with the Depth-First Search approach
        /// </summary>
        /// <param name="arr"> the array of cells </param>
        /// <param name="_width"> A width for the maze </param>
        /// <param name="_height"> A height for the maze </param>
        private void depthFirstSearchMazeGeneration(Cell[,] arr, int _width, int _height)
        {
            var stack = new Stack<Cell>();
            var randomInner = new Random();

            Cell location = arr[random.Next(_height), random.Next(_width)];
            stack.Push(location);

            while (stack.Count > 0)
            {
                List<Point> neighbours = getNeighbours(arr, location, _width, _height);
                if (neighbours.Count > 0)
                {
                    Point temp = neighbours[randomInner.Next(neighbours.Count)];

                    knockWall(arr, ref location, ref arr[temp.Y, temp.X]);

                    stack.Push(location);
                    location = arr[temp.Y, temp.X];
                }
                else
                {
                    location = stack.Pop();
                }

                Thread.SpinWait(Sleep*sleepPeriod);
            }

            makeMazeBeginEnd(maze);
        }

        /// <summary>
        ///   Generate a maze with the Breadth-First Search approach
        /// </summary>
        /// <param name="arr"> the array of cells </param>
        /// <param name="_width"> A width for the maze </param>
        /// <param name="_height"> A height for the maze </param>
        private void breadthFirstSearchMazeGeneration(Cell[,] arr, int _width, int _height)
        {
            var queue = new Queue<Cell>();
            var randomInner = new Random();

            Cell location = arr[random.Next(_height), random.Next(_width)];
            queue.Enqueue(location);

            while (queue.Count > 0)
            {
                List<Point> neighbours = getNeighbours(arr, location, _width, _height);
                if (neighbours.Count > 0)
                {
                    Point temp = neighbours[randomInner.Next(neighbours.Count)];

                    knockWall(arr, ref location, ref arr[temp.Y, temp.X]);

                    queue.Enqueue(location);
                    location = arr[temp.Y, temp.X];
                }
                else
                {
                    location = queue.Dequeue();
                }

                Thread.SpinWait(Sleep*sleepPeriod);
            }

            makeMazeBeginEnd(maze);
        }

        /// <summary>
        ///   Used to create a begin and end for a maze
        /// </summary>
        /// <param name="arr"> The array of the maze </param>
        private void makeMazeBeginEnd(Cell[,] arr)
        {
            var temp = new Point {Y = random.Next(height)};

            arr[temp.Y, temp.X].LeftWall = false;
            begin = arr[temp.Y, temp.X];

            temp.Y = random.Next(height);
            temp.X = width - 1;
            arr[temp.Y, temp.X].RightWall = false;
            end = arr[temp.Y, temp.X];
        }

        /// <summary>
        ///   Knocks wall between two neighbor cellls
        /// </summary>
        /// <param name="_maze"> The maze array </param>
        /// <param name="current"> the current cell </param>
        /// <param name="next"> the next neighbor cell </param>
        private void knockWall(Cell[,] _maze, ref Cell current, ref Cell next)
        {
            // The next is down
            if (current.Position.X == next.Position.X && current.Position.Y > next.Position.Y)
            {
                _maze[current.Position.Y, current.Position.X].UpWall = false;
                _maze[next.Position.Y, next.Position.X].DownWall = false;
            }
                // the next is up
            else if (current.Position.X == next.Position.X)
            {
                _maze[current.Position.Y, current.Position.X].DownWall = false;
                _maze[next.Position.Y, next.Position.X].UpWall = false;
            }
                // the next is right
            else if (current.Position.X > next.Position.X)
            {
                _maze[current.Position.Y, current.Position.X].LeftWall = false;
                _maze[next.Position.Y, next.Position.X].RightWall = false;
            }
                // the next is left
            else
            {
                _maze[current.Position.Y, current.Position.X].RightWall = false;
                _maze[next.Position.Y, next.Position.X].LeftWall = false;
            }
        }

        /// <summary>
        ///   Determines whether a particular cell has all its walls intact
        /// </summary>
        /// <param name="arr"> the maze array </param>
        /// <param name="cell"> The cell to check </param>
        /// <returns> </returns>
        private bool allWallsIntact(Cell[,] arr, Cell cell)
        {
            for (int i = 0; i < 4; i++)
            {
                if (!arr[cell.Position.Y, cell.Position.X][i])
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        ///   Gets all neighbor cells to a specific cell, where those neighbors exist and not visited already
        /// </summary>
        /// <param name="arr"> The maze array </param>
        /// <param name="cell"> The current cell to get neighbors </param>
        /// <param name="_width"> The width of the maze </param>
        /// <param name="_height"> The height of the maze </param>
        /// <returns> </returns>
        private List<Point> getNeighbours(Cell[,] arr, Cell cell, int _width, int _height)
        {
            Point temp = cell.Position;
            var availablePlaces = new List<Point>();

            // Left
            temp.X = cell.Position.X - 1;
            if (temp.X >= 0 && allWallsIntact(arr, arr[temp.Y, temp.X]))
            {
                availablePlaces.Add(temp);
            }
            // Right
            temp.X = cell.Position.X + 1;
            if (temp.X < _width && allWallsIntact(arr, arr[temp.Y, temp.X]))
            {
                availablePlaces.Add(temp);
            }

            // Up
            temp.X = cell.Position.X;
            temp.Y = cell.Position.Y - 1;
            if (temp.Y >= 0 && allWallsIntact(arr, arr[temp.Y, temp.X]))
            {
                availablePlaces.Add(temp);
            }
            // Down
            temp.Y = cell.Position.Y + 1;
            if (temp.Y < _height && allWallsIntact(arr, arr[temp.Y, temp.X]))
            {
                availablePlaces.Add(temp);
            }
            return availablePlaces;
        }

        /// <summary>
        ///   Used to reset all cells
        /// </summary>
        /// <param name="arr"> The maze array to reset elements </param>
        private void unvisitAll(Cell[,] arr)
        {
            for (int i = 0; i < maze.GetLength(0); i++)
            {
                for (int j = 0; j < maze.GetLength(1); j++)
                {
                    arr[i, j].Visited = false;
                    arr[i, j].Path = Cell.Paths.None;
                }
            }
        }

        /// <summary>
        ///   Solves the current maze using a specific method
        /// </summary>
        /// <param name="method"> The used method to solve with </param>
        public unsafe void Solve(SolveMethod method)
        {
            // initialize
            foundPath.Clear();
            unvisitAll(maze);

            // selecting the method
            switch (method)
            {
                case SolveMethod.DepthFirstSearch:
                    if (height*width < 40*80)
                        fixed (Cell* ptr = &begin)
                            depthFirstSearchSolve(ptr, ref end);
                    else
                        iterativeDepthFirstSearchSolve(begin, end);
                    break;
                case SolveMethod.BreadthFirstSearch:
                    breadthFirstSearchSolve(begin, end);
                    break;
                case SolveMethod.IterativeRightHandRule:
                    iterativeRightHandRuleSolve(begin, Directions.Right);
                    break;
            }
        }

        /// <summary>
        ///   Solves a maze with recursive backtracking DFS - DON'T USE! IT IS FOR DEMONSTRATING PURPOSES ONLY!
        /// </summary>
        /// <param name="st"> The start of the maze cell </param>
        /// <param name="_end"> The end of the maze cell </param>
        /// <returns> returrns true if the path is found </returns>
        private unsafe bool depthFirstSearchSolve(Cell* st, ref Cell _end)
        {
            // base condition
            if (st->Position == _end.Position)
            {
                // make it visited in order to be drawed with green
                maze[st->Position.Y, st->Position.X].Visited = true;
                // add end point to the foundPath
                foundPath.Add(*st);
                return true;
            }


            // has been visited alread, return
            if (maze[st->Position.Y, st->Position.X].Visited)
                return false;

            // used to slow the process
            Thread.SpinWait(Sleep*sleepPeriod);

            // mark as visited
            maze[st->Position.Y, st->Position.X].Visited = true;

            // Check every neighbor cell
            // If it exists (not outside the maze bounds)
            // and if there is no wall between start and it
            // recursive call this method with it
            // if it returns true, add the current start to foundPath and return true too
            // else complete

            // Left
            if (st->Position.X - 1 >= 0 && !maze[st->Position.Y, st->Position.X - 1].RightWall)
            {
                maze[st->Position.Y, st->Position.X].Path = Cell.Paths.Left;
                fixed (Cell* ptr = &maze[st->Position.Y, st->Position.X - 1])
                {
                    if (depthFirstSearchSolve(ptr, ref _end))
                    {
                        foundPath.Add(*st);
                        return true;
                    }
                }
            }
            // used to slow the process
            Thread.SpinWait(Sleep*sleepPeriod);
            // Right
            if (st->Position.X + 1 < width && !maze[st->Position.Y, st->Position.X + 1].LeftWall)
            {
                maze[st->Position.Y, st->Position.X].Path = Cell.Paths.Right;
                fixed (Cell* ptr = &maze[st->Position.Y, st->Position.X + 1])
                {
                    if (depthFirstSearchSolve(ptr, ref _end))
                    {
                        foundPath.Add(*st);
                        return true;
                    }
                }
            }
            // used to slow the process
            Thread.SpinWait(Sleep*sleepPeriod);

            // Up
            if (st->Position.Y - 1 >= 0 && !maze[st->Position.Y - 1, st->Position.X].DownWall)
            {
                maze[st->Position.Y, st->Position.X].Path = Cell.Paths.Up;
                fixed (Cell* ptr = &maze[st->Position.Y - 1, st->Position.X])
                {
                    if (depthFirstSearchSolve(ptr, ref _end))
                    {
                        foundPath.Add(*st);
                        return true;
                    }
                }
            }

            // used to slow the process
            Thread.SpinWait(Sleep*sleepPeriod);

            // Down
            if (st->Position.Y + 1 < height && !maze[st->Position.Y + 1, st->Position.X].UpWall)
            {
                maze[st->Position.Y, st->Position.X].Path = Cell.Paths.Down;
                fixed (Cell* ptr = &maze[st->Position.Y + 1, st->Position.X])
                {
                    if (depthFirstSearchSolve(ptr, ref _end))
                    {
                        foundPath.Add(*st);
                        return true;
                    }
                }
            }
            maze[st->Position.Y, st->Position.X].Path = Cell.Paths.None;
            return false;
        }

        /// <summary>
        ///   Solves a maze with iterative backtracking DFS
        /// </summary>
        /// <param name="start"> The start of the maze cell </param>
        /// <param name="_end"> The end of the maze cell </param>
        /// <returns> returrns true if the path is found </returns>
        private unsafe bool iterativeDepthFirstSearchSolve(Cell start, Cell _end)
        {
            // unsafe indicates that this method uses pointers
            var stack = new Stack<Cell>();

            stack.Push(start);

            while (stack.Count > 0)
            {
                Cell temp = stack.Pop();

                // base condition
                if (temp.Position == _end.Position)
                {
                    // add end point to foundPath
                    foundPath.Add(temp);
                    // dereference all pointers chain until you reach the begin
                    while (temp.Previous != null)
                    {
                        foundPath.Add(temp);
                        temp = *temp.Previous;
                    }
                    // add begin point to foundPath
                    foundPath.Add(temp);
                    // to view green square on it
                    maze[temp.Position.Y, temp.Position.X].Visited = true;
                    return true;
                }

                // mark as visited to prevent infinite loops
                maze[temp.Position.Y, temp.Position.X].Visited = true;

                // used to slow operation
                Thread.SpinWait(Sleep*sleepPeriod);


                // Check every neighbor cell
                // If it exists (not outside the maze bounds)
                // and if there is no wall between start and it
                // set the next.Previous to the current cell
                // push next into stack
                // else complete


                // Left
                if (temp.Position.X - 1 >= 0
                    && !maze[temp.Position.Y, temp.Position.X - 1].RightWall
                    && !maze[temp.Position.Y, temp.Position.X - 1].Visited)
                {
                    // fixed must be used to indicate that current memory-location won't be changed
                    fixed (Cell* cell = &maze[temp.Position.Y, temp.Position.X])
                        maze[temp.Position.Y, temp.Position.X - 1].Previous = cell;
                    stack.Push(maze[temp.Position.Y, temp.Position.X - 1]);
                }

                // Right
                if (temp.Position.X + 1 < width
                    && !maze[temp.Position.Y, temp.Position.X + 1].LeftWall
                    && !maze[temp.Position.Y, temp.Position.X + 1].Visited)
                {
                    fixed (Cell* cell = &maze[temp.Position.Y, temp.Position.X])
                        maze[temp.Position.Y, temp.Position.X + 1].Previous = cell;
                    stack.Push(maze[temp.Position.Y, temp.Position.X + 1]);
                }

                // Up
                if (temp.Position.Y - 1 >= 0
                    && !maze[temp.Position.Y - 1, temp.Position.X].DownWall
                    && !maze[temp.Position.Y - 1, temp.Position.X].Visited)
                {
                    fixed (Cell* cell = &maze[temp.Position.Y, temp.Position.X])
                        maze[temp.Position.Y - 1, temp.Position.X].Previous = cell;
                    stack.Push(maze[temp.Position.Y - 1, temp.Position.X]);
                }

                // Down
                if (temp.Position.Y + 1 < height
                    && !maze[temp.Position.Y + 1, temp.Position.X].UpWall
                    && !maze[temp.Position.Y + 1, temp.Position.X].Visited)
                {
                    fixed (Cell* cell = &maze[temp.Position.Y, temp.Position.X])
                        maze[temp.Position.Y + 1, temp.Position.X].Previous = cell;
                    stack.Push(maze[temp.Position.Y + 1, temp.Position.X]);
                }
            }
            // no solution found
            return false;
        }

        /// <summary>
        ///   Solves a maze with iterative backtracking BFS
        /// </summary>
        /// <param name="start"> The start of the maze cell </param>
        /// <param name="_end"> The end of the maze cell </param>
        /// <returns> returrns true if the path is found </returns>
        private unsafe bool breadthFirstSearchSolve(Cell start, Cell _end)
        {
            // unsafe indicates that this method uses pointers
            var queue = new Queue<Cell>();

            queue.Enqueue(start);

            while (queue.Count > 0)
            {
                Cell temp = queue.Dequeue();

                // base condition
                if (temp.Position == _end.Position)
                {
                    // add end point to foundPath
                    foundPath.Add(temp);
                    // dereference all pointers chain until you reach the begin
                    while (temp.Previous != null)
                    {
                        foundPath.Add(temp);
                        temp = *temp.Previous;
                    }
                    // add begin point to foundPath
                    foundPath.Add(temp);
                    // to view green square on it
                    maze[temp.Position.Y, temp.Position.X].Visited = true;
                    return true;
                }

                // mark as visited to prevent infinite loops
                maze[temp.Position.Y, temp.Position.X].Visited = true;

                // used to slow operation
                Thread.SpinWait(Sleep*sleepPeriod);


                // Check every neighbor cell
                // If it exists (not outside the maze bounds)
                // and if there is no wall between start and it
                // set the next.Previous to the current cell
                // add next into queue
                // else complete


                // Left
                if (temp.Position.X - 1 >= 0
                    && !maze[temp.Position.Y, temp.Position.X - 1].RightWall
                    && !maze[temp.Position.Y, temp.Position.X - 1].Visited)
                {
                    // fixed must be used to indicate that current memory-location won't be changed
                    fixed (Cell* cell = &maze[temp.Position.Y, temp.Position.X])
                        maze[temp.Position.Y, temp.Position.X - 1].Previous = cell;
                    queue.Enqueue(maze[temp.Position.Y, temp.Position.X - 1]);
                }

                // Right
                if (temp.Position.X + 1 < width
                    && !maze[temp.Position.Y, temp.Position.X + 1].LeftWall
                    && !maze[temp.Position.Y, temp.Position.X + 1].Visited)
                {
                    fixed (Cell* cell = &maze[temp.Position.Y, temp.Position.X])
                        maze[temp.Position.Y, temp.Position.X + 1].Previous = cell;
                    queue.Enqueue(maze[temp.Position.Y, temp.Position.X + 1]);
                }

                // Up
                if (temp.Position.Y - 1 >= 0
                    && !maze[temp.Position.Y - 1, temp.Position.X].DownWall
                    && !maze[temp.Position.Y - 1, temp.Position.X].Visited)
                {
                    fixed (Cell* cell = &maze[temp.Position.Y, temp.Position.X])
                        maze[temp.Position.Y - 1, temp.Position.X].Previous = cell;
                    queue.Enqueue(maze[temp.Position.Y - 1, temp.Position.X]);
                }

                // Down
                if (temp.Position.Y + 1 < height
                    && !maze[temp.Position.Y + 1, temp.Position.X].UpWall
                    && !maze[temp.Position.Y + 1, temp.Position.X].Visited)
                {
                    fixed (Cell* cell = &maze[temp.Position.Y, temp.Position.X])
                        maze[temp.Position.Y + 1, temp.Position.X].Previous = cell;
                    queue.Enqueue(maze[temp.Position.Y + 1, temp.Position.X]);
                }
            }
            // no solution found
            return false;
        }

        /// <summary>
        ///   Solves the maze with the right-hand role iteratively
        /// </summary>
        /// <param name="start"> The maze begin </param>
        /// <param name="dir"> the initial direction </param>
        private void iterativeRightHandRuleSolve(Cell start, Directions dir)
        {
            // look at your right (with respect to your direction)
            // No wall? go in it.
            // Wall? look at your front (with respect to your direction)
            // Wall too? look at your left (with respect to your direction)
            // Wall too? go back (in the reverse of your direction)

            // note that the right of the right is down, the left of down is right, ect

            // repeat while you didn't reach the end
            while (start.Position != end.Position)
            {
                // for graphics
                maze[start.Position.Y, start.Position.X].Visited = true;

                // to slow operation
                Thread.SpinWait(Sleep*sleepPeriod);

                bool flag;
                switch (dir)
                {
                    case Directions.Right:
                        // has up wall?
                        flag = start.Position.Y + 1 < height;
                        if (!flag || maze[start.Position.Y + 1, start.Position.X].UpWall)
                        {
                            // has left wall?
                            flag = start.Position.X + 1 < width;
                            if (!flag || maze[start.Position.Y, start.Position.X + 1].LeftWall)
                            {
                                // has down wall ?
                                flag = start.Position.Y - 1 >= 0;
                                if (!flag || maze[start.Position.Y - 1, start.Position.X].DownWall)
                                {
                                    start = maze[start.Position.Y, start.Position.X];
                                    dir = Directions.Left;
                                }
                                else
                                {
                                    start = maze[start.Position.Y - 1, start.Position.X];
                                    dir = Directions.Up;
                                }
                            }
                            else
                            {
                                start = maze[start.Position.Y, start.Position.X + 1];
                                dir = Directions.Right;
                            }
                        }
                        else
                        {
                            start = maze[start.Position.Y + 1, start.Position.X];
                            dir = Directions.Down;
                        }
                        break;
                    case Directions.Left:
                        flag = start.Position.Y - 1 >= 0;
                        if (!flag || maze[start.Position.Y - 1, start.Position.X].DownWall)
                        {
                            flag = start.Position.X - 1 >= 0;
                            if (!flag || maze[start.Position.Y, start.Position.X - 1].RightWall)
                            {
                                flag = start.Position.Y + 1 < height;
                                if (!flag || maze[start.Position.Y + 1, start.Position.X].UpWall)
                                {
                                    start = maze[start.Position.Y, start.Position.X];
                                    dir = Directions.Right;
                                }
                                else
                                {
                                    start = maze[start.Position.Y + 1, start.Position.X];
                                    dir = Directions.Down;
                                }
                            }
                            else
                            {
                                start = maze[start.Position.Y, start.Position.X - 1];
                                dir = Directions.Left;
                            }
                        }
                        else
                        {
                            start = maze[start.Position.Y - 1, start.Position.X];
                            dir = Directions.Up;
                        }
                        break;
                    case Directions.Up:
                        flag = start.Position.X + 1 < width;
                        if (!flag || maze[start.Position.Y, start.Position.X + 1].LeftWall)
                        {
                            flag = start.Position.Y - 1 >= 0;
                            if (!flag || maze[start.Position.Y - 1, start.Position.X].DownWall)
                            {
                                flag = start.Position.X - 1 >= 0;
                                if (!flag || maze[start.Position.Y, start.Position.X - 1].RightWall)
                                {
                                    start = maze[start.Position.Y, start.Position.X];
                                    dir = Directions.Down;
                                }
                                else
                                {
                                    start = maze[start.Position.Y, start.Position.X - 1];
                                    dir = Directions.Left;
                                }
                            }
                            else
                            {
                                start = maze[start.Position.Y - 1, start.Position.X];
                                dir = Directions.Up;
                            }
                        }
                        else
                        {
                            start = maze[start.Position.Y, start.Position.X + 1];
                            dir = Directions.Right;
                        }
                        break;
                    default:
                        flag = start.Position.X - 1 >= 0;
                        if (!flag || maze[start.Position.Y, start.Position.X - 1].RightWall)
                        {
                            flag = start.Position.Y + 1 < height;
                            if (!flag || maze[start.Position.Y + 1, start.Position.X].UpWall)
                            {
                                flag = start.Position.X + 1 < width;
                                if (!flag || maze[start.Position.Y, start.Position.X + 1].LeftWall)
                                {
                                    start = maze[start.Position.Y, start.Position.X];
                                    dir = Directions.Up;
                                }
                                else
                                {
                                    start = maze[start.Position.Y, start.Position.X + 1];
                                    dir = Directions.Right;
                                }
                            }
                            else
                            {
                                start = maze[start.Position.Y + 1, start.Position.X];
                                dir = Directions.Down;
                            }
                        }
                        else
                        {
                            start = maze[start.Position.Y, start.Position.X - 1];
                            dir = Directions.Left;
                        }
                        break;
                }
            }

            maze[start.Position.Y, start.Position.X].Visited = true;
        }

        #region Nested type: Directions

        /// <summary>
        ///   Used to distinguish different directions in the right-hand rule
        /// </summary>
        private enum Directions
        {
            Up,
            Down,
            Left,
            Right
        }

        public enum SolveMethod
        {
            DepthFirstSearch,
            BreadthFirstSearch,
            IterativeRightHandRule
        }

        public enum GenerateMethod
        {
            DepthFirstSearch,
            BreadthFirstSearch
        }

        #endregion
    }
}