using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JP.AIclass
{
	/// <summary>Represents a cipher where every letter is rotated a constant number of places.</summary>
	partial class RotationCipher :IEnumerable<RotationCipher.Rotation>
	{
		private const char
			LastLetter  = 'z',
			FirstLetter = 'a';
		private const int AlphabetSize = LastLetter - FirstLetter + 1;

		/// <summary>Solves a cipher by assigning a score to each possible rotation.</summary>
		/// <param name="input">Cipher to solve.</param>
		/// <param name="wordDictionary"><see cref="WordDictionaryByLength"/> to use for scoring.</param>
		public RotationCipher(string input, WordDictionaryByLength wordDictionary)
		{
			this.Input = input;

			Builder = new StringBuilder(input.Length);

			this.WordDictionary = wordDictionary;

			NonLetters = (
				from c in input
				where !char.IsLetter(c)
				select c
				).ToArray();

			Rotations = new Rotation[AlphabetSize];
			int best = -1;
			for(int i = 0; i < AlphabetSize; ++i)
			{
				Rotations[i] = new Rotation(this, i);
				if(Rotations[i].Score > best)
				{
					best = Rotations[i].Score;
					Solution = Rotations[i];
				}
			}
		}

		/// <summary><see cref="Rotation"/> resulting from any offset.</summary>
		/// <param name="offset"></param>
		public Rotation this[int offset]
		{
			get
			{
				if(offset >= AlphabetSize)
					return this[offset % AlphabetSize];
				else if(offset < 0)
					return this[AlphabetSize + offset % AlphabetSize];
				else
					return Rotations[offset];
			}
		}

		/// <summary>Solution of the cipher, based on the best score
		/// from the provided <see cref="WordDictionaryByLength"/>.</summary>
		public readonly Rotation Solution;

		/// <summary>All possible <see cref="Rotation"/>s--publicly accessible through the indexer.</summary>
		private readonly Rotation[] Rotations;

		/// <summary>Original cipher.</summary>
		private readonly string Input;

		/// <summary>Dictionary used to calculate scoring of each <see cref="Rotation"/>'s text.</summary>
		private readonly WordDictionaryByLength WordDictionary;

		private readonly StringBuilder Builder;

		/// <summary>Set of characters that can separate words.</summary>
		private readonly char[] NonLetters;

		/// <summary>Score of a string calculated from the provided <see cref="WordDictionaryByLength"/>.</summary>
		/// <param name="text"></param>
		/// <returns></returns>
		private int GetScore(string text)
		{
			int score = 0;
			var words = text.Split(NonLetters, StringSplitOptions.RemoveEmptyEntries);
			foreach(var word in words)
				if(WordDictionary.Contains(word))
					score += word.Length;
			return score;
		}

		public IEnumerator<Rotation> GetEnumerator()
		{
			foreach(var rotation in Rotations)
				yield return rotation;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<Rotation>)this).GetEnumerator();
		}
	}
}
