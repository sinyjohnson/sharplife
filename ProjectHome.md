# Sharp Life #
**Sharp Life** is a C# Life Simulator based on the rules of Conway's Game of Life.
An overview of Life can be found here: [Conway's Game of Life](http://en.wikipedia.org/wiki/Conway's_Game_of_Life)

Sharp Life, in it's current form, is to try different techniques for the game of life with an emphasis on optimizations.

In it's current state, Sharp Life is very basic and has a ways to go, See [RoadMap](RoadMap.md), but has been designed to enable different rendering clients the ability to use several different life engines.

The solution/projects are created with Visual Studio 2010 targeting .NET 3.5 and should be able to be used with Developer Express 2008 and 2010 as well as Sharp Develop on Windows. Use on Linux and Mac may be possible but has not been tried yet. Two areas that may be a problem on Linux/Mac would be the High Performance Timer and possibly the System.Form and related windowing controls.