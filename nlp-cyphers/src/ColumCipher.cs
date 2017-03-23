using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JP.AIclass
{
	using RightOrLeft = Boolean; // for readability--see ColumCipher.Right and Left

	/// <summary>Represents a columns cipher, where a multi-line text is
	/// cut up in columns and these are shuffled.</summary>
	abstract class ColumCipher
	{
		/// <summary>Enum-like alias for the sake of readability.</summary>
		protected const RightOrLeft Right = true, Left = false;
		protected readonly static RightOrLeft[] BothSides = new[] { Right, Left };

		/// <summary>Any code in overriding constructors will be called after
		/// <see cref="Solve"/>() has returned within this.</summary>
		/// <param name="linesByCols">Cipher to solve. The first dimension is for the lines and the second for the columns.</param>
		/// <param name="wordDictionary"><see cref="WordDictionaryByLength"/> to use for scoring.</param>
		protected ColumCipher(string[,] linesByCols, WordDictionaryByLength wordDictionary)
		{
			this.WordDictionary = wordDictionary;

			LineCount = linesByCols.GetLength(0);
			var nCols = linesByCols.GetLength(1);

			var chars = new StringBuilder();
			foreach(var fragment in linesByCols)
				foreach(var c in fragment)
					if(!char.IsLetter(c))
						chars.Append(c);
			NonLetters = chars.ToString().ToCharArray();

			Unsorted = new List<string[]>(new string[nCols][]);
			LineLengths = new int[LineCount]; // zeroes
			for(int j = 0; j < nCols; ++j)
			{
				Unsorted[j] = new string[LineCount];
				for(int i = 0; i < LineCount; ++i)
				{
					Unsorted[j][i] = linesByCols[i, j];
					LineLengths[i] += linesByCols[i, j].Length;
				}
			}
			Solve();
		}

		/// <summary>Columns that haven't been ordered yet.</summary>
		protected readonly List<string[]> Unsorted;

		/// <summary>Number of lines.</summary>
		protected readonly int LineCount;

		/// <summary>Lengths of the lines in the cipher.</summary>
		protected readonly int[] LineLengths;

		/// <summary>Set of characters that can separate words.</summary>
		protected readonly char[] NonLetters;

		/// <summary>Dictionary used to calculate scoring.</summary>
		private readonly WordDictionaryByLength WordDictionary;

		/// <summary>The algorithm to order the columns goes here.</summary>
		protected abstract void Solve();

		/// <summary>If a string is found in the dictionary, its length is returned, otherwise 0.</summary>
		/// <param name="fragment">Possible word or fragment thereof.</param>
		protected int GetScore(string fragment)
		{
			if(WordDictionary.Contains(fragment))
				return fragment.Length;
			else
				return 0;
		}

		/// <summary>Solution of the cipher after ordering the columns.</summary>
		public abstract string Solution { get; }
	}
}
