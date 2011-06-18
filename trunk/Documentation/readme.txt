Life Simulator

The Game of Life, also known simply as Life, is a cellular automaton devised 
by the British mathematician John Horton Conway in 1970. It is the best-known 
example of a cellular automaton.
The "game" is a zero-player game, meaning that its evolution is determined by 
its initial state, requiring no further input from humans. One interacts with 
the Game of Life by creating an initial configuration and observing how it evolves.

The Life Simulator runs the Conway's Game Of Life.
See http://en.wikipedia.org/wiki/Conway's_Game_of_Life

Additional resources:
Golly Life Application		http://golly.sourceforge.net/
Life Wiki					http://www.conwaylife.com/wiki/index.php?title=Main_Page

This simulator implements what has been defined as Rule B3/S23, which is the classic life algorithm rules.
Defined at: http://en.wikipedia.org/wiki/Conway's_Game_of_Life#Variations_on_Life

 The Life field for the engines are currently defined where the left and right edges of the field 
 are stitched together, and the top and bottom edges are also, yielding a toroidal http://en.wikipedia.org/wiki/Torus
 array. The result is that active areas that move across a field edge reappear at the opposite edge.

This Microsoft Visual Studio 2010 solution consists of the following projects.

	SharpLife
	
		The Life project is a C# Windows Forms Application.

		This application currently has simplified operations.
		
			File - New		Resets and clears any loaded RLE file.
			File - Open		Open an RLE based life file.
			File - Save		Currently not implemented
			File - Save As	Currently not implemented.
			
			Life - Run		Begin running a currently loaded life RLE file.
			Life - Pause	Pause running of a currently loaded life RLE file.
			Life - Step		Step once with the currently loaded life RLE file.
			
			Help - About	Display the about dialog

	SharpLifeConsole
	
		The LifeConsole project is a C# Windows Console Application.
		This project has had the most work done with regard to features
		and optimizations and has been used exclusively to develop and 
		test the SimEngine project that contains the Life logic.
		
		Usage:
			SharpLifeConsole [-engine engine name] [-width n] [-height n] [-mode p|r] -file file name [-help/?]
			-engine Optional EngineType, Engine1 ...
			-width  Optional number, if number is greater than the maximum console size, then maximum console size is used
			-height Optional number, if number is greater than the maximum console size, then maximum console size is used
			-mode   Optional p=paused r=running
			-file   Required file name of a supported life file pattern
			-help/? Optional Display usage
		
		The Life Console, when first ran, starts in "Step" mode and will
		be stopped at the first generation of life. Press "S" to step to
		the next Life generation.
		
		Press the "H" key to display help, and press it again to exit help.
		The help will display the following:
		
		P Pause
        M Toggle Step Mode
        S Step
        H Help
        X Exit

	SpeedTestConsole
	
		This console application is used to test the speed of the various engines.
	
		Usage: SpeedTestConsole -engine [engine name]
			-engine  Optional engine name. If none given, then Engine1 is used.
				Engine names are in the format EngineN where N is an integer.
				If a given engine number does not exist, the application exists.

	SimEngine

		The SimEngine is a C# Assembly Library / DLL that contains the Life
		Simulator logic. The SimEngine contains three different engine 
		implementations, but only the first engine is currently used. The
		other two engines are experiments in optimizations and are not
		complete.
		
		All three engines derive from an abstract base engine class. An
		abstract class allows for the definition of abstract methods that
		any deriving class must implement. This allows us to use a common
		method of accessing all engines without having to make code changes
		outside of the engine.
		
		Some information on the engines
		
			Engine1
			
				This engine uses two 2 dimensional (2D) integer arrays to represent
				the life game board. This allows for the Life generation algorithm
				to determine life, death and birth around a cell by simply looking
				at the X and Y coordinates around it.
				
				This engine also uses a wrap around logic, where the top, bottom and side
				cells look back or forward across the game board.
			
			Engine2
			
				This Engine uses a 1 dimensional character array. Using this the engine
				must find neighbors of a cell by a different method than in a 2 dimensional
				array. Details of this are in the code method CountNeighbors.
				
				An anticipated optimization is to be able to draw the board simply by
				writing the game board directly to the console with one call, versus
				the method in Engine1 where each cell must be traversed and converted
				into a character and added to a string.
			
			Engine3
			
				This engine uses class objects representing each cell. This engine is
				the furthest from being complete. The idea is to have each cell able to
				access its neighbor easily by having a reference to each neighbor available
				in the cell, and simply asking the neighbor if it is alive, counting each
				one that is alive and based on the total, becoming alive or dead.
		
			Engine4
			
				Engine uses a 2 Dimensional int array and Scan List
				The Life field wraps around at the sides, tops and corners, i.e a toroidal array

				The scan list is a List of start and end x,y coordinates indicating what areas of the
				Life field have activity to scan, effectively skipping areas that have no possibility of
				life/death.

				Engine4 is currently the fastest implementation.
				Engine4 has not yet had the scan list implemented.

			Engine5

				Is the same as Engine4 but uses .NET Parallel Task Library's Parallel.For on the outer
				x loop traversal in the NextGeneration() method.

				It is currently slightly faster than Engine4

	Utility
	
		.NET Assembly with utility classes used for common things across the projects

	Installer
	
		Nullsoft Install System
		An NSIS based installer script is included.
		Using version 2.46
		http://nsis.sourceforge.net
