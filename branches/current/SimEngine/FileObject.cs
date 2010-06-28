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
using System.IO;

namespace SimEngine
{
    #region Enum: FileType

    /// <summary>
    /// Life supported file format types
    /// </summary>
    public enum FileType
    {
        Rle,
        Erle,
    }

    #endregion

    #region Class: FileObject

    /// <summary>
    /// Life file formats access class.
    /// This class allows for generic file loading and saving
    /// </summary>
    public static class FileObject
    {
        #region Method: Load

        /// <summary>
        /// Load the given file into the engine
        /// </summary>
        /// <param name="file"></param>
        /// <param name="engine"></param>
        public static void Load(string file, LifeEngine engine)
        {
            // TODO Add logic to determine the file format and use the appropriate reader/writer class

            if (Path.GetExtension(file).ToLower() != ".rle")
                throw new Exception("Unsupported file format");

            // Since we are currently supporting only RLE, we can use ERLE to load both types
            ERleReaderWriter eRleReaderWriter = new ERleReaderWriter();
            eRleReaderWriter.Read(file, engine);
        }

        #endregion

        #region Method: Save

        /// <summary>
        /// Save the current board to default file type of Extended RLE
        /// </summary>
        /// <param name="file"></param>
        /// <param name="engine"></param>
        public static void Save(string file, LifeEngine engine)
        {
            Save(file, engine, FileType.Erle);
        }

        /// <summary>
        /// Save the current board to the given file type
        /// </summary>
        /// <param name="file"></param>
        /// <param name="engine"></param>
        /// <param name="type"></param>
        public static void Save(string file, LifeEngine engine, FileType type)
        {
            switch (type)
            {
                case FileType.Rle:
                case FileType.Erle:
                    ERleReaderWriter eRleReaderWriter = new ERleReaderWriter();
                    eRleReaderWriter.Write(file, engine);
                    break;
            }
        }

        #endregion
    }

    #endregion

    #region Class: ERleReaderWriter

    /// <summary>
    /// Read and Write an RLE or Extended RLE life file format
    /// http://psoup.math.wisc.edu/mcell/ca_files_formats.html#RLE
    /// </summary>
    internal class ERleReaderWriter
    {
        #region Fields

        private int _width;
        private int _height;
        private int _centerX;
        private int _centerY;
        private int _startX;
        private int _startY;

        #endregion

        #region Method: Read

        /// <summary>
        /// Read an RLE or Extended RLE life file format
        /// 
        /// If the data being read is found not to be an RLE/ERLE file, an exception is thrown.
        /// An exception will also be thrown if the pattern size being read is larger than the
        /// given engines max width and height
        /// 
        /// Example RLE file:
        /// #C Creator: Lifestat application
        /// #C Author : Andrew Okrasinski
        /// #C Dies out completely after 658 gens
        /// x = 20, y = 20, rule = B3/S23
        /// 6bo3bo4bo3bo$bo3bobob3o2bo$obo8bobo$2bo3bo2bo6bo$b2o2bo7bo4b2o
        /// $b2o5b2o2bo2b2ob2o$b2o6bo2bo3b2o$2bo2bob2o5bobo$3b2o2b2obo6bo
        /// $bo4b2o2bobo5bo$2bo3bobo$bobo3b2ob3o3bo$3bo3bo2b2o2bo$2bo4bo3b
        /// o2bo$3bo2bo2bobobob2o2bo$o2bo15bo$b2o4b2obo2bo3b3o$4bo4bo2b4o
        /// 2bo$bo4bobo2bo5b2o$o2bo6bo2bo!
        /// </summary>
        /// <param name="file"></param>
        /// <param name="engine"></param>
        public void Read(string file, LifeEngine engine)
        {
            if (engine.Width == 0 || engine.Height == 0)
                throw new Exception(Properties.Settings.Default.INVALID_SIZE);

            _width = engine.Width;
            _height = engine.Height;
            _centerX = engine.Width/2;
            _centerY = engine.Height/2;

            // First pass to read the boards size and type
            // If the file does not exist, StreamReader throws and is expected to do so
            // Processes comment lines and a Coordinate/Rule line for character case of #, x, and X
            using (StreamReader sr = new StreamReader(file))
            {
                String line;

                while ((line = sr.ReadLine()) != null)
                {
                    if (!String.IsNullOrEmpty(line))
                    {
                        switch (line[0])
                        {
                            case '#':
                                ProcessComment(line);
                                break;
                            case 'x':
                            case 'X':
                                ProcessCoordinateRuleLine(line);
                                break;
                        }
                    }
                }
            }

            // Second pass to process the line as an RLE - Run Length Encoded line and sets cells in the given engine
            using (StreamReader sr = new StreamReader(file))
            {
                // At this point, the board size is read in from the file and we have out board centered
                int x = _startX;
                int y = _startY;
                String line;

                while ((line = sr.ReadLine()) != null)
                {
                    if (!String.IsNullOrEmpty(line) && line[0] != '#' && line[0] != 'x' && line[0] != 'X')
                    {
                        ProcessRleData(line, ref x, ref y, engine);
                    }
                }
            }
        }

        #endregion

        #region Method: Write

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="engine"></param>
        public void Write(string file, LifeEngine engine)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Method: ProcessComment

        /// <summary>
        /// Currently only looking for two types of comments
        /// 
        /// #R - gives the coordinates of the top left-hand corner of the pattern.
        /// RLE files produced by XLife usually have this line, and the coordinates 
        /// are usually negative with the intention of placing the centre of the 
        /// pattern at the origin.
        /// Example: #R -22 -57
        /// 
        /// #r - gives the rule for a totalistic cellular automaton in the form 
        /// survival_counts/birth_counts (e.g. 23/3 for Life). This line is 
        /// usually present in RLE files created by XLife.
        /// Example: #r 23/3
        /// </summary>
        /// <param name="line"></param>
        private void ProcessComment(string line)
        {
            string subLine;

            if (line.Length < 6)
                return;

            if (line[1] == 'R')
            {
                subLine = line.Substring(2);
                string[] parsed = subLine.Split(new[] {' '});

                try
                {
                    int x = Math.Abs(Convert.ToInt32(parsed[0]));
                    int y = Math.Abs(Convert.ToInt32(parsed[1]));

                    if (_centerX - x >= 0)
                        _startX = _centerX - x;
                    else
                        _startX = 0;

                    if (_centerY - y >= 0)
                        _startY = _centerY - y;
                    else
                        _startY = 0;
                }
                catch (Exception)
                {
                    _startX = 0;
                    _startY = 0;
                }
            }

            if (line[1] == 'r')
            {
                subLine = line.Substring(2);
                if (subLine != "23/3")
                    throw new Exception(Properties.Settings.Default.UNSUPPORTED_TYPE);
            }
        }

        #endregion

        #region Method: ProcessCoordinateRuleLine

        /// <summary>
        /// Coordinates and rule line can exist outside of an #R and #r comment
        /// Line contains maximum width and height of the pattern, and the rule
        /// Example:
        /// x = 20, y = 20, rule = B3/S23
        /// </summary>
        /// <param name="line"></param>
        private void ProcessCoordinateRuleLine(string line)
        {
            int x = 0;
            int y = 0;
            string rule = String.Empty;
            string[] parsed = line.ToLower().Split(new[] {' ', '=', ','}, StringSplitOptions.RemoveEmptyEntries);

            for (int idx=0; idx < parsed.Length; idx++)
            {
                switch (parsed[idx])
                {
                    case "x":
                        x = Convert.ToInt32(parsed[idx + 1]);
                        break;
                    case "y":
                        y = Convert.ToInt32(parsed[idx + 1]);
                        break;
                    case "rule":
                        rule = parsed[idx + 1];
                        break;
                }
            }

            if (!String.IsNullOrEmpty(rule))
            {
                if (rule.ToLower() != "b3/s23")
                    throw new Exception(Properties.Settings.Default.UNSUPPORTED_TYPE);
            }

            if (x == 0 || y == 0)
                throw new Exception(Properties.Settings.Default.NO_COORDINATES);

            if (x > _width - 1 || y > _height - 1)
                throw new Exception(Properties.Settings.Default.COORDINATES_TOO_BIG);

            // Center pattern on the board
            _startX = _centerX - x / 2;
            _startY = _centerY - y / 2;
        }

        #endregion

        #region Method: ProcessRleData

        /// <summary>
        /// Process a line of RLE encoded data
        /// 
        /// Any line that is not blank, or does not start with a "#" or "x " or "x=" is treated as run-length encoded
	    /// pattern data. The data is ordered a row at a time from top to bottom, and each row is ordered left to right.
	    /// A "$" represents the end of each row and an optional "!" represents the end of the pattern.
	    /// Note that a $ end of row can occur anywhere within a line of RLE text
        /// 
        /// N - Any number, represents a run of what comes next, i.e a run of empty cells, or a run of live cells
        /// b - A blank cell
        /// o - A live cell
        /// $ - End of row
        /// ! - End of pattern
        /// 
        /// Example RLE data:
        /// 6bo3bo4bo3bo$bo3bobob3o2bo$obo8bobo$2bo3bo2bo6bo$b2o2bo7bo4b2o
        /// $b2o5b2o2bo2b2ob2o$b2o6bo2bo3b2o$2bo2bob2o5bobo$3b2o2b2obo6bo
        /// $bo4b2o2bobo5bo$2bo3bobo$bobo3b2ob3o3bo$3bo3bo2b2o2bo$2bo4bo3b
        /// o2bo$3bo2bo2bobobob2o2bo$o2bo15bo$b2o4b2obo2bo3b3o$4bo4bo2b4o
        /// 2bo$bo4bobo2bo5b2o$o2bo6bo2bo!
        /// </summary>
        /// <param name="line"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="engine"></param>
        private void ProcessRleData(string line, ref int x, ref int y, LifeEngine engine)
        {
            // I walk the line
            for (int idx=0; idx < line.Length; idx++)
            {
                switch (line[idx])
                {
                    case 'b':
                        x++;
                        break;
                    case 'o':
                        engine.SetCell(x, y, true);
                        x++;
                        break;
                    case '$':
                        x = _startX;
                        y++;
                        break;
                    case '!':
                        return;
                    default:
                        // Convert number, do run of next character
                        int numberOfDigits;
                        int n = Utility.StringMethods.StringToInteger(line.Substring(idx), out numberOfDigits);

                        if (n < 1)
                            throw new Exception(Properties.Settings.Default.INVALID_ROW);

                        char type = line[idx + numberOfDigits];
                        switch (type)
                        {
                            case 'b':
                                x += n;
                                break;

                            case 'o':
                                for (int i = 0; i < n; i++)
                                {
                                    if (type == 'o')
                                        engine.SetCell(x, y, true);

                                    x++;
                                }
                                break;

                            case '$':
                                x = _startX;
                                y += n;
                                break;
                        }

                        // Skip the RLE b/o/$ run we just processed
                        idx += numberOfDigits;
                        break;
                }
            }
        }

        #endregion
    }

    #endregion
}