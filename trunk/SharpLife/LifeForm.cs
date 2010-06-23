using System;
using System.Drawing;
using System.Windows.Forms;
using SimEngine;
using Timer = System.Windows.Forms.Timer;

namespace Life
{
    public partial class LifeForm : Form
    {
        private static LifeEngine _engine;
        private static readonly Timer _timer = new Timer();
        readonly Pen _pen = new Pen(Color.Red);
        private bool _mouseDown = false;

        public LifeForm()
        {
            InitializeComponent();

            _engine = CreateEngine(EngineType.Engine1, lifeWindow.Width, lifeWindow.Height);

            #region Random

            Random rnd = new Random((int)DateTime.Now.Ticks);
            for (int i = 0; i < 30000; i++)
                _engine.SetCell(rnd.Next(1, lifeWindow.Width-1), rnd.Next(1, lifeWindow.Height-1), true);

            #endregion

            #region One cell high broken line supposed to exhibt infinte growth

            //int x = 5;
            //for (int y = 10; y < 300; y += 20)
            //{
            //    _engine.SetCell(x, y, true);
            //    _engine.SetCell(x + 1, y, true);
            //    _engine.SetCell(x + 2, y, true);
            //    _engine.SetCell(x + 3, y, true);
            //    _engine.SetCell(x + 4, y, true);
            //    _engine.SetCell(x + 5, y, true);
            //    _engine.SetCell(x + 6, y, true);
            //    _engine.SetCell(x + 7, y, true);

            //    _engine.SetCell(x + 9, y, true);
            //    _engine.SetCell(x + 10, y, true);
            //    _engine.SetCell(x + 11, y, true);
            //    _engine.SetCell(x + 12, y, true);
            //    _engine.SetCell(x + 13, y, true);

            //    _engine.SetCell(x + 17, y, true);
            //    _engine.SetCell(x + 18, y, true);
            //    _engine.SetCell(x + 19, y, true);

            //    _engine.SetCell(x + 26, y, true);
            //    _engine.SetCell(x + 27, y, true);
            //    _engine.SetCell(x + 28, y, true);
            //    _engine.SetCell(x + 29, y, true);
            //    _engine.SetCell(x + 30, y, true);
            //    _engine.SetCell(x + 31, y, true);
            //    _engine.SetCell(x + 32, y, true);

            //    _engine.SetCell(x + 34, y, true);
            //    _engine.SetCell(x + 35, y, true);
            //    _engine.SetCell(x + 36, y, true);
            //    _engine.SetCell(x + 37, y, true);
            //    _engine.SetCell(x + 38, y, true);

            //    x += 20;
            //}

            #endregion

            lifeWindow.Invalidate();

            _timer.Tick += TimerEventProcessor;
            _timer.Interval = 60;
            _timer.Start();
        }

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

        private static void TimerEventProcessor(Object myObject, EventArgs myEventArgs)
        {
            _timer.Stop();
            {
                _engine.NextGeneration();
                lifeWindow.Invalidate();
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
    }
}
