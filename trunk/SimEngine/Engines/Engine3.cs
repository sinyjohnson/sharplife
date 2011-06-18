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
    /// Engine uses object references
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
        private List<Cell> _workCells;
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
                    * North      = Neighbor 2 x,   y-1
                    * North East = Neighbor 3 x+1, y-1
                    * East       = Neighbor 4 x+1, y
                    * South East = Neighbor 5 x+1, y+1
                    * South      = Neighbor 6 x,   y+1
                    * South West = Neighbor 7 x-1, y+1
                    * West       = Neighbor 8 x-1, y
                    * NorthWest  = Neighbor 1 x-1, y-1
                    */

                    Cell c1 = GetCellObj(x, y, ref _cells);
                    Cell c2 = GetCellObj(x, y, ref _workCells);
                    int negX = (x - 1 + Width) % (Width);
                    int posX = (x + 1) % Width;
                    int negY = (y - 1 + Height) % (Height);
                    int posY = (y + 1) % Height;

                    c1.N = GetCellObj(x, negY, ref _cells);
                    c1.Ne = GetCellObj(posX, negY, ref _cells);
                    c1.E = GetCellObj(posX, y, ref _cells);
                    c1.Se = GetCellObj(posX, posY, ref _cells);
                    c1.S = GetCellObj(x, posY, ref _cells);
                    c1.Sw = GetCellObj(negX, posY, ref _cells);
                    c1.W = GetCellObj(negX, y, ref _cells);
                    c1.Nw = GetCellObj(negX, negY, ref _cells);

                    c2.N = GetCellObj(x, negY, ref _workCells);
                    c2.Ne = GetCellObj(posX, negY, ref _workCells);
                    c2.E = GetCellObj(posX, y, ref _workCells);
                    c2.Se = GetCellObj(posX, posY, ref _workCells);
                    c2.S = GetCellObj(x, posY, ref _workCells);
                    c2.Sw = GetCellObj(negX, posY, ref _workCells);
                    c2.W = GetCellObj(negX, y, ref _workCells);
                    c2.Nw = GetCellObj(negX, negY, ref _workCells);
                }
            }

            CopyCells(ref _cells, ref _workCells);
        }

        #endregion

        #region Method: Title

        /// <summary>
        /// Returns a title describing the implementation
        /// </summary>
        /// <returns></returns>
        public override string Title()
        {
            return @"Object reference engine";
        }

        #endregion

        #region Method: Summary

        /// <summary>
        /// Returns a summary of engine implementation information
        /// </summary>
        /// <returns></returns>
        public override string Summary()
        {
            return @"Uses a List of cells with each cell containing references to its 8 neighbors'. Scans the entire List each generation. Life field wraps around at the sides, tops and corners, i.e a toroidal array";
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
        /// Any live cell with fewer than two live neighbors' dies, as if caused by under population.
        /// Any live cell with more than three live neighbors' dies, as if by overcrowding.
        /// Any live cell with two or three live neighbors' lives on to the next generation.
        /// Any dead cell with exactly three live neighbors' becomes a live cell.
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
                    // Any live cell with fewer than two live neighbors' dies, as if caused by under population.
                    // Any live cell with more than three live neighbors' dies, as if by overcrowding.
                    if (neighbours < 2 || neighbours > 3) _workCells[idx].Alive = 0;
                    // Any live cell with two or three live neighbors' lives on to the next generation.
                    else _workCells[idx].Alive = 1;
                }
                else
                {
                    // Any dead cell with exactly three live neighbors' becomes a live cell.
                    if (neighbours == 3)
                        _workCells[idx].Alive = 1;
                }
            }

            CopyCells(ref _workCells, ref _cells);
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
        /// <param name="cells"></param>
        /// <returns></returns>
        private Cell GetCellObj(int x, int y, ref List<Cell> cells)
        {
            return cells[y * Width + x];
        }

        #endregion

        #region Method: CopyCells

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        private static void CopyCells(ref List<Cell> source, ref List<Cell> destination)
        {
            // Both lists must be same length
            if (source.Count != destination.Count) throw new Exception("The given Lists are not of equal length");

            for (int x=0; x < source.Count; x++)
            {
                destination[x].Alive = source[x].Alive;
            }
        }

        #endregion
    }

    #endregion
}