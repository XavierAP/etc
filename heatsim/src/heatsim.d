/*
Javier A. Porras
June 2009, March 2017
*/

@safe:

import std.exception: enforce;
import std.math: ceil, isFinite;

/**
Integrates by finite differences 1D heat conduction + convection
(or any equivalent problem), and serves the solution output.
All SI units, including temperatures in kelvins.

Params:

output = Function to receive the solution, step by step in time.
dtOut  = If larger than dt, how often to call output();
         otherwise no effect and output() called every dt instead.

T      = Size of time dimension, how long to integrate.
dt     = Integration step size in time dimension.
R      = Size of space dimension i.e. radius of the disk.
dr     = Step size in space dimension.

u0     = Initial uniform temperature.
uc     = Temperature of the edge r=R.
ug     = Temperature of the convective flow.

k      = 2D conductivity in W K^-1
a      = 2D diffusivity in m^2 s^-1
h      = Convective coefficient in W m^-2 K^-1
         relative to the frontal area of the disc.

Throws: if output() does.

Description:

Evolution of temperature on a permeable disk, subject to constant temperature
on its edge, and convection from a flow that traverses said disk.
Partial differential equation integrated by a finite difference method.

We model the disk in 2D and solve the heat diffusion equation in polar
(cylindrical) coordinates with axial symmetry, with a (convective) source term:

	1/r·d/dr(r·du/dr) + h/k·(ug-u) = 1/a·du/dt

Parameters are the effective ones for the 2D model, not to the 3D material,
for example conductivity is expressed in W/K instead of W/m·K. They must
account for the true 3D geometry of the disk as well as its material.
For further theoretical elaboration see:

J. Xu, R.A. Wirtz,
In-plane effective thermal conductivity of plain-weave screen laminates,
IEEE Transactions on Components and Packaging Technologies,
vol. 25, no. 4 (December 2002) pp. 615-620.
doi: 10.1109/TCAPT.2002.807993

Boundary conditions:
	du/dr = 0, for r=0, t>0
	u = uc,    for r=R, t>0
Initial condition:
	u = u0,    for t=0, 0<r<R
*/
export void solve(
	void delegate(real time, in real[] u) @trusted output,
	real dtOut, real T, real dt, real R, real dr,
	real u0, real uc, real ug,
	real k, real a, real h )
in {
	enforce(output, "No function passed to receive solution output.
No point in calculating it if you're not going to use it.");
	
	foreach(x; [dtOut, T, dt, R, dr, u0, uc, ug, k, a, h])
		enforce(isFinite(x) && x > 0,
			"Parameters must be strictly positive real numbers.");

	enforce(R >= dr, "Integration length must be larger than step size.");
	enforce(T >= dt, "Integration length must be larger than step size.");

	// If dtOut < dt we just ignore it.
}
body
{
	debug
	{
		import std.stdio: stderr;
		stderr.writef("=== Input:
dtOut = %s s
T  = %s s
dt = %s s
R  = %s m
dr = %s m
u0 = %s C
uc = %s C
ug = %s C
k  = %s W K^-1
a  = %s m^2 s^-1
h  = %s W m^-2 K^-1
\n", dtOut, T, dt, R, dr, u0, uc, ug, k, a, h);
	}
	// Number of steps along space dimension:
	auto nr1 = ceil(R/dr);
	assert(nr1 >= 1);
	enforce(nr1 <= size_t.max, "Too fine step size in relation to integration length: insufficient space.");
	auto nr = cast(size_t)nr1;
	assert(nr >= 1);

	// Field solution storage:
	auto u = new real[nr];
	// The latest time step is stored here and overwritten as integration proceeds.

	// Initial condition:
	foreach(j, Tj; u[0..$-1])
		u[j] = u0;
	// Boundary condition:
	u[$-1] = uc; // the other boundary condition is already implicit in the above foreach loop.
	output(0, u);

	// Integrate:
	auto u1 = new real[nr-2]; // need to store two time steps for the finite differences integration,
		// but not the end points r = 0, R, therefore indices are shifted wrt u for each r.
	real
		c1 = a * dt,
		c2 = 1. / dr,
		c3 = c2 / dr,
		c4 = h / k,
		c5 = 1./c1 - c3*2 - c4;
	
	if(dtOut < dt) dtOut = dt;
	real tOut = dtOut; // next time to send output instead of every step.

	for(real t = dt; t <= T; t += dt) // integrate along time.
	{
		for(size_t j = 1; j <= nr-2; ++j) // r from dr to R-dr.
		{
			real r = dr * j;
			assert(r > 0 && r < R);
			real cj = c2 / r;
			// Integration by finite differences approximation:
			u1[j-1] = c1 * // u1 is shifted, see declaration.
				( u[j-1] * c3
				+ u[j  ] * (c5-cj)
				+ u[j+1] * (c3+cj)
				+ ug * c4 );
			//debug { stderr.writefln("t=%s r=%s u=%s", t, r, u1[j-1]); }
		}

		// Prepare for the next step:
		u[1..$-1] = u1; // u1 is shifted, see declaration.
		// - Boundary conditions:
		u[0] = u[1];
		u[$-1] = uc;

		// Send output:
		if(t >= tOut)
		{
			output(t, u);
			tOut += dtOut;
		}
	}
}

import std.stdio;

/**
Prints in a row the values in `field[]`, preceded by `time`, into `stream`.
Throws: stdio
*/
void output(real time, in real[] field,
		File stream = stdout,
		string separator = "\t",
		string timeFormat  = "%7.3f",
		string fieldFormat = "%7.1f" )
{
	stream.writef(timeFormat ~ separator, time);
	foreach(uj; field) stream.writef(fieldFormat ~ separator, uj);
	stream.writeln();
}

/**
Gets the arguments for `solve()` from `stream`. If `stream` is the
console `stdin` (as by default), prints explanatory prompts.
Throws: stdio; or in case non numeric or blank input comes out of `stream`.
*/
@trusted
void getInput(
	out real dtOut, out real T, out real dt, out real R, out real dr,
	out real u0, out real uc, out real ug,
	out real k, out real a, out real h,
	File stream = stdin )
{
	enum params = [
		"Radius of disk in m",
		"Integration step size along radius",
		"Total integration time in s",
		"Integration step size along time",
		"Time step size for output",
		"Initial temperature in C",
		"Edge temperature in C",
		"Flow temperature in C",
		"2D conductivity in W K^-1",
		"2D diffusivity in m^2 s^-1",
		"Convective coefficient in W m^-2 K^-1" ];
	
	real*[params.length] vals = [
		&R, &dr, &T, &dt, &dtOut,
		&u0, &uc, &ug,
		&k, &a, &h ];

	import std.conv: to;
	import std.exception: enforce;
	import std.array: split;

	bool consoleIn = stream == stdin;
	foreach(i, param; params)
	{
		if(consoleIn) writeln(param ~ ':');
		auto words = stream.readln().split();
		enforce(words.length > 0, "Blank input for parameter:
" ~ param);
		auto val = words[0];
		assert(val.length > 0); // otherwise it would have been obviated by split().
		*vals[i] = to!real(val);
	}
}
