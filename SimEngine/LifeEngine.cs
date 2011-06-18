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

using System.Diagnostics;

namespace SimEngine
{
    #region Enum: EngineType

    public enum EngineType { Engine1, Engine2, Engine3, Engine4 }

    #endregion

    #region Class LifeEngine

    /// <summary>
    /// Game Of Life Engine base class
    /// </summary>
    public abstract class LifeEngine
    {
        protected Stopwatch StopWatch = new Stopwatch();

        #region Properties

        public int Width { get; set; }
        public int Height { get; set; }
        public int Generation { get; set; }
        public long TotalTime { get; set; }
        public long AvgTimePerGeneration { get { return TotalTime/Generation; } }

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

        public abstract string Title();
        public abstract string Summary();
        public abstract void SetCell(int x, int y, bool alive);
        public abstract bool GetCell(int x, int y);
        public abstract void Clear();
        public abstract string RowToString(int row);
        public abstract void NextGeneration();

        #endregion
    }

    #endregion
}
