using System;

namespace JP.CryptoClass
{
	static class WeakPRG
	{
		const long N = 295075153L;

		static void Main(string[] args)
		{
			var outputs = new long[]
			{
				210205973L,
				22795300L,
				58776750L,
				121262470L,
				264731963L,
				140842553L,
				242590528L,
				195244728L,
				86752752L
			};
			long x, y;
			int
				i,
				n = outputs.Length;
			for(long x0 = 0L; x0 < N; ++x0)
			{
				x = x0;
				y = x ^ outputs[0];

				for(i = 1; i < n; ++i)
				{
					Iterate(ref x, ref y);
					if((x ^ y) != outputs[i])
						break;
				}
				if(i >= n) // for loop did complete.
				{
					Iterate(ref x, ref y);
					Console.WriteLine(x ^ y);
					Console.ReadKey(true);
					return;
				}
			}
			Console.WriteLine("Failed!");
			Console.ReadKey(true);
		}

		static void Iterate(ref long x, ref long y)
		{
			x = (2L * x + 5L) % N;
			y = (3L * y + 7L) % N;
		}
	}
}
