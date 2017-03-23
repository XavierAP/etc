using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace JP.AIclass
{
	/// <summary>Dictionary of words takes from a plain text file, optimized
	/// to check fragments of words instead of whole ones.</summary>
	class WordDictionaryByLength :SortedDictionary<int, string>
	{
		/// <summary>Creates a new instance from a plain text file.</summary>
		/// <param name="filePathName">Location of the file. It must have
		/// each word in a line, and no other characters besides \n and \r.</param>
		public WordDictionaryByLength(string filePathName)
		{
			var builders = new Dictionary<int, StringBuilder>();
			foreach(var word in File.ReadAllLines(filePathName))
			{
				if(string.IsNullOrWhiteSpace(word))
					continue;
				if(!builders.ContainsKey(word.Length))
					builders.Add(word.Length, new StringBuilder());
				builders[word.Length].AppendLine(word);
				++WordCount;
			}
			foreach(var chapter in builders)
				Add(chapter.Key, chapter.Value.ToString());
		}

		/// <summary>Number of words in the dictionary.</summary>
		public readonly int WordCount;

		/// <summary>Returns true if a string is a word in the dictionary,
		/// or is contained in a longer word.</summary>
		public bool Contains(string letters)
		{
			var possible =
				from k in Keys
				where k >= letters.Length
				select k;
			foreach(var length in possible)
				if(this[length].Contains(letters))
					return true;
			return false;
		}

		/*
		public void Add(string word)
		{
			if (string.IsNullOrWhiteSpace(word))
				throw new ArgumentNullException();
			if (!ContainsKey(word.Length))
				base.Add(word.Length, string.Empty);
			this[word.Length] = string.Concat(this[word.Length], word, Environment.NewLine);
			++WordCount;
		}
		*/
	}
}
