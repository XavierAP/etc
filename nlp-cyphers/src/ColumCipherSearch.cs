using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace JP.AIclass
{
	/// <summary>Represents a <see cref="ColumCipher"/> that is solved by an A* search algorithm, guaranteeing that
	/// the global optimum (according to the provided <see cref="WordDictionaryByLength"/>) is found.</summary>
	sealed partial class ColumCipherSearch :ColumCipher
	{
		public ColumCipherSearch(string[,] linesByCols, WordDictionaryByLength wordDictionary)
			:base(linesByCols, wordDictionary) { }

		/// <summary>State frontier of the A* search algorithm.</summary>
		private readonly List<ColumnOrder> Frontier = new List<ColumnOrder>();

		protected override void Solve()
		{
			Console.CursorTop += LineCount; // before the first call to Print()

			var best = ColumnOrder.GetInitial(this);
			Frontier.Add(best);

			while(best.NotGoal)
			{
				best = Frontier.First();

				foreach(var next in Frontier)
					if(next.TotalCost < best.TotalCost)
						best = next;

				Frontier.Remove(best);
				Frontier.AddRange(best.GetDescendants());

				Print(best);
			}

			Goal = best;
		}

		/// <summary>Prints the current hypothesis to the console, overwriting the previous one, creating an animation.</summary>
		private void Print(ColumnOrder current)
		{
			Console.CursorTop -= LineCount;
			Console.Write(current);
		}

		public override string Solution { get { return Goal.ToString(); } }

		private ColumnOrder Goal;
	}
}
