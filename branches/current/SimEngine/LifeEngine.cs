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
using System.Collections.Generic;

namespace SimEngine
{
    #region Enum: EngineType

    public enum EngineType { Engine1, Engine2, Engine3 }

    #endregion

    #region Class LifeEngine

    /// <summary>
    /// Game Of Life Engine base class
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
        public abstract void Clear();
        public abstract string RowToString(int row);
        public abstract void NextGeneration();

        #endregion
    }

    #endregion

    #region Class: Engine1

    /// <summary>
    /// Engine uses a 2 Dimensional int array
    /// The Life field wraps around at the sides, tops and corners, i.e a toroidal array
    /// </summary>
    public class Engine1 : LifeEngine
    {
        #region Fields

        private readonly int[,] _cells;
        private readonly int[,] _workCells;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes the engines Life field
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Engine1(int width, int height) : base(width, height)
        {
            _cells = new int[Width, Height];
            _workCells = new int[Width, Height];
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

        #region Method: Clear

        /// <summary>
        /// Set all cells to dead
        /// </summary>
        public override void Clear()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    _cells[x, y] = 0;
                    _workCells[x, y] = 0;
                }
            }
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
                    int neighbours = CountNeighbours(x, y);

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

        #region Method: CountNeighbours

        /// <summary>
        /// Return the number of neighbors around the given cell
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private int CountNeighbours(int x, int y)
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
    /// TODO Engine is not complete
    /// </summary>
    public class Engine2 : LifeEngine
    {
        #region Fields

        private Char[] _cells;
        private readonly Char[] _workCells;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes the engines Life field
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Engine2(int width, int height) : base(width, height)
        {
            _cells = new Char[Width * Height];
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
            _cells[y * Width + x] = alive ? 'O' : ' ';
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
            return _cells[y * Width + x] == 1 ? true : false;
        }

        #endregion

        #region Method: Clear

        /// <summary>
        /// Set all cells to dead
        /// </summary>
        public override void Clear()
        {
            for (int idx=0; idx < _cells.Length; idx++)
            {
                _cells[idx] = ' ';
                _workCells[idx] = ' ';
            }
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
            string strRow = String.Empty;

            for (int x = 0; x < Width; x++)
            {
                strRow += GetCell(x, row) ? 'O' : ' ';
            }

            return strRow;
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

            for (int idx = 0; idx < _cells.Length; idx++)
            {
                int neighbours = CountNeighbours(idx);

                // Live cell
                if (_cells[idx] == 1)
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

            _cells = _workCells;
        }

        #endregion

        #region Method: CountNeighbours

        /// <summary>
        /// Return the number of neighbors around the given cell
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        private int CountNeighbours(int idx)
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
            //int x = idx % Width;
            //int y = idx / Width;

            // No corners and sides case
            // TODO Add corners and side cases
            if (idx > Width && idx < Width*Height-Width-1)
            {
                int pos1 = idx - Width - 1;
                int pos2 = idx - Width;
                int pos3 = idx - Width + 1;
                int pos4 = idx + 1;
                int pos5 = idx + Width + 1;
                int pos6 = idx + Width;
                int pos7 = idx + Width - 1;
                int pos8 = idx - 1;

                if (_cells[pos1] == 'O') neighbors++;
                if (_cells[pos2] == 'O') neighbors++;
                if (_cells[pos3] == 'O') neighbors++;
                if (_cells[pos4] == 'O') neighbors++;
                if (_cells[pos5] == 'O') neighbors++;
                if (_cells[pos6] == 'O') neighbors++;
                if (_cells[pos7] == 'O') neighbors++;
                if (_cells[pos8] == 'O') neighbors++;
            }

            return neighbors;
        }

        #endregion
    }

    #endregion

    #region Class Engine3

    /// <summary>
    /// Engine uses object refernces
    /// TODO Engine is not complete, currently eating cells
    /// </summary>
    public class Engine3 : LifeEngine
    {
        #region Class: Cell

        /// <summary>
        /// 
        /// </summary>
        public class Cell
        {
            public int Alive { get; set; }
            public Cell N  { private get; set; }
            public Cell Ne { private get; set; }
            public Cell E  { private get; set; }
            public Cell Se { private get; set; }
            public Cell S  { private get; set; }
            public Cell Sw { private get; set; }
            public Cell W  { private get; set; }
            public Cell Nw { private get; set; }
            public int Neighbors
            {
                get { return N.Alive + Ne.Alive + E.Alive + Se.Alive + S.Alive + Sw.Alive + W.Alive + Nw.Alive; }
            }
        }

        #endregion

        #region Fields

        private List<Cell> _cells;
        private readonly List<Cell> _workCells;
        private readonly int _length;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes the engine
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Engine3(int width, int height) : base(width, height)
        {
            // Create the cells
            _length = Width * Height;

            _cells = new List<Cell>(_length);
            _workCells = new List<Cell>(_length);

            for (int i = 0; i < _length; i++)
            {
                _cells.Add(new Cell {Alive = 0});
                _workCells.Add(new Cell { Alive = 0 });
            }

            // Assign neighbors
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    /*
                        1 2 3
                        8 c 4
                        7 6 5
                    * From top around clockwise
                    * North      = Neighbour 2 x,   y-1
                    * North East = Neighbour 3 x+1, y-1
                    * East       = Neighbour 4 x+1, y
                    * South East = Neighbour 5 x+1, y+1
                    * South      = Neighbour 6 x,   y+1
                    * South West = Neighbour 7 x-1, y+1
                    * West       = Neighbour 8 x-1, y
                    * NorthWest  = Neighbour 1 x-1, y-1
                    */

                    Cell c = GetCellObj(x, y);
                    int negX = (x - 1 + Width) % (Width);
                    int posX = (x + 1) % Width;
                    int negY = (y - 1 + Height) % (Height);
                    int posY = (y + 1) % Height;

                    c.N = GetCellObj(x, negY);
                    c.Ne = GetCellObj(posX, negY);
                    c.E = GetCellObj(posX, y);
                    c.Se = GetCellObj(posX, posY);
                    c.S = GetCellObj(x, posY);
                    c.Sw = GetCellObj(negX, posY);
                    c.W = GetCellObj(negX, y);
                    c.Nw = GetCellObj(negX, negY);
                }
            }

            _workCells = _cells;
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
            _cells[y * Width + x].Alive = alive ? 1 : 0;
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
            return _cells[y * Width + x].Alive == 1 ? true : false;
        }

        #endregion

        #region Method: Clear

        /// <summary>
        /// Set all cells to dead
        /// </summary>
        public override void Clear()
        {
            for (int idx = 0; idx < _length; idx++)
            {
                _cells[idx].Alive = 0;
                _workCells[idx].Alive = 0;
            }
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
            string strRow = String.Empty;

            for (int x = 0; x < Width; x++)
                strRow += GetCell(x, row) ? 'O' : ' ';

            return strRow;
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

            for (int idx=0; idx < _length; idx++)
            {
                int neighbours = CountNeighbours(idx);

                // Live cell
                if (_cells[idx].Alive == 1)
                {
                    // Any live cell with fewer than two live neighbours dies, as if caused by underpopulation.
                    // Any live cell with more than three live neighbours dies, as if by overcrowding.
                    if (neighbours < 2 || neighbours > 3) _workCells[idx].Alive = 0;
                    // Any live cell with two or three live neighbours lives on to the next generation.
                    else _workCells[idx].Alive = 1;
                }
                else
                {
                    // Any dead cell with exactly three live neighbours becomes a live cell.
                    if (neighbours == 3)
                        _workCells[idx].Alive = 1;
                }
            }

            _cells = _workCells;
        }

        #endregion

        #region Method: CountNeighbours

        /// <summary>
        /// Return the number of neighbors around the given cell
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        private int CountNeighbours(int idx)
        {
            return _cells[idx].Neighbors;
        }

        #endregion

        #region Method: GetCellObj

        /// <summary>
        /// Return a <see cref="Cell"/> at the given coordinates
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private Cell GetCellObj(int x, int y)
        {
            return _cells[y * Width + x];
        }

        #endregion
    }

    #endregion
}
