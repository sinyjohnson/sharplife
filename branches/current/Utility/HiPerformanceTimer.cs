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

using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;

namespace Utility
{
    #region Class: HiPerformanceTimer

    public class HiPerformanceTimer
    {
        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceCounter(
            out long lpPerformanceCount);

        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(
            out long lpFrequency);

        private long _startTime, _stopTime;
        private readonly long _freq;

        // Constructor
        public HiPerformanceTimer()
        {
            _startTime = 0;
            _stopTime = 0;
#if (WINDOWS)
            if (QueryPerformanceFrequency(out _freq) == false)
            {
                // high-performance counter not supported

                throw new Win32Exception();
            }
#endif
        }

        // Start the timer
        public void Start()
        {
            // Let the waiting threads work
            Thread.Sleep(0);
#if (WINDOWS)
            QueryPerformanceCounter(out _startTime);
#endif
        }

        // Stop the timer
        public void Stop()
        {
            QueryPerformanceCounter(out _stopTime);
        }

        // Returns the duration of the timer (in seconds)
        public double Duration
        {
            get
            {
#if (WINDOWS)
                return (double)(_stopTime - _startTime) / _freq;
#else
                return 1;
#endif
            }
        }
    }

    #endregion Class: HiPerformanceTimer
}