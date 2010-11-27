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
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using SimEngine;
using SimEngine.Engines;

namespace SharpLife
{
    public sealed partial class LifeForm : Form
    {
        #region Fields

        private static LifeEngine _engine;
        private static Form _thisForm;
        private static ToolStripStatusLabel _thisToolStripStatusLabelGeneration;
        private static readonly Timer _timer = new Timer();
        private readonly Pen _pen;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public LifeForm()
        {
            InitializeComponent();

            // Our background and foreground colors
            BackColor = Color.White;
            _pen = new Pen(Color.Green);
            
            // We need a static reference to our form, as our timer event handler for drawing is a static method
            _thisForm = this;
            _thisToolStripStatusLabelGeneration = toolStripStatusLabelGeneration;
            
            // TODO Add a drop down control to allow user to select the Life Engine to use
            _engine = CreateEngine(EngineType.Engine1, Width, Height);
            _timer.Tick += TimerEventProcessor;
            _timer.Interval = 10;
        }

        #endregion

        #region Method: CreateEngine

        /// <summary>
        /// Create the given Life Engine Type at the given width and height
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
                case EngineType.Engine4: return new Engine3(width, height);
                default: return new Engine1(width, height);
            }
        }

        #endregion

        #region Menu Events

        private void NewToolStripMenuItemClick(object sender, EventArgs e)    { NewLife();  }
        private void OpenToolStripMenuItemClick(object sender, EventArgs e)   { LoadFile(); }
        private void SaveToolStripMenuItemClick(object sender, EventArgs e)   { SaveFile(); }
        private void SaveAsToolStripMenuItemClick(object sender, EventArgs e) { SaveFile(); }
        private void ExitToolStripMenuItemClick(object sender, EventArgs e)   { Exit();     }
        private void RunToolStripMenuItemClick(object sender, EventArgs e)    { Run();      }
        private void PauseToolStripMenuItemClick(object sender, EventArgs e)  { Pause();    }
        private void StepToolStripMenuItemClick(object sender, EventArgs e)   { Step();     }
        private void AboutToolStripMenuItemClick(object sender, EventArgs e)  { About();    }

        #endregion

        #region Toolbar Events

        private void NewToolStripButtonClick(object sender, EventArgs e)   { NewLife();  }
        private void OpenToolStripButtonClick(object sender, EventArgs e)  { LoadFile(); }
        private void SaveToolStripButtonClick(object sender, EventArgs e)  { SaveFile(); }
        private void ToolStripButtonRunClick(object sender, EventArgs e)   { Run();      }
        private void ToolStripButtonPauseClick(object sender, EventArgs e) { Pause();    }
        private void ToolStripButtonStepClick(object sender, EventArgs e)  { Step();     }
        private void HelpToolStripButtonClick(object sender, EventArgs e)  { ShowHelp(); }

        #endregion

        #region Other Events

        #region Event: TimerEventProcessor

        /// <summary>
        /// Timer timeout method to generate next life generation and render it to our window
        /// </summary>
        /// <param name="myObject"></param>
        /// <param name="myEventArgs"></param>
        private static void TimerEventProcessor(Object myObject, EventArgs myEventArgs)
        {
            _timer.Stop();
            {
                _engine.NextGeneration();
                _thisForm.Invalidate();
                _thisToolStripStatusLabelGeneration.Text = @"Generation: " + _engine.Generation;
            }
            _timer.Start();
        }

        #endregion

        #region Event: LifeFormPaint

        /// <summary>
        /// Render the current Life generation to our window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LifeFormPaint(object sender, PaintEventArgs e)
        {
            for (int x = 0; x < _engine.Width; x++)
            {
                for (int y = 0; y < _engine.Height; y++)
                {
                    if (_engine.GetCell(x, y))
                        e.Graphics.DrawRectangle(_pen, x, y, 1, 1);
                }
            }
        }

        #endregion

        #endregion

        #region Private Methods

        #region Method: NewLife

        /// <summary>
        /// Stop any current life rendering, and clear out the pattern and window
        /// </summary>
        private void NewLife()
        {
            Pause();
            _engine.Clear();
            Invalidate();
            toolStripStatusLabelLifePattern.Text = @"No pattern loaded.";
            toolStripStatusLabelGeneration.Text = @"Generation:";
        }

        #endregion

        #region Method: LoadFile

        /// <summary>
        /// Load a Life file from a supported file type into the engine
        /// </summary>
        private void LoadFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog {Filter = @"RLE Files|*.rle"};

            Pause();

            try
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    _engine.Clear();
                    FileObject.Load(openFileDialog.FileName, _engine);
                    Invalidate();
                    toolStripStatusLabelLifePattern.Text = @"Pattern: " + Path.GetFileName(openFileDialog.FileName);
                    toolStripStatusLabelGeneration.Text = @"Generation: " + _engine.Generation;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(Properties.Settings.Default.FILE_READ_EXCEPTION + @" " + ex);
            }
        }

        #endregion

        #region Method: SaveFile

        /// <summary>
        /// Save the current Life state to a supported file type
        /// </summary>
        private static void SaveFile()
        {
            // TODO Implement the SaveFile method
            MessageBox.Show(@"Not implemented yet!", @"Notice");
        }

        #endregion

        #region Method: Exit

        /// <summary>
        /// Exit the application
        /// </summary>
        private void Exit()
        {
            Close();
        }

        #endregion

        #region Method: Run

        /// <summary>
        /// Run the current Life pattern
        /// </summary>
        private void Run()
        {
            Invalidate();
            _timer.Start();
        }

        #endregion

        #region Method: Pause

        /// <summary>
        /// Pause running of the current Life pattern
        /// </summary>
        private static void Pause()
        {
            _timer.Stop();
        }

        #endregion

        #region Method: Step

        /// <summary>
        /// Step one generation of the current Life pattern, pausing after the step
        /// </summary>
        private void Step()
        {
            Pause();
            _engine.NextGeneration();
            Invalidate();
            toolStripStatusLabelGeneration.Text = @"Generation: " + _engine.Generation;
        }

        #endregion

        #region Method: About

        /// <summary>
        /// Display the about box
        /// </summary>
        private static void About()
        {
            AboutBox aboutBox = new AboutBox();
            aboutBox.ShowDialog();
        }

        #endregion

        #region Method: ShowHelp

        /// <summary>
        /// Display help contents
        /// </summary>
        private static void ShowHelp()
        {
            // TODO Implement the SaveFile method
            MessageBox.Show(@"Not implemented yet! View the readme.txt file for now.", @"Notice");
        }

        #endregion

        #endregion
    }
}
