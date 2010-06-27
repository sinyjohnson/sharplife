#region Apache License 2.0
/*
   Copyright 2010 SF Games

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/
#endregion

using System;

// Other stuff to look into
// http://dotat.at/prog/life/life.html
// http://developer.download.nvidia.com/SDK/9.5/Samples/samples.html#GL_GameOfLife ( NVidia GeForce2 Ultra 72 million cells per second)
// http://members.tip.net.au/~dbell/ hashlife implementation
namespace SimEngine
{
    #region Enum: EngineType

    public enum EngineType { Engine1, Engine2, Engine3 }

    #endregion

    #region Class LifeEngine

    /// <summary>
    /// Game Of Life Engine http://en.wikipedia.org/wiki/Conway's_Game_of_Life
    /// 
    /// Engine base class
    /// </summary>
    public abstract class LifeEngine
    {
        protected HiPerformanceTimer _timer = new HiPerformanceTimer();

        #region Properties

        public int Width { get; set; }
        public int Height { get; set; }
        public int Generation { get; set; }

        public double TotalTime { get; set; }
        public double AvgTimePerGeneration
        {
            get
            {
                return TotalTime/Generation;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Base constructor sets game board width and height
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        protected LifeEngine(int width, int height)
        {
            Width = width;
            Height = height;
            Generation = 0;
        }

        #endregion

        #region Abstract Methods

        public abstract void SetCell(int x, int y, bool alive);
        public abstract bool GetCell(int x, int y);
        public abstract string RowToString(int row);
        public abstract void NextGeneration();

        #endregion

        #region Pre-determined boards creation

        #region Method: CreateBrokenLine

        /// <summary>
        /// One cell high broken line supposed to exhibt infinte growth
        /// OOOOOOOOxxOOOOOxxxxOOOxxxxxxxOOOOOOOxxOOOOO
        /// </summary>
        public void CreateBrokenLine()
        {
            SetCell(5, 10, true);
            SetCell(6, 10, true);
            SetCell(7, 10, true);
            SetCell(8, 10, true);
            SetCell(9, 10, true);
            SetCell(10, 10, true);
            SetCell(11, 10, true);
            SetCell(12, 10, true);

            SetCell(14, 10, true);
            SetCell(15, 10, true);
            SetCell(16, 10, true);
            SetCell(17, 10, true);
            SetCell(18, 10, true);

            SetCell(22, 10, true);
            SetCell(23, 10, true);
            SetCell(24, 10, true);

            SetCell(31, 10, true);
            SetCell(32, 10, true);
            SetCell(33, 10, true);
            SetCell(34, 10, true);
            SetCell(35, 10, true);
            SetCell(36, 10, true);
            SetCell(37, 10, true);

            SetCell(39, 10, true);
            SetCell(40, 10, true);
            SetCell(41, 10, true);
            SetCell(42, 10, true);
            SetCell(43, 10, true);
        }

        #endregion

        #region Method: CreateOooPattern

        /// <summary>
        /// OOO pattern should Oscillate
        /// 
        /// Generation 1:    OOO
        /// 
        ///                   O
        /// Generation 2:    xOx
        ///                   O
        /// 
        ///                   x
        /// Generation 3:    OOO
        ///                   x
        /// </summary>
        public void CreateOooPattern()
        {
            SetCell(1, 3, true);
            SetCell(2, 3, true);
            SetCell(3, 3, true);
        }

        #endregion

        #region Method: CreateSquare

        /// <summary>
        /// First Gen
        ///  XXXXX
        ///  XOOOX
        ///  XOOOX
        ///  XOOOX
        ///  XXXXX
        /// 
        /// Second Gen
        ///    O
        ///   O O
        ///  O   O 
        ///   O O
        ///    O
        /// </summary>
        public void CreateSquare()
        {
            SetCell(1, 1, true);
            SetCell(2, 1, true);
            SetCell(3, 1, true);

            SetCell(1, 2, true);
            SetCell(2, 2, true);
            SetCell(3, 2, true);

            SetCell(1, 3, true);
            SetCell(2, 3, true);
            SetCell(3, 3, true);
        }

        #endregion

        #endregion
    }

    #endregion

    #region Class: Engine1

    /// <summary>
    /// Engine uses a 2 Dimensional int array
    /// </summary>
    public class Engine1 : LifeEngine
    {
        #region Fields

        private readonly int[,] _workCells;

        #endregion

        #region Properties

        public int[,] _cells;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes the engine
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Engine1(int width, int height) : base(width, height)
        {
            _cells = new int[Width, Height];
            _workCells = new int[Width, Height];

            // 80*25 = 2000
            //BitArray bits = new BitArray(Width*Height, false);
            
            // 2D to 1D (y * maxX) + x;
            // 1D to 2D y = n / maxX x = n % maxX
            // xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx
            // xOxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx
            //
            // OOO
            // OcO = 11111111
            // OOO
            //
            // xxx
            // xcx = 00000000
            // xxx
            //
            // Oxx
            // xcx = 10000000
            // xxx
            //
            // 01000000
            // 00100000
            // 
        }

        #endregion

        #region Method: SetCell

        /// <summary>
        /// Sets the given cell x,y location to dead or alive
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="alive"></param>
        public override void SetCell(int x, int y, bool alive)
        {
            _cells[x, y] = alive ? 1 : 0;
        }

        #endregion

        #region Method: GetCell

        /// <summary>
        /// Return true or false if the given x, y for a cell is alive
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override bool GetCell(int x, int y)
        {
            return _cells[x, y] == 1 ? true : false;
        }

        #endregion

        #region Method: RowToString

        /// <summary>
        /// Returns a string of 'O' or ' ' representing life at the given row
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public override string RowToString(int row)
        {
            string rowStr = string.Empty;

            for (int x = 0; x < Width; x++)
                rowStr += _cells[x, row] == 1 ? "O" : " ";

            return rowStr;
        }

        #endregion

        #region Method: NextGeneration

        /// <summary>
        /// Compute the next generation using the below rules
        /// 
        /// Any live cell with fewer than two live neighbours dies, as if caused by underpopulation.
        /// Any live cell with more than three live neighbours dies, as if by overcrowding.
        /// Any live cell with two or three live neighbours lives on to the next generation.
        /// Any dead cell with exactly three live neighbours becomes a live cell.
        /// </summary>
        public override void NextGeneration()
        {
            _timer.Start();

            Generation++;

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    int neighbours = CountNeighbors(x, y);

                    // Live cell
                    if (_cells[x,y] == 1)
                    {
                        // Any live cell with fewer than two live neighbours dies, as if caused by underpopulation.
                        // Any live cell with more than three live neighbours dies, as if by overcrowding.
                        if (neighbours < 2 || neighbours > 3) _workCells[x, y] = 0;
                        // Any live cell with two or three live neighbours lives on to the next generation.
                        else _workCells[x, y] = 1;
                    }
                    else
                    {
                        // Any dead cell with exactly three live neighbours becomes a live cell.
                        if (neighbours == 3)
                            _workCells[x, y] = 1;
                    }
                }
            }

            // C# is week with multi-dimensional arrays, copy the hard way
            CopyCells(_workCells, _cells, Width, Height);

            _timer.Stop();
            TotalTime += _timer.Duration;
        }

        #endregion

        #region Method: CountNeighbors

        /// <summary>
        /// Return the number of neighbors around the given cell
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private int CountNeighbors(int x, int y)
        {
            /*
                1 2 3
                8 c 4
                7 6 5
            * From top left around clockwise
            * Neighbour 1 x-1, y-1
            * Neighbour 2 x,   y-1
            * Neighbour 3 x+1, y-1
            * Neighbour 4 x+1, y
            * Neighbour 5 x+1, y+1
            * Neighbour 6 x,   y+1
            * Neighbour 7 x-1, y+1
            * Neighbour 8 x-1, y
            */

            int negX = (x - 1 + Width) % (Width);
            int posX = (x + 1) % Width;
            int negY = (y - 1 + Height) % (Height);
            int posY = (y + 1) % Height;
            return _cells[negX, negY] + _cells[x, negY] + _cells[posX, negY] + _cells[posX, y] +
                   _cells[posX, posY] + _cells[x, posY] + _cells[negX, posY] + _cells[negX, y];
        }

        #endregion

        #region Method: CopyCells

        /// <summary>
        /// Copy the given from array to the given to array
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="maxX"></param>
        /// <param name="maxY"></param>
        private static void CopyCells(int[,] from, int[,] to, int maxX, int maxY)
        {
            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    to[x, y] = from[x, y];
                }
            }
        }

        #endregion
    }

    #endregion

    #region Class Engine2

    /// <summary>
    /// Engine uses a 1 Dimensional Char array
    /// Engine is not complete
    /// </summary>
    public class Engine2 : LifeEngine
    {
        #region Fields

        private readonly Char[] _workCells;

        #endregion

        #region Properties

        public Char[] Cells { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes the engine
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Engine2(int width, int height) : base(width, height)
        {
            Cells = new Char[Width * Height];
            _workCells = new Char[Width * Height];
        }

        #endregion

        #region Method: SetCell

        /// <summary>
        /// Sets the given cell x,y location to dead or alive
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="alive"></param>
        public override void SetCell(int x, int y, bool alive)
        {
            Cells[y * Width + x] = alive ? 'O' : ' ';
        }

        #endregion

        #region Method: GetCell

        /// <summary>
        /// Return true or false if the given x, y for a cell is alive
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override bool GetCell(int x, int y)
        {
            return Cells[y*Width + x] == 1 ? true : false;
        }

        #endregion

        #region Method: RowToString

        /// <summary>
        /// Returns a string of 'O' or ' ' representing life at the given row
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public override string RowToString(int row)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Method: NextGeneration

        /// <summary>
        /// Compute the next generation using the below rules
        /// 
        /// Any live cell with fewer than two live neighbours dies, as if caused by underpopulation.
        /// Any live cell with more than three live neighbours dies, as if by overcrowding.
        /// Any live cell with two or three live neighbours lives on to the next generation.
        /// Any dead cell with exactly three live neighbours becomes a live cell.
        /// </summary>
        public override void NextGeneration()
        {
            Generation++;

            for (int idx = 0; idx < Cells.Length; idx++)
            {
                int neighbours = CountNeighbors(idx);

                // Live cell
                if (Cells[idx] == 1)
                {
                    // Any live cell with fewer than two live neighbours dies, as if caused by underpopulation.
                    // Any live cell with more than three live neighbours dies, as if by overcrowding.
                    if (neighbours < 2 || neighbours > 3) _workCells[idx] = ' ';
                    // Any live cell with two or three live neighbours lives on to the next generation.
                    else _workCells[idx] = 'O';
                }
                else
                {
                    // Any dead cell with exactly three live neighbours becomes a live cell.
                    if (neighbours == 3)
                        _workCells[idx] = 'O';
                }
            }

            Cells = _workCells;
        }

        #endregion

        #region Method: CountNeighbors

        /// <summary>
        /// Return the number of neighbors around the given cell
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        private int CountNeighbors(int idx)
        {
            // idx = 81
            // p = ((idx / Width) - 1) + idx % Width
            // 
            //    0                                                                                    79
            // 0  xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx
            // 1  xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx
            // 2  xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx
            // 3  xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx
            // 4  xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx
            // 5  xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx
            // 6  xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx
            // 7  xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx
            // 8  xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx
            // 9  xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx
            // 10 xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx
            // 11 xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx
            // 12 xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx
            // 13 xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx
            // 14 xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx
            // 15 xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx
            // 16 xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx
            // 17 xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx
            // 18 xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx
            // 19 xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx
            // 20 xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx
            // 21 xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx
            // 22 xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx
            // 23 xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx
            // 24 xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx xxxxxxxxxx
            //
            //  1 2 3
            //  8 c 4
            //  7 6 5

            int neighbors = 0;
            int x = idx % Width;
            int y = idx / Width;
            int pos1, pos2, pos3, pos4, pos5, pos6, pos7, pos8;

            // No corners and sides case
            if (idx > Width && idx < Width*Height-Width-1)
            {
                pos1 = idx - Width - 1;
                pos2 = idx - Width;
                pos3 = idx - Width + 1;
                pos4 = idx + 1;
                pos5 = idx + Width + 1;
                pos6 = idx + Width;
                pos7 = idx + Width - 1;
                pos8 = idx - 1;

                if (Cells[pos1] == 'O') neighbors++;
                if (Cells[pos2] == 'O') neighbors++;
                if (Cells[pos3] == 'O') neighbors++;
                if (Cells[pos4] == 'O') neighbors++;
                if (Cells[pos5] == 'O') neighbors++;
                if (Cells[pos6] == 'O') neighbors++;
                if (Cells[pos7] == 'O') neighbors++;
                if (Cells[pos8] == 'O') neighbors++;
            }

            return neighbors;
        }

        #endregion
    }

    #endregion

    #region Class Engine3

    /// <summary>
    /// Engine uses objects
    /// </summary>
    public class Engine3 : LifeEngine
    {
        #region Constructor

        /// <summary>
        /// Initializes the engine
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Engine3(int width, int height) : base(width, height)
        {
            
        }

        #endregion

        #region Method: SetCell

        /// <summary>
        /// Sets the given cell x,y location to dead or alive
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="alive"></param>
        public override void SetCell(int x, int y, bool alive)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Method: GetCell

        /// <summary>
        /// Return true or false if the given x, y for a cell is alive
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override bool GetCell(int x, int y)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Method: RowToString

        /// <summary>
        /// Returns a string of 'O' or ' ' representing life at the given row
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public override string RowToString(int row)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Method: NextGeneration

        /// <summary>
        /// Compute the next generation using the below rules
        /// 
        /// Any live cell with fewer than two live neighbours dies, as if caused by underpopulation.
        /// Any live cell with more than three live neighbours dies, as if by overcrowding.
        /// Any live cell with two or three live neighbours lives on to the next generation.
        /// Any dead cell with exactly three live neighbours becomes a live cell.
        /// </summary>
        public override void NextGeneration()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Method: CountNeighbors

        /// <summary>
        /// Return the number of neighbors around the given cell
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        private int CountNeighbors(int idx)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    #endregion
}
