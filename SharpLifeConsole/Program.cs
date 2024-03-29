﻿#region Apache License 2.0
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
using System.Diagnostics;
using SimEngine;
using SimEngine.Engines;
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
        /// 
        /// Usage:
        ///     SharpLifeConsole [-engine engine name] [-width n] [-height n] [-mode p|r] [-file file name] [-help/?]
        ///         -engine Optional EngineType, Engine1 ...
        ///         -width  Optional number, if number is greater than the maximum console size, then maximum console size is used
        ///         -height Optional number, if number is greater than the maximum console size, then maximum console size is used
        ///         -mode   Optional p=paused r=running
        ///         -file   Required file name of a supported life file pattern
        ///         -help/? Optional Display usage
        /// </summary>
        static void Main(string[] args)
        {
            #region Setup

            // Set defaults. Can be changed by command line parameters
            _step = false;
            _screenWidth = Console.LargestWindowWidth - 10;
            _screenHeight = Console.LargestWindowHeight - 10;

            #region Process Command Line

            CommandLineProcessor.ProcessCommandLine(args, true);

            // Help
            if (CommandLineProcessor.ParameterExits("help") || CommandLineProcessor.ParameterExits("?"))
            {
                Usage();
                return;
            }

            // Width
            if (CommandLineProcessor.ParameterExits("width"))
            {
                int width = Convert.ToInt32(CommandLineProcessor.Value("width"));
                if (width <= (Console.LargestWindowWidth - 10) )
                    _screenWidth = width;
            }

            // Height
            if (CommandLineProcessor.ParameterExits("height"))
            {
                int height = Convert.ToInt32(CommandLineProcessor.Value("height"));
                if (height <= (Console.LargestWindowHeight - 10))
                    _screenHeight = height;
            }

            // Mode
            if (CommandLineProcessor.ParameterExits("mode"))
                _step = CommandLineProcessor.Value("mode") == "p" ? true : false;

            // Engine
            if (CommandLineProcessor.ParameterExits("engine"))
            {
                switch (CommandLineProcessor.Value("engine").ToLower())
                {
                    case "engine1": _engine = CreateEngine(EngineType.Engine1, _screenWidth, _screenHeight - 2); break;
                    case "engine2": _engine = CreateEngine(EngineType.Engine2, _screenWidth, _screenHeight - 2); break;
                    case "engine3": _engine = CreateEngine(EngineType.Engine3, _screenWidth, _screenHeight - 2); break;
                    default: _engine = CreateEngine(EngineType.Engine1, _screenWidth, _screenHeight - 2); break;
                }
            }
            else
                _engine = CreateEngine(EngineType.Engine1, _screenWidth, _screenHeight - 2);

            // File
            try
            {
                FileObject.Load(CommandLineProcessor.Value("file"), _engine);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception processing the given file");
                Console.WriteLine(ex);
                return;
            }

            #endregion

#if (WINDOWS)
            Console.WindowWidth = _screenWidth;
            Console.WindowHeight = _screenHeight;
#endif
            Console.CursorVisible = false;

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
            Stopwatch stopwatch = new Stopwatch(); 

            Console.Clear();

            stopwatch.Start();
            {
                // Write a row of cells
                for (int y = 0; y < _engine.Height; y++)
                {
                    Console.SetCursorPosition(0, y);
                    Console.Write(_engine.RowToString(y));
                }
            }
            stopwatch.Stop();

            Console.SetCursorPosition(0, _screenHeight - 1);
            Console.Write("Generation: " + _engine.Generation + " Time: " + stopwatch.ElapsedMilliseconds);
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

        #region Method: Usage

        /// <summary>
        /// Display command line usage
        /// </summary>
        private static void Usage()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("     SharpLifeConsole [-engine engine name] [-width n] [-height n] [-mode p|r] -file file name [-help/?]");
            Console.WriteLine("         -engine Optional EngineType, Engine1 ...");
            Console.WriteLine("         -width  Optional number, if number is greater than the maximum console size, then maximum console size is used");
            Console.WriteLine("         -height Optional number, if number is greater than the maximum console size, then maximum console size is used");
            Console.WriteLine("         -mode   Optional p=paused r=running");
            Console.WriteLine("         -file   Required file name of a supported life file pattern");
            Console.WriteLine("         -help/? Optional Display usage");
        }

        #endregion:
    }
}
