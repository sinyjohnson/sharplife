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
using Utility;
using Timer = System.Timers.Timer;

namespace SpeedTestConsole
{
    class Program
    {
        #region Fields

        private static LifeEngine _engine;
        private static Timer _timer1Sec;
        private const double ONE_SECOND = 500;
        private const int WIDTH = 100;
        private const int HEIGHT = 100;
        private const int NUM_CELLS = WIDTH*HEIGHT;
        private static bool _pause;
        private static bool _exit;

        #endregion

        #region Method: Main

        /// <summary>
        /// Usage: SpeedTestConsole -engine [engine name]
        ///        -engine  Optional engine name. If none given, then Engine1 is used.
        ///                 Engine names are in the format EngineN where N is an integer.
        ///                 If a given engine number does not esist, the application exists.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            _pause = true;
            _exit = false;
            _timer1Sec = new Timer { Interval = ONE_SECOND };
            _timer1Sec.Elapsed += CellsPerSecond;
            _timer1Sec.Start();

            #region Command Line Processing

            CommandLineProcessor.ProcessCommandLine(args, true);

            // Create the engine given or the default
            if (CommandLineProcessor.ParameterExits("engine"))
            {
                switch (CommandLineProcessor.Value("engine").ToLower())
                {
                    case "engine1": _engine = CreateEngine(EngineType.Engine1, WIDTH, HEIGHT); break;
                    case "engine2": _engine = CreateEngine(EngineType.Engine2, WIDTH, HEIGHT); break;
                    case "engine3": _engine = CreateEngine(EngineType.Engine3, WIDTH, HEIGHT); break;
                    default: _engine = CreateEngine(EngineType.Engine1, WIDTH, HEIGHT); break;
                }
            }
            else
                _engine = CreateEngine(EngineType.Engine1, WIDTH, HEIGHT);

            #endregion

            Console.CursorVisible = false;
            Console.WriteLine("Press the 'p' key to start and/or pause. 'x' to exit");

            while (!_exit)
            {
                if (!_pause)
                    _engine.NextGeneration();

                CheckInput();
            }

            Console.CursorVisible = true;
        }

        #endregion

        #region Method: CreateEngine

        /// <summary>
        /// Create and return the given Life Engine Type
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        private static LifeEngine CreateEngine(EngineType engine, int width, int height)
        {
            switch (engine)
            {
                case EngineType.Engine2: return new Engine2(width, height);
                case EngineType.Engine3: return new Engine3(width, height);
                default: return new Engine1(width, height);
            }
        }

        #endregion

        #region Method: CellsPerSecond

        /// <summary>
        /// Engine1 cellsPerSecond = 1,400,000 1.4 million on average 150 Generations per second
        /// Engine3 cellsPerSecond = 1,590,000 1.6 million on average 159 Generations per second
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        static void CellsPerSecond(object o, ElapsedEventArgs e)
        {
            _timer1Sec.Stop();
            {
                if (!_pause)
                {
                    int cellsPerSecond = NUM_CELLS * _engine.Generation;

                    Console.SetCursorPosition(0, 3);
                    Console.Write("Engine:                 " + _engine.GetType());

                    Console.SetCursorPosition(0, 4);
                    Console.Write("Cells Per Second:       " + cellsPerSecond);

                    Console.SetCursorPosition(0, 5);
                    Console.Write("Generations Per Second: " + _engine.Generation);

                    _engine.Generation = 0;
                }
            }
            _timer1Sec.Start();
        }

        #endregion

        #region Method: CheckInput

        /// <summary>
        /// Process any keyboard input we care about
        /// </summary>
        private static void CheckInput()
        {
            if (!Console.KeyAvailable) return;

            ConsoleKeyInfo key = Console.ReadKey(true);
            switch (key.KeyChar)
            {
                case 'x':   _exit = true;       break;
                case 'p':   _pause = !_pause;   break;
            }
        }

        #endregion
    }
}
