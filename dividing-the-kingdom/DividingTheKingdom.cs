using System.Diagnostics;

/// <summary>Requirement: given two arrays of K integers X and Y, denoting coordinates of boroughs containing the gold mines, compute the number of fair divisions of the kingdom.</summary>
/// <remarks>Task description:
/// An old king wants to divide his kingdom between his two sons. He is well known for his justness and wisdom, and he plans to make a good use of both of these attributes while dividing his kingdom.
/// The kingdom is administratively split into square boroughs that form an N × M grid. Some of the boroughs contain gold mines. The king knows that his sons do not care as much about the land as they do about gold, so he wants both parts of the kingdom to contain exactly the same number of mines. Moreover, he wants to split the kingdom with either a horizontal or a vertical line that goes along the borders of the boroughs (splitting no borough into two parts).</remarks>
static class DividingTheKingdom
{
	/// <summary>Tests.</summary>
	private static void
	Main()
	{
		Debug.Assert(3 == solve(5, 5, new[]{0, 4, 2, 0}, new[]{0, 0, 1, 4}));
	}

	/// <summary>Returns the number of fair partitions.</summary>
	public static int
	solve(int N, int M, int[] X, int[] Y)
	{
		int nmines = X.Length;
		Debug.Assert(nmines == Y.Length);
		if(nmines == 0 || nmines % 2 != 0) return 0;
		nmines /= 2; // fair amount for each heir.
		
		return
			divMines(X, N, nmines) +
			divMines(Y, M, nmines) ;
	}

	/// <summary>Scans one of the coordinates and returns the
	/// number of fair divisions possible along it.</summary>
	private static int
	divMines(int[] coords, int width, int countHalf)
	{
		int
			countFair = 0, // number of fair divisions found 
			countMines = 0; // number of mines found.
		
		/* Prepare a map of the distribution of mines with
		 * respect to this (X/Y) coordinate. This is in order
		 * to avoid iterating more than needed O(n^2). */
		var map = new int[width]; // initialized with zeros.
		foreach(int x in coords)
			++map[x];
		
		// Count the number of fair divisions:
		foreach(int loc in map)
		{
			countMines += loc;
			if(countMines > countHalf)
				break; // no more hope to find fair divisions.
			else if(countMines == countHalf)
				++countFair;
		}
		
		return countFair;
	}
}