/*
Javier A. Porras
June 2009 - March 2017
*/

/*
Everything in SI units, except temperatures in Celsius
which makes no difference.

Output and input are either in text files passed as arguments
-- first output then input -- or the console -- by default
or passing "." instead of a file pathname.

The format expected in the input file is:
11 lines, led with a number, optionally followed by
whitespace and any text, which is ignored.
Example:

0.0127   Radius of disk in m
0.0001   Integration step size along radius
4        Total integration time in s
0.01     Integration step size along time
0.1      Time step size for output
20       Initial temperature in C
200      Edge temperature in C
100      Flow temperature in C
1e-3     2D conductivity in W K^-1
2e-6     2D diffusivity in m^2 s^-1
2e3      Convective coefficient in W m^-2 K^-1

*/
void main(string[] argv)
{
	import heatsim;
	import std.stdio;

	try // good to crash all the same but at least print only msg, no messy trace info pointless for the user in this case.
	{
		File fileIn = argv.length < 3 || argv[2] == "."
			? stdin  : File(argv[2], "r") ;

		File fileOut = argv.length < 2 || argv[1] == "."
			? stdout : File(argv[1], "w") ;

		real
			dtOut, T, dt, R, dr, u0, uc, ug, k, a, h;
		getInput(
			dtOut, T, dt, R, dr, u0, uc, ug, k, a, h, fileIn );

		solve( (t, u) => output(t, u, fileOut),
			dtOut, T, dt, R, dr, u0, uc, ug, k, a, h );

		debug(peek)
		{
			if(fileOut != stdout) // plot or open output file to inspect.
			{
				fileOut.close(); // needed early for other process to open the same file.
				// Check if Octave is available:
				import std.process;
				writeln("Checking if Octave is installed and available in PATH... ");
				if(0 != executeShell("octave -h").status)
				{
					writeln("Not found, attempting to open output file in editor.");
					spawnShell(fileOut.name);
				}
				else
				{
					// Generate Octave script to plot file:
					enum fileScriptName = "plotFileOut.m";
					writefln("Generating Octave code %s ... ", fileScriptName);
					auto fileScript = File(fileScriptName, "w");
					fileScript.writef(
"disp('Plotting... ');
data = load('%s');
%% A surface plot is a good way to study a 1D PDE solution.
figure;
surf(linspace(0, %s, size(data,2)-1), data(:,1), data(:,2:end));
xlabel('Radius (m)');
ylabel('Time (s)');
zlabel('Temperature (C)');
disp('Press any key to close figure... ');
pause;
", fileOut.name, R );

					fileScript.close();
					writeln("Running Octave... ");
					spawnShell("octave --no-gui " ~ fileScriptName);
				}
			}
		}
	}
	catch(Exception err)
	{
		writeln(typeid(err), "! ", err.msg); // don't print trace info.
	}
	writeln();
}
