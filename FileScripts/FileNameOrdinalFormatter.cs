﻿using System;
using System.Collections.Generic;
using System.Text;

namespace JP
{
	using FastString = ReadOnlySpan<char>;

	public delegate void PathNameChanger(string oldPathName, string newPathName);

	/// <summary>
	/// Windows can helpfully number files (1) (2) ... (10) etc
	/// but this results in wrong ordering with other software.
	/// This class fixes it into (01) (02) ... (10) etc.
	/// </summary>
	public static class FileNameOrdinalFormatter
	{
		private const int NullLength = int.MinValue;

		public static void
		ChangeNames(IReadOnlyList<string> filePathNames, PathNameChanger callback)
		{
			var cache = new StringBuilder(filePathNames[0].Length + 9);
			for(int i = 0; i < filePathNames.Count; i++)
			{
				var pathName = filePathNames[i];
				if(HasNumberInBrackets(pathName,
					out var prefixIncludingBracket,
					out var digitLength))
				{
					var (k, maxDigitLength) = FindLastFileWithSamePrefix(filePathNames,
						prefixIncludingBracket, i, digitLength);

					if(maxDigitLength > 1)
					{
						for(int j = i; j <= k; j++)
						{
							pathName = filePathNames[j];
							callback(pathName, GetNewName(pathName,
								prefixIncludingBracket, maxDigitLength, cache));
						}
					}
					i = k;
				}
			}
		}

		private static string
		GetNewName(FastString pathName,
			FastString prefixIncludingBracket, int maxDigitLength,
			StringBuilder cache)
		{
			var numberPos = prefixIncludingBracket.Length;
			if(HasNumberAt(pathName, numberPos, out var digitLength))
			{
				var zeroPaddingLength = maxDigitLength - digitLength;
				cache.EnsureCapacity(pathName.Length + zeroPaddingLength);
				return cache.Clear()
					.Append(prefixIncludingBracket)
					.Append('0', zeroPaddingLength)
					.Append(pathName[numberPos ..])
					.ToString();
			}
			else throw new InvalidProgramException();
		}

		private static (int LastIndex, int MaxDigitLength)
		FindLastFileWithSamePrefix(IReadOnlyList<string> files,
			FastString prefixIncludingBracket,
			int firstIndex, int firstDigitLength)
		{
			var lastIndex = firstIndex;
			var maxDigitLength = firstDigitLength;

			for(int i = firstIndex + 1; i < files.Count; i++)
			{
				var pathName = files[i].AsSpan();
				if(pathName.StartsWith(prefixIncludingBracket) &&
					HasNumberAt(pathName, prefixIncludingBracket.Length,
						out var digitLength))
				{
					lastIndex = i;
					if(digitLength > maxDigitLength)
						maxDigitLength = digitLength;
				}
			}
			return (lastIndex, maxDigitLength);
		}

		private static bool
		HasNumberInBrackets(FastString pathName,
			out FastString prefixIncludingBracket, out int digitLength)
		{
			for(int i = 0; i < pathName.Length; i++)
			{
				char pre = pathName[i];
				if(pre == '(' &&
					HasNumberAt(pathName, i + 1, out digitLength))
				{
					prefixIncludingBracket = pathName[..(i + 1)];
					return true;
				}
			}
			prefixIncludingBracket = null;
			digitLength = NullLength;
			return false;
		}

		private static bool
		HasNumberAt(FastString pathName,
			int startIndex, out int digitLength)
		{
			for(int i = startIndex; i < pathName.Length; i++)
			{
				char c = pathName[i];
				if(char.IsDigit(c))
				{
					continue;
				}
				else if(c == ')')
				{
					digitLength = i - startIndex;
					return digitLength > 0;
				}
				else
				{
					break;
				}
			}
			digitLength = NullLength;
			return false;
		}
	}
}
