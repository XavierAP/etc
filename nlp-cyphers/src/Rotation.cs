using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JP.AIclass
{
	partial class RotationCipher
	{
		/// <summary>Represents a possible solution to a <see cref="RotationCipher"/>.</summary>
		internal class Rotation
		{
			/// <summary>Creates a new instance.</summary>
			/// <param name="cipher">Where this rotation comes from.</param>
			/// <param name="offset">Number of places in the alphabet to rotate each letter.</param>
			public Rotation(RotationCipher cipher, int offset)
			{
				this.Offset = offset;
				if(offset == 0)
					Text = cipher.Input;
				else
				{
					cipher.Builder.Clear();
					foreach(var c in cipher.Input)
						cipher.Builder.Append(RotateLetter(c, offset));
					Text = cipher.Builder.ToString();
				}
				Score = cipher.GetScore(Text);
			}

			/// <summary>Number of places in the alphabet that each letter has been rotated.</summary>
			public readonly int Offset;

			/// <summary>Score of this rotation's <see cref="Text"/>, based on the
			/// <see cref="WordDictionaryByLength"/> of the <see cref="RotationCipher"/>.</summary>
			public readonly int Score;

			/// <summary>Text of this rotation.</summary>
			public readonly string Text;

			/// <summary>Rotates a single character a number of places in the alphabet.
			/// Has no effect on non letters.</summary>
			/// <param name="input"></param>
			/// <param name="offset"></param>
			/// <returns></returns>
			private char RotateLetter(char input, int offset)
			{
				if(!char.IsLetter(input))
					return input;
				var limits = char.IsLower(input) ?
					new { first = FirstLetter, last = LastLetter } :
					new { first = char.ToUpper(FirstLetter), last = char.ToUpper(LastLetter) };
				int output = input + offset;
				if(output > limits.last)
					return RotateLetter(input, offset - AlphabetSize);
				else if(output < limits.first)
					return RotateLetter(input, offset + AlphabetSize);
				else
					return (char)output;
			}

			public override string ToString()
			{
				return Text;
			}
		}
	}
}
