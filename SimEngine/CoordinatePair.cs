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

namespace SimEngine
{
    public struct CoordinatePair
    {
        public Point2D Start;
        public Point2D Stop;

        public CoordinatePair(Point2D start, Point2D stop)
        {
            Start = start;
            Stop = stop;
        }

        public CoordinatePair(int x1, int y1, int x2, int y2)
        {
            Start = new Point2D(x1, y1);
            Stop = new Point2D(x2, y2);
        }
    }
}