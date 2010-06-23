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
using System.Timers;
using SimEngine;
using Timer = System.Timers.Timer;

namespace SpeedTestConsole
{
    class Program
    {
        private static Engine1 _engine;
        private static Timer _timer1Sec;
        private const int WIDTH = 100;
        private const int HEIGHT = 100;
        private const int NUM_CELLS = WIDTH*HEIGHT;

        static void Main(string[] args)
        {
            _engine = new Engine1(WIDTH, HEIGHT);
            _timer1Sec = new Timer { Interval = 500 };
            _timer1Sec.Elapsed += CalculateCellsPerSecond;
            _timer1Sec.Start();

            while (true)
            {
                _engine.NextGeneration();
            }
        }

        /// <summary>
        /// cellsPerSecond = 1,400,000 1.5 million on average
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        static void CalculateCellsPerSecond(object o, ElapsedEventArgs e)
        {
            _timer1Sec.Stop();
            int cellsPerSecond = NUM_CELLS * _engine.Generation;
            Console.WriteLine("Cells Per Second: " + cellsPerSecond + " Generations: " + _engine.Generation);
            _engine.Generation = 0;
            _timer1Sec.Start();
        }
    }
}
