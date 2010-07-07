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

using System.Collections.Generic;

namespace SimEngine.Engines
{
    #region Class: Engine4

    /// <summary>
    /// Engine uses a 2 Dimensional int array and Scan List
    /// The Life field wraps around at the sides, tops and corners, i.e a toroidal array
    /// 
    /// The scan list is a List of start and end x,y coordinates indicating what areas of the
    /// Life field have activity to scan, effectively skipping areas that have no possibility of
    /// life/death
    /// </summary>
    public class Engine4 : LifeEngine
    {
        #region Fields

        private readonly int[,] _cells;
        private readonly int[,] _workCells;
        private List<CoordinatePair> _scanList = new List<CoordinatePair>();

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes the engines Life field
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Engine4(int width, int height)
            : base(width, height)
        {
            _cells = new int[Width, Height];
            _workCells = new int[Width, Height];
            
            // Initialize the scan list to check the entire field
            _scanList.Add(new CoordinatePair(new Point2D(0, 0), new Point2D(Width-1, Height-1)));
        }

        #endregion

        #region Method: Title

        /// <summary>
        /// Returns a title describing the implementation
        /// </summary>
        /// <returns></returns>
        public override string Title()
        {
            return @"2D integer array engine with scan list";
        }

        #endregion

        #region Method: Summary

        /// <summary>
        /// Returns asummary of engine implementation information
        /// </summary>
        /// <returns></returns>
        public override string Summary()
        {
            return @"Uses a 2D integer array. A list of cells to scan is used to skip over dead space. Life field wraps around at the sides, tops and corners, i.e a toroidal array";
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
            _stopWatch.Start();
            Generation++;

            // TODO Use scan list
            // Prepare to rebuild the scan list as we process the main scan list
            // Start by assuming we will need to scan the entire field next generation
            List<CoordinatePair> newScanList = new List<CoordinatePair>();
            CoordinatePair coordinate = new CoordinatePair(0, 0, Width - 1, Height - 1);
            bool findingStartCoordinate = true;

            // While having CoordinatePairs
            foreach (CoordinatePair coordinatePair in _scanList)
            {
                // Loop over start y, and x until the end y and x
                for (int y = coordinatePair._start._y; y <= coordinatePair._stop._y; y++)
                {
                    for (int x = coordinatePair._start._x; x <= coordinatePair._stop._x; x++)
                    {
                        // Determine life
                        // Build new scan list entries
                        int neighbours = CountNeighbours(x, y);
                        findingStartCoordinate = neighbours == 0;

                        if (findingStartCoordinate)
                        {
                            coordinate._start._x = x;
                            coordinate._start._y = y;
                        }
                        else
                        {
                            coordinate._stop._x = x;
                            coordinate._stop._y = y;
                        }

                        // Live cell
                        if (_cells[x, y] == 1)
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
            }

            // C# is week with multi-dimensional arrays, copy the hard way
            CopyCells(_workCells, _cells, Width, Height);

            // Set the scan list to our new rebuilt scan list
            _scanList = newScanList;

            _stopWatch.Stop();
            TotalTime += _stopWatch.ElapsedMilliseconds;
            _stopWatch.Reset();
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
}