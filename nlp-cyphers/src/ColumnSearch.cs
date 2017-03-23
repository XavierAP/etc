using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JP.AIclass
{
	using RightOrLeft = Boolean; // for readability--see ColumCipher.Right and Left

	partial class ColumCipherSearch
	{
		/// <summary>Represents a column in a <see cref="ColumCipherSearch"/> paired with its heuristic.</summary>
		class Column
		{
			public Column(string[] text)
			{
				this.Text = text;
				CharCount = Heuristic = 0;
				foreach(var fragment in text)
					foreach(var c in fragment)
					{
						++CharCount;
						if(!char.IsLetter(c))
							++Heuristic;
					}
			}
			public readonly string[] Text;
			public readonly int CharCount;
			public readonly int Heuristic;
		}

		/// <summary>Represents a possible hypothesis in a <see cref="ColumCipherSearch"/>, with part
		/// of the columns in some order, the rest remaining to be ordered, and a <see cref="Cost"/>.</summary>
		class ColumnOrder
		{
			/// <summary>Gets the starting <see cref="ColumnOrder"/>s for the frontier of a <see cref="ColumCipherSearch"/>.</summary>
			public static ColumnOrder GetInitial(ColumCipherSearch cipher)
			{
				var empty = new ColumnOrder(cipher);
				var best = empty.Unsorted.FirstOrDefault();
				foreach(var col in empty.Unsorted)
					if(col.Heuristic < best.Heuristic)
						best = col;
				return new ColumnOrder(empty, best, Right);
			}
			/// <summary>Creates a <see cref="ColumnOrder"/> with no <see cref="Column"/>s ordered.
			/// To be called exclusivelly by <see cref="GetInitial"/>().</summary>
			private ColumnOrder(ColumCipherSearch cipher)
			{
				this.Cipher = cipher;
				Unsorted = new List<Column>(cipher.Unsorted.Count);
				Lines = new string[cipher.LineCount];
				Cost = Heuristic = 0;
				Column col;
				foreach(var text in cipher.Unsorted)
				{
					Unsorted.Add(col = new Column(text));
					Heuristic += col.Heuristic;
				}
			}

			/// <summary>Number of lines.</summary>
			private readonly ColumCipherSearch Cipher;

			/// <summary>Columns that remain to be ordered.</summary>
			private readonly List<Column> Unsorted;

			/// <summary>The cost of adding a column somewhere is defined as the increment of the number of characters
			/// that don't form part of words. This can be higher than the actual number of characters in the column, if adding it
			/// breaks already formed word fragments.</summary>
			private readonly int Cost;

			/// <summary>Number of non-letters remaining in the unsorted columns. Optimistic (admissible) estimation of the
			/// number of characters that won't fit in the dictionary after adding the rest of <see cref="Unsorted"/>.</summary>
			private readonly int Heuristic;

			private string[] Lines;

			/// <summary>Gets the possible <see cref="ColumnOrder"/> resulting from adding any remaining unordered column right or left.</summary>
			public IEnumerable<ColumnOrder> GetDescendants()
			{
				foreach(var side in BothSides)
					foreach(var col in Unsorted)
						yield return new ColumnOrder(this, col, side);
			}

			/// <summary>Gets a possible <see cref="ColumnOrder"/> resulting from adding a particular column right or left.</summary>
			private ColumnOrder(ColumnOrder parent, Column addition, RightOrLeft side)
			{
				Cipher = parent.Cipher;
				Unsorted = (
					from col in parent.Unsorted
					where col != addition
					select col
					).ToList();
				Score = 0;
				Lines = new string[Cipher.LineCount];
				for(int i = 0; i < Cipher.LineCount; ++i)
				{
					switch(side)
					{
					case Right: Lines[i] = parent.Lines[i] + addition.Text[i]; break;
					case Left: Lines[i] = addition.Text[i] + parent.Lines[i]; break;
					}
					var fragments = Lines[i].Split(Cipher.NonLetters, StringSplitOptions.RemoveEmptyEntries);
					foreach(var letters in fragments)
						Score += Cipher.GetScore(letters);
				}
				Heuristic = parent.Heuristic - addition.Heuristic;
				Cost = addition.CharCount - Score + parent.Score;
				TotalCost = Heuristic + Cost * 10;
				//Heuristic = parent.Heuristic - addition.Heuristic;
				//Cost = parent.Cost + addition.CharCount - Score + parent.Score;
				//TotalCost = Heuristic + Cost;
			}

			/// <summary>Word (fragments) matching score.</summary>
			private readonly int Score;

			/// <summary><see cref="Cost"/> plus <see cref="Heuristic"/> (estimated cost to gal).</summary>
			public readonly int TotalCost;

			public bool NotGoal
			{
				get
				{
					return Unsorted.Count != 0;
				}
			}

			public override string ToString()
			{
				Text.Clear();
				for(int i = 0; i < Cipher.LineCount; ++i)
				{
					Text.Append(Lines[i])
						.Append(' ', Cipher.LineLengths[i] - Lines[i].Length)
						.AppendLine();
				}
				return Text.ToString();
			}
			// Remind me why static local variables would be bad data hiding again?
			private static StringBuilder Text = new StringBuilder();
		}
	}
}
