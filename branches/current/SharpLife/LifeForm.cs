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
using System.Windows.Forms;
using SimEngine;

namespace SharpLife
{
    public partial class LifeForm : Form
    {
        #region Fields

        private static LifeEngine _engine;
        private static Panel _lifeWindow;
        private static readonly Timer _timer = new Timer();
        readonly Pen _pen = new Pen(Color.Red);
        private bool _mouseDown = false;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public LifeForm()
        {
            InitializeComponent();

            _lifeWindow = lifeWindow;
            _engine = CreateEngine(EngineType.Engine1, lifeWindow.Width, lifeWindow.Height);
            _timer.Tick += TimerEventProcessor;
            _timer.Interval = 60;
        }

        #endregion

        #region Method: CreateEngine

        /// <summary>
        /// Create the given Life Engine Type
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

        #region Menu Item Events

        private void NewToolStripMenuItemClick(object sender, EventArgs e)
        {
            NewLife();
        }

        private void OpenToolStripMenuItemClick(object sender, EventArgs e)
        {
            OpenFile();
        }

        private void SaveToolStripMenuItemClick(object sender, EventArgs e)
        {
            SaveFile();
        }

        private void SaveAsToolStripMenuItemClick(object sender, EventArgs e)
        {
            SaveFile();
        }

        private void ExitToolStripMenuItemClick(object sender, EventArgs e)
        {
            Exit();
        }

        private void RunToolStripMenuItemClick(object sender, EventArgs e)
        {
            Run();
        }

        private void PauseToolStripMenuItemClick(object sender, EventArgs e)
        {
            Pause();
        }

        private void StepToolStripMenuItemClick(object sender, EventArgs e)
        {
            Step();
        }

        #endregion

        #region Toolbar Events

        private void NewToolStripButtonClick(object sender, EventArgs e)
        {
            NewLife();
        }

        private void OpenToolStripButtonClick(object sender, EventArgs e)
        {
            OpenFile();
        }

        private void SaveToolStripButtonClick(object sender, EventArgs e)
        {
            SaveFile();
        }

        private void ToolStripButtonRunClick(object sender, EventArgs e)
        {
            Run();
        }

        private void ToolStripButtonPauseClick(object sender, EventArgs e)
        {
            Pause();
        }

        private void ToolStripButtonStepClick(object sender, EventArgs e)
        {
            Step();
        }

        #endregion

        #region Other Events

        private static void TimerEventProcessor(Object myObject, EventArgs myEventArgs)
        {
            _timer.Stop();
            {
                _engine.NextGeneration();
                _lifeWindow.Invalidate();
            }
            _timer.Start();
        }

        private void LifeWindowPaint(object sender, PaintEventArgs e)
        {
            Bitmap offScreenBmp = new Bitmap(Width, Height);
            Graphics offScreenDc = Graphics.FromImage(offScreenBmp);

            for (int x = 0; x < _engine.Width; x++)
            {
                for (int y = 0; y < _engine.Height; y++)
                {
                    if (_engine.GetCell(x, y))
                        offScreenDc.DrawRectangle(_pen, x, y, 1, 1);
                }
            }

            // TODO Test with hi performance timer
            //e.Graphics.DrawImage(offScreenBmp, 0, 0);
            e.Graphics.DrawImageUnscaled(offScreenBmp, 0, 0);
        }

        private void LifeWindowMouseDown(object sender, MouseEventArgs e)
        {
            _mouseDown = true;
        }

        private void LifeWindowMouseUp(object sender, MouseEventArgs e)
        {
            _mouseDown = false;
        }

        private void LifeWindowMouseMove(object sender, MouseEventArgs e)
        {

        }

        #endregion

        #region Private Methods

        private static void NewLife()
        {
            throw new NotImplementedException();
        }

        private static void OpenFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog {Filter = @"RLE Files|*.rle"};

            try
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                    FileObject.Load(openFileDialog.FileName, _engine);
            }
            catch (Exception ex)
            {
                MessageBox.Show(Properties.Settings.Default.FILE_READ_EXCEPTION + @" " + ex);
            }
        }

        private static void SaveFile()
        {
            throw new NotImplementedException();
        }

        private void Exit()
        {
            Close();
        }

        private void Run()
        {
            lifeWindow.Invalidate();
            _timer.Start();
        }

        private static void Pause()
        {
            _timer.Stop();
        }

        private static void Step()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
