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

namespace SimEngine.Engines
{
    #region Class: Engine3

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
            public Cell N { private get; set; }
            public Cell Ne { private get; set; }
            public Cell E { private get; set; }
            public Cell Se { private get; set; }
            public Cell S { private get; set; }
            public Cell Sw { private get; set; }
            public Cell W { private get; set; }
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
        public Engine3(int width, int height)
            : base(width, height)
        {
            // Create the cells
            _length = Width * Height;

            _cells = new List<Cell>(_length);
            _workCells = new List<Cell>(_length);

            for (int i = 0; i < _length; i++)
            {
                _cells.Add(new Cell { Alive = 0 });
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

        #region Method: Title

        /// <summary>
        /// Returns a title describing the implementation
        /// </summary>
        /// <returns></returns>
        public override string Title()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Method: Summary

        /// <summary>
        /// Returns asummary of engine implementation information
        /// </summary>
        /// <returns></returns>
        public override string Summary()
        {
            throw new NotImplementedException();
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

            for (int idx = 0; idx < _length; idx++)
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