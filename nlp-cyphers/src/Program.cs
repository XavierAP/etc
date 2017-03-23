using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace JP.AIclass
{
	static class Program
	{
		static void Main(string[] args)
		{
			Time.Restart();
			WordDictionary = new WordDictionaryByLength(Cfg.DictionaryFilePath);
			Time.Stop();
			Console.WriteLine(
@"Dictionary with {0:n0} words memorized from disk, indexed and ordered in {1:n0} miliseconds.

Solving rotation cipher...", WordDictionary.WordCount, Time.ElapsedMilliseconds);

			SolveRotation(Cfg.RotationCipher);			
			Console.Clear();
			SolveColumns(Cfg.ColumnCipher);
			Console.WriteLine("Press any key to continue... ");
			Console.ReadKey(true);
		}

		static Properties.Settings Cfg = Properties.Settings.Default;

		static WordDictionaryByLength WordDictionary;

		static Stopwatch Time = new Stopwatch();

		static void SolveRotation(string msg)
		{
			var cipher = new RotationCipher(msg.ToLower(), WordDictionary);

			double normalizer = (double)(Console.BufferWidth - 4) / msg.Length;
			foreach(var rotation in cipher)
				Console.WriteLine("{0,2} {1}", rotation.Offset,
					new string('#', (int)Math.Round(rotation.Score * normalizer)));

			Console.WriteLine("The solution is offset {0}:", cipher.Solution.Offset);
			Console.WriteLine(cipher.Solution + Environment.NewLine);

			Console.Write("Which other rotation do you wish to see (enter nothing to continue)? ");
			int i,
				x = Console.CursorLeft,
				y = Console.CursorTop;
			string line;
			while(int.TryParse(line = Console.ReadLine(), out i))
			{
				Console.SetCursorPosition(0, y+1);
				Console.Write(cipher[i]);
				Console.SetCursorPosition(x, y);
				Console.Write(new string(' ', line.Length));
				Console.SetCursorPosition(x, y);
			}
		}

		static void SolveColumns(string msg)
		{
			Console.WriteLine(msg);
			Console.WriteLine();

			var lines = new List<string[]>();
			int ncols = 0;
			using(var reader = new StringReader(msg.ToLower()))
			{
				var separator = new[] { Cfg.ColumnSeparator };
				string line;
				while(!string.IsNullOrWhiteSpace(
					line = reader.ReadLine() ))
				{
					var fragments = line.Split(separator, StringSplitOptions.None);
					if(ncols < fragments.Length)
						ncols = fragments.Length;
					lines.Add(fragments);
				}
			}
			var linesByCols = new string[lines.Count, ncols];
			int i = 0;
			foreach(var line in lines)
			{
				int j = 0;
				foreach(var fragment in line)
					linesByCols[i, j++] = fragment;
				++i;
			}
			Time.Restart();
			new ColumCipherGreedy(linesByCols, WordDictionary);
			Time.Stop();
			Console.WriteLine("\nColumn cipher solved by greedy algorithm in {0:n0} miliseconds.\n", Time.ElapsedMilliseconds);
			Time.Restart();
			new ColumCipherSearch(linesByCols, WordDictionary);
			Time.Stop();
			Console.WriteLine("\nColumn cipher solved by search algorithm in {0:n0} miliseconds.\n", Time.ElapsedMilliseconds);
		}
	}
}
