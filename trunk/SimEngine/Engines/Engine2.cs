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

namespace SimEngine.Engines
{
    #region Class: Engine2

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
        public Engine2(int width, int height)
            : base(width, height)
        {
            _cells = new Char[Width * Height];
            _workCells = new Char[Width * Height];
        }

        #endregion

        #region Method: Title

        /// <summary>
        /// Returns a title describing the implementation
        /// </summary>
        /// <returns></returns>
        public override string Title()
        {
            return @"1D character array engine";
        }

        #endregion

        #region Method: Summary

        /// <summary>
        /// Returns a summary of engine implementation information
        /// </summary>
        /// <returns></returns>
        public override string Summary()
        {
            return @"Uses a 1D character array and scans the entire array each generation. Life field wraps around at the sides, tops and corners, i.e a toroidal array";
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
            for (int idx = 0; idx < _cells.Length; idx++)
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
        /// Any live cell with fewer than two live neighbors' dies, as if caused by under population.
        /// Any live cell with more than three live neighbors' dies, as if by overcrowding.
        /// Any live cell with two or three live neighbors' lives on to the next generation.
        /// Any dead cell with exactly three live neighbors' becomes a live cell.
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
                    // Any live cell with fewer than two live neighbors' dies, as if caused by under population.
                    // Any live cell with more than three live neighbors' dies, as if by overcrowding.
                    if (neighbours < 2 || neighbours > 3) _workCells[idx] = ' ';
                    // Any live cell with two or three live neighbors' lives on to the next generation.
                    else _workCells[idx] = 'O';
                }
                else
                {
                    // Any dead cell with exactly three live neighbors' becomes a live cell.
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
            if (idx > Width && idx < Width * Height - Width - 1)
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
}