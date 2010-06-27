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
using System.Threading;
using System.Timers;
using SimEngine;
using Utility;
using Timer = System.Timers.Timer;

namespace SharpLifeConsole
{
    class Program
    {
        #region Fields

        private static LifeEngine _engine;
        private static Timer _timer;
        private static int _screenWidth;
        private static int _screenHeight;
        private static bool _pause;
        private static bool _exit;
        private static bool _step;
        private static bool _stepNow;

        #endregion

        #region Method: Main

        /// <summary>
        /// Application entry point
        /// </summary>
        static void Main(string[] args)
        {
            #region Setup

            CommandLineProcessor.ProcessCommandLine(args);

            _step = false;
            _screenWidth = Console.LargestWindowWidth - 10;
            _screenHeight = Console.LargestWindowHeight - 10;
            Console.WindowWidth = _screenWidth;
            Console.WindowHeight = _screenHeight;
            Console.CursorVisible = false;
            _engine = CreateEngine(EngineType.Engine1, _screenWidth, _screenHeight - 2);

            // Select a pre-defined pattern
            _engine.CreateBrokenLine();
            //_engine.CreateOooPattern();
            //_engine.CreateSquare();

            RenderBoard();

            // Start rendering timer
            _timer = new Timer {Interval = 25};
            _timer.Elapsed += Render;
            _timer.Start();

            #endregion

            #region Simulation Loop

            while (true)
            {
                CheckInput();

                if (_exit)
                {
                    Console.CursorVisible = true;
                    return;
                }

                if (_step && _stepNow)
                {
                    Render();
                    _stepNow = false;
                }
                Thread.Sleep(5);
            }

            #endregion
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

        #region Method: Render

        /// <summary>
        /// Timer timeout target to render board
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        static void Render(object o, ElapsedEventArgs e)
        {
            _timer.Stop();
            if (!_step) Render();
            _timer.Start();
        }

        /// <summary>
        /// Render current board generation
        /// </summary>
        private static void Render()
        {
            if (_pause) return;
            _engine.NextGeneration();
            RenderBoard();
        }

        #endregion

        #region Method: RenderBoard

        /// <summary>
        /// Display the board on the console window
        /// </summary>
        private static void RenderBoard()
        {
            HiPerformanceTimer timer = new HiPerformanceTimer();

            Console.Clear();

            timer.Start();
            {
                // Write a row of cells
                for (int y = 0; y < _engine.Height; y++)
                {
                    Console.SetCursorPosition(0, y);
                    Console.Write(_engine.RowToString(y));
                }
            }
            timer.Stop();

            Console.SetCursorPosition(0, _screenHeight - 1);
            Console.Write("Generation: " + _engine.Generation + " Time: " + timer.Duration);
        }

        #endregion

        #region Method: CheckInput

        /// <summary>
        /// Process any keyboard input we care about
        /// </summary>
        private static void CheckInput()
        {
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);

                switch (key.KeyChar)
                {
                    // Exit
                    case 'x':
                        _exit = true;
                        break;

                    // Pause
                    case 'p':
                        _pause = !_pause;
                        break;

                    // Toggle Step Mode
                    case 'm':
                        _step = !_step;
                        break;

                    // Step
                    case 's':
                        _stepNow = true;
                        break;

                    // Help
                    case 'h':
                        {
                            _pause = !_pause;

                            if (_pause)
                            {
                                Console.Clear();
                                Console.SetCursorPosition(0, 0);
                                Console.WriteLine("Help");
                                Console.WriteLine("===============================");
                                Console.WriteLine("P Pause");
                                Console.WriteLine("M Toggle Step Mode");
                                Console.WriteLine("S Step");
                                Console.WriteLine("H Help");
                                Console.WriteLine("X Exit");
                            }
                            else
                            {
                                RenderBoard();
                            }
                        }
                        break;
                }
            }
        }

        #endregion
    }
}
