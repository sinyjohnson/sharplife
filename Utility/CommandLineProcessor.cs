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
using System.Collections.Generic;

namespace Utility
{
    #region Class: CommandLineProcessor

    /// <summary>
    /// Process a string array as a command line to an easy key, value Dictionary lookup
    /// 
    /// The CommandLineProcessor will handle command lines in the following formats:
    /// -command
    /// -command value
    /// -command = value
    /// -command=value
    /// 
    /// --command
    /// --command value
    /// --command = value
    /// --command=value
    /// 
    /// /command
    /// /command value
    /// /command = value
    /// /command=value
    /// </summary>
    public static class CommandLineProcessor
    {
        #region Fields

        /// <summary>
        /// Parsed command line key value storage
        /// </summary>
        private static Dictionary<string, string> _commandLine;

        #endregion

        #region Method: ProcessCommandLine

        /// <summary>
        /// Process the given string array as a command line
        /// All commands will be made lower case
        /// </summary>
        /// <param name="args"></param>
        public static void ProcessCommandLine(string[] args)
        {
            ProcessCommandLine(args, true);
        }

        /// <summary>
        /// Process a string array as a command line to an easy key, value Dictionary lookup
        /// </summary>
        /// <param name="args"></param>
        /// <param name="toLower"></param>
        public static void ProcessCommandLine(string[] args, bool toLower)
        {
            if (null == _commandLine)
                _commandLine = new Dictionary<string, string>();

            for (int idx = 0; idx < args.Length; idx++)
            {
                string sValue;
                string arg = args[idx];
                string arg2 = idx < args.Length - 1 ? args[idx + 1] : String.Empty;

                string sCommand = arg;
                if (toLower)
                    sCommand = sCommand.ToLower();

                // See if it is a command by looking at the possible command indicators of / - --
                if (sCommand[0] == '/' || sCommand[0] == '-')
                {
                    // Remove any - / and --
                    if (sCommand[0] == '-')
                    {
                        sCommand = sCommand.Remove(0, 1);
                        if (sCommand[0] == '-')
                            sCommand = sCommand.Remove(0, 1);
                    }
                    else if (sCommand[0] == '/')
                        sCommand = sCommand.Remove(0, 1);
                }

                // Check composit command=value
                if (sCommand.IndexOf('=') != -1)
                {
                    sValue = sCommand.Substring(sCommand.IndexOf('=') + 1, sCommand.Length - sCommand.IndexOf('=') - 1);
                    sCommand = sCommand.Substring(0, sCommand.IndexOf('='));
                }
                else if (arg2 == "=")
                {
                    // command = value
                    if (idx < args.Length - 1)
                    {
                        arg2 = args[idx + 2];
                        sValue = arg2;
                        idx++;
                        idx++;
                    }
                    else
                        throw new InvalidOperationException(String.Format("There is no value to be associated with the command: {0}", arg));
                }
                else
                {
                    // Command value, where there is a possible value, and this value is not another command
                    if (arg2.Length > 0 && arg2[0] != '/' && arg2[0] != '-')
                    {
                        sValue = arg2;
                        idx++;
                    }
                    else
                    {
                        // Command, stand alone command no value
                        sValue = String.Empty;
                    }
                }

                _commandLine.Add(sCommand, sValue);
            }
        }

        #endregion

        #region Method: ParameterExists

        /// <summary>
        /// Returns true if the given command line parameter exists
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static bool ParameterExits(string parameter)
        {
            return _commandLine.ContainsKey(parameter);
        }

        #endregion

        #region Method: Value

        /// <summary>
        /// Returns the value for the given command line parameter
        /// If the parameter does not exist an empty string is returned
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public static string Value(string parameter)
        {
            string value;

            _commandLine.TryGetValue(parameter, out value);

            return value;
        }

        #endregion
    }

    #endregion
}
