using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace JP.CryptoClass
{
	static class BirthdayAttack
	{
		static void Main(string[] args)
		{
			const int
				lenHash = 50,
				ntries = 1 << lenHash / 2;
			const UInt64 truncate = (1UL << lenHash) - 1UL;

			var rnd = new Random();
			var hasher = new SHA256Managed();
			var dic = new Dictionary<UInt64, byte[]>(ntries);
			var buf = new byte[8];

			for(int i = 0; i < ntries; ++i)
			{
				if(i % 1000000 == 0)
					Console.WriteLine("{0:n0} of {1:n0}", i, ntries);

				rnd.NextBytes(buf);
				var hash256 = hasher.ComputeHash(buf);
				UInt64 hash = 0UL;

				//for(int j = 0, power = 0; power < lenHash; ++j, power += 8) // big-endian
				for(int j = hash256.Length - 1, power = 0; power < lenHash; --j, power += 8) // little-endian
					hash += (UInt64)hash256[j] << power;

				hash &= truncate;
				if(dic.ContainsKey(hash))
					throw new Exception(string.Format(@"{1}{0}{2}{0}hash: {3}", Environment.NewLine,
						Hex.GetFromBytes(buf),
						Hex.GetFromBytes(dic[hash]),
						hash.ToString("x")));
				else
					dic.Add(hash, buf);
			}
		}
	}
}
