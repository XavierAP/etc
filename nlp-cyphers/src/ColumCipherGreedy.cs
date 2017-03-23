using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace JP.AIclass
{
	using RightOrLeft = Boolean; // for readability--see ColumCipher.Right and Left

	/// <summary>Represents a <see cref="ColumCipher"/> that is solved by a greedy algorithm.</summary>
	sealed class ColumCipherGreedy :ColumCipher
	{
		public ColumCipherGreedy(string[,] linesByCols, WordDictionaryByLength wordDictionary)
			:base(linesByCols, wordDictionary) { }

		/// <summary>Initializes the infrastructure, and picks the column with most letters
		/// to start the algorithm with, in order to improve our chances hopefully.</summary>
		private void Initialize()
		{
			// Pick the column with most letters:
			var seed = new Tuple<string[], int>(null, -1);
			foreach(var col in Unsorted)
			{
				int n = 0;
				for(int i = 0; i < LineCount; ++i)
					foreach(var c in col[i])
						if(char.IsLetter(c))
							++n;
				if(n > seed.Item2)
					seed = new Tuple<string[], int>(col, n);
			}

			// Initialize infrastructure:
			Lines = new StringBuilder[LineCount];
			LeftBits  = new string[LineCount];
			RightBits = new string[LineCount];
			for(int i = 0; i < LineCount; ++i)
				Lines[i] = new StringBuilder(LineLengths[i]);

			// For the first call to Print():
			Console.CursorTop += LineCount;
			// Initialize the hypothesis with the chosen column:
			Commit(seed.Item1, Right);
		}

		/// <summary>Adds the column (right or left) that resuilts in the best word matches
		/// repeatedly until there are no unsorted columns left..</summary>
		protected override void Solve()
		{
			Initialize();
			int score;
			while(Unsorted.Count() > 0) 
			{
				var best = new Tuple<string[], RightOrLeft, int>(null, Right, int.MinValue); // best match so far
				foreach(var side in BothSides)
					foreach(var col in Unsorted)
						if((score = GetScore(col, side)) > best.Item3)
							best = new Tuple<string[], RightOrLeft, int>(col, side, score);
				Commit(best.Item1, best.Item2);
			}
		}

		/// <summary>Stores the growing hypothesis during the run of the algorithm.</summary>
		private StringBuilder[] Lines;

		/// <summary>Exludes the words in the center of the solution from dictionary checks so they're faster.</summary>
		private string[] LeftBits, RightBits;

		/// <summary>Calculates a score that would be gained by a possible addition.</summary>
		/// <param name="column"></param>
		/// <param name="side"></param>
		/// <returns></returns>
		private int GetScore(string[] column, RightOrLeft side)
		{
			var trial = new string[LineCount];
			switch(side)
			{
			case Right:
				for(int i = 0; i < LineCount; ++i)
					trial[i] = char.IsLetter(column[i].FirstOrDefault()) ?
						RightBits[i] + column[i] :
						column[i] ;
				break;
			case Left:
				for(int i = 0; i < LineCount; ++i)
					trial[i] = char.IsLetter(column[i].LastOrDefault()) ?
						column[i] + LeftBits[i] :
						column[i] ;
				break;
			}
			int score = 0;
			foreach(var line in trial)
				foreach(var fragment in line.Split(NonLetters, StringSplitOptions.RemoveEmptyEntries))
					score += GetScore(fragment);
			return score;
		}

		/// <summary>Makes an addition to the hypothesis, and removes the added column from from <see cref="Unsorted"/>.</summary>
		/// <param name="column"></param>
		/// <param name="side"></param>
		private void Commit(string[] column, RightOrLeft side)
		{
			string[] separated;
			for(int i = 0; i < LineCount; ++i)
			{
				switch(side)
				{
				case Right: Lines[i].Append(column[i]); break;
				case Left: Lines[i].Insert(0, column[i]); break;
				}
				separated = Lines[i].ToString().Split(NonLetters, StringSplitOptions.RemoveEmptyEntries);
				LeftBits[i] = separated.FirstOrDefault();
				RightBits[i] = separated.LastOrDefault();
			}
			Unsorted.Remove(column);
			Print();
		}

		/// <summary>Prints the current hypothesis to the console, overwriting the previous one, creating an animation.</summary>
		private void Print()
		{
			Console.CursorTop -= LineCount;
			foreach(var line in Lines)
				Console.WriteLine(line.ToString());
		}

		/// <summary>Final solution, to be available after the algorithm is complete.</summary>
		public override string Solution
		{
			get
			{
				var text = new StringBuilder();
				foreach(var line in Lines)
					text.AppendLine(line.ToString());
				return text.ToString();
			}
		}
	}
}
